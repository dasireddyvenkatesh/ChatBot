using ChatBot.Models;
using ChatBot.Repoistory.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChatBot.Repoistory.Classes
{
    public class ChatBotRepo : IChatBotRepo
    {
        private readonly IConfiguration _configuration;

        public ChatBotRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<ChatHistoryModel>> GetHistory(string userName)
        {

            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Username", userName);

                var history = await connection.QueryAsync<ChatHistoryModel>("UserChatHistory", dynamicParameters, commandType: CommandType.StoredProcedure);

                return (List<ChatHistoryModel>)history;
            }

        }

        public async Task<string> NewUser(string userName, string email, string passWord, string deviceIp)
        {

            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@userName", userName);
                dynamicParameters.Add("@email", email);
                dynamicParameters.Add("@passWord", BCrypt.Net.BCrypt.HashPassword(passWord));
                dynamicParameters.Add("deviceIp", deviceIp);
                dynamicParameters.Add("@message", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

                await connection.ExecuteAsync("NewUserRegister", dynamicParameters, commandType: CommandType.StoredProcedure);

                string message = dynamicParameters.Get<string>("@message");

                return message;
            }

        }

        public async Task<string?> ValidateUserName(string userName)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@userName", userName);

                string? userPw = await connection.QueryFirstOrDefaultAsync<string>("ValidateUserName", dynamicParameters, commandType: CommandType.StoredProcedure);

                return userPw;
            }
        }

        public async Task<Dictionary<int, string>> ExistingUsers(string existingChatUsers)
        {

            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ExistingChatUsers", existingChatUsers);

                var result = await connection.QueryAsync<(int ChatUserId, string ChatUserName)>("ExistingUsers", dynamicParameters, commandType: CommandType.StoredProcedure);

                return result.ToDictionary(user => user.ChatUserId, user => user.ChatUserName);
            }

        }

        public async Task<DateTime> GetLastSeen(int userId)
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

        public async Task<bool> UpdateEmailOtp(string email, int emailOtp)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@email", email);
                dynamicParameters.Add("@emailOtp", emailOtp);

                await connection.ExecuteAsync("UpdateEmailOtp", dynamicParameters, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        public async Task<string> VerifyEmailOtp(string email, int emailOtp)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@email", email);
                dynamicParameters.Add("@emailOtp", emailOtp);
                dynamicParameters.Add("@Message", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

                await connection.ExecuteAsync("VerifyEmailOtp", dynamicParameters, commandType: CommandType.StoredProcedure);

                string result = dynamicParameters.Get<string>("@Message");

                return result;
            }
        }

        public async Task UpdateLastSeen(int userId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@messageId", userId);

                await connection.ExecuteAsync("UpdateLastSeenStatus", dynamicParameters, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task<bool> InsertHistory(int fromUserId, int toUserId)
        {

            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@fromUserId", fromUserId);
                dynamicParameters.Add("@toUserId", toUserId);

                var result = await connection.ExecuteAsync("InsertHistory", dynamicParameters, commandType: CommandType.StoredProcedure);

                return true;
            }

        }

        public async Task<int> LastMessageId(int fromUserId, int toUserId)
        {

            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@fromUserId", fromUserId);
                dynamicParameters.Add("@toUserId", toUserId);

                var result = await connection.QueryFirstOrDefaultAsync<int>("LastMessageId", dynamicParameters, commandType: CommandType.StoredProcedure);

                return result;
            }

        }

        public async Task<MessageDetailsModel> MessageDetails(int messageId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@messageId", messageId);

                var messageDetails = await connection.QueryFirstAsync<MessageDetailsModel>("MessageDetails", dynamicParameters, commandType: CommandType.StoredProcedure);

                return messageDetails;
            }
        }

        public async Task<LastMessageModel> LastMessageStatus(int fromUserId, int toUserId, int lastMessageId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@fromUserId", fromUserId);
                dynamicParameters.Add("@toUserId", toUserId);
                dynamicParameters.Add("@lastMessageId", lastMessageId);

                var messageDetails = await connection.QueryFirstOrDefaultAsync<LastMessageModel>("LastMessageStatus", dynamicParameters, commandType: CommandType.StoredProcedure);

                return messageDetails == null ? new LastMessageModel() : messageDetails;
            }
        }

        public async Task<string> Base64Image(int messageId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@messageId", messageId);

                string base64 = await connection.QueryFirstAsync<string>("ImageBase", dynamicParameters, commandType: CommandType.StoredProcedure);

                return base64;
            }
        }

        public async Task<List<ChatDetailsModel>> History(int fromChatId, int toChatId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@FromUserId", fromChatId);
                dynamicParameters.Add("@ToUserId", toChatId);

                var history = await connection.QueryAsync<ChatDetailsModel>("ChatDetails", dynamicParameters, commandType: CommandType.StoredProcedure);

                return (List<ChatDetailsModel>)history;
            }
        }

        public async Task<int> SaveHistory(int fromUserId, int toUserId, string? message, string? imageBytes)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@FromUserId", fromUserId);
                dynamicParameters.Add("@ToUserId", toUserId);
                dynamicParameters.Add("@Message", message);
                dynamicParameters.Add("@ImageBytes", imageBytes);

                int messageId = await connection.QueryFirstAsync<int>("SaveChatHistory", dynamicParameters, commandType: CommandType.StoredProcedure);

                return messageId;
            }
        }

        public async Task<string> GetUserNameById(int userId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@userId", userId);

                string? userName = await connection.QueryFirstOrDefaultAsync<string>("GetUserName", dynamicParameters, commandType: CommandType.StoredProcedure);

                return userName != null ? userName : string.Empty;
            }
        }

        public async Task<int> GetIdByUsername(string userName)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@userName", userName);

                int userId = await connection.QueryFirstOrDefaultAsync<int>("GetIdbyUserName", dynamicParameters, commandType: CommandType.StoredProcedure);

                return userId;
            }
        }
    }
}
