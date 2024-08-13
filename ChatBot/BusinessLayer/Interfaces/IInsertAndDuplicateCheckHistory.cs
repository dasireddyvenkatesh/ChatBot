using ChatBot.Models;

namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IInsertAndDuplicateCheckHistory
    {
        public Task<string> DuplicateCheck(ChatDetailsRequestModel detailsRequestModel);
    }
}
