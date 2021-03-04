using CardsAgainstWhatever.Server.Models;
using CardsAgainstWhatever.Shared;
using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services.Interfaces
{
    public interface IGameService
    {
        Task<string> Create(IEnumerable<QuestionCard> questionCards, IEnumerable<AnswerCard> answerCards);

        Task<Player> Join(string gameCode, string username, string connectionId);

        Task Leave(string gameCode, string username);
        Task<List<Player>> GetPlayers(string gameCode);
        Task StartRound(string gameCode);
    }
}
