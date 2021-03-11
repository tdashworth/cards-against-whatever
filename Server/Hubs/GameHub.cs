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
        private readonly IGameService gameService;

        public GameHub(IGameService gameService)
        {
            this.gameService = gameService;
        }

        public async Task<GameCreatedEvent> CreateGame(CreateGameAction request)
            => new GameCreatedEvent { 
                GameCode = await gameService.Create(request.QuestionCards, request.AnswerCards) 
            };

        public async Task<GameJoinedEvent> JoinGame(JoinGameAction request)
        {
            await gameService.Join(request.GameCode, request.Username, Context.ConnectionId);

            return new GameJoinedEvent { 
                Code = request.GameCode,
                Username = request.Username,
                ExistingPlayersInGame = await gameService.GetPlayers(request.GameCode) 
            };
        }

        public Task StartRound(StartRoundAction startRoundEvent) => gameService.StartRound(startRoundEvent.GameCode);

        public Task PlayPlayerMove(PlayMovePlayerAction playCardsEvent) => gameService.PlayCards(playCardsEvent.GameCode, playCardsEvent.Username, playCardsEvent.PlayedCards);

        public Task PlayCardCzarMove(PlayMoveCardCzarAction pickWinningCardsEvent) => gameService.PickWinner(pickWinningCardsEvent.GameCode, pickWinningCardsEvent.WinningCards);

    }
}
