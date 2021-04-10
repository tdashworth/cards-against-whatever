using CardsAgainstWhatever.Client.Stores.Game;
using CardsAgainstWhatever.Client.Stores.Toasts;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using Fluxor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Services
{
    public class GameClientListener : IGameClient
    {
        private readonly IDispatcher dispatcher;

        public GameClientListener(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task GameJoined(GameStatus gameStatus, IEnumerable<Player> existingPlayersInGame, IEnumerable<AnswerCard> cardsInHand, IEnumerable<IList<AnswerCard>> cardsOnTable, int? currentRoundNumber, QuestionCard? currentQuestion, Player? currentCardCzar)
        {
            dispatcher.Dispatch(new GameJoinedEvent(gameStatus, existingPlayersInGame, cardsInHand, cardsOnTable, currentRoundNumber, currentQuestion, currentCardCzar));
            return Task.CompletedTask;
        }

        public Task PlayerJoined(Player newPlayer)
        {
            dispatcher.Dispatch(new PlayerJoinedEvent(newPlayer));
            dispatcher.Dispatch(new AddToast(new Toast("New Player", $"{newPlayer.Username} has joined the game!")));
            return Task.CompletedTask;
        }

        public Task PlayerLeft(Player newPlayer)
        {
            dispatcher.Dispatch(new PlayerLeftEvent(newPlayer));
            dispatcher.Dispatch(new AddToast(new Toast("Player Left", $"{newPlayer.Username} has left the game 👋")));
            return Task.CompletedTask;
        }

        public Task RoundStarted(int currentRoundNumber, QuestionCard currentQuestion, Player currentCardCzar, IEnumerable<AnswerCard> dealtCards)
        {
            dispatcher.Dispatch(new RoundStartedEvent(currentRoundNumber, currentQuestion, currentCardCzar.Username, dealtCards));
            return Task.CompletedTask;
        }

        public Task PlayerMoved(Player player)
        {
            dispatcher.Dispatch(new AnswerPlayedEvent(player.Username));
            return Task.CompletedTask;
        }

        public Task RoundClosed(IEnumerable<IEnumerable<AnswerCard>> playedCardsGroupedPerPlayer)
        {
            dispatcher.Dispatch(new RoundClosedEvent(playedCardsGroupedPerPlayer));
            return Task.CompletedTask;
        }

        public Task RoundEnded(Player winningPlayer)
        {
            dispatcher.Dispatch(new RoundEndedEvent(winningPlayer.Username));
            dispatcher.Dispatch(new AddToast(new Toast("Round Ended", $"Well done {winningPlayer.Username}! You won the game 🎉")));
            return Task.CompletedTask;
        }
    }
}
