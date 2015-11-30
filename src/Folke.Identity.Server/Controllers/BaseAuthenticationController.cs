﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Folke.Identity.Server.Enumeration;
using Folke.Identity.Server.Services;
using Folke.Identity.Server.Views;
using Folke.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

namespace Folke.Identity.Server.Controllers
{
    public abstract class BaseAuthenticationController<TUser, TKey, TUserView> : TypedControllerBase
         where TKey : IEquatable<TKey>
         where TUser : class
         where TUserView : class
    {
        public IUserService<TUser, TUserView> UserService { get; set; }
        public SignInManager<TUser> SignInManager { get; set; }
        public IUserEmailService<TUser> EmailService { get; set; }

        protected BaseAuthenticationController(IUserService<TUser, TUserView> userService, SignInManager<TUser> signInManager, IUserEmailService<TUser> emailService)
        {
            UserService = userService;
            SignInManager = signInManager;
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
            return BadRequest<LoginResultView>("Mot-de-passe ou e-mail non valide");
        }

        [HttpPut("verifycode")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeView verifyCodeView)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var result =
                await
                    SignInManager.TwoFactorSignInAsync(verifyCodeView.Provider, verifyCodeView.Code,
                        isPersistent: verifyCodeView.RememberMe, rememberClient: verifyCodeView.RememberBrowser);
            if (result.Succeeded)
            {
                return Ok();
            }
            return HttpUnauthorized();
        }

        [HttpPost("register")]
        public async Task<IHttpActionResult<TUserView>> Register([FromBody] RegisterView registerView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest<TUserView>(ModelState);
            }

            var user = UserService.CreateNewUser(registerView.Email, registerView.Email, false);
            
            var result = await UserService.CreateAsync(user, registerView.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return BadRequest<TUserView>(ModelState);
            }
            await SignInManager.SignInAsync(user, isPersistent: false);

            await EmailService.SendEmailConfirmationEmail(user);
            return Created("GetAccount", Convert.ChangeType(await UserService.GetUserIdAsync(user), typeof(TKey)), UserService.MapToUserView(user));
        }


        [HttpPut("send-account-confirm")]
        public async Task<IActionResult> SendAccountConfirm([FromQuery]int userId)
        {
            var user = await GetCurrentUserAsync();
            await EmailService.SendEmailConfirmationEmail(user);
            return Ok();
        }

        private Task<TUser> GetCurrentUserAsync()
        {
            return UserService.FindByIdAsync(HttpContext.User.GetUserId());
        }

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]int userId, [FromQuery]string code)
        {
            if (code == null)
            {
                return HttpBadRequest();
            }

            var user = await GetCurrentUserAsync();
            var result = await UserService.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return HttpBadRequest(ModelState);
            }
            return Ok();
        }

        [HttpPut("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordView forgotPasswordView)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = await UserService.FindByEmailAsync(forgotPasswordView.Email);
            if (user == null || !(await UserService.IsEmailConfirmedAsync(user)))
            {
                return HttpBadRequest();
            }

            await EmailService.SendPasswordResetEmail(user);
            return Ok();
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordView resetPasswordView)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            TUser user;
            if (!string.IsNullOrEmpty(resetPasswordView.Email))
            {
                user = await UserService.FindByEmailAsync(resetPasswordView.Email);

            }
            else
            {
                user = await UserService.FindByIdAsync(resetPasswordView.UserId);
            }

            var result =
                await UserService.ResetPasswordAsync(user, resetPasswordView.Code, resetPasswordView.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return HttpBadRequest(ModelState);
            }
            return Ok();
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl)
        {
            var redirectUrl = Request.Scheme + "://" + Request.Host + "/api/authentication/callback";
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        [HttpGet("send-code")]
        public async Task<IHttpActionResult<SendCodeView>> GetSendCode([FromQuery] bool rememberMe)
        {
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest<SendCodeView>("No user");
            }
            var userFactors = await UserService.GetValidTwoFactorProvidersAsync(user);
            return Ok(new SendCodeView { RememberMe = rememberMe, Providers = userFactors });
        }
        
        [HttpGet("callback")]
        public async Task<ActionResult> ExternalLoginCallback([FromQuery]string returnUrl)
        {
            var loginInfo = await SignInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return HttpBadRequest("No login info");
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

            var userName = Regex.Replace(loginInfo.ExternalPrincipal.GetUserName() ?? Guid.NewGuid().ToString(), @"[^\w]", "");
            while (await UserService.FindByNameAsync(userName) != null)
                userName += "_";
            var email = loginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = UserService.CreateNewUser(userName, email, true);
            var creationResult = await UserService.CreateAsync(user);
            if (creationResult.Succeeded)
            {
                creationResult = await UserService.AddLoginAsync(user, loginInfo);
                if (creationResult.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return View((object)"success");
                }
            }
            return View((object)"failure");
        }

        [HttpDelete("")]
        public async Task LogOff()
        {
            await SignInManager.SignOutAsync();
        }

        [HttpGet("external-login-provider")]
        public IEnumerable<string> GetExternalAuthenticationProviders()
        {
            return SignInManager.GetExternalAuthenticationSchemes().Select(x => x.AuthenticationScheme);
        }
    }
}
