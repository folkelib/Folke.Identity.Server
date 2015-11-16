using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Identity;

namespace Folke.Identity.Server.Services
{
    public interface IUserSignInManager<TUser>
        where TUser : class
    {
        Task<ClaimsPrincipal> CreateUserPrincipalAsync(TUser user);
        Task<bool> CanSignInAsync(TUser user);
        Task RefreshSignInAsync(TUser user);
        Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null);
        Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string authenticationMethod);
        Task SignOutAsync();
        Task<TUser> ValidateSecurityStampAsync(ClaimsPrincipal principal, string userId);
        Task<SignInResult> PasswordSignInAsync(TUser user, string password, bool isPersistent, bool lockoutOnFailure);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        Task<bool> IsTwoFactorClientRememberedAsync(TUser user);
        Task RememberTwoFactorClientAsync(TUser user);
        Task ForgetTwoFactorClientAsync();
        Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient);
        Task<TUser> GetTwoFactorAuthenticationUserAsync();
        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);
        IEnumerable<AuthenticationDescription> GetExternalAuthenticationSchemes();
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId);
    }
}
