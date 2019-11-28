using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Identity.Repo.Abstract;
using Utilities.Identity.Repo.Provider;

namespace Utilities.Identity.Repo
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder UseRepositoryAdaptor<TUser>(
            this IdentityBuilder builder,
            IIdentityRepository<IdentityUser> identityRepository
        ) => builder
            .AddRepositoryUserStore(identityRepository);
                //.AddRepositoryRoleStore(identityRepository);

        private static IdentityBuilder AddRepositoryUserStore(
            this IdentityBuilder builder,
            IIdentityRepository<IdentityUser> identityRepository
        )
        {
            var userStoreType = typeof(UserStore<>).MakeGenericType(builder.UserType);

            builder.Services.AddScoped(
                typeof(IUserStore<>).MakeGenericType(builder.UserType),
                userStoreType
            );

            return builder;
        }

        private static IdentityBuilder AddRepositoryRoleStore(
            this IdentityBuilder builder,
            IIdentityRepository<IdentityUser> identityRepository
        )
        {
            var roleStoreType = typeof(RoleStore<>).MakeGenericType(builder.RoleType);

            builder.Services.AddScoped(
                typeof(IRoleStore<>).MakeGenericType(builder.RoleType),
                roleStoreType
            );

            return builder;
        }
    }
}
