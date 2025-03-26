using MongoDB.Bson;

namespace ChatBot.Repoistory.Models
{
    public class ChatMessagesEntity
    {
        public ObjectId Id { get; set; }
        public string ChatHistoryId { get; set; } = string.Empty;
        public string ChatMessage { get; set; } = string.Empty;
        public string ChatContent { get; set; } = string.Empty;
        public string MessageStatus { get; set; } = string.Empty;
        public DateTime DeliveryDate { get; set; }
        public DateTime SeenDate { get; set; }
        public DateTime TimeStamp { get; set; } 
    }
}
