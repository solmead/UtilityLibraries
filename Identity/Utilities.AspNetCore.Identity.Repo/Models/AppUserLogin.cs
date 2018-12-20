using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Utilities.AspNetCore.Identity.Repo.Models
{
    public class AppUserLogin<TKey> : IdentityUserLogin<TKey>
        where TKey : IEquatable<TKey>
    {
    }
}
