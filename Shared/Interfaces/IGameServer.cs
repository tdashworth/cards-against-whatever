using CardsAgainstWhatever.Shared.Dtos.Actions;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameServer
    {
        Task StartRound();
        Task PlayAnswer(IEnumerable<AnswerCard> answerCards);
        Task PickWinningAnswer(IEnumerable<AnswerCard> answerCards);
    }
}
