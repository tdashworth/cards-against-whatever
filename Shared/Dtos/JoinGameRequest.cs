using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Dtos
{
    public class JoinGameRequest
    {
        public string GameCode { get; set; }
        public string Username { get; set; }
    }
}
