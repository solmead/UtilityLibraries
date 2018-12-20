using System.Threading.Tasks;

namespace Utilities.Identity.Repo.Abstract
{
    public interface IEmailHandler
    {
        Task SendEmailAsync(string to, string subject, string body);

    }
}
