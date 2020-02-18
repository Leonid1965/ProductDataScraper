using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace Security.Services
{

    public class SmtpEmail : IEmailSender
    {
        IConfiguration config;
        public SmtpEmail(IConfiguration config)
        {
            this.config = config;
        }


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient()
            {
                Host = Def.EmailSmtpHost, // config.GetValue<String>("Email:Smtp:Host")
                Port = Def.EmailSmtpPort,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                     Def.EmailSmtpUsername,
                     Def.EmailSmtpPassword
                )
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(Def.EmailSmtpFrom) // account-security-noreply@yourdomain.com
            };
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlMessage;
            // mailMessage.Attachments
            await client.SendMailAsync(mailMessage);
        }

        public static async Task SendMail(string fromAddress, List<string> toAddresses, string subject, string body,
                List<Attachment> attachments = null, bool isBodyHtml = true)
        {
            using (MailMessage mail = new MailMessage())
            {
                using (SmtpClient SmtpServer = new SmtpClient()) //gets default client configuration from web.config
                {
                    mail.From = new MailAddress(fromAddress);
                    foreach (string toAddress in toAddresses)
                    {
                        mail.To.Add(toAddress);
                    }

                    mail.IsBodyHtml = isBodyHtml;
                    mail.Subject = subject;
                    mail.Body = body;

                    attachments = attachments ?? new List<Attachment>();
                    foreach (var attachment in attachments)
                    {
                        mail.Attachments.Add(attachment);
                    }

                    await SmtpServer.SendMailAsync(mail);
                }
            }
        }

    }

    public class Def
    {
        public static bool isDev = true;

        public static string EmailSmtpHost = "smtp.gmail.com";
        public static int EmailSmtpPort = 25;
        public static string EmailSmtpUsername = "gilgulnet4@gmail.com";
        public static string EmailSmtpFrom = "gilgulnet admin <gilgulnet4@gmail.com>";
        public static string EmailSmtpPassword = "gilgul123";

    }


}
