using System;
using Folke.Identity.Server.Services;
using Microsoft.AspNet.Identity;

namespace Microsoft.Framework.DependencyInjection
{
    public static class IdentityServerServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServer<TUser, TKey, TUserEmailService>(this IServiceCollection services)
            where TUser : class
            where TKey : IEquatable<TKey>
            where TUserEmailService: class, IUserEmailService<TUser>
        {
            return services.AddIdentityServer<TUser, TKey, TUserEmailService, UserService<TUser>>();
        }

        public static IServiceCollection AddIdentityServer<TUser, TKey, TUserEmailService, TAccountService>(this IServiceCollection services)
            where TUser : class
            where TKey: IEquatable<TKey>
            where TAccountService: UserManager<TUser>, IUserService<TUser>
            where TUserEmailService : class, IUserEmailService<TUser>
        {
            services.AddScoped<UserManager<TUser>, TAccountService>();
            services.AddScoped<IUserService<TUser>, TAccountService>();
            services.AddScoped<IUserSignInManager<TUser>, UserSignInManager<TUser>>();
            services.AddScoped<IUserEmailService<TUser>, TUserEmailService>();
            return services;
        }
    }
}
