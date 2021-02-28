using CardsAgainstWhatever.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services.Interfaces
{
    public interface IGameService
    {
        Task<string> Create(IEnumerable<QuestionCard> questionCards, IEnumerable<AnswerCard> answerCards);

        Task Join(string gameCode, string username);

        Task Leave(string gameCode, string username);
    }
}
