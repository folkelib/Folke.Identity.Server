namespace Folke.Identity.Server.Services
{
    public interface IRoleService<TRole, TRoleView> where TRole : class
    {
        TRoleView MapToRoleView(TRole user);
        TRole CreateNewRole(TRoleView view);
    }
}
