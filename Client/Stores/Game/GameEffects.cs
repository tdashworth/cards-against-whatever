using CardsAgainstWhatever.Client.Stores.Server;
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
        private readonly IState<ServerState> ServerState;
        private readonly IState<GameState> GameState;

        public GameEffects(IState<ServerState> serverState, IState<GameState> gameState)
        {
            ServerState = serverState;
            GameState = gameState;
        }

        [EffectMethod]
        public async Task Handle(JoinGameAction action, IDispatcher dispatcher)
        {
            if (ServerState.Value.Status != ServerStatus.Connected || ServerState.Value.GameServer is null) return;

            await ServerState.Value.GameServer.JoinGame(action.GameCode, action.Username);
        }

        [EffectMethod]
        public async Task Handle(StartRoundAction action, IDispatcher dispatcher)
        {
            if (ServerState.Value.Status != ServerStatus.Connected || ServerState.Value.GameServer is null) return;

            await ServerState.Value.GameServer.StartRound();
        }

        [EffectMethod]
        public async Task Handle(PlayAnswerAction action, IDispatcher dispatcher)
        {
            if (ServerState.Value.Status != ServerStatus.Connected || ServerState.Value.GameServer is null) return;
            if (GameState.Value.SelectedCardsInHand is null) return;

            await ServerState.Value.GameServer.PlayAnswer(GameState.Value.SelectedCardsInHand);
        }

        [EffectMethod]
        public async Task Handle(PickWinnerAction action, IDispatcher dispatcher)
        {
            if (ServerState.Value.Status != ServerStatus.Connected || ServerState.Value.GameServer is null) return;
            if (GameState.Value.SelectedCardsOnTable is null) return;

            await ServerState.Value.GameServer.PickWinningAnswer(GameState.Value.SelectedCardsOnTable);
        }

        [EffectMethod]
        public async Task Handle(LeaveGameAction action, IDispatcher dispatcher)
        {
            if (ServerState.Value.Status != ServerStatus.Connected || ServerState.Value.GameServer is null) return;

            await ServerState.Value.GameServer.LeaveGame();
        }
    }
}
