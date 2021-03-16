using CardsAgainstWhatever.Server.Commands;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
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

        private string? GameCode => this.Context?.GetHttpContext()?.Request?.Query["GameCode"];
        private string? Username => this.Context?.UserIdentifier;

        public override async Task OnConnectedAsync()
        {
            if (GameCode is null)
            {
                throw new ArgumentNullException(nameof(GameCode));
            }

            if (Username is null)
            {
                throw new ArgumentNullException(nameof(Username));
            }

            await mediator.Send(new JoinGameCommand(GameCode, Username, Context.ConnectionId));

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await mediator.Send(new LeaveGameCommand(GameCode!, Username!));

            await base.OnDisconnectedAsync(exception);
        }

        public Task StartRound()
            => mediator.Send(new StartRoundCommand(GameCode!));

        public Task PlayAnswer(IEnumerable<AnswerCard> answerCards)
            => mediator.Send(new PlayAnswerCommand(GameCode!, Username!, answerCards));

        public Task PickWinningAnswer(IEnumerable<AnswerCard> answerCards)
            => mediator.Send(new PickWinningAnswerCommand(GameCode!, answerCards));

    }
}
