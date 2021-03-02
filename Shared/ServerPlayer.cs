using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared
{
    public class ServerPlayer : Player
    {
        public string ConnectionId { get; set; }
        public List<AnswerCard> CardsInHand { get; set; } = new();
    }
}
