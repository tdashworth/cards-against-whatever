using CardsAgainstWhatever.Server.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services
{
    public class HubContextFacade<THub, T> : IHubContextFacade<T>
        where THub : Hub<T>
        where T : class
    {
        private readonly IHubContext<THub, T> hubContext;

        public HubContextFacade(IHubContext<THub, T> hubContext)
        {
            this.hubContext = hubContext;
        }

        public T GetClient(string connectionId) => hubContext.Clients.Client(connectionId);

        public T GetGroup(string groupName) => hubContext.Clients.Group(groupName);

        public Task JoinGroup(string groupName, string connectionId) => hubContext.Groups.AddToGroupAsync(connectionId, groupName);

        public Task LeaveGroup(string groupName, string connectionId) => hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
    }
}
