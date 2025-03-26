namespace ChatBot.Models
{
    public class ChatHistoryModel
    {
        public string FromUserId { get; set; } = string.Empty;

        public string ToUserId { get; set; } = string.Empty;

        public string UserName {  get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string LoginUserName {  get; set; } = string.Empty;

        public long UnreadMessages { get; set; }

        public Dictionary<string, string> ExistingUsers { get; set; } = new Dictionary<string, string>();

        public DateTime LastSeen { get; set; }

        public string LastMessageId { get; set; } = string.Empty;

        public string LastStatus { get; set; } = string.Empty;

        public string UserPhoto { get; set; } = string.Empty;

    }
}
