using ChatBot.BusinessLayer.Interfaces;
using System.Text;

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



            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html><head><style>");
            sb.AppendLine("body{font-family:Arial,sans-serif;margin:0;padding:0;background-color:#f4f4f4;}");
            sb.AppendLine(".container{width:100%;max-width:600px;margin:0 auto;background-color:#ffffff;padding:20px;box-shadow:0 0 10px rgba(0,0,0,0.1);}");
            sb.AppendLine(".header{background-color:#4CAF50;color:#ffffff;padding:10px 0;text-align:center;}");
            sb.AppendLine(".content{padding:20px;}");
            sb.AppendLine(".footer{text-align:center;padding:10px 0;background-color:#eeeeee;margin-top:20px;}");
            sb.AppendLine(".button{display:inline-block;padding:10px 20px;margin:20px 0;background-color:#4CAF50;color:#ffffff;text-decoration:none;border-radius:5px;}");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine("<div class='container'>");
            sb.AppendLine("<div class='header'><h1>Welcome to the Venky Chat Bot Team!</h1></div>");
            sb.AppendLine("<div class='content'>");
            sb.AppendLine("<p>Hi there,</p>");
            sb.AppendLine("<p>We are thrilled to have you as part of the Venky Chat Bot Team. Thank you for joining our community!</p>");
            sb.AppendLine("<p>As a member, you will have access to exclusive content, updates, and support from our team. We are here to help you get the most out of our chat bot and ensure you have a great experience.</p>");
            sb.AppendLine("<p>To get started, please visit our <a href='https://venkywebsite.azurewebsites.net/'>website</a> and explore the resources available to you.</p>");
            sb.AppendLine("<p>If you have any questions or need assistance, feel free to <a href='mailto:noreply.venkychatbot@gmail.com'>contact our support team</a>.</p>");
            sb.AppendLine("<a class='button' href='https://venkywebsite.azurewebsites.net/'>Get Started</a>");
            sb.AppendLine("<p>Best regards,</p>");
            sb.AppendLine("<p>The Venky Chat Bot Team</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='footer'><p>&copy; 2024 Venky Chat Bot Team. All rights reserved.</p></div>");
            sb.AppendLine("</div></body></html>");

            string body1 = sb.ToString();

            _emailMessage.Send(toEmail, subject, body);
        }
    }
}
