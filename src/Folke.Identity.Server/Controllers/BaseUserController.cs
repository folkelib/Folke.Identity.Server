using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Elm.AspNet.Identity;
using Folke.Elm;
using Folke.Identity.Server.Services;
using Folke.Identity.Server.Views;
using Folke.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

namespace Folke.Identity.Server.Controllers
{
    public class BaseUserController<TUser, TKey> : TypedControllerBase
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TUser, TKey>
    {
        protected IFolkeConnection Connection { get; set; }
        protected IUserService<TUser> UserManager { get; set; }
        protected IUserSignInManager<TUser> SignInManager { get; set; }
        protected IUserEmailService<TUser, TKey> EmailService { get; set; }

        public BaseUserController(IFolkeConnection connection, IUserService<TUser> userManager, IUserSignInManager<TUser> signInManager, IUserEmailService<TUser, TKey> emailService)
        {
            Connection = connection;
            UserManager = userManager;
            SignInManager = signInManager;
            EmailService = emailService;
        }
        
        [Route("password")]
        [HttpPut]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordView view)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            var account = await GetCurrentUserAsync();
            var result =
                await UserManager.ChangePasswordAsync(account, view.OldPassword, view.NewPassword);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(account, isPersistent: false);
                return Ok();
            }
            AddErrors(result);
            return HttpBadRequest(ModelState);
        }

        [HttpPost]
        [Route("password")]
        public async Task<IActionResult> SetPassword([FromBody]SetPasswordView model)
        {
            if (ModelState.IsValid)
            {
                var account = await GetCurrentUserAsync();
                var result = await UserManager.AddPasswordAsync(account, model.NewPassword);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(account, isPersistent: false);
                    return Ok();
                }
                AddErrors(result);
            }
            return HttpBadRequest(ModelState);
        }

        [HttpPut]
        [Route("email")]
        public async Task<IActionResult> SetEmail([FromBody] SetEmailView model)
        {
            if (ModelState.IsValid)
            {
                var account = await GetCurrentUserAsync();
                var result = await UserManager.SetEmailAsync(account, model.Email);
                if (result.Succeeded)
                {
                    await EmailService.SendEmailConfirmationEmail(account);
                    return Ok();
                }
                AddErrors(result);
            }
            return HttpBadRequest(ModelState);
        }

        [NonAction]
        protected async Task<TUser> GetCurrentUserAsync()
        {
            if (!HttpContext.User.Identity.IsAuthenticated) return null;
            return await UserManager.FindByIdAsync(HttpContext.User.GetUserId());
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
