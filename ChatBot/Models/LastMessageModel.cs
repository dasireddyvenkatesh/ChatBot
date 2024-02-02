namespace ChatBot.Models
{
    public class LastMessageModel
    {
        public bool StatusChange {  get; set; }

        public int LatestMessageId { get; set; }

        public string LatestMessage { get; set; } = string.Empty;

        public string LastSeenStatus { get; set; } = "Offline";

        public string LastSeenStatusColor { get; set; } = "maroon";
    }
}
