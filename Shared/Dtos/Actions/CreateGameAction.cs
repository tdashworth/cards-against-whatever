using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;

namespace CardsAgainstWhatever.Shared.Dtos.Actions
{
    public record CreateGameAction(
        List<QuestionCard> QuestionCards,
        List<AnswerCard> AnswerCards);
}
