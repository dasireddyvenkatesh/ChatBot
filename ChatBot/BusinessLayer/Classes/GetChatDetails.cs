using ChatBot.BusinessLayer.Interfaces;
using ChatBot.Models;
using ChatBot.Repoistory.Interfaces;

namespace ChatBot.BusinessLayer.Classes
{
    public class GetChatDetails : IGetChatDetails
    {
        private readonly IChatBotRepo _chatBotRepo;

        public GetChatDetails(IChatBotRepo chatBotRepo)
        {
            _chatBotRepo = chatBotRepo;
        }

        public async Task<List<ChatDetailsModel>> GetChat(int fromUserId, int toUserId)
        {
            var chatDetails = await _chatBotRepo.History(fromUserId, toUserId);

            List<ChatDetailsModel> result = new List<ChatDetailsModel>();

            var messagesByDate = chatDetails.GroupBy(msg => msg.TimeStamp.Date).ToDictionary(grp => grp.Key, grp => grp.ToList());

            if (messagesByDate.Count == 0)
            {
                ChatDetailsModel chatDetailsModel = new ChatDetailsModel()
                {
                    FromUserId = fromUserId,
                    ToUserId = toUserId,
                    UserName = await _chatBotRepo.GetUserNameById(toUserId),
                    LoginUserName = ChatHub.LoginUserName
                };

                result.Add(chatDetailsModel);

                return result;
            }


            foreach (var msgDate in messagesByDate)
            {
                ChatDetailsModel detailsModel = new ChatDetailsModel();

                detailsModel.MessageDate = msgDate.Key.ToString("ddd, dd MMM");
                detailsModel.FromUserId = fromUserId; detailsModel.ToUserId = toUserId;
                detailsModel.UserName = msgDate.Value.First().UserName;
                detailsModel.LoginUserName = ChatHub.LoginUserName;

                foreach (var msg in msgDate.Value)
                {
                    if (msg.FromUserId == fromUserId)
                    {
                        detailsModel.MessageIds.Add(msg.MessageId);
                        detailsModel.MessageStatus.Add(msg.Status);
                        detailsModel.Messages.Add(msg.Message);
                        detailsModel.MessageTime.Add(msg.TimeStamp.ToShortTimeString());
                        detailsModel.ImageBase64.Add(msg.DataContent);
                        detailsModel.IsCurrentUser.Add(true);
                    }
                    else
                    {
                        detailsModel.MessageIds.Add(msg.MessageId);
                        detailsModel.MessageStatus.Add(msg.Status);
                        detailsModel.Messages.Add(msg.Message);
                        detailsModel.MessageTime.Add(msg.TimeStamp.ToShortTimeString());
                        detailsModel.ImageBase64.Add(msg.DataContent);
                        detailsModel.IsCurrentUser.Add(false);
                    }

                }

                result.Add(detailsModel);
            }

            return result;
        }
    }
}
