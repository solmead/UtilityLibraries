using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Utilities.Identity.Repo
{
    public class IdentityUser
    {
        public IdentityUser()
        {
            Roles = new List<string>();
        }

        public IdentityUser(string userName) : this()
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }

        public string Id { get; internal set; }
        public string UserName { get; internal set; }
        //public string NormalizedUserName { get; internal set; }

        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser> manager)
        //{

        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    //manager.GetClaimsAsync()
        //    //var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        //    // Add custom user claims here

        //    Roles.ForEach((r) =>
        //    {
        //        manager.AddClaimAsync()
        //        userIdentity.AddClaim(new Claim(ClaimTypes.Role, r));
        //    });

        //    return userIdentity;
        //}

        public string Email { get; set; }
        public string SocialId { get; set; }
        public List<string> Roles { get; set; }
        

        public string PasswordHash { get; set; }



    }
    
}
