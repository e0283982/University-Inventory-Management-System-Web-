﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SA46Team1_Web_ADProj.Controllers
{
    public class EmailController
    {
        // Old method
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

        [HttpPost]
        public static void EMAILWITHATTACHMENT(string fromEmail, string toEmail, string subj, string body, string filepath)
        {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subj;
                mail.Body = body;

                //Attachment attachment;
                //attachment = new Attachment(filepath);
                //mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential("stationerylogicuniversity@gmail.com", "adproj01");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

        }
    }
}