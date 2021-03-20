using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameClient
    {
        Task GameJoined(
            GameStatus gameStatus,
            List<Player> existingPlayersInGame,
            int? currentRoundNumber,
            QuestionCard? currentQuestion,
            Player? currentCardCzar);

        Task PlayerJoined(Player newPlayer);

        Task PlayerLeft(Player newPlayer);

        Task RoundStarted(
            int currentRoundNumber,
            QuestionCard currentQuestion,
            Player currentCardCzar,
            List<AnswerCard> dealtCards);

        Task PlayerMoved(Player player);

        Task RoundClosed(List<List<AnswerCard>> playedCardsGroupedPerPlayer);

        Task RoundEnded(Player winningPlayer);
    }
}
