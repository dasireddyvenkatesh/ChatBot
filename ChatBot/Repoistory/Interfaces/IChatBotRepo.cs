using ChatBot.Models;

namespace ChatBot.Repoistory.Interfaces
{
    public interface IChatBotRepo
    {
        public Task<List<ChatHistoryModel>> GetHistory(string userName);

        public Task<string> NewUser(string userName, string email, string passWord, string deviceIp);

        public Task<string> ValidateUserName(string userName);

        public Task<Dictionary<string, string>> ExistingUsers(string existingChatUsers);

        public Task<bool> InsertHistory(string fromUserId, string toUserId);

        public Task<string> LastMessageId(string fromUserId, string toUserId);

        public Task<DateTime> GetLastSeen(string userId);

        public Task<bool> UpdateEmailOtp(string email, int emailOtp);

        public Task<string> VerifyEmailOtp(string email, int emailOtp);

        public Task<string> ResendEmailOtp(string email);

        public Task UpdateLastSeen(string userId);

        public Task<LastMessageModel> LastMessageStatus(string fromUserId, string toUserId, string lastMessageId);

        public Task<string> SaveHistory(string fromUserId, string toUserId, string? message, string? imageBytes);

        public Task<List<ChatDetailsModel>> History(string fromChatId, string toChatId);

        public Task<MessageDetailsModel> MessageDetails(string messageId);

        public Task<string> Base64Image(string messageId);

        public Task<string> GetUserNameById(string userId);

        public Task<string> GetIdByUsername(string userName);
    }
}
