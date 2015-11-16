using System;
using Elm.AspNet.Identity;
using Folke.Identity.Server.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;

namespace Folke.Identity.Server
{
    public static class IdentityServerServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServer<TUser, TKey, TUserEmailService>(this IServiceCollection services)
            where TUser : IdentityUser<TUser, TKey>, new()
            where TKey : IEquatable<TKey>
            where TUserEmailService: class, IUserEmailService<TUser, TKey>
        {
            return services.AddIdentityServer<TUser, TKey, TUserEmailService, UserService<TUser>>();
        }

        public static IServiceCollection AddIdentityServer<TUser, TKey, TUserEmailService, TAccountService>(this IServiceCollection services)
            where TUser : IdentityUser<TUser, TKey>, new()
            where TKey: IEquatable<TKey>
            where TAccountService: UserManager<TUser>, IUserService<TUser>
            where TUserEmailService : class, IUserEmailService<TUser, TKey>
        {
            services.AddScoped<UserManager<TUser>, TAccountService>();
            services.AddScoped<IUserService<TUser>, TAccountService>();
            services.AddScoped<IUserStore<TUser>, UserStore<TUser, TKey>>();
            services.AddScoped<IRoleStore<IdentityRole<TKey>>, RoleStore<IdentityRole<TKey>, TKey>>();
            services.AddScoped<IUserSignInManager<TUser>, UserSignInManager<TUser>>();
            services.AddScoped<IUserEmailService<TUser, TKey>, TUserEmailService>();
            return services;
        }
    }
}
