using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Repoistory.Interfaces;
using System.Text.RegularExpressions;

namespace ChatBot.BusinessLayer.Classes
{
    public class ResendEmailOtp : IResendEmailOtp
    {
        private readonly IChatBotRepo _chatBotRepo;
        private readonly IRegisterEmail _registerEmail;

        public ResendEmailOtp(IChatBotRepo chatBotRepo, IRegisterEmail registerEmail)
        {
            _chatBotRepo = chatBotRepo;
            _registerEmail = registerEmail;
        }

        public async Task<string> Send(string email)
        {
            bool isMatch = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            string message = string.Empty;

            if (email == null || !isMatch)
            {
                message = "Enter Valid Email Address";

            }
            else
            {
                message = await _chatBotRepo.ResendEmailOtp(email);

                //user already activated

                //user is their and but it is in the time

                //user id their sent email otp


                if (message == "User registered successfully")
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
