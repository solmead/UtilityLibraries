using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Utilities.Identity.Repo.Abstract;

namespace Utilities.Identity.Repo
{
    public class MyUserStore<TUser>: IUserStore<TUser>, 
                                IUserLoginStore<TUser>,
                                IUserEmailStore<TUser>,
                                IUserLockoutStore<TUser>,
                                IUserTwoFactorStore<TUser>,
                                IUserPasswordStore<TUser>,
                                IIdentityRepository<TUser>
        where TUser: IdentityUser
    {

        private readonly IIdentityRepository<TUser> _personRepository;

        public MyUserStore(IIdentityRepository<TUser> personRepository)
        {
            _personRepository = personRepository;
        }


        public void Dispose()
        {
        }

        #region IUserStore
        public async Task CreateAsync(TUser user)
        {
            var person = await _personRepository.FindByUsernameAsync(user.UserName);
            if (person == null)
            {
                person = await _personRepository.CreatePersonAsync(user);
                var p2 = await _personRepository.FindByUsernameAsync(user.UserName);
                if (p2 == null)
                {
                    await _personRepository.AddUserOnPersonAsync(person.PersonId, user.UserName, Guid.NewGuid().ToString(), "EXT");
                }
                person = await _personRepository.FindByUsernameAsync(user.UserName);
            }

            var tst = user.SocialId?.Split('@');
            var iCode = "EXT";
            if ((tst?.Length ?? 0) > 1)
            {
                iCode = tst[1];
            }

            await _personRepository.AddUserOnPersonAsync(person.PersonId, user.SocialId, Guid.NewGuid().ToString(), iCode);

        }

        public async Task UpdateAsync(TUser user)
        {
            return;
        }

        public async Task DeleteAsync(TUser user)
        {
            return;
        }

        public async Task<TUser> FindByIdAsync(string userId)
        {
            return await _personRepository.FindByUsernameAsync(userId);
        }

        public async Task<TUser> FindByNameAsync(string userName)
        {
            

            return await _personRepository.FindByUsernameAsync(userName);
        }

#endregion


        #region IUserLoginStore

        public async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            return;
        }

        public async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            return;
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return null;
        }

        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            var nm = login.ProviderKey + "@" + login.LoginProvider;
            if (login.LoginProvider == "UC Login")
            {
                nm = login.ProviderKey;
            }
            return await _personRepository.FindByUsernameAsync(nm);
        }
        #endregion



        #region IUserEmailStore
        public async Task SetEmailAsync(TUser user, string email)
        {
            return;
        }

        public async Task<string> GetEmailAsync(TUser user)
        {
            return user.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return true;
        }

        public async Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            return;
        }

        public async Task<TUser> FindByEmailAsync(string email)
        {
            return await _personRepository.FindByEmailAsync(email);
        }
        #endregion




        #region IUserLockoutStore
        public async Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return new DateTimeOffset(DateTime.Now);
        }

        public async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            return;
        }

        public async Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return 0;
        }

        public async Task ResetAccessFailedCountAsync(TUser user)
        {
            return;
        }
        
        public async Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return 0;
        }

        public async Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return false;
        }

        public async Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            return;
        }
        #endregion



        #region IUserTwoFactorStore
        public async Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            return;
        }

        public async Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return false;
        }
        #endregion

        public Task<TUser> CreatePersonAsync(TUser p)
        {
            return _personRepository.CreatePersonAsync(p);
        }

        public Task SetPasswordAsync(int id, string password)
        {
            return _personRepository.SetPasswordAsync(id, password);
        }

        public Task SetPasswordAsync(string name, string password)
        {
            return null;
        }

        public Task<TUser> FindByIdAsync(int id)
        {
            return _personRepository.FindByIdAsync(id);
        }

        //public Task<TUser> FindByEmailAsync(string email)
        //{
        //    return _personRepository.FindByEmailAsync(email);
        //}
        public Task<TUser> FindByUsernameAsync(string username)
        {
            return _personRepository.FindByUsernameAsync(username);
        }

        public Task<bool> ValidateUserAsync(string username, string password)
        {
            return _personRepository.ValidateUserAsync(username, password);
        }

        public Task<TUser> AddUserOnPersonAsync(int personId, string username, string password, string socialCode)
        {
            return _personRepository.AddUserOnPersonAsync(personId, username, password, socialCode);
        }

        public async Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            await _personRepository.SetPasswordAsync(user.Id, passwordHash);
        }

        public async Task<string> GetPasswordHashAsync(TUser user)
        {
            var p = await _personRepository.FindByUsernameAsync(user.Id);
            return p.PasswordHash;
        }

        public async Task<bool> HasPasswordAsync(TUser user)
        {
            var p = await _personRepository.FindByUsernameAsync(user.Id);
            return !string.IsNullOrWhiteSpace( p.PasswordHash);
        }
    }
}
