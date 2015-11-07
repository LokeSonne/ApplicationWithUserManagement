using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SoniReports.DataAccessLayer;
using SoniReports.DomainModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System;
using System.Collections.Generic;

namespace SoniReports.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationUserManager _userManager;
        protected ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        private RoleAccess _roleAccess;
        protected RoleAccess RoleAccess
        {
            get
            {
                if (_roleAccess == null) _roleAccess = new RoleAccess();
                return _roleAccess;
            }
            set { _roleAccess = value; }
        }

        private UserGroupAccess _userGroupAccess;
        protected UserGroupAccess UserGroupAccess
        {
            get
            {
                if (_userGroupAccess == null) _userGroupAccess = new UserGroupAccess();
                return _userGroupAccess;
            }
            set { _userGroupAccess = value; }
        }


        private RoleActivityAuthorizor _roleActivityAuthorizor;
        protected RoleActivityAuthorizor RoleActivityAuthorizor
        {
            get
            {
                if (_roleActivityAuthorizor == null) _roleActivityAuthorizor = new RoleActivityAuthorizor(UserManager, RoleAccess);
                return _roleActivityAuthorizor;
            }
            set { _roleActivityAuthorizor = value; }
        }


        //for ioc
        //public (ApplicationUserManager userManager, ApplicationSignInManager signInManager,
        //    RoleAccess roleAccess, UserGroupAccess userGroupAccess)
        //{
        //    UserManager = userManager;
        //    RoleAccess = roleAccess;
        //    UserGroupAccess = userGroupAccess;
        //}


        protected async Task<bool> Can(AdminActivity activity, ApplicationUser targetUser)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var result = RoleActivityAuthorizor.Can(user, activity, targetUser);
            return result;
        }

        protected async Task<IEnumerable<ApplicationUser>> Can(AdminActivity activity, IEnumerable<ApplicationUser> targetUsers)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var result = RoleActivityAuthorizor.Can(user, activity, targetUsers).ToList();
            return result;
        }


        protected async Task<IEnumerable<T>> Can<T>(AdminActivity activity, IEnumerable<T> targets, Func<T, string> userIdGetter)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var result = RoleActivityAuthorizor.Can(user, activity, targets, userIdGetter).ToList();
            return result;
        }

        protected async Task<IEnumerable<string>> GetManageableRoles(AdminActivity activity)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var result = RoleActivityAuthorizor.GetManageableRoles(user, activity).ToList();
            return result;
        }

        protected void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected JsonResult ModelStateToJsonResult()
        {
            var errorsFields = ModelState.Where(m => m.Value.Errors.Count > 0);
            var errorMessages = errorsFields.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage);
            var errorResultModel = new { Succeeded = false, Errors = errorMessages };
            var jsonResult = Json(errorResultModel);

            return jsonResult;
        }


    }
}