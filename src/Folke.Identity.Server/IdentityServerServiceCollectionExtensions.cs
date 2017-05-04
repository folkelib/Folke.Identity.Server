using System;
using System.Reflection;
using Folke.Identity.Server.Controllers;
using Folke.Identity.Server.Services;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServer<TUser, TKey, TUserEmailService, TUserService, TUserView>(this IServiceCollection services)
            where TUser : class
            where TUserView : class
            where TKey : IEquatable<TKey>
            where TUserService : class, IUserService<TUser, TUserView>
            where TUserEmailService : class, IUserEmailService<TUser>
        {
            services.AddScoped<IUserService<TUser, TUserView>, TUserService>();
            services.AddScoped<IUserEmailService<TUser>, TUserEmailService>();
            services.AddSingleton<IIdentityServerOptionsService, IdentityServerOptionsService>();
            return services;
        }

        public static IServiceCollection AddRoleIdentityServer<TRole, TRoleService, TRoleView>(
            this IServiceCollection services)
            where TRole : class
            where TRoleView : class
            where TRoleService : class, IRoleService<TRole, TRoleView>
        {
            services.AddScoped<IRoleService<TRole, TRoleView>, TRoleService>();
            return services;
        }
    }
}
