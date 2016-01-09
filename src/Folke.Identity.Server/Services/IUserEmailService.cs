using System.Threading.Tasks;

namespace Folke.Identity.Server.Services
{
    /// <summary>
    /// An interface to a service that sends emails.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public interface IUserEmailService<in TUser>
        where TUser : class
    {
        /// <summary>
        /// Sends an e-mail address confirmation e-mail. Called when a
        /// new user is registered using an e-mail or when the user change his/her
        /// e-mail.
        /// </summary>
        /// <param name="user">The user that changes his/her e-mail</param>
        /// <param name="code">The confirmation code (should be printed in the e-mail)</param>
        /// <returns></returns>
        Task SendEmailConfirmationEmail(TUser user, string code);

        /// <summary>
        /// Sends an e-mail when the user asks to reset his/her password because
        /// he/she forgot it.
        /// </summary>
        /// <param name="user">The user that wants to reset his/her password</param>
        /// <param name="code">The code that must be used to reset the password</param>
        /// <returns></returns>
        Task SendPasswordResetEmail(TUser user, string code);
    }
}
