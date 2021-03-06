using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos
{
    public class AllMovesPlayedEvent
    {
        public List<List<AnswerCard>> PlayedCardsGroupedPerPlayer { get; set; }
    }
}
