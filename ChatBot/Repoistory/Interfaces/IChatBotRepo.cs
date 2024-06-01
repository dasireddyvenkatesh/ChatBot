using ChatBot.Models;

namespace ChatBot.Repoistory.Interfaces
{
    public interface IChatBotRepo
    {
        public Task<List<ChatHistoryModel>> GetHistory(string userName);

        public Task<string> NewUser(string userName, string email, string passWord, string deviceIp);

        public Task<string?> ValidateUserName(string userName);

        public Task<Dictionary<int, string>> ExistingUsers(string existingChatUsers);

        public Task<bool> InsertHistory(int fromUserId, int toUserId);

        public Task<int> LastMessageId(int fromUserId, int toUserId);

        public Task<DateTime> GetLastSeen(int userId);

        public Task<bool> UpdateEmailOtp(string email, int emailOtp);

        public Task<string> VerifyEmailOtp(string email, int emailOtp);

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
