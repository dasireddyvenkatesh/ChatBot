using ChatBot.Models;

namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IInsertAndDuplicateCheckHistory
    {
        public Task<UnAuthroizedModel> DuplicateCheck(int fromUserId, int toUserId, bool newUser);
    }
}
