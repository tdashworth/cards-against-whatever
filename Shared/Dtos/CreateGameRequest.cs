using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos
{
    public class CreateGameRequest
    {
        public List<QuestionCard> QuestionCards { get; set; }
        public List<AnswerCard> AnswerCards { get; set; }
    }
}
