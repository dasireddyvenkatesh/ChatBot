using ChatBot.Models;

namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IGetChatDetails
    {
        public Task<List<ChatDetailsModel>> GetChat(string fromUserId, string toUserId, string loginUserName);
    }
}
