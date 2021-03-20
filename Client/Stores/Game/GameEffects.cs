using CardsAgainstWhatever.Shared.Interfaces;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Strong;
using System;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Stores.Game
{
    public class GameEffects
    {
        private readonly NavigationManager NavigationManager;
        private readonly IServiceProvider ServiceProvider;
        private readonly IState<GameState> GameState;

        private HubConnection? HubConnection;
        private IGameServer? Server;

        public GameEffects(NavigationManager navigationManager, IServiceProvider serviceProvider, IState<GameState> gameState)
        {
            NavigationManager = navigationManager;
            ServiceProvider = serviceProvider;
            GameState = gameState;
        }

        [EffectMethod]
        public async Task Handle(JoinGameAction action, IDispatcher dispatcher)
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(
                    NavigationManager.ToAbsoluteUri($"/gamehub?GameCode={action.GameCode}&Username={action.Username}")
                )
                .WithAutomaticReconnect()
                .Build();

            var gameClient = ServiceProvider.GetService(typeof(IGameClient));
            HubConnection.RegisterSpoke<IGameClient>(gameClient);
            Server = HubConnection.AsGeneratedHub<IGameServer>();

            await HubConnection.StartAsync();
        }

        [EffectMethod]
        public async Task Handle(StartRoundAction action, IDispatcher dispatcher)
        {
            if (Server is null) return;

            await Server.StartRound();
        }

        [EffectMethod]
        public async Task Handle(PlayAnswerAction action, IDispatcher dispatcher)
        {
            if (Server is null || GameState.Value.SelectedCardsInHand is null) return;

            await Server.PlayAnswer(GameState.Value.SelectedCardsInHand);
        }

        [EffectMethod]
        public async Task Handle(PickWinnerAction action, IDispatcher dispatcher)
        {
            if (Server is null || GameState.Value.SelectedCardsOnTable is null) return;

            await Server.PickWinningAnswer(GameState.Value.SelectedCardsOnTable);
        }

        [EffectMethod]
        public async Task Handle(LeaveGameAction action, IDispatcher dispatcher)
        {
            if (HubConnection is not null)
            {
                await HubConnection.StopAsync();
            }

            HubConnection = null;
            Server = null;
        }
    }
}
