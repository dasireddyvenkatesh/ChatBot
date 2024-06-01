using ChatBot.BusinessLayer.Interfaces;

namespace ChatBot.BusinessLayer.Classes
{
    public class RegisterVerifiedEmail : IRegisterVerifiedEmail
    {
        private readonly IEmailMessage _emailMessage;

        public RegisterVerifiedEmail(IEmailMessage emailMessage)
        {
            _emailMessage = emailMessage;
        }

        public void Send(string toEmail)
        {
            string subject = "Thank You for Joining the Venky Chat Bot Team Family";

            string body = @"
            <html>
            <head>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f4f4f4;
                    }
                    .container {
                        width: 100%;
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: #ffffff;
                        padding: 20px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }
                    .header {
                        background-color: #4CAF50;
                        color: #ffffff;
                        padding: 10px 0;
                        text-align: center;
                    }
                    .content {
                        padding: 20px;
                    }
                    .footer {
                        text-align: center;
                        padding: 10px 0;
                        background-color: #eeeeee;
                        margin-top: 20px;
                    }
                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        margin: 20px 0;
                        background-color: #4CAF50;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Welcome to the Venky Chat Bot Team!</h1>
                    </div>
                    <div class='content'>
                        <p>Hi there,</p>
                        <p>We are thrilled to have you as part of the Venky Chat Bot Team. Thank you for joining our community!</p>
                        <p>As a member, you will have access to exclusive content, updates, and support from our team. We are here to help you get the most out of our chat bot and ensure you have a great experience.</p>
                        <p>To get started, please visit our <a href='https://venkywebsite.azurewebsites.net/'>website</a> and explore the resources available to you.</p>
                        <p>If you have any questions or need assistance, feel free to <a href='mailto:noreply.venkychatbot@gmail.com'>contact our support team</a>.</p>
                        <a class='button' href='https://venkywebsite.azurewebsites.net/'>Get Started</a>
                        <p>Best regards,</p>
                        <p>The Venky Chat Bot Team</p>
                    </div>
                    <div class='footer'>
                        <p>&copy; 2024 Venky Chat Bot Team. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";

            _emailMessage.Send(toEmail, subject, body);
        }
    }
}
