namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IEmailMessage
    {
        public void Send(string toEmail, string subject, string body);
    }
}
