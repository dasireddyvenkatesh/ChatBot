namespace ChatBot.Models
{
    public class UnAuthroizedModel
    {
        public bool Duplicate { get; set; }

        public bool HistoryExists { get; set; } = true;

        public bool UnAuthroizeEntry { get; set; } = true;

        public bool SessionExists { get; set; }
    }
}
