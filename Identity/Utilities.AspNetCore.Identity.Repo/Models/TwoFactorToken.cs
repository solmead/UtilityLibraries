using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Utilities.AspNetCore.Identity.Repo.Models
{
    public class TwoFactorToken<TKey> : IdentityUserToken<TKey>
        where TKey : IEquatable<TKey>
    {


        //public void test()
        //{
        //    var u = this.UserId;
        //    var q = this.LoginProvider;
        //    var q2 = this.Name;
        //    var q3 = this.Value;
        //    //this.
        //}
    }
}
