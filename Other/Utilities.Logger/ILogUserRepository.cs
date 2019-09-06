namespace Utilities.Logging
{
    public interface ILogUserRepository
    {
        string CurrentUserName();
        string UserHostAddress();

        string MapPath(string path);
    }
}
