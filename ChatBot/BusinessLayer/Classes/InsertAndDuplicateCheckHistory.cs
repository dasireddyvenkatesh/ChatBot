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

        public async Task<string> DuplicateCheck(ChatDetailsRequestModel detailsRequestModel)
        {
            if (detailsRequestModel.NewUser)
            {
                await _chatBotRepo.InsertHistory(detailsRequestModel.FromUserId, detailsRequestModel.ToUserId);
            }

            var loginUserName = await _chatBotRepo.GetUserNameById(detailsRequestModel.ToUserId);

            return loginUserName;
        }
    }
}
