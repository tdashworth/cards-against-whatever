using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos.Events
{
    public class GameJoinedEvent
    {
        public string Code { get; set; }
        public string Username { get; set; }
        public List<Player> ExistingPlayersInGame { get; set; }
    }
}
