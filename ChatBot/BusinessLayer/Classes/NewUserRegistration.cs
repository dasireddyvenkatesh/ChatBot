using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Repoistory.Interfaces;
using System.Text.RegularExpressions;

namespace ChatBot.BusinessLayer.Classes
{
    public class NewUserRegistration : INewUserRegistration
    {
        private readonly IChatBotRepo _chatBotRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRegisterEmail _registerEmail;

        public NewUserRegistration(IChatBotRepo chatBotRepo, IHttpContextAccessor contextAccessor, IRegisterEmail registerEmail)
        {
            _chatBotRepo = chatBotRepo;
            _contextAccessor = contextAccessor;
            _registerEmail = registerEmail;
        }

        public async Task<string> Register(string username, string email, string password)
        {
            string message = string.Empty;

            bool isMatch = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(username) || username.Length <= 3)
            {
                message = "Please create a userId with min 3 char.";
            }
            else if (string.IsNullOrEmpty(email) || !isMatch)
            {
                message = "Enter a valid email address";
            }
            else if (string.IsNullOrEmpty(password) || password.Length <= 5)
            {
                message = "Enter Password Min 5 Charcters";
            }

            if (string.IsNullOrEmpty(message))
            {
                string ipaddress = _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

                message = await _chatBotRepo.NewUser(username, email, password, ipaddress);

                if(message == "User registered successfully")
                {
                    //Create a random 6 digit otp
                    Random random = new Random();
                    int randomNumber = random.Next(100000, 1000000);
                    //Save the otp and datetime in the database
                    await _chatBotRepo.UpdateEmailOtp(email, randomNumber);

                    //sent an email 
                    _registerEmail.Send(email, randomNumber);
                }
            }

            return message;
        }
    }
}
