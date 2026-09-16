using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;
using System;
using System.Linq;
using System.Text.Json;

namespace eSSLAttendanceDesktopClient.Repository
{
    public class UserRepository : IUserRepository
    {
        public Domain.User GetBy(string emailAddress, string password)
        {
            var mUser = new Domain.User();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efUser = (from user in context.Users
                                  where user.ToDate == null
                                  select new
                                  {
                                      Id = user.Id,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      EmailAddress = user.EmailAddress,
                                      Password = user.Password,
                                      IsActive = user.IsActive
                                  }).AsEnumerable();

                    if (emailAddress.IsNotNullOrEmpty())
                    {
                        efUser = efUser.AsQueryable().Where(x => !(x.EmailAddress == null || x.EmailAddress == string.Empty) && x.EmailAddress.Trim().ToUpper() == emailAddress.Trim().ToUpper()).AsEnumerable();
                    }

                    if (password.IsNotNullOrEmpty())
                    {
                        efUser = efUser.AsQueryable().Where(x => !(x.Password == null || x.Password == string.Empty) && x.Password == password).AsEnumerable();
                    }

                    mUser = JsonSerializer.Deserialize<Domain.User>(JsonSerializer.Serialize(efUser.FirstOrDefault()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mUser;
        }
    }
}
