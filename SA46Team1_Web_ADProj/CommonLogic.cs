using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SA46Team1_Web_ADProj
{
    public static class CommonLogic
    {
        public static class Email
        {
            // To use this simply type: EmailController.SendEmail("", "", "", "", "filepath"); or you can omit the file path

            [HttpPost]
            public static void sendEmail(string fromEmail, string toEmail, string subj, string body, string filepath)
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress(fromEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = subj;
                    mail.Body = body;

                    Attachment attachment;
                    attachment = new Attachment(filepath);
                    mail.Attachments.Add(attachment);

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new NetworkCredential("stationerylogicuniversity@gmail.com", "adproj01");
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                }
                catch (Exception)
                {
                    // Add some message here
                }

            }

            [HttpPost]
            public static void sendEmail(string fromEmail, string toEmail, string subj, string body)
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress(fromEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = subj;
                    mail.Body = body;

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new NetworkCredential("stationerylogicuniversity@gmail.com", "adproj01");
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                }
                catch (Exception)
                {
                    // Add some message here
                }

            }
        }

        public static class SearchInventory
        {
            public static void searchBar()
            {

            }
        }
    }
}