using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared;
using CardsAgainstWhatever.Shared.Dtos;
using CardsAgainstWhatever.Shared.Dtos.Actions;
using CardsAgainstWhatever.Shared.Dtos.Events;
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

        public async Task<GameCreatedEvent> CreateGame(CreateGameAction request)
            => new GameCreatedEvent { 
                GameCode = await GameService.Create(request.QuestionCards, request.AnswerCards) 
            };

        public async Task<GameJoinedEvent> JoinGame(JoinGameAction request)
        {
            await GameService.Join(request.GameCode, request.Username, Context.ConnectionId);

            return new GameJoinedEvent { 
                ExistingPlayersInGame = await GameService.GetPlayers(request.GameCode) 
            };
        }

        public Task StartRound(StartRoundAction startRoundEvent) => GameService.StartRound(startRoundEvent.GameCode);

        public Task PlayMove(PlayMoveAction playCardsEvent) => GameService.PlayCards(playCardsEvent.GameCode, playCardsEvent.Username, playCardsEvent.PlayedCards);

    }
}
