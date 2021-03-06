using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos.Actions
{
    public class PlayMovePlayerAction
    {
        public string GameCode { get;  set; }
        public string Username { get; set; }
        public List<AnswerCard> PlayedCards { get; set; }
    }
}
