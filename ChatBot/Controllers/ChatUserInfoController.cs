using ChatBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Controllers
{
    public class ChatUserInfoController : Controller
    {
        public IActionResult UserInfo(int fromUserId, int toUserId)
        {
            UserInfoModel model = new UserInfoModel();

            model.FromUserId = fromUserId;
            model.ToUserId = toUserId;

            return View(model);
        }
    }
}
