using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameServer
    {
        Task JoinGame(string gameCode, string username);
        Task StartRound();
        Task PlayAnswer(IEnumerable<AnswerCard> answerCards);
        Task PickWinningAnswer(IEnumerable<AnswerCard> answerCards);
        Task LeaveGame();
    }
}
