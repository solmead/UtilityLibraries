using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Utilities.AspNetCore.Identity.Repo.Models
{
    public class AppRole<TKey> : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
    }
}
