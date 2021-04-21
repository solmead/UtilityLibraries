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


        //public static void AddRepositoryStores<TUser, TRole, TKey, identityRepository>(this IServiceCollection services)
        //    where TRole : AppRole<TKey>
        //    where TUser : AppUser<TKey>
        //    where TKey : IEquatable<TKey>
        //    where identityRepository : class, IIdentityRepository<TUser, TRole, TKey>
        //{



        //    //services.AddScoped<IPasswordHasher<AppUser<string>>, PasswordHasherRepo<AppUser<string>, AppRole<string>, string>>();


        //    services.AddScoped<IIdentityRepository<TUser, TRole, TKey>, identityRepository>();


        //    AddStores<TUser, TRole, TKey>(services, typeof(TUser), typeof(TRole));
        //}

        //public static void AddRepositoryStores<identityRepository>(this IServiceCollection services)
        //    where identityRepository : class, IIdentityRepository<AppUser<string>, AppRole<string>, string>
        //{

        //    AddRepositoryStores<AppUser<string>, AppRole<string>, string, identityRepository>(services);

        //}

        /// <summary>
        /// Adds an Repository based implementation of identity information stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddRepositoryStores(this IdentityBuilder builder)
        {
            return builder.AddRepositoryStores<AppUser<string>, AppRole<string>, string>();
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
            builder = builder.AddUserManager<UserManager<TUser, TRole, TKey>>();
           // builder = builder.AddRoleManager<RoleManager<TRole>>();
            //builder = builder.AddSignInManager<SignInManager<TUser>>();
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
            


            var userStoreType = typeof(UserStore<,,>).MakeGenericType(userType, roleType, keyType);
            var roleStoreType = typeof(RoleStore<,,>).MakeGenericType(userType, roleType, keyType);

            services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
            services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);

            //services.TryAddScoped(typeof(IUserManager<>).MakeGenericType(userType), userStoreType);

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
