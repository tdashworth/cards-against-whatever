using CardsAgainstWhatever.Server.Models;
using CardsAgainstWhatever.Shared;
using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services.Interfaces
{
    public interface IGameRepositoy
    {
        Task<string> Create(IEnumerable<QuestionCard> questionCards, IEnumerable<AnswerCard> answerCards);

        Task<ServerGame> GetByCode(string code);
    }
}
