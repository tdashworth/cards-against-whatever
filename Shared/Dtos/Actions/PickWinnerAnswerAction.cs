using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos.Actions
{
    public class PickWinnerAnswerAction
    {
        public string GameCode { get; set; }
        public List<AnswerCard> WinningCards { get; set; }
    }
}
