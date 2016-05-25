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
            where TUserService: class, IUserService<TUser, TUserView>
            where TUserEmailService : class, IUserEmailService<TUser>
        {
            services.AddScoped<IUserService<TUser, TUserView>, TUserService>();
            services.AddScoped<IUserEmailService<TUser>, TUserEmailService>();
            return services;
        }
        
        public static IMvcBuilder AddIdentityServerControllers<TKey, TUser, TUserView, TRole, TRoleView>(
            this IMvcBuilder builder)
             where TUser : class
            where TUserView : class
            where TKey : IEquatable<TKey>
            where TRole : class
            where TRoleView : class
        {
            var feature = new ControllerFeature();
            feature.Controllers.Add(typeof (AuthenticationController<TUser, TKey, TUserView>).GetTypeInfo());
            feature.Controllers.Add(typeof(RoleController<TRole, TRoleView, TKey, TUser>).GetTypeInfo());
            feature.Controllers.Add(typeof(AccountController<TUser, TUserView, TKey>).GetTypeInfo());

            builder.ConfigureApplicationPartManager(manager => manager.PopulateFeature(feature));
            return builder;
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
