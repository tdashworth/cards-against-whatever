using CardsAgainstWhatever.Shared.Interfaces;
using Fluxor;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Stores.Server
{
    public enum ServerStatus
    {
        Connecting,
        Connected,
        Reconnecting,
        Disconnected
    }

    public record ServerState(
        bool IsLoading,
        string? ErrorMessage,
        ServerStatus Status,
        HubConnection? HubConnection,
        IGameServer? GameServer);

    public class GameFeature : Feature<ServerState>
    {
        public override string GetName() => "Server";

        protected override ServerState GetInitialState() => new ServerState(false, null, ServerStatus.Disconnected, null, null);
    }
}
