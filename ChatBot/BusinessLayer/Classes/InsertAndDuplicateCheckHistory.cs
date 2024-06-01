using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Repoistory.Interfaces;

namespace ChatBot.BusinessLayer.Classes
{
    public class InsertAndDuplicateCheckHistory : IInsertAndDuplicateCheckHistory
    {
        private readonly IChatBotRepo _chatBotRepo;

        public InsertAndDuplicateCheckHistory(IChatBotRepo chatBotRepo)
        {
            _chatBotRepo = chatBotRepo;
        }

        public async Task<string> DuplicateCheck(int fromUserId, int toUserId, bool newUser)
        {
            if (newUser)
            {
                await _chatBotRepo.InsertHistory(fromUserId, toUserId);
            }

            var loginUserName = await _chatBotRepo.GetUserNameById(fromUserId);

            return loginUserName;
        }
    }
}
