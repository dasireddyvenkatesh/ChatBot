namespace ChatBot.Models
{
    public class ChatDetailsRequestModel
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public bool NewUser { get; set; }
    }
}
