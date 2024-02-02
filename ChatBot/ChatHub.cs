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

        public static List<Tuple<int, int, bool>> ActiveUsers = new List<Tuple<int, int, bool>>();

        public static string LoginUserName = string.Empty;

        public async Task<string> GetConnectionId(int fromUserId, int toUserId)
        {
            var existingPair = ActiveUsers.FirstOrDefault(x => x.Item1 == fromUserId && x.Item2 == toUserId);

            if (existingPair == null)
            {
                ActiveUsers.AddRange(await GetLinkedUsersIds(fromUserId, toUserId));
            }
            else
            {
                ActiveUsers[ActiveUsers.IndexOf(existingPair)] = Tuple.Create(fromUserId, toUserId, true);
            }

            var distinctList = ActiveUsers.GroupBy(t => new { t.Item1, t.Item2 })
                                .Select(group => group.OrderByDescending(t => t.Item3).First()).ToList();

            ActiveUsers = new List<Tuple<int, int, bool>>();
            ActiveUsers.AddRange(distinctList);

            return Context.ConnectionId;
        }

        public void RemoveConnectionId(int fromUserId, int toUserId)
        {

            ActiveUsers.Remove(Tuple.Create(fromUserId, toUserId, true));
            ActiveUsers.RemoveAll(x => x.Item1 == fromUserId && x.Item3 == false);
        }

        public bool GetUserStatus(int toUserId)
        {
            return ActiveUsers.Where(x => x.Item1 == toUserId && x.Item3 == true).Any();
        }

        public bool GetUserActiveStatus(int fromUserId, int toUserId)
        {
            bool pair1 = ActiveUsers.Where(x => x.Item1 == fromUserId && x.Item2 == toUserId && x.Item3 == true).Any();
            bool pair2 = ActiveUsers.Where(x => x.Item1 == toUserId && x.Item2 == fromUserId && x.Item3 == true).Any();

            if (pair1 && pair2)
            {
                return true;
            }

            return false;

        }

        public async Task<string> LastSeenStatus(int userId)
        {
            DateTime lastSeenDate = await GetLastSeen(userId);
            string lastseen = lastSeenDate.Humanize(utcDate: false);
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

        private async Task<List<Tuple<int, int, bool>>> GetLinkedUsersIds(int fromUserId, int toUserId)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", fromUserId);

                var history = await connection.QueryAsync<(int, int)>("LinkedUsersIds", dynamicParameters, commandType: CommandType.StoredProcedure);

                return history.Select(tuple => new Tuple<int, int, bool>(tuple.Item1, tuple.Item2, tuple.Item1 == fromUserId && tuple.Item2 == toUserId)).ToList();
            }
        }

    }
}
