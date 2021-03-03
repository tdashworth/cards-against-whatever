using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Models
{
    public class ServerGame
    {
        public readonly string Code;
        public readonly CardDeck CardDeck;
        public readonly List<ServerPlayer> Players;

        public ServerGame(string code, CardDeck cardDeck)
        {
            Code = code;
            CardDeck = cardDeck;
            Players = new();
        }
    }
}
