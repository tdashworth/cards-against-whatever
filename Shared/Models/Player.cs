using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Models
{
    public enum PlayerState
    {
        InLobby,
        PlayingAnswer,
        AnswerPlayed,
        AwatingAnswers,
        PickingWinner
    }

    public class Player
    {
        public string Username { get; set; }
        public int Score { get; set; }
        public PlayerState State { get; set; }
    }
}
