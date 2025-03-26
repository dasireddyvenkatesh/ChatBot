namespace ChatBot.Models
{
    public class ChatDetailsRequestModel
    {
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public bool NewUser { get; set; }
    }
}
