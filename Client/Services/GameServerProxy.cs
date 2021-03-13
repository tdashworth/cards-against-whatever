using CardsAgainstWhatever.Shared.Dtos.Actions;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Services
{
    public class GameServerProxy : IGameServer
    {
        private readonly HubConnection HubConnection;
        private readonly IGameClient GameClient;

        public GameServerProxy(HubConnection hubConnection, IGameClient gameClient)
        {
            HubConnection = hubConnection;
            GameClient = gameClient;
        }

        public Task<GameCreatedEvent> CreateGame(CreateGameAction action) => HubConnection.InvokeAsync<GameCreatedEvent>(nameof(CreateGame), action);

        public Task<GameJoinedEvent> JoinGame(JoinGameAction action) => HubConnection.InvokeAsync<GameJoinedEvent>(nameof(JoinGame), action);

        public Task StartRound(StartRoundAction action) => HubConnection.InvokeAsync(nameof(StartRound), action);

        public Task PlayAnswer(PlayAnswerAction action) => HubConnection.InvokeAsync(nameof(PlayAnswer), action);

        public Task PickWinningAnswer(PickWinnerAnswerAction action) => HubConnection.InvokeAsync(nameof(PickWinningAnswer), action);

        private void RegisterListeners()
        {
            HubConnection.On<PlayerJoinedEvent>(nameof(IGameClient.PlayerJoined), GameClient.PlayerJoined);
            HubConnection.On<RoundStartedEvent>(nameof(IGameClient.RoundStarted), GameClient.RoundStarted);
            HubConnection.On<PlayerMovedEvent>(nameof(IGameClient.PlayerMoved), GameClient.PlayerMoved);
            HubConnection.On<RoundClosedEvent>(nameof(IGameClient.RoundClosed), GameClient.RoundClosed);
            HubConnection.On<RoundEndedEvent>(nameof(IGameClient.RoundEnded), GameClient.RoundEnded);
        }
    }
}
