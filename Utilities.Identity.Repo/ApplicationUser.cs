using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Utilities.Identity.Repo
{
    public class ApplicationUser :  IUser<string>
    {
        public ApplicationUser()
        {
            Roles = new List<string>();
        }


        public  async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager)
        {
            
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            Roles.ForEach((r) =>
            {
                userIdentity.AddClaim(new Claim(ClaimTypes.Role, r));
            });
            
            return userIdentity;
        }

        public int PersonId { get; set; }

        public string Id => this.UserName;
        public string UserName { get; set; }
        public string Email { get; set; }
        public string SocialId { get; set; }
        public List<string> Roles { get; set; }
        

        public string PasswordHash { get; set; }



    }
    
}
