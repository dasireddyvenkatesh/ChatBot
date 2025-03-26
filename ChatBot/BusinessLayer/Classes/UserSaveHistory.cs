using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Repoistory.Interfaces;

namespace ChatBot.BusinessLayer.Classes
{
    public class UserSaveHistory : IUserSaveHistory
    {
        private readonly IChatBotRepo _chatBotRepo;

        public UserSaveHistory(IChatBotRepo chatBotRepo)
        {
            _chatBotRepo = chatBotRepo;
        }

        public async Task<string> SaveMessage(string fromUserId, string toUserId, string? message, string? imageBytes)
        {
            string result = await _chatBotRepo.SaveHistory(fromUserId, toUserId, message, imageBytes);

            return result;
        }
    }
}
