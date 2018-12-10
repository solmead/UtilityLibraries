namespace Utilities.Logging
{
    public interface ILogUserRepository
    {
        string CurrentUserName();
        string UserHostAddress();
    }
}
