using System;
using Microsoft.Extensions.DependencyInjection;

namespace Folke.Identity.Server
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddIdentityServer<TKey, TUser, TUserView, TRole, TRoleView>(
            this IMvcBuilder builder)
             where TUser : class
            where TUserView : class
            where TKey : IEquatable<TKey>
            where TRole : class
            where TRoleView : class
        {
            var part = new IdentityServerApplicationPart<TKey, TUser, TUserView, TRole, TRoleView>();
            builder.ConfigureApplicationPartManager(manager => manager.ApplicationParts.Add(part));
            return builder;
        }
    }
}
