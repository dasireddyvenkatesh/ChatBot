using ChatBot.BusinessLayer.Interfaces;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace ChatBot.BusinessLayer.Classes
{
    public class EmailMessage : IEmailMessage
    {
        public void Send(string toEmail, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.From = new MailAddress("noreply.venkychatbot@gmail.com");
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            mail.BodyEncoding = Encoding.UTF8;
            mail.SubjectEncoding = Encoding.Default;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("noreply.venkychatbot@gmail.com", "obhsmjirjiaxabns");
            smtp.Send(mail);
        }
    }
}
