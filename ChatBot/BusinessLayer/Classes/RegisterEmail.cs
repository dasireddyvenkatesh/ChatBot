using ChatBot.BusinessLayer.Interfaces;
using System.Security.Cryptography;

namespace ChatBot.BusinessLayer.Classes
{
    public class RegisterEmail : IRegisterEmail
    {
        private readonly IEmailMessage _emailMessage;

        public RegisterEmail(IEmailMessage emailMessage)
        {
            _emailMessage = emailMessage;
        }

        public void Send(string userName, string email, int emailOtp)
        {
            string subject = "Thank you for signing up!";
            string body = $@"
            <html>
            <body>
            <h1>Welcome!</h1>
            <p>Thank you for signing up. We are excited to have you on board!</p>
            <p>Your Username is :</p> <h2>{userName}</h2>
            <p>Your One-Time Password (OTP) for email verification is:</p>
            <h2>{emailOtp}</h2>
            <p>Please use this OTP to complete your registration.</p>
            <p>Thank you,<br>The VenkyChatBot Team</p>
            </body>
            </html>";

            _emailMessage.Send(email, subject, body);
        }
    }
}
