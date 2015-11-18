using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;

namespace Folke.Identity.Server.Services
{
    public class UserSignInManager<TUser> : SignInManager<TUser>, IUserSignInManager<TUser>
        where TUser : class
    {
        public UserSignInManager(UserManager<TUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
        }
    }
}
