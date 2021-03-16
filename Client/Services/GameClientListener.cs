using CardsAgainstWhatever.Client.Stores.Game;
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

        public Task GameJoined(List<Player> existingPlayersInGame, int? currentRoundNumber, QuestionCard? currentQuestion, Player? currentCardCzar)
            => Task.Run(() => dispatcher.Dispatch(new GameJoinedEvent(existingPlayersInGame, currentRoundNumber, currentQuestion, currentCardCzar)));

        public Task PlayerJoined(Player newPlayer)
            => Task.Run(() => dispatcher.Dispatch(new PlayerJoinedEvent(newPlayer)));

        public Task RoundStarted(int currentRoundNumber, QuestionCard currentQuestion, Player currentCardCzar, List<AnswerCard> dealtCards)
            => Task.Run(() => dispatcher.Dispatch(new RoundStartedEvent(currentRoundNumber, currentQuestion, currentCardCzar.Username, dealtCards)));

        public Task PlayerMoved(Player player)
            => Task.Run(() => dispatcher.Dispatch(new AnswerPlayedEvent(player.Username)));

        public Task RoundClosed(List<List<AnswerCard>> playedCardsGroupedPerPlayer)
            => Task.Run(() => dispatcher.Dispatch(new RoundClosedEvent(playedCardsGroupedPerPlayer)));

        public Task RoundEnded(Player winningPlayer)
            => Task.Run(() => dispatcher.Dispatch(new RoundEndedEvent(winningPlayer.Username)));
    }
}
