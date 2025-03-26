using ChatBot.Repoistory.Interfaces;
using Dapper;
using Humanizer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChatBot
{
    public class ChatHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly IChatBotRepo _chatBotRepo;

        public ChatHub(IConfiguration configuration, IChatBotRepo chatBotRepo)
        {
            _configuration = configuration;
            _chatBotRepo = chatBotRepo;
        }

        public async Task<bool> GetUserStatus(string toUserId)
        {
            DateTime userLastStatus = await _chatBotRepo.GetLastSeen(toUserId);

            return userLastStatus >= DateTime.UtcNow.AddSeconds(-10);
        }

        public async Task<bool> GetUserActiveStatus(string fromUserId, string toUserId)
        {
            DateTime fromUserStatus = await _chatBotRepo.GetLastSeen(fromUserId);
            DateTime toUserStatus = await _chatBotRepo.GetLastSeen(toUserId);

            return fromUserStatus >= DateTime.UtcNow.AddSeconds(-10) && toUserStatus >= DateTime.UtcNow.AddSeconds(-10);
        }

        public async Task<string> LastSeenStatus(string userId)
        {
            DateTime lastSeenDate = await _chatBotRepo.GetLastSeen(userId);
            string lastseen = lastSeenDate.Humanize(utcDate: true);
            return lastseen;
        }

        public async Task UpdateSeenStatus(string messageId)
        {
            //await UpdateLastSeen(messageId);

        }

        private async Task UpdateLastSeen(int messageId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@messageId", messageId);

                await connection.ExecuteAsync("UpdateLastSeenStatus", dynamicParameters, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task DeleteMessage(int messageId)
        {
            await Delete(messageId);
        }

        private async Task Delete(int messageId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@messageId", messageId);

                await connection.ExecuteAsync("DeleteMessage", dynamicParameters, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task UpdateMessageStatus(int messageId, string status)
        {
            await UpdateStatus(messageId, status);
        }

        private async Task UpdateStatus(int messageId, string status)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@messageId", messageId);
                dynamicParameters.Add("@status", status);

                await connection.ExecuteAsync("UpdateMessageStatus", dynamicParameters, commandType: CommandType.StoredProcedure);

            }
        }

    }
}
