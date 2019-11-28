// <copyright file="IInstructorRepository.cs" company="University of Cincinnati">
// 	Copyright (c) 2012. All rights reserved.
// </copyright>
// <author>Bjorg Prodan</author>
// <summary>The IInstructorRepository.cs source file. Created: 05/31/2012 2:19 PM</summary> 

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Utilities.AspNetCore.Identity.Repo.Models;

namespace Utilities.AspNetCore.Identity.Repo.Abstract
{

    //public interface IRoleRepository<TRole, TKey>
    //    where TRole : IdentityRole<TKey>
    //    where TKey : IEquatable<TKey>
    //{

    //    //Task<TUser> CreatePersonAsync(TUser p);
    //    //Task<TUser> UpdatePersonAsync(TUser p);
    //    ////Task SetPasswordAsync(string id, string password);
    //    //Task SetPasswordAsync(string name, string password);
    //    //Task<TUser> FindByIdAsync(string id);
    //    //Task<TUser> FindByEmailAsync(string email);
    //    //Task<TUser> FindByUsernameAsync(string username);
    //    //Task<bool> ValidateUserAsync(string username, string password);

    //    //Task<TUser> AddUserOnPersonAsync(string personId, string username, string password, string socialCode);





    //}

    public interface IIdentityRepository : IIdentityRepository<AppUser<string>, AppRole<string>, string>
    {

    }

    /// <summary>
    /// The repository interface for the <see cref="Person"/> aggregate.
    /// </summary>
    public interface IIdentityRepository<TUser, TRole, TKey>
        where TRole : AppRole<TKey>
        where TUser : AppUser<TKey>
        where TKey : IEquatable<TKey>
    {

        Task<TUser> CreateUserAsync(TUser p);
        Task<TUser> UpdateUserAsync(TUser p);
        Task DeleteUserAsync(TUser p);


        //IEnumerable<TUser> GetUsers();

        Task<TUser> FindUserByIdAsync(TKey id);
        Task<TUser> FindUserByEmailAsync(string email);
        Task<TUser> FindUserByUsernameAsync(string username);


        //Task SetPasswordAsync(string id, string password);
        Task SetPasswordAsync(TUser user, string password);
        Task<bool> CheckPasswordAsync(TUser user, string password);


        Task<TUser> AddUserLoginAsync(TUser user, string loginProvider, string providerKey);
        //Task<IEnumerable<TUser>> GetUserLoginsAsync(TKey personId);
        Task<TUser> FindUserByUserLoginAsync(string loginProvider, string providerKey);
        Task<TUser> DeleteUserLoginAsync(TUser user, string loginProvider, string providerKey);



        //IEnumerable<TRole> GetRoles();
        Task<TRole> FindRoleByIdAsync(TKey id);
        Task<TRole> FindRoleByNameAsync(string name);


    }
}