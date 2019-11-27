using System.Threading.Tasks;
using Utilities.Identity.Repo.Abstract;

namespace Utilities.Identity.Repo
{
    public class EmailService : IIdentityMessageService
    {
        private readonly IEmailHandler _emailHandler;
        public EmailService(IEmailHandler emailHandler)
        {
            _emailHandler = emailHandler;
        }

        public Task SendAsync(IdentityMessage message)
        {
            return _emailHandler.SendEmailAsync(message.Destination, message.Subject, message.Body);
        }
    }
}
