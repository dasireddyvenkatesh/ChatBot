using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Models;
using ChatBot.Repoistory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatBot.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly IGetUserChatHistory _chatHistory;
        private readonly IGetChatDetails _chatDetails;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserSaveHistory _userSaveHistory;
        private readonly ICompressImage _compressImage;
        private readonly IChatBotRepo _chatBotRepo;
        private readonly IInsertAndDuplicateCheckHistory _insertAndDuplicateCheck;

        public ChatBotController(IGetUserChatHistory chatHistory,
                                    IGetChatDetails chatDetails,
                                    IHubContext<ChatHub> hubContext,
                                    IUserSaveHistory userSaveHistory,
                                    ICompressImage compressImage,
                                    IChatBotRepo chatBotRepo,
                                    IInsertAndDuplicateCheckHistory insertAndDuplicateCheck)
        {
            _chatHistory = chatHistory;
            _chatDetails = chatDetails;
            _hubContext = hubContext;
            _userSaveHistory = userSaveHistory;
            _compressImage = compressImage;
            _chatBotRepo = chatBotRepo;
            _insertAndDuplicateCheck = insertAndDuplicateCheck;
        }

        public IActionResult ChatBotInital()
        {

            return View();
        }

        [Route("NewUserRegister")]
        public async Task<int> NewUserRegister(string newUserName)
        {
            int newUserId = await _chatBotRepo.NewUser(newUserName);

            return newUserId;
        }

        [Route("MessageDetails")]
        public async Task<IActionResult> MessageDetails(int messageId)
        {
            var messageDetails = await _chatBotRepo.MessageDetails(messageId);

            return Json(messageDetails);
        }

        [Route("ImageBase")]
        public async Task<string> ImageBase(int messageId)
        {
            string base64 = await _chatBotRepo.Base64Image(messageId);

            return base64;
        }

        [Route("LastMessageStatus")]
        public async Task<IActionResult> LastMessageStatus(int fromUserId, int toUserId, int lastMessageId)
        {
            var lastMessageDetails = await _chatBotRepo.LastMessageStatus(fromUserId, toUserId, lastMessageId);

            bool statusOnline = ChatHub.ActiveUsers.Where(x => x.Item1 == toUserId && x.Item2 == fromUserId && x.Item3 == true).Any();

            if (statusOnline)
            {
                lastMessageDetails.LastSeenStatus = "Online";
                lastMessageDetails.LastSeenStatusColor = "green";
            }

            return Json(lastMessageDetails);
        }

        [Route("UserChatHistory")]
        public async Task<IActionResult> UserChatHistory(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return View("UnAuthorized", new UnAuthroizedModel { UnAuthroizeEntry = false });
            }

            var history = await _chatHistory.History(userName);

            if (history.Count == 0)
            {
                return View("ChatBotInital", "Enter Valid Username");
            }

            return View(history);
        }

        [Route("ChatDetails")]
        public async Task<IActionResult> ChatDetails(int fromUserId, int toUserId, bool newUser = false)
        {
            UnAuthroizedModel unAuthroizedModel = await _insertAndDuplicateCheck.DuplicateCheck(fromUserId, toUserId, newUser);

            if (unAuthroizedModel.Duplicate || (!unAuthroizedModel.HistoryExists))
            {
                return View("UnAuthorized", unAuthroizedModel);
            }

            var userDetails = await _chatDetails.GetChat(fromUserId, toUserId);

            return View(userDetails);
        }

        [Route("SendMessage")]
        public async Task SendMessage(int fromUserId, int toUserId, string message, IFormFile imageFile)
        {

            if (imageFile != null && imageFile.Length > 0)
            {

                string imageBase64 = _compressImage.Compress(imageFile);
                int messageId = await _userSaveHistory.SaveMessage(fromUserId, toUserId, null, imageBase64);
                await _hubContext.Clients.All.SendAsync("Receive", fromUserId, toUserId, null, imageBase64, messageId);

            }
            else
            {
                int messageId = await _userSaveHistory.SaveMessage(fromUserId, toUserId, message, null);
                await _hubContext.Clients.All.SendAsync("Receive", fromUserId, toUserId, message, null, messageId);
            }

        }
    }
}
