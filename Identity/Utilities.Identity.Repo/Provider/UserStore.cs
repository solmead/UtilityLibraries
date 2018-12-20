using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Utilities.Identity.Repo.Abstract;

namespace Utilities.Identity.Repo.Provider
{
    public class UserStore<TUser> : IUserStore<TUser>
    where TUser : IdentityUser
    {

        private readonly IIdentityRepository<TUser> _personRepository;

        public UserStore(IIdentityRepository<TUser> personRepository)
        {
            _personRepository = personRepository;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.Id;
        }

        public async Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.UserName;
        }

        public async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
        }

        public async Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return user.UserName;
        }

        public async Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            //user.UserName = userName;
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            var person = await _personRepository.FindByUsernameAsync(user.UserName);
            if (person == null)
            {
                person = await _personRepository.CreatePersonAsync(user);
                var p2 = await _personRepository.FindByUsernameAsync(user.UserName);
                if (p2 == null)
                {
                    await _personRepository.AddUserOnPersonAsync(person.Id, user.UserName, Guid.NewGuid().ToString(), "EXT");
                }
                person = await _personRepository.FindByUsernameAsync(user.UserName);
            }

            var tst = user.SocialId?.Split('@');
            var iCode = "EXT";
            if ((tst?.Length ?? 0) > 1)
            {
                iCode = tst[1];
            }

            await _personRepository.AddUserOnPersonAsync(person.Id, user.SocialId, Guid.NewGuid().ToString(), iCode);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return IdentityResult.Failed();
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _personRepository.FindByIdAsync(userId);
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _personRepository.FindByUsernameAsync(normalizedUserName);
        }
    }
}
