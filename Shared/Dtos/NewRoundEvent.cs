using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos
{
    public class NewRoundEvent
    {
        public int RoundNumber { get; set; }
        public List<AnswerCard> DealtCards { get; set; }
        public QuestionCard QuestionCard { get; set; }
    }
}
