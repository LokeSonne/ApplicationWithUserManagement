using System.Collections.Generic;
using System.Linq;
using SoniReports.Controllers;

namespace SoniReports.DataAccessLayer
{
    public enum AdminActivity { SeeUserInRole, EditUserInRole, DeleteUserInRole, SeeAndAssignRole, SetUserPassword, ActivateUser, AssignGroup, ViewGroup }

    public static class RoleActivityRightsTable
    {
        private static List<AccessRule> _rules;
        private static List<AccessRule> Rules
        {
            get
            {
                if (_rules == null)
                {
                    _rules = new List<AccessRule> {
                        AccessRule.Create(SoniRoles.Admin, AdminActivity.ActivateUser, SoniRoles.AllExceptManagers),
                        AccessRule.Create(SoniRoles.Admin, AdminActivity.AssignGroup, SoniRoles.AllExceptSuperUser),
                        AccessRule.Create(SoniRoles.Admin, AdminActivity.DeleteUserInRole, SoniRoles.AllExceptManagers), 
                        AccessRule.Create(SoniRoles.Admin, AdminActivity.EditUserInRole, SoniRoles.AllExceptSuperUser),
                        AccessRule.Create(SoniRoles.Admin, AdminActivity.SeeAndAssignRole, SoniRoles.AllExceptManagers),
                        AccessRule.Create(SoniRoles.Admin, AdminActivity.SeeUserInRole, SoniRoles.AllExceptSuperUser),
                        AccessRule.Create(SoniRoles.Admin, AdminActivity.ViewGroup, SoniRoles.AllExceptSuperUser),

                        AccessRule.Create(SoniRoles.SuperUser, AdminActivity.ActivateUser, SoniRoles.All),
                        AccessRule.Create(SoniRoles.SuperUser, AdminActivity.AssignGroup, SoniRoles.All),
                        AccessRule.Create(SoniRoles.SuperUser, AdminActivity.DeleteUserInRole, SoniRoles.All),
                        AccessRule.Create(SoniRoles.SuperUser, AdminActivity.EditUserInRole, SoniRoles.All),
                        AccessRule.Create(SoniRoles.SuperUser, AdminActivity.SeeAndAssignRole, SoniRoles.All),
                        AccessRule.Create(SoniRoles.SuperUser, AdminActivity.SeeUserInRole, SoniRoles.All),
                        AccessRule.Create(SoniRoles.SuperUser, AdminActivity.SetUserPassword, SoniRoles.All),
                        AccessRule.Create(SoniRoles.SuperUser, AdminActivity.ViewGroup, SoniRoles.All)
                    };
                }
                return _rules;
            }
        }

       
        public static IEnumerable<string> UserCanAffect(IEnumerable<string> userRoles, IEnumerable<string> rolesToFilter, AdminActivity activity)
        {
            var userRules = Rules.Where(ru => ru.Activity == activity && userRoles.Any(ro => ro == ru.RoleOfActingUser));
            var result = userRules.SelectMany(r => r.AllowedTargetRoles).Distinct();
            return result;
        }
    }

}
