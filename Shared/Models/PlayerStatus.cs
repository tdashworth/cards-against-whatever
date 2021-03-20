using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Models
{
    public enum PlayerStatus
    {
        Lobby,
        PlayingAnswer,
        AnswerPlayed,
        AwatingAnswers,
        SelectingWinner,
        Left,
    }
}
