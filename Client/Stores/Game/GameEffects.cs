using CardsAgainstWhatever.Shared.Interfaces;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignalR.Strong;

namespace CardsAgainstWhatever.Client.Stores.Game
{
    public class GameEffects
    {
        private readonly NavigationManager NavigationManager;
        private readonly IServiceProvider ServiceProvider;
        private readonly IState<GameState> GameState;

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
            var connection = new HubConnectionBuilder()
                .WithUrl(
                    NavigationManager.ToAbsoluteUri($"/gamehub?GameCode={action.GameCode}&Username={action.Username}")
                )
                .WithAutomaticReconnect()
                .Build();

            var gameClient = ServiceProvider.GetService(typeof(IGameClient));
            connection.RegisterSpoke<IGameClient>(gameClient);
            Server = connection.AsGeneratedHub<IGameServer>();

            await connection.StartAsync();
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
            if (Server is null) return;

            await Server.PlayAnswer(GameState.Value.SelectedCardsInHand);
        }

        [EffectMethod]
        public async Task Handle(PickWinnerAction action, IDispatcher dispatcher)
        {
            if (Server is null) return;

            await Server.PickWinningAnswer(GameState.Value.SelectedCardsOnTable);
        }
    }
}
