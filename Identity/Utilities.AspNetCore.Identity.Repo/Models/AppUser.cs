using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Utilities.AspNetCore.Identity.Repo.Models
{
    //public class ApplicationUser : AppUser<string>
    //{

    //}


    public class AppUser<TKey> : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        public AppUser()
        {
            Roles = new List<string>();
            Claims = new List<IdentityUserClaim<TKey>>();
            TwoFactorTokens = new List<TwoFactorToken<TKey>>();
            SocialLogins = new List<IdentityUserLogin<TKey>>();

            SecurityStamp = Guid.NewGuid().ToString();
        }

        public AppUser(string userName) : this()
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }

        //public string Id { get; internal set; }
        //public string UserName { get; internal set; }
        
        //public string Email { get; set; }
        //public string SocialId { get; set; }

        [NotMapped]
        public IList<string> Roles { get; set; }


        [NotMapped]
        public IList<IdentityUserClaim<TKey>> Claims { get; set; }

        [NotMapped]
        public IList<TwoFactorToken<TKey>> TwoFactorTokens { get; set; }

        [NotMapped]
        public IList<IdentityUserLogin<TKey>> SocialLogins { get; set; }
        
        //public string LoginProvider { get; set; }
        //public string ProviderKey { get; set; }
        //public string PasswordHash { get; set; }


        public override string NormalizedEmail
        {
            get => Email.ToUpper();
            set
            {

            }
        }

        public override string NormalizedUserName => UserName.ToUpper();

    }
    
}
