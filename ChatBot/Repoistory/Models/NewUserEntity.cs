using MongoDB.Bson;

namespace ChatBot.Repoistory.Models
{
    public class NewUserEntity
    {
        public ObjectId Id { get; set; }
        public string ChatUserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string ChatEmail { get; set; } = string.Empty;
        public string ChatPassword { get; set; } = string.Empty;
        public string ChatDeviceIp { get; set; } = string.Empty;
        public DateTime LastSeen { get; set; }
        public string ChatUserPhoto { get; set; } = string.Empty;
        public int ChatEmailOtp { get; set; }
        public DateTime ChatEmailDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } 

    }
}
