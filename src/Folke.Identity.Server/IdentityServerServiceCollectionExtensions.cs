using System;
using Folke.Identity.Server;
using Folke.Identity.Server.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServer<TUser, TKey, TUserEmailService, TUserService, TUserView>(this IServiceCollection services, Action<IdentityServerOptions> options = null)
            where TUser : class
            where TUserView : class
            where TKey : IEquatable<TKey>
            where TUserService: class, IUserService<TUser, TUserView>
            where TUserEmailService : class, IUserEmailService<TUser>
        {
            if (options != null)
            {
                services.Configure(options);
            }

            services.AddScoped<IUserService<TUser, TUserView>, TUserService>();
            services.AddScoped<IUserEmailService<TUser>, TUserEmailService>();
            return services;
        }
        
        public static IServiceCollection AddRoleIdentityServer<TRole, TRoleService, TRoleView>(
            this IServiceCollection services)
            where TRole : class
            where TRoleView : class
            where TRoleService: class, IRoleService<TRole, TRoleView>
        {
            services.AddScoped<IRoleService<TRole, TRoleView>, TRoleService>();
            return services;
        }
    }
}
