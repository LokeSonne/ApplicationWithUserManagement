using System.Collections.Generic;

namespace SoniReports.DataAccessLayer
{
    public class AccessRule
    {
        public string RoleOfActingUser;
        public AdminActivity Activity;
        public IEnumerable<string> AllowedTargetRoles;
    
        public static AccessRule Create(string roleOfActingUser, AdminActivity activity, IEnumerable<string> allowedTargetRoles)
        {
            return new AccessRule { RoleOfActingUser = roleOfActingUser, Activity = activity, AllowedTargetRoles = allowedTargetRoles };
        }
    }
}
