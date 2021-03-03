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

        Task<List<Player>> JoinGame(string code, string username);

        Task StartRound(string code);
    }
}
