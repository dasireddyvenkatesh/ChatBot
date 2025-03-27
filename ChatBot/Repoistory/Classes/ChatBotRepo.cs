using ChatBot.Models;
using ChatBot.Repoistory.Interfaces;
using ChatBot.Repoistory.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;

namespace ChatBot.Repoistory.Classes
{
    public class ChatBotRepo : IChatBotRepo
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<NewUserEntity> _newUserCollection;
        private readonly IMongoCollection<ChatHistoryEntity> _chatHistoryCollection;
        private readonly IMongoCollection<ChatMessagesEntity> _chatMessagesCollection;


        public ChatBotRepo(IConfiguration configuration, IMongoClient mongoClient)
        {
            _configuration = configuration;
            _mongoDatabase = mongoClient.GetDatabase("venkymongodb");
            _newUserCollection = _mongoDatabase.GetCollection<NewUserEntity>("NewUser");
            _chatHistoryCollection = _mongoDatabase.GetCollection<ChatHistoryEntity>("ChatHistory");
            _chatMessagesCollection = _mongoDatabase.GetCollection<ChatMessagesEntity>("ChatMessages");
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            string collectionName = GetCollectionName<T>();
            return _mongoDatabase.GetCollection<T>(collectionName);
        }

        private string GetCollectionName<T>()
        {
            string modelName = typeof(T).Name;

            if (modelName.EndsWith("y"))
            {
                return modelName.Substring(0, modelName.Length - 1) + "ies";
            }
            else
            {
                return modelName + "s";
            }
        }

        public async Task<List<ChatHistoryModel>> GetHistory(string userName)
        {

            var user = await _newUserCollection
                                .Find(u => u.ChatUserName == userName)
                                .FirstOrDefaultAsync();

            string chatUserId = user.Id.ToString();

            // Step 2: Get all ChatHistories involving this user
            var chatHistories = _chatHistoryCollection
                                .Find(h => h.ChatFromUserId == chatUserId || h.ChatToUserId == chatUserId)
                                .ToList();

            var latestMessages = _chatMessagesCollection
                                    .Find(m => chatHistories.Any(h => h.Id.ToString() == m.ChatHistoryId))
                                    .SortByDescending(m => m.TimeStamp)
                                    .ToList()
                                    .GroupBy(m => m.ChatHistoryId)
                                    .ToDictionary(g => g.Key, g => g.First());


            var latestIds = chatHistories.Where(x => x.ChatFromUserId == user.Id.ToString()).ToList();

            if(latestIds != null && latestIds.Count != 0)
            {

                chatHistories.RemoveAll(h => latestIds.Any(id => id.Id == h.Id));

            }

            List<ChatHistoryModel> chatHistoryList = new List<ChatHistoryModel>();

            foreach (var history in chatHistories)
            {
                var latestMessage = latestMessages.GetValueOrDefault(history.Id.ToString());

                // Skip if no message found for this history
                if (latestMessage == null)
                    continue;

                //// Determine the from/to user
                //var fromUserId = history.ChatFromUserId == chatUserId ? history.ChatToUserId : history.ChatFromUserId;
                //var toUserId = history.ChatToUserId == chatUserId ? history.ChatFromUserId : history.ChatToUserId;

                // Fetch user details for the "FromUserId"
                var fromUser = _newUserCollection.Find(u => u.Id == new ObjectId(history.ChatFromUserId)).FirstOrDefault();

                if (fromUser == null)
                    continue;

                // Count unread messages for the "FromUserId"
                var unreadMessagesCount = _chatMessagesCollection.CountDocuments(m => m.ChatHistoryId == history.Id.ToString() && m.MessageStatus == "Sent");

                // Create the ChatHistoryModel
                chatHistoryList.Add(new ChatHistoryModel
                {
                    FromUserId = history.ChatFromUserId, 
                    ToUserId = history.ChatToUserId, 
                    UserName = fromUser.ChatUserName,
                    Message = latestMessage.ChatMessage ?? "Image",
                    LoginUserName = userName, 
                    UnreadMessages = unreadMessagesCount,
                    LastSeen = fromUser.LastSeen,
                    LastMessageId = latestMessage.Id.ToString(), 
                    LastStatus = Status(fromUser.LastSeen),
                    UserPhoto = fromUser.ChatUserPhoto
                });

            }

            return chatHistoryList.OrderByDescending(c => c.LastSeen).ToList();


            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@Username", userName);

            //    var history = await connection.QueryAsync<ChatHistoryModel>("UserChatHistory", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return (List<ChatHistoryModel>)history;
            //}

        }

