namespace ChatBot.Models
{
    public class ChatDetailsModel
    {

        public string LoginUserName { get; set; } = string.Empty;

        public string UserName {  get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string MessageId { get; set; } = string.Empty;

        public List<string> MessageIds { get; set; } = new List<string>();

        public List<string> MessageStatus { get; set; } = new List<string>();

        public List<string> Messages {  get; set; }= new List<string>();

        public List<string> ImageBase64 { get; set; } = new List<string>();

        public List<string> MessageTime { get; set; } = new List<string>();

        public List<bool> IsCurrentUser { get; set; } = new List<bool>();

        //public List<string> ToMessage { get; set; } = new List<string>();

        //public List<string> ToMessageTime { get; set; } = new List<string>();

        public string MessageDate { get; set; } = string.Empty;

        public string FromUserId { get; set; } = string.Empty;

        public string ToUserId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public string DataContent { get; set; } = string.Empty;

        //public bool IsCurrentUser { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
