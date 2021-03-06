using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Models
{
    public class ServerPlayer : Player
    {
        public string ConnectionId { get; set; }
        public List<AnswerCard> CardsInHand { get; set; } = new();
        public List<AnswerCard> PlayedCards { get; set; } = new();
        public List<QuestionCard> WonCards { get; set; } = new();
    }
}
