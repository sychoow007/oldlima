using OldLima.Models;
using System.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using System;
using System.Net;

namespace OldLima.Controllers
{
    public class ContactController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult Contact()
        {
            var model = new ContactVM();

            return PartialView("ContactForm", model);
        }

        [HttpPost]
        public ActionResult Contact(ContactVM contactVM)
        {
            if (ModelState.IsValid)
            {
                var sb = new StringBuilder();                
                sb.AppendFormat("<p>Nieuw contact</p>");
                sb.AppendFormat("<p>Naam: {0}</p>", contactVM.Name);
                sb.AppendFormat("<p>E-mail: {0}</p>", contactVM.Email);
                sb.AppendFormat("<p>Telefoon: {0}</p>", contactVM.PhoneNumber);
                sb.AppendFormat("<p>Boodschap:<br /> {0}</p>", contactVM.Message);

                try
                {
                    SendMail("info@oldlima.be", ConfigurationSettings.AppSettings["ContactAddress"], "Nieuwe contactaanvraag Oldlima", sb.ToString(), true);

                    TempData["InfoMessage"] = "Bedankt voor je bericht, we contacteren je zo snel mogelijk.";
                }
                catch (SmtpException smtpEx)
                {
                    TempData["ErrorMessage"] = "Er is een fout gebeurd. Probeer het later nogmaals opnieuw aub.";
                    LogHelper.Error(MethodBase.GetCurrentMethod().DeclaringType, "Error occured while sending mail", smtpEx);
                }

                return RedirectToCurrentUmbracoPage();
            }
            return CurrentUmbracoPage();
        }

        private void SendMail(string FromMail, string ToMail, string Subject, string Body, bool IsHtml)
        {
            try
            {
                MailMessage message = new MailMessage(FromMail.Trim(), ToMail.Trim());
                message.Subject = Subject;
                if (IsHtml)
                {
                    message.IsBodyHtml = true;
                }
                else
                {
                    message.IsBodyHtml = false;
                }
                message.Body = Body;
                
                using (SmtpClient client = new SmtpClient())
                {
                    client.Send(message);
                }
            }
            catch (Exception exception)
            {
                LogHelper.Error(MethodBase.GetCurrentMethod().DeclaringType, "Error occured while sending mail", exception);
                throw new SmtpException();
            }
        }
    }
}