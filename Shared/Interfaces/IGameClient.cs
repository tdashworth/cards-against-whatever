using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameClient
    {
        Task GameJoined(
            GameStatus gameStatus,
            IEnumerable<Player> existingPlayersInGame,
            IEnumerable<AnswerCard> cardsInHand,
            IEnumerable<IList<AnswerCard>> cardsOnTable,
            int? currentRoundNumber,
            QuestionCard? currentQuestion,
            Player? currentCardCzar);

        Task PlayerJoined(Player newPlayer);

        Task PlayerLeft(Player newPlayer);

        Task RoundStarted(
            int currentRoundNumber,
            QuestionCard currentQuestion,
            Player currentCardCzar,
            IEnumerable<AnswerCard> dealtCards);

        Task PlayerMoved(Player player);

        Task RoundClosed(IEnumerable<IEnumerable<AnswerCard>> playedCardsGroupedPerPlayer);

        Task RoundEnded(Player winningPlayer);
    }
}
