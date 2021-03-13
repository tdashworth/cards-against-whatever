using CardsAgainstWhatever.Shared.Dtos.Actions;
using CardsAgainstWhatever.Shared.Dtos.Events;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameServer
    {
        Task StartRound(StartRoundAction startRoundEvent);
        Task PlayAnswer(PlayAnswerAction playCardsEvent);
        Task PickWinningAnswer(PickWinnerAnswerAction playCardsEvent);
    }
}
