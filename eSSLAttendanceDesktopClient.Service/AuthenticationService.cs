using System.Threading.Tasks;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Repository;

namespace eSSLAttendanceDesktopClient.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepo;

        public AuthenticationService()
        {
            _userRepo = new UserRepository();
        }

        public Task<Domain.User> Authenticate(Domain.Authentication mAuthentication)
        {
            if (string.IsNullOrWhiteSpace(mAuthentication?.EmailAddress) || string.IsNullOrWhiteSpace(mAuthentication?.Password))
                throw new BadRequest("Email address and password are required.");

            var mUser = _userRepo.GetBy(mAuthentication.EmailAddress, mAuthentication.Password);

            if (mUser == null || mUser.Id <= 0)
                throw new BadRequest("Invalid login details.");

            return Task.FromResult(mUser);
        }
    }
}
