using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Repoistory.Interfaces;

namespace ChatBot.BusinessLayer.Classes
{
    public class VerifyEmailOtp : IVerifyEmailOtp
    {
        private readonly IChatBotRepo _chatBotRepo;
        private readonly IRegisterVerifiedEmail _verifiedEmail;

        public VerifyEmailOtp(IChatBotRepo chatBotRepo, IRegisterVerifiedEmail verifiedEmail)
        {
            _chatBotRepo = chatBotRepo;
            _verifiedEmail = verifiedEmail;
        }

        public async Task<string> Verify(string email, int emailOtp)
        {

            string message = string.Empty;

            if (string.IsNullOrEmpty(email) || emailOtp.ToString().Length < 6 || emailOtp.ToString().Length >= 7)
            {
                message = "Invalid Email Or Otp Attempt.";
                return message;
            }

            message = await _chatBotRepo.VerifyEmailOtp(email, emailOtp);

            if (message == "Thank you, Email Is Verified")
            {
                _verifiedEmail.Send(email);
            }

            return message;
        }
    }
}
