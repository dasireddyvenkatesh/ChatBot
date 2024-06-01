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

        public PublicController(IHttpContextAccessor contextAccessor, IChatBotRepo chatBotRepo,
                                    INewUserRegistration newUser, IVerifyEmailOtp verifyEmailOtp)
        {
            _contextAccessor = contextAccessor;
            _chatBotRepo = chatBotRepo;
            _newUser = newUser;
            _verifyEmailOtp = verifyEmailOtp;
        }

        public IActionResult ChatBotInital()
        {

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("UserChatHistory", "ChatBot");
            }



            //var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            //var context = _contextAccessor?.HttpContext ?? throw new InvalidOperationException("HttpContext is null.");

            //context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

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
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var context = _contextAccessor?.HttpContext ?? throw new InvalidOperationException("HttpContext is null.");

                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(30)
                    });

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
    }
}
