using System;
using System.Reflection;
using Folke.Identity.Server.Controllers;
using Folke.Identity.Server.Services;
using Folke.Mvc.Extensions;
using Microsoft.AspNet.Mvc.Controllers;

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

        public static IServiceCollection AddIdentityServerControllers<TKey, TUser, TUserView, TRole, TRoleView>(this IServiceCollection services)
            where TUser : class
            where TUserView : class
            where TKey : IEquatable<TKey>
            where TRole : class
            where TRoleView : class
        {
            var controllerTypeProvider = new StaticControllerTypeProvider();
            var types = new[]
            {
                typeof (AuthenticationController<TUser, TKey, TUserView>),
                typeof (RoleController<TRole, TRoleView, TKey, TUser>),
                typeof (AccountController<TUser, TUserView, TKey>),
            };
            foreach (var type in types)
            {
                services.AddTransient(type);
                controllerTypeProvider.ControllerTypes.Add(type.GetTypeInfo());
            }

            services.AddInstance<IControllerTypeProvider>(controllerTypeProvider);
            services.AddTransient<IControllerTypeProvider, ControllerTypeMerger>();
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