        private string Status(DateTime lastSeen)
        {
            DateTime now = DateTime.UtcNow;

            if (lastSeen >= now.AddSeconds(-10))
            {
                return "Online";
            }
            else if (lastSeen.Date == now.Date)
            {
                return "Away";
            }
            else
            {
                return "Offline";
            }
        }

        public async Task<string> NewUser(string userName, string email, string passWord, string deviceIp)
        {

            NewUserEntity newUser = new NewUserEntity()
            {
                ChatUserName = userName,
                ChatEmail = email,
                ChatPassword = BCrypt.Net.BCrypt.HashPassword(passWord),
                ChatDeviceIp = deviceIp,
                LastSeen = DateTime.Now,
                ChatUserPhoto = "",
                IsActive = false,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now,
            };

            var existingUser = await _newUserCollection.Find(x => x.ChatEmail == email).FirstOrDefaultAsync();

            if (existingUser != null && (existingUser.ChatEmail == email || existingUser.ChatUserName == userName))
            {
                return "Username Or Email Already Exists";
            }

            await _newUserCollection.InsertOneAsync(newUser);

            return "User registered successfully";


            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@userName", userName);
            //    dynamicParameters.Add("@email", email);
            //    dynamicParameters.Add("@passWord", BCrypt.Net.BCrypt.HashPassword(passWord));
            //    dynamicParameters.Add("deviceIp", deviceIp);
            //    dynamicParameters.Add("@message", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

            //    await connection.ExecuteAsync("NewUserRegister", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    string message = dynamicParameters.Get<string>("@message");

            //    return message;
            //}

        }

        public async Task<string> ValidateUserName(string userName)
        {

            var existingUser = await _newUserCollection.Find(x => x.ChatUserName == userName).FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return existingUser.ChatPassword;
            }

            return string.Empty;

            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@userName", userName);

