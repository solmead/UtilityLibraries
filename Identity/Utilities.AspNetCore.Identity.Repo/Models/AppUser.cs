using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Utilities.AspNetCore.Identity.Repo.Models
{
    public class AppUser<TKey> : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        public AppUser()
        {
            Roles = new List<string>();
            Claims = new Dictionary<string, string>();
            TwoFactorTokens = new List<TwoFactorToken<TKey>>();
        }

        public AppUser(string userName) : this()
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }

        //public string Id { get; internal set; }
        //public string UserName { get; internal set; }
        
        //public string Email { get; set; }
        //public string SocialId { get; set; }


        public List<string> Roles { get; set; }

        public Dictionary<string, string> Claims { get; set; }

        public List<TwoFactorToken<TKey>> TwoFactorTokens { get; set; }


        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
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
