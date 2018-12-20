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
        }

        public AppUser(string userName) : this()
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }

        //public string Id { get; internal set; }
        //public string UserName { get; internal set; }
        
        //public string Email { get; set; }
        public string SocialId { get; set; }
        public List<string> Roles { get; set; }
        

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
