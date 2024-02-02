namespace ChatBot.Models
{
    public class UnAuthroizedModel
    {
        public Tuple<int,int, bool>? Duplicate { get; set; }

        public int HistoryExists { get; set; }
    }
}
