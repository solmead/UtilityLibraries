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

    //public interface IIdentityRepository : IIdentityRepository<IdentityUser<string>, IdentityRole<string>, string>
    //{

    //}

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


        Task DeletePersonAsync(TUser p);

        Task<TUser> FindAsync(TKey id);
        Task<TUser> FindUserByIdAsync(string id);
        Task<TUser> FindUserByEmailAsync(string email);
        Task<TUser> FindUserByUsernameAsync(string username);


        //Task SetPasswordAsync(string id, string password);
        Task SetPasswordAsync(string name, string password);
        Task<bool> ValidateUserLoginAsync(string username, string password);

        Task<TUser> AddUserLoginAsync(string personId, string username, string password, string socialCode);
        Task<TUser> GetUserLogin(string personId, string username, string socialCode);
        Task DeleteUserLogin(string personId, string username, string socialCode);



        IEnumerable<TRole> GetRoles();
        //Task<TRole> CreateRoleAsync(TRole p);
        //Task<TRole> UpdateRoleAsync(TRole p);

        //Task DeleteRoleAsync(TRole p);
        Task<TRole> FindRoleByIdAsync(string id);
        Task<TRole> FindRoleByNameAsync(string name);


    }
}