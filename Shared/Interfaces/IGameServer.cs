using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameServer
    {
        Task<string> CreateGame(List<QuestionCard> questionCards, List<AnswerCard> answerCards);

        Task<List<Player>> JoinGame(string code, string username);

        Task StartRound(string code);
    }
}
