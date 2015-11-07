using Microsoft.AspNet.Identity;
using SoniReports.DataAccessLayer;
using SoniReports.DomainModel;
using SoniReports.ViewModels;
using System;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SoniReports.Controllers
{
    public class HomeController : BaseController
    {
        readonly LoggingAccess _loggingAccess = new LoggingAccess();
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = SoniRoles.AllCommaSeparated)]
        public ActionResult ViewReport()
        {
            LogActivity();

            ViewBag.Message = "Your report page.";
            var model = new DashboardViewModel
            {
                ReportServerURL = new Uri(WebConfigurationManager.AppSettings["ReportServerUrl"]),
                ReportPath = WebConfigurationManager.AppSettings["ReportPath"]
            };
            return View(model);
        }

        private void LogActivity()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            _loggingAccess.Register(new UserActivityLogEntry 
            {  
                Id = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
                UserId = user.Id, 
                ActivityKind = ActivityKind.View, 
                ObjectKind = ObjectKind.Report 
            });
        }

    }
}