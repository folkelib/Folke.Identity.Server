using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Folke.Identity.Server.Services;
using Folke.Identity.Server.Views;
using Folke.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Linq;
using Microsoft.AspNet.Authorization;

namespace Folke.Identity.Server.Controllers
{
    public class BaseUserController<TUser, TUserView, TKey> : TypedControllerBase
        where TKey : IEquatable<TKey>
        where TUserView : class
        where TUser : class
    {
        protected IUserService<TUser, TUserView> UserService { get; }
        protected SignInManager<TUser> SignInManager { get; }
        protected IUserEmailService<TUser> EmailService { get; set; }
        protected UserManager<TUser> UserManager { get; }

        public BaseUserController(IUserService<TUser, TUserView> userService, UserManager<TUser> userManager, SignInManager<TUser> signInManager, IUserEmailService<TUser> emailService)
        {
            UserService = userService;
            SignInManager = signInManager;
            EmailService = emailService;
            UserManager = userManager;
        }
        
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordView view)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            var account = await UserService.GetCurrentUserAsync();
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

        [HttpPost("password")]
        public async Task<IActionResult> SetPassword([FromBody]SetPasswordView model)
        {
            if (ModelState.IsValid)
            {
                var account = await UserService.GetCurrentUserAsync();
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

        [HttpPut("email")]
        public async Task<IActionResult> SetEmail([FromBody] SetEmailView model)
        {
            if (ModelState.IsValid)
            {
                var account = await UserService.GetCurrentUserAsync();
                var result = await UserManager.SetEmailAsync(account, model.Email);
                if (result.Succeeded)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(account);
                    await EmailService.SendEmailConfirmationEmail(account, code);
                    return Ok();
                }
                AddErrors(result);
            }
            return HttpBadRequest(ModelState);
        }

        [HttpGet("me")]
        public async Task<IHttpActionResult<TUserView>> GetMe()
        {
            return Ok(UserService.MapToUserView(await UserService.GetCurrentUserAsync()));
        }

        [HttpGet("{id}", Name = "GetAccount")]
        public virtual async Task<IHttpActionResult<TUserView>> Get(TKey id)
        {
            var userId = id.ToString();
            var user = await UserManager.FindByIdAsync(userId);
            if (!HttpContext.User.Identity.IsAuthenticated) return Unauthorized<TUserView>();
            if (HttpContext.User.GetUserId() != userId) return Unauthorized<TUserView>();
            return Ok(UserService.MapToUserView(user));
        }

        [HttpPut("search")]
        [Authorize("UserList")]
        public async Task<IHttpActionResult<IEnumerable<TUserView>>> Search([FromBody] UserSearchFilter filter,
            [FromQuery]int offset, [FromQuery]int limit, [FromQuery]string sortColumn)
        {
            return Ok((await UserService.Search(filter, offset, limit, sortColumn)).Select(x => UserService.MapToUserView(x)));
        }

        [HttpGet("~/api/user/me/role")]
        public async Task<IHttpActionResult<IList<string>>> GetUserRoles()
        {
            var roles = await UserManager.GetRolesAsync(await UserService.GetCurrentUserAsync());
            return Ok(roles);
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
