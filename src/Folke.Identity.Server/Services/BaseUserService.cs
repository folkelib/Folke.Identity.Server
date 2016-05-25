using System.Collections.Generic;
using System.Threading.Tasks;
using Folke.Identity.Server.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Folke.Identity.Server.Services
{
    /// <summary>
    /// A base implementation of IUserService
    /// </summary>
    /// <typeparam name="TUser">The user type</typeparam>
    /// <typeparam name="TUserView">The user view type</typeparam>
    public abstract class BaseUserService<TUser, TUserView> : IUserService<TUser, TUserView>
        where TUser : class
        where TUserView : class
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<TUser> userManager;

        protected BaseUserService(IHttpContextAccessor httpContextAccessor, UserManager<TUser> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        protected UserManager<TUser> UserManager => userManager;

        public abstract TUser CreateNewUser(string userName, string email, bool emailConfirmed);

        public async Task<TUser> GetCurrentUserAsync()
        {
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated) return null;
            return await userManager.FindByIdAsync(UserManager.GetUserId(httpContextAccessor.HttpContext.User));
        }

        public abstract Task<IList<TUser>> Search(UserSearchFilter name, int offset, int limit, string sortColumn);

        public abstract TUserView MapToUserView(TUser user);
    }
}
