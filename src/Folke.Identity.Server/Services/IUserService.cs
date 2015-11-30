using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Folke.Identity.Server.Views;

namespace Folke.Identity.Server.Services
{
    public interface IUserService<TUser, TUserView>
        where TUser : class
        where TUserView : class
    {
        Task<string> GenerateConcurrencyStampAsync(TUser user);
        Task<IdentityResult> CreateAsync(TUser user);
        Task<IdentityResult> UpdateAsync(TUser user);
        Task<IdentityResult> DeleteAsync(TUser user);
        Task<TUser> FindByIdAsync(string userId);
        Task<TUser> FindByNameAsync(string userName);
        Task<IdentityResult> CreateAsync(TUser user, string password);
        string NormalizeKey(string key);
        Task UpdateNormalizedUserNameAsync(TUser user);
        Task<string> GetUserNameAsync(TUser user);
        Task<IdentityResult> SetUserNameAsync(TUser user, string userName);
        Task<string> GetUserIdAsync(TUser user);
        Task<bool> CheckPasswordAsync(TUser user, string password);
        Task<bool> HasPasswordAsync(TUser user);
        Task<IdentityResult> AddPasswordAsync(TUser user, string password);
        Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);
        Task<IdentityResult> RemovePasswordAsync(TUser user, CancellationToken cancellationToken);
        Task<string> GetSecurityStampAsync(TUser user);
        Task<IdentityResult> UpdateSecurityStampAsync(TUser user);
        Task<string> GeneratePasswordResetTokenAsync(TUser user);
        Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword);
        Task<TUser> FindByLoginAsync(string loginProvider, string providerKey);
        Task<IdentityResult> RemoveLoginAsync(TUser user, string loginProvider, string providerKey);
        Task<IdentityResult> AddLoginAsync(TUser user, UserLoginInfo login);
        Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user);
        Task<IdentityResult> AddClaimAsync(TUser user, Claim claim);
        Task<IdentityResult> AddClaimsAsync(TUser user, IEnumerable<Claim> claims);
        Task<IdentityResult> ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim);
        Task<IdentityResult> RemoveClaimAsync(TUser user, Claim claim);
        Task<IdentityResult> RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims);
        Task<IList<Claim>> GetClaimsAsync(TUser user);
        Task<IdentityResult> AddToRoleAsync(TUser user, string role);
        Task<IdentityResult> AddToRolesAsync(TUser user, IEnumerable<string> roles);
        Task<IdentityResult> RemoveFromRoleAsync(TUser user, string role);
        Task<IdentityResult> RemoveFromRolesAsync(TUser user, IEnumerable<string> roles);
        Task<IList<string>> GetRolesAsync(TUser user);
        Task<bool> IsInRoleAsync(TUser user, string role);
        Task<string> GetEmailAsync(TUser user);
        Task<IdentityResult> SetEmailAsync(TUser user, string email);
        Task<TUser> FindByEmailAsync(string email);
        Task UpdateNormalizedEmailAsync(TUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(TUser user);
        Task<IdentityResult> ConfirmEmailAsync(TUser user, string token);
        Task<bool> IsEmailConfirmedAsync(TUser user);
        Task<string> GenerateChangeEmailTokenAsync(TUser user, string newEmail);
        Task<IdentityResult> ChangeEmailAsync(TUser user, string newEmail, string token);
        Task<string> GetPhoneNumberAsync(TUser user);
        Task<IdentityResult> SetPhoneNumberAsync(TUser user, string phoneNumber);
        Task<IdentityResult> ChangePhoneNumberAsync(TUser user, string phoneNumber, string token);
        Task<bool> IsPhoneNumberConfirmedAsync(TUser user);
        Task<string> GenerateChangePhoneNumberTokenAsync(TUser user, string phoneNumber);
        Task<bool> VerifyChangePhoneNumberTokenAsync(TUser user, string token, string phoneNumber);
        Task<bool> VerifyUserTokenAsync(TUser user, string tokenProvider, string purpose, string token);
        Task<string> GenerateUserTokenAsync(TUser user, string tokenProvider, string purpose);
        void RegisterTokenProvider(string providerName, IUserTokenProvider<TUser> provider);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(TUser user);
        Task<bool> VerifyTwoFactorTokenAsync(TUser user, string tokenProvider, string token);
        Task<string> GenerateTwoFactorTokenAsync(TUser user, string tokenProvider);
        Task<bool> GetTwoFactorEnabledAsync(TUser user);
        Task<IdentityResult> SetTwoFactorEnabledAsync(TUser user, bool enabled);
        Task<bool> IsLockedOutAsync(TUser user);
        Task<IdentityResult> SetLockoutEnabledAsync(TUser user, bool enabled);
        Task<bool> GetLockoutEnabledAsync(TUser user);
        Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user);
        Task<IdentityResult> SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd);
        Task<IdentityResult> AccessFailedAsync(TUser user);
        Task<IdentityResult> ResetAccessFailedCountAsync(TUser user);
        Task<int> GetAccessFailedCountAsync(TUser user);
        Task<IList<TUser>> GetUsersForClaimAsync(Claim claim);
        Task<IList<TUser>> GetUsersInRoleAsync(string roleName);
        bool SupportsUserTwoFactor { get; }
        bool SupportsUserPassword { get; }
        bool SupportsUserSecurityStamp { get; }
        bool SupportsUserRole { get; }
        bool SupportsUserLogin { get; }
        bool SupportsUserEmail { get; }
        bool SupportsUserPhoneNumber { get; }
        bool SupportsUserClaim { get; }
        bool SupportsUserLockout { get; }
        bool SupportsQueryableUsers { get; }
        IQueryable<TUser> Users { get; }
        TUserView MapToUserView(TUser user);
        TUser CreateNewUser(string userName, string email, bool emailConfirmed);
    }
}
