using System;
using System.Collections.Generic;
using System.Reflection;
using Folke.Identity.Server.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Folke.Identity.Server
{
    public class IdentityServerApplicationPart<TKey, TUser, TUserView, TRole, TRoleView> : ApplicationPart, IApplicationPartTypeProvider
        where TUser : class
            where TUserView : class
            where TKey : IEquatable<TKey>
            where TRole : class
            where TRoleView : class
    {
        public override string Name => "Folke.Identity.Server";

        private readonly List<TypeInfo> types = new List<TypeInfo>
        {
            typeof(AuthenticationController<TUser, TKey, TUserView>).GetTypeInfo(),
            typeof(RoleController<TRole, TRoleView, TKey, TUser>).GetTypeInfo(),
            typeof(AccountController<TUser, TUserView, TKey>).GetTypeInfo()
        };

        public IEnumerable<TypeInfo> Types => types;
    }
}
