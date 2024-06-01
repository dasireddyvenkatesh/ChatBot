namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IRegisterEmail
    {
        public void Send(string email, int emailOtp);
    }
}
