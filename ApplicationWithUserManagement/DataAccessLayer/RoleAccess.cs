using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Threading.Tasks;


namespace SoniReports.DataAccessLayer
{
    public class RoleAccess
    {
        public IEnumerable<IdentityRole> GetRoles()
        {
            var result = ApplicationDbContext.DbAction(db => db.Roles.OrderBy(r => r.Name).ToList());
            return result;
        }


        public IdentityRole GetRole(string name)
        {
            var result = ApplicationDbContext.DbAction(db => db.Roles.SingleOrDefault(r => r.Name == name));
            return result;
        }

        internal static IEnumerable<IdentityUserRole> GetAllUsersWithRole(string roleName)
        {
            var result = ApplicationDbContext.DbAction(db => 
                {
                    var role = db.Roles.SingleOrDefault(r => r.Name == roleName);
                    if (role == null) return null;
                    var userRoles = role.Users.ToList();
                    return userRoles;
                });
            return result;
        }
    }
}