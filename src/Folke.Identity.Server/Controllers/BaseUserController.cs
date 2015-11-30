using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Folke.Identity.Server.Services;
using Folke.Identity.Server.Views;
using Folke.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

namespace Folke.Identity.Server.Controllers
{
    public class BaseUserController<TUser, TUserView, TKey> : TypedControllerBase
        where TKey : IEquatable<TKey>
        where TUserView : class
        where TUser : class
    {
        protected IUserService<TUser, TUserView> UserService { get; set; }
        protected IUserSignInManager<TUser> SignInManager { get; set; }
        protected IUserEmailService<TUser> EmailService { get; set; }

        public BaseUserController(IUserService<TUser, TUserView> userService, IUserSignInManager<TUser> signInManager, IUserEmailService<TUser> emailService)
        {
            UserService = userService;
            SignInManager = signInManager;
            EmailService = emailService;
        }
        
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordView view)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            var account = await GetCurrentUserAsync();
            var result =
                await UserService.ChangePasswordAsync(account, view.OldPassword, view.NewPassword);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(account, isPersistent: false);
                return Ok();
            }
            AddErrors(result);
            return HttpBadRequest(ModelState);
        }

        [HttpPost("password")]
        public async Task<IActionResult> SetPassword([FromBody]SetPasswordView model)
        {
            if (ModelState.IsValid)
            {
                var account = await GetCurrentUserAsync();
                var result = await UserService.AddPasswordAsync(account, model.NewPassword);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(account, isPersistent: false);
                    return Ok();
                }
                AddErrors(result);
            }
            return HttpBadRequest(ModelState);
        }

        [HttpPut("email")]
        public async Task<IActionResult> SetEmail([FromBody] SetEmailView model)
        {
            if (ModelState.IsValid)
            {
                var account = await GetCurrentUserAsync();
                var result = await UserService.SetEmailAsync(account, model.Email);
                if (result.Succeeded)
                {
                    await EmailService.SendEmailConfirmationEmail(account);
                    return Ok();
                }
                AddErrors(result);
            }
            return HttpBadRequest(ModelState);
        }

        [HttpGet("me")]
        public async Task<IHttpActionResult<TUserView>> GetMe()
        {
            return Ok(UserService.MapToUserView(await GetCurrentUserAsync()));
        }

        [NonAction]
        protected async Task<TUser> GetCurrentUserAsync()
        {
            if (!HttpContext.User.Identity.IsAuthenticated) return null;
            return await UserService.FindByIdAsync(HttpContext.User.GetUserId());
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
