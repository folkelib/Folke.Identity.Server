namespace Folke.Identity.Server.Services
{
    /// <summary>
    /// A service used to set and pull Folke Identity Server options
    /// </summary>
    public interface IIdentityServerOptionsService
    {
        bool IsRegistrationEnabled();
    }
}