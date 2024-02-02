using ChatBot.Models;

namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IGetUserChatHistory
    {
        public Task<List<ChatHistoryModel>> History(string userName);
    }
}
