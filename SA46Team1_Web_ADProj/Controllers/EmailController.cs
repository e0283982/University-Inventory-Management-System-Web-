using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class EmailController
    {
        [HttpPost]
        public static void SendEmail(string fromEmail, string toEmail, string subj, string body)
        {
            try
            {
                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails  
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = "stationerylogicuniversity@gmail.com";
                WebMail.Password = "adproj01";

                //Sender email address.  
                WebMail.From = fromEmail;

                //Send email  
                WebMail.Send(to: toEmail, subject: subj, body: body, isBodyHtml: true);
            }
            catch (Exception)
            {

            }
        }
    }
}