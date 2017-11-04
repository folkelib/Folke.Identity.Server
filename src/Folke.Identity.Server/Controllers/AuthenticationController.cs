using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Folke.Identity.Server.Enumeration;
using Folke.Identity.Server.Services;
using Folke.Identity.Server.Views;
using Folke.Mvc.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Folke.Identity.Server.Controllers
{
    [Route("api/authentication")]
    public class AuthenticationController<TUser, TKey, TUserView> : TypedControllerBase
         where TKey : IEquatable<TKey>
         where TUser : class
         where TUserView : class
    {
        private readonly ILogger<AuthenticationController<TUser, TKey, TUserView>> logger;
        private readonly IOptions<IdentityServerOptions> options;
        protected IUserService<TUser, TUserView> UserService { get; }
        protected UserManager<TUser> UserManager { get; }
        protected SignInManager<TUser> SignInManager { get; }
        protected IUserEmailService<TUser> EmailService { get; }

        public AuthenticationController(IUserService<TUser, TUserView> userService,
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager, 
            IUserEmailService<TUser> emailService, 
            ILogger<AuthenticationController<TUser, TKey, TUserView>> logger,
            IOptions<IdentityServerOptions> options)
        {
            this.logger = logger;
            this.options = options;
            UserService = userService;
            SignInManager = signInManager;
            UserManager = userManager;
            EmailService = emailService;
        }

        [HttpPut("login")]
        public async Task<IHttpActionResult<LoginResultView>> Login([FromBody] LoginView loginView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest<LoginResultView>(ModelState);
            }

            var result =
                await
                    SignInManager.PasswordSignInAsync(loginView.Email, loginView.Password, loginView.RememberMe,
                        lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok(new LoginResultView { Status = LoginStatusEnum.Success });
            }

            if (result.IsLockedOut)
            {
                return Ok(new LoginResultView { Status = LoginStatusEnum.LockedOut });
            }

            if (result.RequiresTwoFactor)
            {
                return Ok(new LoginResultView { Status = LoginStatusEnum.RequiresVerification });
            }
            
            // TODO localization
            ModelState.AddModelError(nameof(LoginView.Password), "Mot-de-passe ou e-mail non valide");
            return BadRequest<LoginResultView>(ModelState);
        }

        [HttpPut("verifycode")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeView verifyCodeView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =
                await
                    SignInManager.TwoFactorSignInAsync(verifyCodeView.Provider, verifyCodeView.Code,
                        isPersistent: verifyCodeView.RememberMe, rememberClient: verifyCodeView.RememberBrowser);
            if (result.Succeeded)
            {
                return Ok();
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IHttpActionResult<TUserView>> Register([FromBody] RegisterView registerView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest<TUserView>(ModelState);
            }

            if (!options.Value.RegistrationEnabled)
            {
                return BadRequest<TUserView>("Registration is disabled");
            }

            var user = UserService.CreateNewUser(registerView.Email, registerView.Email, false);
            
            var result = await UserManager.CreateAsync(user, registerView.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return BadRequest<TUserView>(ModelState);
            }
            await SignInManager.SignInAsync(user, isPersistent: false);

            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            await EmailService.SendEmailConfirmationEmail(user, code);
            return Created("GetAccount", Convert.ChangeType(await UserManager.GetUserIdAsync(user), typeof(TKey)), UserService.MapToUserView(user));
        }


        [HttpPut("send-account-confirm")]
        public async Task<IActionResult> SendAccountConfirm([FromQuery]int userId)
        {
            var user = await GetCurrentUserAsync();
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            await EmailService.SendEmailConfirmationEmail(user, code);
            return Ok();
        }

        private Task<TUser> GetCurrentUserAsync()
        {
            return UserManager.FindByIdAsync(UserManager.GetUserId(HttpContext.User));
        }

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]int userId, [FromQuery]string code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var user = await GetCurrentUserAsync();
            var result = await UserManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpPut("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordView forgotPasswordView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await UserManager.FindByEmailAsync(forgotPasswordView.Email);
            if (user == null)
            {
                return BadRequest();
            }

            string code = await UserManager.GeneratePasswordResetTokenAsync(user);
            await EmailService.SendPasswordResetEmail(user, code);
            return Ok();
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordView resetPasswordView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TUser user;
            if (!string.IsNullOrEmpty(resetPasswordView.Email))
            {
                user = await UserManager.FindByEmailAsync(resetPasswordView.Email);

            }
            else
            {
                user = await UserManager.FindByIdAsync(resetPasswordView.UserId);
            }

            var result =
                await UserManager.ResetPasswordAsync(user, resetPasswordView.Code, resetPasswordView.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpGet("link-external-login")]
        public IActionResult LinkLogin([FromQuery]string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Request.Scheme + "://" + Request.Host + "/api/authentication/link-callback";
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, UserManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }
        
        [HttpGet("link-callback")]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("ExternalLoginCallback", "failure");
            }
            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginCallback", "failure");
            }
            var result = await UserManager.AddLoginAsync(user, info);
            if (result.Succeeded)
                result = await SaveExternalLoginInfoClaims(user, info);
            return View("ExternalLoginCallback", result.Succeeded ? "success" : "failure");
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl)
        {
            var redirectUrl = Request.Scheme + "://" + Request.Host + "/api/authentication/callback";
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet("callback")]
        public async Task<ActionResult> ExternalLoginCallback([FromQuery]string returnUrl)
        {
            var loginInfo = await SignInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return View((object)"failure");
            }

            var result = await SignInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return View((object)"success");
            }
            if (result.IsLockedOut)
            {
                return View((object)"lockedout");
            }
            if (result.RequiresTwoFactor)
            {
                return View((object)"requires-verification");
            }

            string userName;
            if (loginInfo.Principal.Identity.Name != null)
            {
                logger.LogInformation(
                    $"Proposed external principal user name: {loginInfo.Principal.Identity.Name}");
                userName = loginInfo.Principal.Identity.Name;
            }
            else
            {
                userName = Guid.NewGuid().ToString("N");
            }
            userName = Regex.Replace(userName, @"[^a-zA-Z0-9]", "", RegexOptions.CultureInvariant);
            while (await UserManager.FindByNameAsync(userName) != null)
                userName += Guid.NewGuid().ToString("N")[0];
            logger.LogInformation($"Creating new user {userName}");
            var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if (email != null && await UserManager.FindByEmailAsync(email) != null)
            {
                return View((object) "password");
            }

            var user = UserService.CreateNewUser(userName, email, true);
            var creationResult = await UserManager.CreateAsync(user);
            if (creationResult.Succeeded)
            {
                creationResult = await UserManager.AddLoginAsync(user, loginInfo);
                if (creationResult.Succeeded)
                    creationResult = await SaveExternalLoginInfoClaims(user, loginInfo);
                if (creationResult.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return View((object)"success");
                }
            }
            return View((object)"failure");
        }

        [HttpGet("send-code")]
        public async Task<IHttpActionResult<SendCodeView>> GetSendCode([FromQuery] bool rememberMe)
        {
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest<SendCodeView>("No user");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(user);
            return Ok(new SendCodeView { RememberMe = rememberMe, Providers = userFactors });
        }

        [HttpDelete("")]
        public async Task LogOff()
        {
            await SignInManager.SignOutAsync();
        }

        [HttpGet("external-login-providers")]
        public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationProviders()
        {
            return await SignInManager.GetExternalAuthenticationSchemesAsync();
        }

        [HttpGet("external-logins")]
        public async Task<IEnumerable<UserLoginInfo>> GetExternalLogins()
        {
            return await UserManager.GetLoginsAsync(await GetCurrentUserAsync());
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        
        private async Task<IdentityResult> SaveExternalLoginInfoClaims(TUser user, ExternalLoginInfo info)
        {
            return await UserManager.AddClaimsAsync(user, info.Principal.Claims.Where(x => x.Type != ClaimTypes.Email && x.Type != ClaimTypes.NameIdentifier));
        }
    }
}
