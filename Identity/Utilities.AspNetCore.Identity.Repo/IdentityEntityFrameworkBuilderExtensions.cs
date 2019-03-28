using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Utilities.AspNetCore.Identity.Repo.Abstract;
using Utilities.AspNetCore.Identity.Repo.Models;

namespace Utilities.AspNetCore.Identity.Repo
{
    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding entity framework stores.
    /// </summary>
    public static class IdentityEntityFrameworkBuilderExtensions
    {
        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores.
        /// </summary>
        /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddRepositoryStores(this IdentityBuilder builder)//, IIdentityRepository<AppUser<string>, AppRole<string>, string> context)
        {
            AddStores<AppUser<string>, AppRole<string>, string>(builder.Services, builder.UserType, builder.RoleType);//, context);
            return builder;
        }



        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores.
        /// </summary>
        /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddRepositoryStores<TUser, TRole, TKey>(this IdentityBuilder builder)//, IIdentityRepository<TUser, TRole, TKey> context)
            where TRole : AppRole<TKey>
            where TUser : AppUser<TKey>
            where TKey : IEquatable<TKey>
        {
            AddStores<TUser,TRole,TKey>(builder.Services, builder.UserType, builder.RoleType);//, context);
            return builder;
        }

        private static void AddStores<TUser, TRole, TKey>(IServiceCollection services, Type userType, Type roleType)//, IIdentityRepository<TUser, TRole, TKey> context)
            where TRole : AppRole<TKey>
            where TUser : AppUser<TKey>
            where TKey : IEquatable<TKey>
        {


            var identityUserType = FindGenericBaseType(userType, typeof(AppUser<>));
            if (identityUserType == null)
            {
                userType = typeof(TUser);
                //throw new InvalidOperationException(Resources.NotIdentityUser);
            }

            var keyType = userType.GenericTypeArguments[0];
            if (keyType == null)
            {
                keyType = typeof(TKey);
                //throw new InvalidOperationException(Resources.NotIdentityRole);
            }
            //if (roleType != null)
            //{
            var identityRoleType = FindGenericBaseType(roleType, typeof(AppRole<>));
                if (identityRoleType == null)
            {
                roleType = typeof(TRole);
                //throw new InvalidOperationException(Resources.NotIdentityRole);
            }

                Type userStoreType = null;
                Type roleStoreType = null;
                //var identityContext = FindGenericBaseType(contextType, typeof(IdentityDbContext<,,,,,,,>));
                //if (identityContext == null)
                //{
                    // If its a custom DbContext, we can only add the default POCOs
                    userStoreType = typeof(UserStore<,,>).MakeGenericType(userType, roleType, keyType);
                    roleStoreType = typeof(RoleStore<,,>).MakeGenericType(userType, roleType, keyType);
                //}
                //else
                //{
                //    userStoreType = typeof(UserStore<,,,,,,,,>).MakeGenericType(userType, roleType, contextType,
                //        identityContext.GenericTypeArguments[2],
                //        identityContext.GenericTypeArguments[3],
                //        identityContext.GenericTypeArguments[4],
                //        identityContext.GenericTypeArguments[5],
                //        identityContext.GenericTypeArguments[7],
                //        identityContext.GenericTypeArguments[6]);
                //    roleStoreType = typeof(RoleStore<,,,,>).MakeGenericType(roleType, contextType,
                //        identityContext.GenericTypeArguments[2],
                //        identityContext.GenericTypeArguments[4],
                //        identityContext.GenericTypeArguments[6]);
                //}
                services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
                services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
            //}
            //else
            //{   // No Roles
            //    Type userStoreType = null;
            //    //var identityContext = FindGenericBaseType(contextType, typeof(IdentityUserContext<,,,,>));
            //    //if (identityContext == null)
            //    //{
            //        // If its a custom DbContext, we can only add the default POCOs
            //        userStoreType = typeof(UserOnlyStore<,,>).MakeGenericType(userType, roleType, keyType);
            //    //}
            //    //else
            //    //{
            //    //    userStoreType = typeof(UserOnlyStore<,,,,,>).MakeGenericType(userType, contextType,
            //    //        identityContext.GenericTypeArguments[1],
            //    //        identityContext.GenericTypeArguments[2],
            //    //        identityContext.GenericTypeArguments[3],
            //    //        identityContext.GenericTypeArguments[4]);
            //    //}
            //    services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
            //}

        }

        private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType)
        {
            var type = currentType;
            while (type != null)
            {
                var typeInfo = type.GetTypeInfo();
                var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (genericType != null && genericType == genericBaseType)
                {
                    return typeInfo;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
