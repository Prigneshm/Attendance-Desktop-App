using System.Threading.Tasks;

namespace eSSLAttendanceDesktopClient.Infrastructure.IService
{
    public interface IAuthenticationService
    {
        Task<Domain.User> Authenticate(Domain.Authentication mAuthentication);
    }
}
