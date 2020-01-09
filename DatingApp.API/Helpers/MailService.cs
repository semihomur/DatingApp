using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public static class MailService
    {
        public static void SendMail(string subject, string email,string content)
        {
            MailMessage msg = new MailMessage();
            msg.Subject = subject;
            msg.From = new MailAddress("MAIL", "DatingApp");
            msg.To.Add(new MailAddress(email));
            msg.Body = CreateBody(subject,content);
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.High;
            // Host ve Port Gereklidir!
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // Güvenli bağlantı gerektiğinden kullanıcı adı ve şifrenizi giriniz.
            NetworkCredential AccountInfo = new NetworkCredential("MAIL", "PASSWORD");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = AccountInfo;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(msg);
        }
        private static string CreateBody(string subject,string content)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.IO.Path.GetFullPath("htmlMail.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{subject}", subject);
            body = body.Replace("{content}", content);
            return body;
        }
    }
}
