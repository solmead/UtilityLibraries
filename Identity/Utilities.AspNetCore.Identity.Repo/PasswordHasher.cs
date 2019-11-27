using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.AspNetCore.Identity.Repo.Abstract;
using Utilities.AspNetCore.Identity.Repo.Models;

namespace Utilities.AspNetCore.Identity.Repo
{
    public class PasswordHasherRepo<TUser, TRole, TKey> : PasswordHasher<TUser>
        where TUser : AppUser<TKey>
        where TRole : AppRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IIdentityRepository<TUser, TRole, TKey> _context;

        public PasswordHasherRepo(IIdentityRepository<TUser, TRole, TKey> context)
        {
            _context = context;
        }

        public override string HashPassword(TUser user, string password)
        {
            return base.HashPassword(user, password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            var p = base.VerifyHashedPassword(user, hashedPassword, providedPassword);
            if (p == PasswordVerificationResult.Failed)
            {
                //p = _context.ValidateUserLogin(user.UserName, providedPassword);
            }



            return p;
        }


    }
}
