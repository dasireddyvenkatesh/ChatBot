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

        public ChatHub(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> GetUserStatus(int toUserId)
        {
            DateTime userLastStatus = await GetLastSeen(toUserId);

            return userLastStatus >= DateTime.UtcNow.AddSeconds(-10);
        }

        public async Task<bool> GetUserActiveStatus(int fromUserId, int toUserId)
        {
            DateTime fromUserStatus = await GetLastSeen(fromUserId);
            DateTime toUserStatus = await GetLastSeen(toUserId);

            return fromUserStatus >= DateTime.UtcNow.AddSeconds(-10) && toUserStatus >= DateTime.UtcNow.AddSeconds(-10);
        }

        public async Task<string> LastSeenStatus(int userId)
        {
            DateTime lastSeenDate = await GetLastSeen(userId);
            string lastseen = lastSeenDate.Humanize(utcDate: true);
            return lastseen;
        }

        private async Task<DateTime> GetLastSeen(int userId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@userId", userId);

                var result = await connection.QueryFirstAsync<DateTime>("LastSeenStatus", dynamicParameters, commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task UpdateSeenStatus(int messageId)
        {
            await UpdateLastSeen(messageId);

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
