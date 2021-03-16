using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;

namespace CardsAgainstWhatever.Shared.Dtos.Actions
{
    public record PickWinnerAnswerAction(
        string GameCode,
        List<AnswerCard> WinningCards);
}
