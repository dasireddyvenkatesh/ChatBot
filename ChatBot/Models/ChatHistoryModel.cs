namespace ChatBot.Models
{
    public class ChatHistoryModel
    {
        public int FromUserId {  get; set; }

        public int ToUserId { get; set; }

        public string UserName {  get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string LoginUserName {  get; set; } = string.Empty;

        public int UnreadMessages { get; set; }

        public Dictionary<int, string> ExistingUsers { get; set; } = new Dictionary<int, string>();

        public DateTime LastSeen { get; set; }

        public int LastMessageId {  get; set; }

        public string LastStatus { get; set; } = string.Empty;

        public string UserPhoto { get; set; } = string.Empty;

    }
}
