using System;
using Folke.Identity.Server.Services;
using Microsoft.AspNet.Identity;

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
    }
}
