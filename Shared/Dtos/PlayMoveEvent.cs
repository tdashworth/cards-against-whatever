using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos
{
    public class PlayMoveEvent
    {
        public string GameCode { get;  set; }
        public string Username { get; set; }
        public List<AnswerCard> PlayedCards { get; set; }
    }
}
