using System.Linq;
using System.Security.Principal;

namespace SoniReports.DataAccessLayer
{
    public static class IPrincipalExt
    { 
        public static bool IsInAnyRole(this IPrincipal user, params string[] roles)
        {
            return roles.Any(user.IsInRole);
        }
    }
}
