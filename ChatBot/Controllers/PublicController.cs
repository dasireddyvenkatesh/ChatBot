using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Repoistory.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatBot.Controllers
{
    public class PublicController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IChatBotRepo _chatBotRepo;
        private readonly INewUserRegistration _newUser;
        private readonly IVerifyEmailOtp _verifyEmailOtp;
        private readonly IResendEmailOtp _resendEmailOtp;

        public PublicController(IHttpContextAccessor contextAccessor, IChatBotRepo chatBotRepo,
                                    INewUserRegistration newUser, IVerifyEmailOtp verifyEmailOtp,
                                    IResendEmailOtp resendEmailOtp)
        {
            _contextAccessor = contextAccessor;
            _chatBotRepo = chatBotRepo;
            _newUser = newUser;
            _verifyEmailOtp = verifyEmailOtp;
            _resendEmailOtp = resendEmailOtp;
        }

        public IActionResult ChatBotInital()
        {

            var encryptedData = Request.Cookies["MUID"] ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(encryptedData))
            {
                return RedirectToAction("UserChatHistory", "ChatBot");
            }
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string userName, string passWord)
        {

            string? userPw = await _chatBotRepo.ValidateUserName(userName);

            if (!string.IsNullOrEmpty(userPw))
            {
                bool isValid = BCrypt.Net.BCrypt.Verify(passWord, userPw);

                if (isValid)
                {
                    CookieOptions option = new CookieOptions()
                    {
                        SameSite = SameSiteMode.Strict,
                        Secure = true,
                        HttpOnly = true,
                        IsEssential = true,
                        Expires = DateTime.Now.AddDays(30)
                    };
                    Response.Cookies.Append("MUID", userName, option);

                    //await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),
                    //new AuthenticationProperties
                    //{
                    //    IsPersistent = true,
                    //    ExpiresUtc = DateTime.UtcNow.AddDays(30)
                    //});

                    return RedirectToAction("UserChatHistory", "ChatBot");

                }
            }

            return View("ChatBotInital", "Enter Valid Username and Password");
        }

        [HttpPost("NewUserRegister")]
        public async Task<string> NewUserRegister(string newUserName, string newEmail, string newPassword)
        {
            
            string response = await _newUser.Register(newUserName, newEmail, newPassword);

            return response;
        }

        [Route("VerifyEmailOtp")]
        public async Task<string> VerifyEmailOtp(string email, int emailOtp)
        {

            string response = await _verifyEmailOtp.Verify(email, emailOtp);

            return response;
        }

        [Route("ResendEmailOtp")]
        public async Task<string> ResendEmailOtp(string email)
        {
            string messaage = await _resendEmailOtp.Send(email);

            return messaage;
        }
    }
}
