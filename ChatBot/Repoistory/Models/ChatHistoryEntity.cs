using MongoDB.Bson;

namespace ChatBot.Repoistory.Models
{
    public class ChatHistoryEntity
    {
        public ObjectId Id { get; set; }
        public string ChatFromUserId { get; set; } = string.Empty;   
        public string ChatToUserId { get; set; } = string.Empty;
    }
}
