using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using SoniReports.Controllers;
using SoniReports.DomainModel;

namespace SoniReports.DataAccessLayer
{
    public class RoleActivityAuthorizor
    {
        readonly ApplicationUserManager _userManager;
        readonly RoleAccess _roleAccess;
    
        public RoleActivityAuthorizor(ApplicationUserManager userManager, RoleAccess roleAccess)
        {
            _userManager = userManager;
            _roleAccess = roleAccess;
        }
    
    
        /// <returns>Returns true if the user is allowed to perform the activity on the target user. 
        /// The activity must be allowed for the user when all roles of the target user are taken into account.</returns>
        public bool Can(ApplicationUser user, AdminActivity activity, ApplicationUser targetUser)
        {
            var canAffectRoles = GetManageableRoles(user, activity);
            var targetUserRoles = _userManager.GetRoles(targetUser.Id);
            var result = targetUserRoles.All(tr => canAffectRoles.SingleOrDefault(ar => ar == tr) != null);
            return result;
        }
    
        /// <returns>Returns true if the user is allowed to perform the activity on the target user. 
        /// The activity must be allowed for the user when all roles of the target user are taken into account.</returns>
        public IEnumerable<ApplicationUser> Can(ApplicationUser user, AdminActivity activity, IEnumerable<ApplicationUser> targetUsers)
        {
            var canAffectRoles = GetManageableRoles(user, activity);
            return targetUsers.Where(u =>
                {
                    var targetUserRoles = _userManager.GetRoles(u.Id);
                    var result = targetUserRoles.All(tr => canAffectRoles.SingleOrDefault(ar => ar == tr) != null);
                    return result;
                });
        }
    
        /// <returns>Returns true if the user is allowed to perform the activity on the target user. 
        /// The activity must be allowed for the user when all roles of the target user are taken into account.</returns>
        public IEnumerable<T> Can<T>(ApplicationUser user, AdminActivity activity, IEnumerable<T> targets, Func<T, string> userIdGetter)
        {
            var canAffectRoles = GetManageableRoles(user, activity);
            return targets.Where(t =>
            {
                var id = userIdGetter(t);
                var targetUserRoles = _userManager.GetRoles(id);
                var result = targetUserRoles.All(tr => canAffectRoles.SingleOrDefault(ar => ar == tr) != null);
                return result;
            });
        }
    
        public IEnumerable<string> GetManageableRoles(ApplicationUser user, AdminActivity activity)
        {
            var userRoles = _userManager.GetRoles(user.Id);
            var allRoles = _roleAccess.GetRoles().Select(r => r.Name);
            var result = RoleActivityRightsTable.UserCanAffect(userRoles, allRoles, activity);
            return result;
        }
    
    
    }
}
