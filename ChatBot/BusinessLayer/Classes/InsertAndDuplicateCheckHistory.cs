using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Models;
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

        public async Task<UnAuthroizedModel> DuplicateCheck(int fromUserId, int toUserId, bool newUser)
        {
            if (newUser)
            {
                await _chatBotRepo.InsertHistory(fromUserId, toUserId);
            }

            UnAuthroizedModel unAuthroizedModel = new UnAuthroizedModel();

            unAuthroizedModel.HistoryExists = await _chatBotRepo.HistoryExists(fromUserId, toUserId);

            return unAuthroizedModel;
        }
    }
}
