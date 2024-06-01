namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IInsertAndDuplicateCheckHistory
    {
        public Task<string> DuplicateCheck(int fromUserId, int toUserId, bool newUser);
    }
}
