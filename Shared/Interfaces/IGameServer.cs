using CardsAgainstWhatever.Shared.Dtos;
using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameServer
    {
        Task<CreateGameResponse> CreateGame(CreateGameRequest request);
        Task<JoinGameResponse> JoinGame(JoinGameRequest request);
        Task StartRound(string code);
    }
}
