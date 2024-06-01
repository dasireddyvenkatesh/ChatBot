namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IVerifyEmailOtp
    {
        public Task<string> Verify(string email, int emailOtp);
    }
}
