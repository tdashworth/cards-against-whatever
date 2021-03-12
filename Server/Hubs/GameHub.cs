using CardsAgainstWhatever.Server.Commands;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared;
using CardsAgainstWhatever.Shared.Dtos;
using CardsAgainstWhatever.Shared.Dtos.Actions;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
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
        private readonly IMediator mediator;
        private readonly IGameService gameService;

        public GameHub(IGameService gameService, IMediator mediator)
        {
            this.gameService = gameService;
            this.mediator = mediator;
        }

        public async Task<GameCreatedEvent> CreateGame(CreateGameAction request)
            => new GameCreatedEvent { 
                GameCode = await gameService.Create(request.QuestionCards, request.AnswerCards) 
            };

        public Task<GameJoinedEvent> JoinGame(JoinGameAction request) => mediator.Send(new JoinGameCommand
        {
            GameCode = request.GameCode,
            Username = request.Username,
            ConnectionId = Context.ConnectionId
        });

        public Task StartRound(StartRoundAction request) => mediator.Send(new StartRoundCommand
        {
            GameCode = request.GameCode
        });

        public Task PlayPlayerMove(PlayAnswerAction request) => mediator.Send(new PlayAnswerCommand
        {
            GameCode = request.GameCode,
            Username = request.Username,
            SelectedAnswerCards = request.PlayedCards
        });

        public Task PlayCardCzarMove(PickWinnerAnswerAction request) => mediator.Send(new PickWinningAnswerCommand
        {
            GameCode = request.GameCode,
            SelectedWinningAnswerCards = request.WinningCards
        });

    }
}
