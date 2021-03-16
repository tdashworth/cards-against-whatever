using CardsAgainstWhatever.Shared.Models;
using System.Collections.Generic;

namespace CardsAgainstWhatever.Server.Models
{
    public class ServerPlayer : Player
    {
        public ServerPlayer(string username, string connectionId) : base(username)
        {
            ConnectionId = connectionId;
        }

        public string? ConnectionId { get; set; }
        public List<AnswerCard> CardsInHand { get; set; } = new();
        public List<AnswerCard> PlayedCards { get; set; } = new();
        public List<QuestionCard> WonCards { get; set; } = new();
    }
}
