namespace eSSLAttendanceDesktopClient.Infrastructure.IRepository
{
    public interface IUserRepository
    {
        Domain.User GetBy(string emailAddress, string password);
    }
}
