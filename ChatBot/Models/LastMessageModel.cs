namespace ChatBot.Models
{
    public class LastMessageModel
    {
        public bool StatusChange {  get; set; }

        public string LatestMessageId { get; set; } = string.Empty;

        public string LatestMessage { get; set; } = string.Empty;

        public string LastSeenStatus { get; set; } = "Offline";

        public string LastSeenStatusColor { get; set; } = "maroon";
    }
}
