using System.Threading.Tasks;

namespace Folke.Identity.Server.Services
{
    public interface IUserEmailService<in TUser>
        where TUser : class
    {
        Task SendEmailConfirmationEmail(TUser user, string code);
        Task SendPasswordResetEmail(TUser user, string code);
    }
}
