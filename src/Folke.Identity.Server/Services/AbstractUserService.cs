using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;

namespace Folke.Identity.Server.Services
{
    public abstract class AbstractUserService<TUser, TUserView> : UserManager<TUser>, IUserService<TUser, TUserView>
        where TUser : class
        where TUserView: class
    {
        public AbstractUserService(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger, IHttpContextAccessor contextAccessor) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger, contextAccessor)
        {
        }

        public abstract TUserView MapToUserView(TUser user);

        public abstract TUser CreateNewUser(string userName, string email, bool emailConfirmed);
    }
}
