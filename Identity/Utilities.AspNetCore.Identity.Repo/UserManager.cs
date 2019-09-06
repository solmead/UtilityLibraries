using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities.AspNetCore.Identity.Repo.Abstract;
using Utilities.AspNetCore.Identity.Repo.Models;

namespace Utilities.AspNetCore.Identity.Repo
{
    public class UserManager<TUser, TRole, TKey> : Microsoft.AspNetCore.Identity.UserManager<TUser>
        where TUser : AppUser<TKey>
        where TRole : AppRole<TKey>
        where TKey : IEquatable<TKey>
    {

        private readonly IIdentityRepository<TUser, TRole, TKey> _identityRepository;

        public UserManager(IUserStore<TUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger,
            IIdentityRepository<TUser, TRole, TKey> identityRepository) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _identityRepository = identityRepository;
        }



        public override async Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            var success = await base.CheckPasswordAsync(user, password);

            if (!success)
            {
                success = await _identityRepository.CheckPasswordAsync(user, password);
            }

            user.SecurityStamp = user.UserName;
            return success;
        }

    }
}
