using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Web.Helpers;
using System.Web.Mvc;
using SA46Team1_Web_ADProj.Models;

//-----------------------------------------------------------------
//   Author: Wong Wei Jie
//-----------------------------------------------------------------

namespace SA46Team1_Web_ADProj
{
    public static class CommonLogic
    {
        public static string SerialNo(int count, string id)
        {
            //data is item desc, index is list index
            if (count < 10)
            {
                id = id + "-00" + count.ToString();
            }
            else
            if (count < 100 && count >=10)
            {
                id = id + "-0" + count.ToString();
            }
            else
            {
                id = id + "-" + count.ToString();
            }
            return id;
        }

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
                }

            }            
        }
    }
}