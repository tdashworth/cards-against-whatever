using CardsAgainstWhatever.Shared.Interfaces;
using Fluxor;
using Microsoft.AspNetCore.SignalR.Client;

namespace CardsAgainstWhatever.Client.Stores.Server
{
    public record ConnectAction()
    {
        [ReducerMethod]
        public static ServerState Reduce(ServerState state, ConnectAction action)
            => state with
            {
                IsLoading = true,
                Status = ServerStatus.Connecting,
            };
    }

    public record ConnectedEvent(HubConnection HubConnection, IGameServer GameServer)
    {
        [ReducerMethod]
        public static ServerState Reduce(ServerState state, ConnectedEvent action)
            => state with
            {
                IsLoading = false,
                Status = ServerStatus.Connected,
                HubConnection = action.HubConnection,
                GameServer = action.GameServer
            };
    }

    public record ReconnectingEvent()
    {
        [ReducerMethod]
        public static ServerState Reduce(ServerState state, ReconnectingEvent action)
            => state with
            {
                IsLoading = true,
                Status = ServerStatus.Reconnecting,
            };
    }

    public record ReconnectedEvent()
    {
        [ReducerMethod]
        public static ServerState Reduce(ServerState state, ReconnectedEvent action)
            => state with
            {
                IsLoading = false,
                Status = ServerStatus.Connected,
            };
    }

    public record DisconnectedEvent()
    {
        [ReducerMethod]
        public static ServerState Reduce(ServerState state, DisconnectedEvent action)
            => state with
            {
                IsLoading = false,
                Status = ServerStatus.Disconnected,
                HubConnection = null,
                GameServer = null,
            };
    }
}
