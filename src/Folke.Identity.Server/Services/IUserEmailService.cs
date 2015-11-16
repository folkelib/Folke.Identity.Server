using System;
using System.Threading.Tasks;
using Elm.AspNet.Identity;

namespace Folke.Identity.Server.Services
{
    public interface IUserEmailService<in TUser, TKey>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TUser, TKey>
    {
        Task SendEmailConfirmationEmail(TUser user);
        Task SendPasswordResetEmail(TUser user);
    }
}
