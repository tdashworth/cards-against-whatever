using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos.Events
{
    public class PlayerJoinedEvent
    {
        public Player NewPlayer { get; set; }
    }
}
