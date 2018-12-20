using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Utilities.Identity.Repo.Abstract;

namespace Utilities.Identity.Repo
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {

        public static IDataProtectionProvider DataProtectionProvider { get; set; }
        //private static readonly byte[] KEY_64 = { 12, 16, 93, 156, 90, 4, 218, 32 };

        //private static readonly byte[] IV_64 = {2, 5, 246, 79, 36, 32, 167, 3};

        public ApplicationUserManager(IUserStore<ApplicationUser> store,
            IEmailHandler emailHandler)
            : base(store)
        {
            var provider = DataProtectionProvider; //new DpapiDataProtectionProvider("FileManager");
            //var dataProtectionProvider = DataProtectionProvider;

            UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
    provider.Create("EmailConfirmation"));
            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = false;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            EmailService = new EmailService(emailHandler);

        }

        //private static string encrypt(string value)
        //{
        //    if (value != "")
        //    {
        //        var cryptoProvider = new DESCryptoServiceProvider();
        //        var ms = new MemoryStream();
        //        var cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);

        //        var sw = new StreamWriter(cs);
        //        sw.Write(value);
        //        sw.Flush();
        //        cs.FlushFinalBlock();
        //        ms.Flush();
        //        return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        //    }
        //    return "";
        //}
        public override async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var identityRepo = Store as IIdentityRepository<ApplicationUser>;
            if (identityRepo == null)
            {
                throw new System.Exception("Store must be a IIdentityRepository");
            }

            var check = await identityRepo.ValidateUserAsync(user.UserName, password);
            if (!check)
            {
                //password = encrypt(password);
                //check = await identityRepo.ValidateUser(user.EmailAddress, password);
            }
            return check;


            //return await base.CheckPasswordAsync(user, password);
        }

        //public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context, IPersonRepository personRepository,
        //    ISessionInfo sessionInfo)
        //{
        //    IUserStore<ApplicationUser, string> st = null;
        //    st = new MyUserStore(personRepository, sessionInfo);


        //    var manager = new ApplicationUserManager(st);
        //    // Configure validation logic for usernames
        //    manager.UserValidator = new UserValidator<ApplicationUser, string>(manager)
        //    {
        //        AllowOnlyAlphanumericUserNames = false,
        //        RequireUniqueEmail = true
        //    };

        //    // Configure validation logic for passwords
        //    manager.PasswordValidator = new PasswordValidator
        //    {
        //        RequiredLength = 6,
        //        RequireNonLetterOrDigit = true,
        //        RequireDigit = true,
        //        RequireLowercase = true,
        //        RequireUppercase = true,
        //    };

        //    // Configure user lockout defaults
        //    manager.UserLockoutEnabledByDefault = true;
        //    manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //    manager.MaxFailedAccessAttemptsBeforeLockout = 5;


        //    var dataProtectionProvider = options.DataProtectionProvider;
        //    if (dataProtectionProvider != null)
        //    {
        //        manager.UserTokenProvider =
        //            new DataProtectorTokenProvider<ApplicationUser, string>(dataProtectionProvider.Create("ASP.NET Identity"));
        //    }
        //    return manager;
        //}
    }
}
