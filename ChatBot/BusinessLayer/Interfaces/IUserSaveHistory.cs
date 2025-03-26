namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IUserSaveHistory
    {
        public Task<string> SaveMessage(string fromUserId, string toUserId, string? message, string? imageBytes);
    }
}
