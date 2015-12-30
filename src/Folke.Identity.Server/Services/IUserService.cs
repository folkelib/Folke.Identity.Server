using System.Threading.Tasks;

namespace Folke.Identity.Server.Services
{
    public interface IUserService<TUser, TUserView>
        where TUser : class
        where TUserView : class
    {
        /// <summary>Maps a user to an user view</summary>
        /// <param name="user">The user to map</param>
        /// <returns>A view that may be returned by a service</returns>
        TUserView MapToUserView(TUser user);

        /// <summary>Creates a new user object. The user is not stored in the user base. Call
        /// UserManager.CreateAsync method to save it.</summary>
        /// <param name="userName">The user name</param>
        /// <param name="email">The user e-mail</param>
        /// <param name="emailConfirmed">If the e-mail is confirmed</param>
        /// <returns>The new user.</returns>
        TUser CreateNewUser(string userName, string email, bool emailConfirmed);

        /// <summary>Gets the current user</summary>
        /// <returns>The current user</returns>
        Task<TUser> GetCurrentUserAsync();
    }
}
