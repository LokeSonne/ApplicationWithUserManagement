using SoniReports.DomainModel;
using SoniReports.Tools;
using SoniReports.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SoniReports.DataAccessLayer;

namespace SoniReports.Controllers
{
    [HandleErrorWithAjaxFilter]
    public class UserGroupManagementController : BaseController
    {

        //----------------------------------------------------------------------------------------------------------------
        #region User Groups
        //----------------------------------------------------------------------------------------------------------------

        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public ActionResult Index()
        {
            return View();
        }

        private class UserGroupWithUsers { public UserGroup Group; public List<ApplicationUser> Users; };
            
        /// <summary>
        /// Returns a list of all current users
        /// </summary>
        /// <returns>All known membership users</returns>
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<JsonResult> GetAllUserGroups()
        {
            var groups = UserGroupAccess.GetUserGroups();
            var groupsWithUsers = new List<UserGroupWithUsers>();
            foreach (var group in groups)
            {
                var groupWithUsers = await GetVisibleUsers(group);
                groupsWithUsers.Add(groupWithUsers);
            }
            var result = groupsWithUsers.Select(g => CreateUserGroupViewRow(g.Group, g.Users)).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        private async Task<UserGroupWithUsers> GetVisibleUsers(UserGroup group)
        {
            var users = UserGroupAccess.GetUsersInGroup(group.Id);
            var visibleUsers = await Can(AdminActivity.ViewGroup, users);
            var result = new UserGroupWithUsers {
                 Group = group,
                 Users = visibleUsers.ToList()
            };
            return result;
        }


        private static object CreateUserGroupViewRow(UserGroup group, IEnumerable<ApplicationUser> groupUsers)
        {
            return new [] 
            {
                group.Id,
                group.Name,
                group.Enabled.ToString(),
                string.Join(", ", groupUsers.Select(u => u.UserName))
            };
        }

        #endregion

        //----------------------------------------------------------------------------------------------------------------
        #region CRUD User Groups
        //----------------------------------------------------------------------------------------------------------------


        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public ActionResult CreateUserGroup()
        {
            var model = new EditUserGroupViewModel { IsNewUserGroup = true };
            return PartialView("EditUserGroup", model);
        }


        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public ActionResult EditUserGroup(string id)
        {
            var userGroup = UserGroupAccess.FindById(id);
            if (userGroup == null) return HttpNotFound();
            var model = EditUserGroupViewModel.From(userGroup);
            return PartialView(model);
        }

        [HttpPost]
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        [ValidateAntiForgeryToken]
        public JsonResult EditUserGroup(string id, bool isNewUserGroup, string name, bool enabled)
        {
            EditUserGroupViewModel model = EditUserGroupViewModel.FromParams(id, isNewUserGroup, name, enabled);
            this.Validate(model);

            if (ModelState.IsValid)
            {
                int saveResult;

                if (model.IsNewUserGroup)
                {
                    var userGroup = model.To();
                    userGroup.Id = Guid.NewGuid().ToString();
                    saveResult = UserGroupAccess.Create(userGroup);
                }
                else
                {
                    saveResult = UserGroupAccess.Update(model.Id, model.WriteUpdatesTo);
                }
                return Json(new { Succeeded = saveResult > 0, ChangeCount = saveResult } );
            }
            var jsonResult = ModelStateToJsonResult();
            return jsonResult;
        }

        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public JsonResult DeleteUserGroup(string id)
        {
            var result = UserGroupAccess.Delete(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion

        //----------------------------------------------------------------------------------------------------------------
        #region User Groups Association History
        //----------------------------------------------------------------------------------------------------------------

        public class HistoryEntry
        {
            public UserGroupAssociationHistory Association;
            public UserGroup Group;
            public ApplicationUser User;
        }

        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public ActionResult AssociationHistory()
        {
            return View();
        }

        /// <summary>
        /// Returns a list of all current users
        /// </summary>
        /// <returns>All known membership users</returns>
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<JsonResult> GetUserGroupAssociationHistory()
        {
            var changes = UserGroupAccess.GetUserGroupAssociationHistory();
            var visibleChanges = await Can(AdminActivity.ViewGroup, changes, c => c.UserId);
            var groups = UserGroupAccess.GetUserGroups();
            var users = UserManager.Users.ToList();

            var orderedChanges = visibleChanges.Select(c => new HistoryEntry
                { 
                    Association = c, 
                    Group =  groups.SingleOrDefault(g => g.Id == c.UserGroupId), 
                    User = users.SingleOrDefault(u => u.Id == c.UserId)
                })
                .OrderBy(c => c.User.UserName)
                .ThenBy(c => c.Association.From);

            var result = orderedChanges.Select(CreateUserGroupAssociationViewRows);

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        private object CreateUserGroupAssociationViewRows(HistoryEntry entry)
        {
            return new [] { 
                entry.Association.Id, 
                entry.User.UserName,
                entry.Association.From.ToString(), 
                entry.Association.To == SqlDateTime.MaxValue ? "..." : entry.Association.To.ToString(),
                entry.Group.Name
            };
        }

        #endregion

    }

}

