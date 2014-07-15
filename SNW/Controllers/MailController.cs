using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using SNW.Models.Mail;

namespace SNW.Controllers
{
    /// <summary>
    /// Controls system's emails
    /// </summary>
    public class MailController : Controller
    {
        public ActionResult Confirmation(string destination, string subject, string body)
        {
            /// Save some message information to reuse it in a View
            MailInformation information = new MailInformation();
            information.subject = subject;
            information.message = body;

            /// Send email
            EmailSystem.Send(destination, subject, body);

            return View(information);
        }

        /// Solution taken from http://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient(v=vs.110).aspx
        public class EmailSystem
        {
            public static void Send(string destination, string subject, string body)
            {

                /// Command line argument must the the SMTP host.
                    /// Solution taken from http://www.codeproject.com/Articles/66257/Sending-Mails-in-NET-Framework
                    SmtpClient client = new SmtpClient();
                    client.Host = "mail.wpapps.mobi";
                    client.Port = 25;
                    client.EnableSsl = false;
                    // The server requires user's credentialsm 
                    // not the default credentials
                    client.UseDefaultCredentials = false;
                    // Provide your credentials
                    client.Credentials = new System.Net.NetworkCredential("test@wpapps.mobi", "u0aVFFWr");
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    // TO HERE
                /// Specify the e-mail sender. 
                MailAddress from = new MailAddress("test@wpapps.mobi",
                   "SNW System",
                System.Text.Encoding.UTF8);
                /// Set destinations for the e-mail message.
                MailAddress to = new MailAddress(destination);
                /// Specify the message content.
                MailMessage message = new MailMessage(from, to);
                message.Body = body;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                /// Send the message
                client.Send(message);
                /// Clean up.
                message.Dispose();
            }
        }
	}
}