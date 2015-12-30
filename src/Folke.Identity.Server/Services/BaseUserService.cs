using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Folke.Identity.Server.Services
{
    public abstract class BaseUserService<TUser, TUserView> : IUserService<TUser, TUserView>
        where TUser : class
        where TUserView : class
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<TUser> userManager;

        public BaseUserService(IHttpContextAccessor httpContextAccessor, UserManager<TUser> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public abstract TUser CreateNewUser(string userName, string email, bool emailConfirmed);

        public async Task<TUser> GetCurrentUserAsync()
        {
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated) return null;
            return await userManager.FindByIdAsync(httpContextAccessor.HttpContext.User.GetUserId());
        }

        public abstract TUserView MapToUserView(TUser user);
    }
}
