using ChatBot.BusinessLayer.Interfaces;

namespace ChatBot.BusinessLayer.Classes
{
    public class LastSeenStatus : ILastSeenStatus
    {
        public string Status(DateTime lastSeen)
        {
            DateTime now = DateTime.UtcNow;

            if (lastSeen >= now.AddSeconds(-10)) 
            {
                return "Online";
            }
            else if (lastSeen.Date == now.Date)
            {
                return "Away";
            }
            else
            {
                return "Offline";
            }
        }
    }
}
