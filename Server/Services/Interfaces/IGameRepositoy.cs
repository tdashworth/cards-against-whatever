using CardsAgainstWhatever.Server.Models;
using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services.Interfaces
{
    public interface IGameRepository
    {
        Task<string> Create(IEnumerable<QuestionCard> questionCards, IEnumerable<AnswerCard> answerCards);
        Task Delete(string code);
        Task<ServerGame> GetByCode(string code);
    }
}
