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

            unAuthroizedModel.Duplicate = ChatHub.ActiveUsers.Where(x => x.Item1 == fromUserId && x.Item2 == toUserId && x.Item3 == true).Any();

            unAuthroizedModel.HistoryExists = await _chatBotRepo.HistoryExists(fromUserId, toUserId);

            //unAuthroizedModel.UnAuthroizeEntry = ChatHub.LoginUserName.Where(x => x.Item1 == fromUserId).Any();

            return unAuthroizedModel;
        }
    }
}
