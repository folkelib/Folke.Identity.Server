using System;
using Folke.Identity.Server.Services;
using Microsoft.AspNet.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServer<TUser, TKey, TUserEmailService, TAccountService, TUserView>(this IServiceCollection services)
            where TUser : class
            where TUserView : class
            where TKey : IEquatable<TKey>
            where TAccountService: UserManager<TUser>, IUserService<TUser, TUserView>
            where TUserEmailService : class, IUserEmailService<TUser>
        {
            services.AddScoped<UserManager<TUser>, TAccountService>();
            services.AddScoped<IUserService<TUser, TUserView>, TAccountService>();
            services.AddScoped<IUserSignInManager<TUser>, UserSignInManager<TUser>>();
            services.AddScoped<IUserEmailService<TUser>, TUserEmailService>();
            return services;
        }
    }
}
