using CardsAgainstWhatever.Shared.Dtos;
using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameServer
    {
        Task<Response> JoinGame(string gameCode, string username);
        Task<Response> StartRound();
        Task<Response> PlayAnswer(IEnumerable<AnswerCard> answerCards);
        Task<Response> PickWinningAnswer(IEnumerable<AnswerCard> answerCards);
        Task<Response> LeaveGame();
    }
}
