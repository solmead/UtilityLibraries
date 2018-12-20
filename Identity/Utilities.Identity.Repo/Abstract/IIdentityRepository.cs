// <copyright file="IInstructorRepository.cs" company="University of Cincinnati">
// 	Copyright (c) 2012. All rights reserved.
// </copyright>
// <author>Bjorg Prodan</author>
// <summary>The IInstructorRepository.cs source file. Created: 05/31/2012 2:19 PM</summary> 

using System.Threading.Tasks;

namespace Utilities.Identity.Repo.Abstract
{
    /// <summary>
    /// The repository interface for the <see cref="Person"/> aggregate.
    /// </summary>
    public interface IIdentityRepository<TUser> where TUser:IdentityUser
    {
        
        Task<TUser> CreatePersonAsync(TUser p);
        Task<TUser> UpdatePersonAsync(TUser p);
        //Task SetPasswordAsync(string id, string password);
        Task SetPasswordAsync(string name, string password);
        Task<TUser> FindByIdAsync(string id);
        Task<TUser> FindByEmailAsync(string email);
        Task<TUser> FindByUsernameAsync(string username);
        Task<bool> ValidateUserAsync(string username, string password);

        Task<TUser> AddUserOnPersonAsync(string personId, string username, string password, string socialCode);





    }
}