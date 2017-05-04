using System;

namespace Folke.Identity.Server.Services
{
    public class IdentityServerOptionsService : IIdentityServerOptionsService
    {
        bool RegistrationEnabled { get; set; }
        public bool IsRegistrationEnabled()
        {
            return this.RegistrationEnabled;
        }
    }
}