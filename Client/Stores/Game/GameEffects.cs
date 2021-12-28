using CardsAgainstWhatever.Client.Stores.Server;
using CardsAgainstWhatever.Client.Stores.Toasts;
using CardsAgainstWhatever.Shared.Models;
using Fluxor;
using System.Collections.Generic;
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

            var response = await ServerState.Value.GameServer.JoinGame(action.GameCode.ToUpper(), action.Username.Trim());

            if (response is not null && response.ErrorMessage is not null)
            {
                dispatcher.Dispatch(new GameJoinedFailedEvent(response.ErrorMessage));
            }
        }

        [EffectMethod]
        public async Task Handle(StartRoundAction action, IDispatcher dispatcher)
        {
            if (ServerState.Value.Status != ServerStatus.Connected || ServerState.Value.GameServer is null) return;

            var response = await ServerState.Value.GameServer.StartRound();

            if (response is not null && response.ErrorMessage is not null)
            {
                dispatcher.Dispatch(new AddToast(new Toast("Oops, something when wrong!", response.ErrorMessage)));
            }
        }

        [EffectMethod]
        public async Task Handle(PlayAnswerAction action, IDispatcher dispatcher)
        {
            if (ServerState.Value.Status != ServerStatus.Connected || ServerState.Value.GameServer is null) return;
            if (GameState.Value.SelectedCardsInHand is null) return;

            var response = await ServerState.Value.GameServer.PlayAnswer((IReadOnlyList<AnswerCard>) GameState.Value.SelectedCardsInHand);

            if (response is not null && response.ErrorMessage is not null)
            {
                dispatcher.Dispatch(new AddToast(new Toast("Oops, something when wrong!", response.ErrorMessage)));
            }
        }

        [EffectMethod]
        public async Task Handle(PickWinnerAction action, IDispatcher dispatcher)
        {
            if (ServerState.Value.Status != ServerStatus.Connected || ServerState.Value.GameServer is null) return;
            if (GameState.Value.SelectedCardsOnTable is null) return;

            var response = await ServerState.Value.GameServer.PickWinningAnswer(GameState.Value.SelectedCardsOnTable);

            if (response is not null && response.ErrorMessage is not null)
            {
                dispatcher.Dispatch(new AddToast(new Toast("Oops, something when wrong!", response.ErrorMessage)));
            }
        }

        [EffectMethod]
        public async Task Handle(LeaveGameAction action, IDispatcher dispatcher)
        {
            if (ServerState.Value.Status != ServerStatus.Connected || ServerState.Value.GameServer is null) return;

            var response = await ServerState.Value.GameServer.LeaveGame();

            if (response is not null && response.ErrorMessage is not null)
            {
                dispatcher.Dispatch(new AddToast(new Toast("Oops, something when wrong!", response.ErrorMessage)));
            }
        }
    }
}
