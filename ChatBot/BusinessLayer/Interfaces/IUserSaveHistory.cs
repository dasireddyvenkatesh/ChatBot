namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IUserSaveHistory
    {
        public Task<int> SaveMessage(int fromUserId, int toUserId, string? message, string? imageBytes);
    }
}
