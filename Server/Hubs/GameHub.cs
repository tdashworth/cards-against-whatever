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
            var player = await GameService.Join(request.GameCode, request.Username, Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, request.GameCode);
            await Clients.OthersInGroup(request.GameCode).NewPlayer(player);

            return new JoinGameResponse { 
                ExistingPlayersInGame = await GameService.GetPlayers(request.GameCode) 
            };
        }

        public async Task StartRound(string gameCode)
        {
            var cardsToDeal = await GameService.StartRound(gameCode);
            await Task.WhenAll(cardsToDeal.Select(kvp => Clients.Client(kvp.Key.ConnectionId).NewRound(kvp.Value)));
        }



    }
}
