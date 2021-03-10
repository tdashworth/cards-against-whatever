using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Events
{
    public class WinningCardSelectedEvent
    {
        public List<AnswerCard> SelectedWinningCards { get; set; }
    }
}
