using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Utilities.Identity.Repo
{
    public static class IdentityExtensions
    {
        public static string FindClaimValue(this IEnumerable<Claim> claims, string name)
        {

            return (from c in claims where c.Type.Contains(name) select c.Value).FirstOrDefault();
        }
        public static DateTime? FindClaimValueDate(this IEnumerable<Claim> claims, string name)
        {
            DateTime finDt;
            var dt = (from c in claims where c.Type.Contains(name) select c.Value).FirstOrDefault();
            if (!DateTime.TryParse(dt, out finDt))
            {
                return null;
            }
            return finDt;

        }
        public static int FindClaimValueInt(this IEnumerable<Claim> claims, string name)
        {

            return 0;
        }
        public static ApplicationUser AsApplicationUser(this ExternalLoginInfo guest)
        {
            var nm = guest.Login.ProviderKey + "@" + guest.Login.LoginProvider;
            var email = guest.Email ?? guest.ExternalIdentity.Claims.FindClaimValue("email");

            nm = email;
            if (guest.Login.LoginProvider == "UC Login")
            {
                nm = guest.Login.ProviderKey;
            }

            return new ApplicationUser
            {
                PersonId = 0,
                //BirthDate = guest.ExternalIdentity.Claims.FindClaimValueDate("birthdate"),
                Email = email,
                //FirstName = guest.ExternalIdentity.Claims.FindClaimValue("givenname"),
                //LastName = guest.ExternalIdentity.Claims.FindClaimValue("surname"),
                //MiddleName = "",
                UserName = nm,
                SocialId = guest.Login.ProviderKey + "@" + guest.Login.LoginProvider
            };
        }
    }
}
