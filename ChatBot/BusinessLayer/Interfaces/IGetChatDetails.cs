using ChatBot.Models;

namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IGetChatDetails
    {
        public Task<List<ChatDetailsModel>> GetChat(int fromUserId, int toUserId, string loginUserName);
    }
}
