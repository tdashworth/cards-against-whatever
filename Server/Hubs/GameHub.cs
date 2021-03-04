using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared;
using CardsAgainstWhatever.Shared.Dtos;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Hubs
{
    public class GameHub : Hub<IGameClient>, IGameServer
    {
        private readonly IGameService GameService;

        public GameHub(IGameService gameService)
        {
            GameService = gameService;
        }

        public async Task<CreateGameResponse> CreateGame(CreateGameRequest request)
            => new CreateGameResponse { 
                GameCode = await GameService.Create(request.QuestionCards, request.AnswerCards) 
            };

        public async Task<JoinGameResponse> JoinGame(JoinGameRequest request)
        {
            await GameService.Join(request.GameCode, request.Username, Context.ConnectionId);

            return new JoinGameResponse { 
                ExistingPlayersInGame = await GameService.GetPlayers(request.GameCode) 
            };
        }

        public Task StartRound(StartRoundRequest request) => GameService.StartRound(request.GameCode);

    }
}
