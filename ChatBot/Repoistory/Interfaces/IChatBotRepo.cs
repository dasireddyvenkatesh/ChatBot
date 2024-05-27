using ChatBot.Models;

namespace ChatBot.Repoistory.Interfaces
{
    public interface IChatBotRepo
    {
        public Task<List<ChatHistoryModel>> GetHistory(string userName);

        public Task<int> NewUser(string userName, string passWord);

        public Task<string?> ValidateUserName(string userName);

        public Task<bool> HistoryExists(int fromUserId, int toUserId);

        public Task<Dictionary<int, string>> ExistingUsers(string existingChatUsers);

        public Task<bool> InsertHistory(int fromUserId, int toUserId);

        public Task<int> LastMessageId(int fromUserId, int toUserId);

        public Task<DateTime> GetLastSeen(int userId);

        public Task UpdateLastSeen(int userId);

        public Task<LastMessageModel> LastMessageStatus(int fromUserId, int toUserId, int lastMessageId);

        public Task<int> SaveHistory(int fromUserId, int toUserId, string? message, string? imageBytes);

        public Task<List<ChatDetailsModel>> History(int fromChatId, int toChatId);

        public Task<MessageDetailsModel> MessageDetails(int messageId);

        public Task<string> Base64Image(int messageId);

        public Task<string> GetUserNameById(int userId);

        public Task<int> GetIdByUsername(string userName);
    }
}
