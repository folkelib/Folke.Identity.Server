namespace Folke.Identity.Server.Services
{
    /// <summary>
    /// A service that is used by the role controller to manage the roles
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TRoleView"></typeparam>
    public interface IRoleService<TRole, out TRoleView> where TRole : class
    {
        /// <summary>
        /// Maps a role to a view that will be used to expose the roles to
        /// the web services.
        /// </summary>
        /// <param name="role">The role to map</param>
        /// <returns>The view</returns>
        TRoleView MapToRoleView(TRole role);

        /// <summary>
        /// Creates a new TRole object. The object will be saved in the
        /// database by the controller.
        /// </summary>
        /// <param name="name">The role name</param>
        /// <returns>The role</returns>
        TRole CreateNewRole(string name);
    }
}
