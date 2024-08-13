namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IResendEmailOtp
    {
        public Task<string> Send(string email);
    }
}
