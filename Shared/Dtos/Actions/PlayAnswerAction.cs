using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;

namespace CardsAgainstWhatever.Shared.Dtos.Actions
{
    public record PlayAnswerAction(
        string GameCode,
        string Username,
        List<AnswerCard> PlayedCards);
}
