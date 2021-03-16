using Microsoft.AspNetCore.SignalR;

namespace CardsAgainstWhatever.Server.Services
{
    public class QueryStringUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
            => connection.GetHttpContext()?.Request?.Query["Username"] ?? string.Empty;

    }
}
