using System.Collections.Generic;
using System.Linq;
using SoniReports.DomainModel;
using System.Data.Entity;


namespace SoniReports.DataAccessLayer
{
    public class LoggingAccess
    {
        public IEnumerable<UserActivityLogEntry> GetUserActivityLog()
        {
            var result = ApplicationDbContext.DbAction(db => db.UserActivityLog.ToList());
            return result;
        }

        public int Register(UserActivityLogEntry entry)
        {
            var result = ApplicationDbContext.DbAction(db =>
            {
                db.UserActivityLog.Add(entry);
                return db.SaveChanges();
            });
            return result;
        }
    }
}