using System.Data.Entity.Validation;

namespace eSSLAttendanceDesktopClient.Infrastructure
{
    public static class DbEntityException
    {
        public static string GetValidationErrors(this DbEntityValidationException e)
        {
            string message = string.Empty;
            foreach (var eve in e.EntityValidationErrors)
            {
                message += System.Environment.NewLine;
                message += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:"
                                        , eve.Entry.Entity.GetType().Name
                                        , eve.Entry.State);

                foreach (var ve in eve.ValidationErrors)
                {
                    message += System.Environment.NewLine;
                    message += string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                }
            }
            return message;
        }
    }
}
