namespace ChatBot.BusinessLayer.Interfaces
{
    public interface ILastSeenStatus
    {
        public string Status(DateTime lastSeen);
    }
}
