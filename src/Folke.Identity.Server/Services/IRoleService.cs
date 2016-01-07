namespace Folke.Identity.Server.Services
{
    public interface IRoleService<TRole, out TRoleView> where TRole : class
    {
        TRoleView MapToRoleView(TRole role);
        TRole CreateNewRole(string name);
    }
}
