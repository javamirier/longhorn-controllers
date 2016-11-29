using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace LonghornMusic.Messaging
{
    public class EmailMessaging
    {
        public static void SendEmail(String toEmailAddress, String emailSubject, String emailBody)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("bot.longhornmusic@gmail.com", "goebbels"),
                EnableSsl = true
            };

            string finalMessage = emailBody + "\n\nThank you for using Longhorn Music!";
            MailAddress senderEmail = new MailAddress("bot.longhornmusic@gmail.com", "Longhorn Music");

            MailMessage mm = new MailMessage();
            mm.Subject = "Team 19 - " + emailSubject;
            mm.Sender = senderEmail;
            mm.From = senderEmail;
            mm.To.Add(new MailAddress(toEmailAddress));
            mm.Body = finalMessage;
            client.Send(mm);

        }
    }
}