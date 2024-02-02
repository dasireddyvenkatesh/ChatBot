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

        public async Task<int> SaveMessage(int fromUserId, int toUserId, string? message, string? imageBytes)
        {
            int result = await _chatBotRepo.SaveHistory(fromUserId, toUserId, message, imageBytes);

            return result;
        }
    }
}
