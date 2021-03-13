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

        public GameHub(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task<GameCreatedEvent> CreateGame(CreateGameAction request) => mediator.Send(new CreateGameCommand
        {
            QuestionCards = request.QuestionCards,
            AnswerCards = request.AnswerCards
        });

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

        public Task PlayAnswer(PlayAnswerAction request) => mediator.Send(new PlayAnswerCommand
        {
            GameCode = request.GameCode,
            Username = request.Username,
            SelectedAnswerCards = request.PlayedCards
        });

        public Task PickWinningAnswer(PickWinnerAnswerAction request) => mediator.Send(new PickWinningAnswerCommand
        {
            GameCode = request.GameCode,
            SelectedWinningAnswerCards = request.WinningCards
        });

    }
}
