using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Models;
using ChatBot.Repoistory.Interfaces;

namespace ChatBot.BusinessLayer.Classes
{
    public class GetUserChatHistory : IGetUserChatHistory
    {
        private readonly IChatBotRepo _chatBot;
        private readonly ILastSeenStatus _lastSeenStatus;

        public GetUserChatHistory(IChatBotRepo chatBot, ILastSeenStatus lastSeenStatus)
        {
            _chatBot = chatBot;
            _lastSeenStatus = lastSeenStatus;
        }

        public async Task<List<ChatHistoryModel>> History(string userName, string passWord, bool loggedInUser)
        {
            if (!loggedInUser)
            {

                string? userPw = await _chatBot.ValidateUserName(userName);

                if (string.IsNullOrEmpty(userPw))
                {
                    return new List<ChatHistoryModel>();
                }
                else
                {
                    bool isValid = BCrypt.Net.BCrypt.Verify(passWord, userPw);

                    if (!isValid) { return new List<ChatHistoryModel>(); }
                }
            }

            var histories = await _chatBot.GetHistory(userName);

            if (histories.Count == 0)
            {

                histories.Add(new ChatHistoryModel());
                histories.First().FromUserId = await _chatBot.GetIdByUsername(userName);

            }

            foreach (var history in histories)
            {
                history.LastStatus = _lastSeenStatus.Status(history.LastSeen);
                history.LastMessageId = await _chatBot.LastMessageId(history.FromUserId, history.ToUserId);
            }

            string existingChatUsers = string.Join(",", histories.Select(x => x.UserName).Append(userName));

            histories.First().ExistingUsers = await _chatBot.ExistingUsers(existingChatUsers);

            return histories;
        }
    }
}
