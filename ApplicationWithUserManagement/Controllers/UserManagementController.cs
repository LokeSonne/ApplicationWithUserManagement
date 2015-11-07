using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SoniReports.DataAccessLayer;
using SoniReports.DomainModel;
using SoniReports.Tools;
using SoniReports.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc; 


namespace SoniReports.Controllers
{
    [HandleErrorWithAjaxFilter]
    public class UserManagementController : BaseController
    {
        readonly LoggingAccess _loggingAccess = new LoggingAccess();

        //----------------------------------------------------------------------------------------------------------------
        #region View User Table
        //----------------------------------------------------------------------------------------------------------------


        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public ActionResult Index()
        {
            var model = new UserManagementViewModel();
            return View(model);
        }


        /// <summary>
        /// Returns a list of all current users
        /// </summary>
        /// <returns>All known membership users</returns>
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<JsonResult> GetAllUsers()
        {
            var users = UserManager.Users.ToList();
            var allRoles = RoleAccess.GetRoles().ToList();
            var visibleUsers = await Can(AdminActivity.SeeUserInRole, users);

            var result = new List<object>();
            foreach (var u in visibleUsers)
            {
                var groups = UserGroupAccess.GetGroupsForUser(u.Id);
                var model = CreateUserViewRows(u, allRoles, groups);
                result.Add(model);
            }

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        private static object CreateUserViewRows(ApplicationUser user, IEnumerable<IdentityRole> roles, IEnumerable<UserGroup> groups)
        {
            var roleTexts = user.Roles.Join(roles, o => o.RoleId, i => i.Id, (r, i) => i.Name);
            var groupTexts = groups.Select(g => g.Name);

            return new [] 
            {
                user.Id,
                user.UserName,
                user.EmailConfirmed.ToString(),
                user.EmailConfirmed ? user.Activated.GetDisplayName() : string.Empty,
                string.Join(", ", roleTexts),
                string.Join(", ", groupTexts),
                string.IsNullOrWhiteSpace(user.PasswordHash) ? string.Empty : "******",
                user.AccessFailedCount.ToString()
            };
        }

        #endregion


        //----------------------------------------------------------------------------------------------------------------
        #region CRUD User
        //----------------------------------------------------------------------------------------------------------------


        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public ActionResult CreateUser()
        {
            var model = new EditUserViewModel { IsNewUser = true };
            return PartialView("EditUser", model);
        }


        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<ActionResult> EditUser(string id)
        {
            var targetUser = await UserManager.FindByIdAsync(id);
            if (targetUser == null) return HttpNotFound();

            var canChange = await Can(AdminActivity.EditUserInRole, targetUser);
            if (!canChange) return new HttpUnauthorizedResult();
                      
            var model = EditUserViewModel.From(targetUser);
            return PartialView(model);
        }

        [HttpPost]
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditUser(string id, bool isNewUser, string email, bool emailConfirmed, bool activated)
        {
            var model = EditUserViewModel.FromParams(id, isNewUser, email, emailConfirmed, activated);
            this.Validate(model);

            if (ModelState.IsValid)
            {
                IdentityResult saveResult;

                if (model.IsNewUser)
                {
                    var user = model.To();
                    user.Id = Guid.NewGuid().ToString();
                    saveResult = UserManager.Create(user);
                }
                else
                {
                    var targetUser = await UserManager.FindByIdAsync(model.Id);
                    if (targetUser == null) return Json(HttpNotFound());

                    var canChange = await Can(AdminActivity.EditUserInRole, targetUser);
                    if (!canChange) return Json(new HttpUnauthorizedResult());

                    if (targetUser.Activated == ActivationStatus.Pending && activated)
                    {
                        var canActivate = await Can(AdminActivity.ActivateUser, targetUser);
                        if (!canActivate) return Json(new HttpUnauthorizedResult());
                        await ActivateUser(model.Id);
                    }

                    model.WriteUpdatesTo(targetUser);
                    saveResult = UserManager.Update(targetUser);
                }
                return Json(saveResult);
            }
            var jsonResult = ModelStateToJsonResult();
            return jsonResult;
        }


        /// <summary>
        /// Deletes a user.
        /// </summary>
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<JsonResult> DeleteUser(string id)
        {
            var targetUser = await UserManager.FindByIdAsync(id);
            if (targetUser == null) return Json(HttpNotFound());

            var canChange = await Can(AdminActivity.DeleteUserInRole, targetUser);
            if (!canChange) return Json(new HttpUnauthorizedResult());


            var result = UserManager.Delete(targetUser);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<ActionResult> SetPassword(string id)
        {
            var targetUser = await UserManager.FindByIdAsync(id);
            if (targetUser == null) return HttpNotFound();

            var canChange = await Can(AdminActivity.SetUserPassword, targetUser);
            if (!canChange) return new HttpUnauthorizedResult();

            var model = AdminSetPasswordViewModel.From(targetUser);
            return PartialView(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<JsonResult> SetPassword(AdminSetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var invalidModelJsonResult = ModelStateToJsonResult();
                return invalidModelJsonResult;
            }

            var targetUser = await UserManager.FindByIdAsync(model.Id);
            if (targetUser == null) return Json(HttpNotFound());

            var canChange = await Can(AdminActivity.SetUserPassword, targetUser);
            if (!canChange) return Json(new HttpUnauthorizedResult());
            
            var result = await UserManager.RemovePasswordAsync(targetUser.Id);
                
            if (result.Succeeded)
            {
                result = await UserManager.AddPasswordAsync(targetUser.Id, model.Password);
                if (result.Succeeded) return Json(new { Succeeded = true });
            }
            AddErrorsToModelState(result);
            var jsonResult = ModelStateToJsonResult();
            return jsonResult;
        }


        #endregion


        //----------------------------------------------------------------------------------------------------------------
        #region Assign Roles
        //----------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Returns an array containing all known role names.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = SoniRoles.SuperUser)]
        public async Task<ActionResult> AssignRoles(string id)
        {
            var targetUser = await UserManager.FindByIdAsync(id);
            if (targetUser == null) return HttpNotFound();

            var roleList = await GetManageableRoles(id);
            var model = AssignRolesViewModel.From(targetUser, roleList);
           
            return PartialView(model);
        }

        private async Task<List<RoleViewModel>> GetManageableRoles(string id)
        {
            var targetUserRoles = UserManager.GetRoles(id);
            var manageableRoles = await GetManageableRoles(AdminActivity.SeeAndAssignRole);
            var roleList = manageableRoles.GroupJoin(targetUserRoles, r => r, i => i, (r, i) => RoleViewModel.From(r, i.Any())).ToList();
            return roleList;
        }

        [HttpPost]
        [Authorize(Roles = SoniRoles.SuperUser)]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AssignRoles(AssignRolesViewModel model)
        {
            var targetUser = await UserManager.FindByIdAsync(model.UserId);
            if (targetUser == null) return Json(HttpNotFound());

            var canChange = await Can(AdminActivity.SeeAndAssignRole, targetUser);
            if (!canChange) return Json(new HttpUnauthorizedResult());

            var userRoles = await UserManager.GetRolesAsync(model.UserId);

            var desiresToRemove = userRoles.Where(ur => model.Roles.Any(r => r.RoleName == ur && !r.Assigned));
            var desiresToAdd = model.Roles.Where(r => userRoles.All(ur => ur != r.RoleName) && r.Assigned);
            var manageableRoles = (await GetManageableRoles(AdminActivity.SeeAndAssignRole)).ToList();
            var toRemove = manageableRoles.Intersect(desiresToRemove).ToList();
            var toAdd = desiresToAdd.Where(a => manageableRoles.Any(r => r == a.RoleName)).ToList();

            foreach (var userRole in toRemove) await UserManager.RemoveFromRoleAsync(model.UserId, userRole);
            foreach (var userRole in toAdd) await UserManager.AddToRoleAsync(model.UserId, userRole.RoleName);
           
            await OptToNotifyUponChanges(toRemove, toAdd, targetUser);

            var saveResult = toRemove.Count() + toAdd.Count();
            var result = Json(new { Succeeded = true, ChangeCount = saveResult });
            return result;
        }

        private async Task OptToNotifyUponChanges(IEnumerable<string> toRemove, IEnumerable<RoleViewModel> toAdd, ApplicationUser affectedUser)
        {
            var removalsToFlag = toRemove.Where(r => SoniRoles.Managers.Any()).ToList();
            var addsToFlag = toAdd.Select(m => m.RoleName).Where(a => SoniRoles.Managers.Any(m => m == a)).ToList();

            if (removalsToFlag.Count > 0 || addsToFlag.Count > 0)
            {
                var body = string.Format("Bruger {0} har foretaget følgende ændringer i brugerrettigheder for bruger {1}: Tilføjede rolle(r): {2}. Fjernede rolle(r): {3}", 
                    User.Identity.Name, affectedUser.UserName, 
                    removalsToFlag.Count == 0 ? "(Ingen)" : string.Join(", ", removalsToFlag),
                    addsToFlag.Count == 0 ? "(Ingen)" : string.Join(", ", addsToFlag));

                var recipient = GetRoleChangeNotificationRecipient();

                await UserManager.SendEmailAsync(recipient.Id, "Ændring i brugerrettigheder", body);
            }   
        }

        private ApplicationUser GetRoleChangeNotificationRecipient()
        {
            var userRoles = RoleAccess.GetAllUsersWithRole(RoleBasedEvents.ManagerRoleChangeRecipientRole);
            var recipientId = userRoles.First().UserId;
            var recipient = UserManager.FindById(recipientId);
            return recipient;
        }

        #endregion



        //----------------------------------------------------------------------------------------------------------------
        #region Assign Groups
        //----------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Returns an array containing all known group names and whether user is associated.
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<ActionResult> AssignGroups(string id)
        {
            var targetUser = await UserManager.FindByIdAsync(id);
            if (targetUser == null) return HttpNotFound();
            
            var canChange = await Can(AdminActivity.AssignGroup, targetUser);
            if (!canChange) return new HttpUnauthorizedResult();

            var allGroups = UserGroupAccess.GetUserGroups();
            var userGroups = UserGroupAccess.GetGroupsForUser(id);
            var groupList = allGroups.GroupJoin(userGroups, r => r.Id, i => i.Id,
                 (r, g) => GroupViewModel.From(r, g.Any())).ToList();

            var model = AssignGroupsViewModel.From(targetUser, groupList);

            return PartialView(model);
        }

        [HttpPost]
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AssignGroups(AssignGroupsViewModel model)
        {
            var targetUser = await UserManager.FindByIdAsync(model.UserId);
            if (targetUser == null) return Json(HttpNotFound());

            var canChange = await Can(AdminActivity.AssignGroup, targetUser);
            if (!canChange) return Json(new HttpUnauthorizedResult());

            var userGroups = UserGroupAccess.GetGroupsForUser(model.UserId).ToList();

            var toRemove = userGroups.Where(ur => model.UserGroups.Any(r => r.GroupId == ur.Id && !r.Assigned)).ToList();
            var toAdd = model.UserGroups.Where(r => userGroups.All(ur => ur.Id != r.GroupId) && r.Assigned).ToList();

            foreach (var userGroup in toRemove) UserGroupAccess.RemoveUserFromGroup(model.UserId, userGroup.Id);
            foreach (var userGroup in toAdd) UserGroupAccess.AddUserToGroup(model.UserId, userGroup.GroupId);

            var saveResult = toRemove.Count() + toAdd.Count();
            return Json(new { Succeeded = true, ChangeCount = saveResult });
        }


        #endregion

        //----------------------------------------------------------------------------------------------------------------
        #region Activate Users
        //----------------------------------------------------------------------------------------------------------------

        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<ActionResult> ViewUserActivationRequest(string userId)
        {
            var targetUser = await UserManager.FindByIdAsync(userId);
            if (targetUser == null) return HttpNotFound();
            if (targetUser.Activated != ActivationStatus.Pending) return new HttpStatusCodeResult(HttpStatusCode.Gone);

            var canChange = await Can(AdminActivity.ActivateUser, targetUser);
            if (!canChange) return new HttpUnauthorizedResult();

            var model = new UserManagementViewModel { ConfirmActivtionOfUser = userId };
            return View("Index", model);
        }

        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<ActionResult> ProcessUserActivationRequest(string id)
        {
            var targetUser = await UserManager.FindByIdAsync(id);
            if (targetUser == null) return HttpNotFound();

            var canChange = await Can(AdminActivity.ActivateUser, targetUser);
            if (!canChange) return new HttpUnauthorizedResult();

            var roleList = (await GetManageableRoles(id)).Select(r => r.RoleName).ToList();
            if (roleList.Count == 0) roleList.Add(SoniRoles.ReportViewer);
            var model = ActivateUserViewModel.From(targetUser, roleList);
            return PartialView("ActivateUser", model);
        }

        /// <summary>
        /// Activates the specified user.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ActivateUser(string id)
        {
            var targetUser = await UserManager.FindByIdAsync(id);
            if (targetUser == null) return Json(HttpNotFound());

            var canChange = await Can(AdminActivity.ActivateUser, targetUser);
            if (!canChange) return Json(new HttpUnauthorizedResult());

            targetUser.Activated = ActivationStatus.Activated;

            var targetUserRoles = await UserManager.GetRolesAsync(id);
            if (targetUserRoles.Count == 0) await UserManager.AddToRoleAsync(id, SoniRoles.ReportViewer);

            var result = await UserManager.UpdateAsync(targetUser);

            if (result.Succeeded)
            {
                var callbackUrl = Url.Action("Index", "Home", null, protocol: Request.Url.Scheme);
                const string reply = "Din konto er blevet aktiveret. Du kan logge ind via <a href=\"{0}\">dette link</a>";

                await UserManager.SendEmailAsync(targetUser.Id, "Behandling af brugers aktiveringsforespørgsel", string.Format(reply, callbackUrl));

                return Json(result);
            }
            AddErrorsToModelState(result);
            var jsonResult = ModelStateToJsonResult();
            return jsonResult;
        }

        /// <summary>
        /// Activates the specified user.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RejectUser(string id)
        {
            var targetUser = await UserManager.FindByIdAsync(id);
            if (targetUser == null) return Json(HttpNotFound());

            var canChange = await Can(AdminActivity.ActivateUser, targetUser);
            if (!canChange) return Json(new HttpUnauthorizedResult());

            targetUser.Activated = ActivationStatus.Rejected;

            var result = await UserManager.UpdateAsync(targetUser);

            if (result.Succeeded)
            {
                var callbackUrl = Url.Action("Index", "Home", null, protocol: Request.Url.Scheme);
                const string reply = "Din konto er ikke blevet godkendt.";

                await UserManager.SendEmailAsync(targetUser.Id, "Behandling af brugers aktiveringsforespørgsel", string.Format(reply, callbackUrl));

                return Json(result);
            }
            AddErrorsToModelState(result);
            var jsonResult = ModelStateToJsonResult();
            return jsonResult;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------------------
        #region User Activity Log
        //----------------------------------------------------------------------------------------------------------------



        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public ActionResult ViewActivityLog()
        {
            var model = new UserManagementViewModel();
            return View(model);
        }

        /// <summary>
        /// Returns a list of all current users
        /// </summary>
        /// <returns>All known membership users</returns>
        [Authorize(Roles = SoniRoles.ManagersCommaSeparated)]
        public async Task<JsonResult> GetAllActivities()
        {
            var logEntries = _loggingAccess.GetUserActivityLog();
            var visibleLogEntries = await Can(AdminActivity.SeeUserInRole, logEntries, e => e.UserId);
            var result = visibleLogEntries.Select(e =>
                            {
                                var targetUser = UserManager.FindById(e.UserId);
                                var model = CreateActivityViewRows(e, targetUser);
                                return model;
                            });

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        private static object CreateActivityViewRows(UserActivityLogEntry entry, ApplicationUser user)
        {
            return new [] 
            {
                user.Id,
                entry.TimeStamp.ToString("yyyy-MM-dd hh:mm"),
                user.UserName,
                entry.ActivityKind.Caption(),
                entry.ObjectKind.Caption()
            };
        }
        


        #endregion

        //----------------------------------------------------------------------------------------------------------------

    }

}