            //    string? userPw = await connection.QueryFirstOrDefaultAsync<string>("ValidateUserName", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return userPw;
            //}
        }

        public async Task<Dictionary<string, string>> ExistingUsers(string existingChatUsers)
        {

            List<string> existingusers = existingChatUsers.Split(",").ToList();

            var filter = Builders<NewUserEntity>.Filter.Nin(user => user.ChatUserName, existingusers);

            var result = await _newUserCollection.Find(filter)
                                     .SortBy(user => user.Id)
                                     .ToListAsync();

            return result.ToDictionary(user => user.Id.ToString(), user => user.ChatUserName);


            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@ExistingChatUsers", existingChatUsers);

            //    var result = await connection.QueryAsync<(int ChatUserId, string ChatUserName)>("ExistingUsers", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return result.ToDictionary(user => user.ChatUserId, user => user.ChatUserName);
            //}

        }

        public async Task<DateTime> GetLastSeen(string userId)
        {

            var result = await _newUserCollection.Find(x => x.Id.ToString() == userId).FirstOrDefaultAsync();

            if (result != null)
            {
                return result.LastSeen;
            }

            return DateTime.Now;

            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@userId", userId);

            //    var result = await connection.QueryFirstAsync<DateTime>("LastSeenStatus", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return result;
            //}
        }

        public async Task<bool> UpdateEmailOtp(string email, int emailOtp)
        {

            var filter = Builders<NewUserEntity>.Filter.Eq(x => x.ChatEmail, email); 

            var update = Builders<NewUserEntity>.Update.Set(x => x.ChatEmailOtp, emailOtp).Set(x =>x.ChatEmailDate, DateTime.UtcNow);

            await _newUserCollection.UpdateOneAsync(filter, update);

            return true;


            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@email", email);
            //    dynamicParameters.Add("@emailOtp", emailOtp);

            //    await connection.ExecuteAsync("UpdateEmailOtp", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return true;
            //}
        }

        public async Task<string> VerifyEmailOtp(string email, int emailOtp)
        {

            DateTime currentTime = DateTime.UtcNow;

            var filter = Builders<NewUserEntity>.Filter.And(
                Builders<NewUserEntity>.Filter.Eq(x => x.ChatEmail, email) 
            );

            var user = await _newUserCollection.Find(filter).FirstOrDefaultAsync();

            if (user != null && user.ChatEmailOtp == emailOtp && user.ChatEmailDate.AddMinutes(+2) >= currentTime)
            {
                var update = Builders<NewUserEntity>.Update.Set(x => x.IsActive, true);
                await _newUserCollection.UpdateOneAsync(x => x.Id == user.Id, update);

                return "Thank you, Email Is Verified";
            }
            else
            {
                return "Email verification failed. Please check the OTP and try again.";
            }


            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@email", email);
            //    dynamicParameters.Add("@emailOtp", emailOtp);
            //    dynamicParameters.Add("@Message", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

            //    await connection.ExecuteAsync("VerifyEmailOtp", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    string result = dynamicParameters.Get<string>("@Message");

            //    return result;
            //}
        }

        public async Task<string> ResendEmailOtp(string email)
        {
            string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(dbConnection))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@email", email);
                dynamicParameters.Add("@Message", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

                await connection.ExecuteAsync("ResendEmailOtp", dynamicParameters, commandType: CommandType.StoredProcedure);

                string result = dynamicParameters.Get<string>("@Message");

                return result;
            }
        }

        public async Task UpdateLastSeen(string userId)
        {

            var filter = Builders<NewUserEntity>.Filter.Eq(x => x.Id.ToString(), userId);

            var update = Builders<NewUserEntity>.Update.Set(x => x.LastSeen, DateTime.UtcNow);

            await _newUserCollection.UpdateOneAsync(filter, update);

            
            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@messageId", userId);

            //    await connection.ExecuteAsync("UpdateLastSeenStatus", dynamicParameters, commandType: CommandType.StoredProcedure);

            //}
        }

        public async Task<bool> InsertHistory(string fromUserId, string toUserId)
        {

            var fromFilter = Builders<ChatHistoryEntity>.Filter.And(
                                Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatFromUserId, fromUserId),
                                Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatToUserId, toUserId)
            );

            var toFilter = Builders<ChatHistoryEntity>.Filter.And(
                            Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatFromUserId, toUserId),
                            Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatToUserId, fromUserId)
            );

            // Check if history exists for fromUserId and toUserId
            var fromHistory = await _chatHistoryCollection.Find(fromFilter).FirstOrDefaultAsync();

            // Check if history exists for toUserId and fromUserId
            var toHistory = await _chatHistoryCollection.Find(toFilter).FirstOrDefaultAsync();

            // If history doesn't exist for fromUserId and toUserId, insert it
            if (fromHistory == null)
            {
                var newHistoryFrom = new ChatHistoryEntity
                {
                    ChatFromUserId = fromUserId,
                    ChatToUserId = toUserId,
                };
                await _chatHistoryCollection.InsertOneAsync(newHistoryFrom);
            }

            // If history doesn't exist for toUserId and fromUserId, insert it
            if (toHistory == null)
            {
                var newHistoryTo = new ChatHistoryEntity
                {
                    ChatFromUserId = toUserId,
                    ChatToUserId = fromUserId,
                };

                await _chatHistoryCollection.InsertOneAsync(newHistoryTo);
            }

            return true;


            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@fromUserId", fromUserId);
            //    dynamicParameters.Add("@toUserId", toUserId);

            //    var result = await connection.ExecuteAsync("InsertHistory", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return true;
            //}

        }

        public async Task<string> LastMessageId(string fromUserId, string toUserId)
        {

            var historyFilter = Builders<ChatHistoryEntity>.Filter.And(
                                Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatFromUserId, toUserId),
                                Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatToUserId, fromUserId)
            );

            var history = await _chatHistoryCollection.Find(historyFilter).FirstOrDefaultAsync();

            if (history == null)
            {
                return string.Empty;
            }

            var messageFilter = Builders<ChatMessagesEntity>.Filter.Eq(x => x.ChatHistoryId, history.Id.ToString());
            var messageSort = Builders<ChatMessagesEntity>.Sort.Descending(x => x.Id.ToString());

            var latestMessage = await _chatMessagesCollection.Find(messageFilter)
                                        .Sort(messageSort).Limit(1).FirstOrDefaultAsync();

            return latestMessage.Id.ToString();

            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@fromUserId", fromUserId);
            //    dynamicParameters.Add("@toUserId", toUserId);

            //    var result = await connection.QueryFirstOrDefaultAsync<int>("LastMessageId", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return result;
            //}

        }

        public async Task<MessageDetailsModel> MessageDetails(string messageId)
        {
            var filter = Builders<ChatMessagesEntity>.Filter.Eq(x => x.Id.ToString(), messageId);
            var message = await _chatMessagesCollection.Find(filter).FirstOrDefaultAsync();

            if (message == null)
            {
                return new MessageDetailsModel(); 
            }

            string deliveryDate = message.DeliveryDate != DateTime.MinValue ? message.DeliveryDate.ToString("dd/MM/yyyy hh:mm tt") : "Not Delivered";

            string seenDate = message.SeenDate != DateTime.MinValue? message.SeenDate.ToString("dd/MM/yyyy hh:mm tt") : "Not Seen";

            // Step 3: Return the formatted message details
            return new MessageDetailsModel
            {
                DeliveryDate = deliveryDate,
                SeenDate = seenDate
            };

            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@messageId", messageId);

            //    var messageDetails = await connection.QueryFirstAsync<MessageDetailsModel>("MessageDetails", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return messageDetails;
            //}
        }

        public async Task<LastMessageModel> LastMessageStatus(string fromUserId, string toUserId, string lastMessageId)
        {
            var historyFilter = Builders<ChatHistoryEntity>.Filter.And(
                                Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatFromUserId, toUserId),
                                Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatToUserId, fromUserId)
            );

            var history = await _chatHistoryCollection.Find(historyFilter).FirstOrDefaultAsync();

            if (history == null)
            {
                return new(); 
            }

            var messageFilter = Builders<ChatMessagesEntity>.Filter.Eq(x => x.ChatHistoryId, history.Id.ToString());
            var messageSort = Builders<ChatMessagesEntity>.Sort.Descending(x => x.Id); 

            var latestMessage = await _chatMessagesCollection.Find(messageFilter)
                .Sort(messageSort)
                .Limit(1)
                .FirstOrDefaultAsync();

            if (latestMessage == null || latestMessage.Id.ToString() == lastMessageId)
            {
                return new();  
            }

            var statusChange = new LastMessageModel
            {
                StatusChange = true, 
                LatestMessageId = latestMessage.Id.ToString(),
                LatestMessage = string.IsNullOrEmpty(latestMessage.ChatMessage) ? "Image" : latestMessage.ChatMessage
            };

            return statusChange;

            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@fromUserId", fromUserId);
            //    dynamicParameters.Add("@toUserId", toUserId);
            //    dynamicParameters.Add("@lastMessageId", lastMessageId);

            //    var messageDetails = await connection.QueryFirstOrDefaultAsync<LastMessageModel>("LastMessageStatus", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return messageDetails == null ? new LastMessageModel() : messageDetails;
            //}
        }

        public async Task<string> Base64Image(string messageId)
        {

            var messageCollection = await _chatMessagesCollection.Find(x => x.Id.ToString() == messageId).FirstOrDefaultAsync();

            if(messageCollection != null)
            {
                return messageCollection.ChatContent;
            }

            return string.Empty;
            
            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@messageId", messageId);

            //    string base64 = await connection.QueryFirstAsync<string>("ImageBase", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return base64;
            //}
        }

        public async Task<List<ChatDetailsModel>> History(string fromUserId, string toUserId)
        {

            var historyFilter = Builders<ChatHistoryEntity>.Filter.And(
                                Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatFromUserId, fromUserId),
                                Builders<ChatHistoryEntity>.Filter.Eq(x => x.ChatToUserId, toUserId)
            );

            //var history = await _chatHistoryCollection.Find(historyFilter).ToListAsync();

            var chatHistories = _chatHistoryCollection
                    .Find(h => h.ChatFromUserId == fromUserId || h.ChatToUserId == fromUserId)
                    .ToList();

            //if (history == null || history.Count == 0)
            //{
            //    return new List<ChatDetailsModel>();
            //}

            var distinctDates = await _chatMessagesCollection.Aggregate()
                .Match(Builders<ChatMessagesEntity>.Filter.In(
                    x => x.ChatHistoryId, chatHistories.Select(h => h.Id.ToString())))
                .Group(x => new { Date = new DateTime(x.TimeStamp.Year, x.TimeStamp.Month, x.TimeStamp.Day) },
                    g => new { Date = g.Key.Date })
                .SortByDescending(x => x.Date)
                .Limit(5)
                .ToListAsync();


            if (distinctDates.Count == 0)
            {
                return new List<ChatDetailsModel>();
            }


            var dateList = distinctDates.Select(d => d.Date).ToList();

            // Create a filter to match messages that fall within each distinct date
            var dateFilters = dateList.Select(date => Builders<ChatMessagesEntity>.Filter.And(
                Builders<ChatMessagesEntity>.Filter.Gte(x => x.TimeStamp, date), // TimeStamp >= date
                Builders<ChatMessagesEntity>.Filter.Lt(x => x.TimeStamp, date.AddDays(1)) // TimeStamp < date + 1 day
            )).ToList();

            // Combine all the date filters with an OR condition
            var messagesFilter = Builders<ChatMessagesEntity>.Filter.Or(dateFilters);

            var messages = await _chatMessagesCollection.Find(messagesFilter)
                .SortBy(x => x.TimeStamp)
                .ToListAsync();


            var user = await _newUserCollection.Find(x => x.Id.ToString() == toUserId).FirstOrDefaultAsync();

            var chatDetails = messages.Select(msg =>
            {
                return new ChatDetailsModel
                {
                    MessageId = msg.Id.ToString(),
                    Status = msg.MessageStatus,
                    UserName = user.ChatUserName,
                    FromUserId = chatHistories.Where(x =>x.Id.ToString() == msg.ChatHistoryId).Select(x =>x.ChatFromUserId).First(),
                    ToUserId = chatHistories.Where(x =>x.Id.ToString() == msg.ChatHistoryId).Select(x =>x.ChatToUserId).First(),
                    Message = string.IsNullOrEmpty(msg.ChatMessage) ? "Image" : msg.ChatMessage,
                    DataContent = msg.ChatContent,
                    TimeStamp = msg.TimeStamp
                };
            }).ToList();

            return chatDetails;

            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@FromUserId", fromChatId);
            //    dynamicParameters.Add("@ToUserId", toChatId);

            //    var history = await connection.QueryAsync<ChatDetailsModel>("ChatDetails", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return (List<ChatDetailsModel>)history;
            //}
        }

        public async Task<string> SaveHistory(string fromUserId, string toUserId, string? message, string? imageBytes)
        {

            var chatHistory = await _chatHistoryCollection
                                .Find(h => h.ChatFromUserId == fromUserId && h.ChatToUserId == toUserId)
                                .FirstOrDefaultAsync();

            var newMessage = new ChatMessagesEntity
            {
                ChatHistoryId = chatHistory.Id.ToString(),
                TimeStamp = DateTime.UtcNow,
                DeliveryDate = DateTime.UtcNow,
                SeenDate = DateTime.MinValue, // Assuming not seen yet
                MessageStatus = "Sent", // Default status, you can modify based on business logic
            };

            // Step 3: Insert message or image depending on the provided data
            if (!string.IsNullOrEmpty(message))
            {
                newMessage.ChatMessage = message;
                newMessage.ChatContent = string.Empty;
            }
            else if (imageBytes != null && imageBytes.Length > 0)
            {
                newMessage.ChatMessage = string.Empty; // No text message
                newMessage.ChatContent = imageBytes; // Store image as Base64 string
            }

            // Step 4: Insert into MongoDB
            _chatMessagesCollection.InsertOne(newMessage);

            return newMessage.Id.ToString();


            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@FromUserId", fromUserId);
            //    dynamicParameters.Add("@ToUserId", toUserId);
            //    dynamicParameters.Add("@Message", message);
            //    dynamicParameters.Add("@ImageBytes", imageBytes);

            //    int messageId = await connection.QueryFirstAsync<int>("SaveChatHistory", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return messageId;
            //}
        }

        public async Task<string> GetUserNameById(string userId)
        {

            var chatMaster = await _newUserCollection.Find(x => x.Id.ToString() == userId).FirstOrDefaultAsync();

            if(chatMaster != null)
            {
                return chatMaster.ChatUserName;
            }

            return string.Empty;
            
            
            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@userId", userId);

            //    string? userName = await connection.QueryFirstOrDefaultAsync<string>("GetUserName", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return userName != null ? userName : string.Empty;
            //}
        }

        public async Task<string> GetIdByUsername(string userName)
        {

            var chatMaster = await _newUserCollection.Find(x => x.ChatUserName == userName).FirstOrDefaultAsync();

            if (chatMaster != null)
            {
                return chatMaster.Id.ToString();
            }

            return string.Empty;

            //string dbConnection = _configuration.GetConnectionString("DefaultConnection");

            //using (SqlConnection connection = new SqlConnection(dbConnection))
            //{
            //    DynamicParameters dynamicParameters = new DynamicParameters();
            //    dynamicParameters.Add("@userName", userName);

            //    int userId = await connection.QueryFirstOrDefaultAsync<int>("GetIdbyUserName", dynamicParameters, commandType: CommandType.StoredProcedure);

            //    return userId;
            //}
        }
    }
}
