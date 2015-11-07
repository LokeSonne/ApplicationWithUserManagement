using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using SoniReports.DataAccessLayer;

namespace SoniReports.Controllers
{
    public class TestController : AccountController
    {
        [Authorize(Roles = SoniRoles.SuperUser)]
        public JsonResult Index()
        {
            return Json("Testing", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = SoniRoles.SuperUser)]
        public async Task<JsonResult> CreateConfirmation()
        {
            var user = await UserManager.FindByNameAsync("ls@sonnenielsen.dk");
            await CreateEmailConfirmation(user);
            return Json("Done", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = SoniRoles.SuperUser)]
        public JsonResult TestMail()
        {
            string to = "test-bruger1@sonnenielsen.dk";
            return SendMail(to);
        }

        [Authorize(Roles = SoniRoles.SuperUser)]
        public JsonResult TestMail2()
        {
            string to = "test-bruger2@sonnenielsen.dk";
            return SendMail(to);
        }

        [Authorize(Roles = SoniRoles.SuperUser)]
        public JsonResult TestMail3()
        {
            string to = "admin@sonnenielsen.dk";
            return SendMail(to);
        }

        [Authorize(Roles = SoniRoles.SuperUser)]
        public JsonResult TestMail4()
        {
            string to = "ls@sonnenielsen.dk";
            return SendMail(to);
        }

        private JsonResult SendMail(string to)
        {
            MailMessage message = new MailMessage(to, "ls@sonnenielsen.dk");
            message.Subject = "Test using the SMTP client.";
            message.Body = @"Using this feature, you can send an e-mail message from an application easily. This mail was sent: "
                + DateTime.Now.ToString();
            SmtpClient client = new SmtpClient();
            //client.UseDefaultCredentials = false;
            client.Send(message);
            return Json("Done test", JsonRequestBehavior.AllowGet);
        }



    }
}
