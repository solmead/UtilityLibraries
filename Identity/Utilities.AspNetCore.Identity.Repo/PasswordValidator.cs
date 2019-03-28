using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Utilities.AspNetCore.Identity.Repo.Abstract;
using Utilities.AspNetCore.Identity.Repo.Models;

namespace Utilities.AspNetCore.Identity.Repo
{
    //public class UserValidator<TUser, TKey> : IUserValidator<TUser>
    //    where TUser : AppUser<TKey>
    //    where TKey : IEquatable<TKey>
    //{
    //    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    //    {

    //        throw new NotImplementedException();
    //    }
    //}


    public class PasswordValidator<TUser, TRole, TKey> : IPasswordValidator<TUser>
        where TUser : AppUser<TKey>
        where TRole : AppRole<TKey>
        where TKey : IEquatable<TKey>
    {

        private IIdentityRepository<TUser, TRole, TKey> _identityRepository;

        public PasswordValidator(IIdentityRepository<TUser, TRole, TKey> identityRepository)
        {
            _identityRepository = identityRepository;
        }


        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {

            var t = await _identityRepository.ValidateUserLoginAsync(user.UserName, password);
            return (t ? IdentityResult.Success : IdentityResult.Failed());

        }
    }

}
