﻿using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos
{
    public class JoinGameResponse
    {
        public List<Player> ExistingPlayersInGame { get; set; }
    }
}
