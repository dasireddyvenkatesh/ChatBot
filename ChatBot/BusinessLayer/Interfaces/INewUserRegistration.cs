namespace ChatBot.BusinessLayer.Interfaces
{
    public interface INewUserRegistration
    {
        public Task<string> Register(string email, string password);
    }
}
