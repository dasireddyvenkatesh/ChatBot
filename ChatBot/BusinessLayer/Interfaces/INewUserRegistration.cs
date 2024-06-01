namespace ChatBot.BusinessLayer.Interfaces
{
    public interface INewUserRegistration
    {
        public Task<string> Register(string username, string email, string password);
    }
}
