﻿namespace ChatBot.BusinessLayer.Interfaces
{
    public interface IRegisterEmail
    {
        public void Send(string userName, string email, int emailOtp);
    }
}
