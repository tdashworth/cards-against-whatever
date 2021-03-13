using BlazorComponentBus;
using CardsAgainstWhatever.Shared.Dtos.Actions;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Services
{
    public class GameServerProxy : IGameServer, IGameServerProxy
    {
        private readonly HttpClient HttpClient;
        private readonly NavigationManager NavigationManager;
        private readonly ComponentBus Bus;
        private readonly IGameClient GameClient;

        private HubConnection HubConnection;

        public GameServerProxy(HttpClient httpClient, NavigationManager navigationManager, ComponentBus bus, IGameClient gameClient)
        {
            HttpClient = httpClient;
            NavigationManager = navigationManager;
            Bus = bus;
            GameClient = gameClient;
        }

        public async Task<GameCreatedEvent> CreateGame(CreateGameAction action)
        {
            var response = await HttpClient.PostAsJsonAsync("api/game", action);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<GameCreatedEvent>();
            return content;
        }

        public async Task JoinGame(JoinGameAction action)
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(
                    NavigationManager.ToAbsoluteUri($"/gamehub?GameCode={action.GameCode}&Username={action.Username}")
                )
                .WithAutomaticReconnect()
                .Build();

            RegisterListeners();

            await HubConnection.StartAsync();

            HubConnection.Closed += (Exception ex) => { Console.WriteLine(ex); return Task.CompletedTask;  };
        }

        public Task StartRound(StartRoundAction action) => HubConnection.InvokeAsync(nameof(StartRound), action);

        public Task PlayAnswer(PlayAnswerAction action) => HubConnection.InvokeAsync(nameof(PlayAnswer), action);

        public Task PickWinningAnswer(PickWinnerAnswerAction action) => HubConnection.InvokeAsync(nameof(PickWinningAnswer), action);

        private void RegisterListeners()
        {
            HubConnection.On<GameJoinedEvent>(nameof(IGameClient.GameJoined), GameClient.GameJoined);
            HubConnection.On<PlayerJoinedEvent>(nameof(IGameClient.PlayerJoined), GameClient.PlayerJoined);
            HubConnection.On<RoundStartedEvent>(nameof(IGameClient.RoundStarted), GameClient.RoundStarted);
            HubConnection.On<PlayerMovedEvent>(nameof(IGameClient.PlayerMoved), GameClient.PlayerMoved);
            HubConnection.On<RoundClosedEvent>(nameof(IGameClient.RoundClosed), GameClient.RoundClosed);
            HubConnection.On<RoundEndedEvent>(nameof(IGameClient.RoundEnded), GameClient.RoundEnded);
        }
    }
}
