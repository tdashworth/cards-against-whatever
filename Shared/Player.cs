using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared
{
    public class Player
    {
        public readonly string Username;
        public int Score;

        public Player(string username)
        {
            Username = username;
        }
    }
}
