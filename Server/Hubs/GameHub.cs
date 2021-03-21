using CardsAgainstWhatever.Server.Commands;
using CardsAgainstWhatever.Server.Services.Interfaces;
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
        private readonly IConnectionUserMapping connectionUserMapping;

        public GameHub(IMediator mediator, IConnectionUserMapping connectionUserMapping)
        {
            this.mediator = mediator;
            this.connectionUserMapping = connectionUserMapping;
        }

        private string? GameCode => connectionUserMapping.ContainsKey(Context.ConnectionId) ? connectionUserMapping[Context.ConnectionId].GameCode : null;
        private string? Username => connectionUserMapping.ContainsKey(Context.ConnectionId) ? connectionUserMapping[Context.ConnectionId].Username : null;

        public async Task JoinGame(string gameCode, string username)
        {
            if (gameCode is null) throw new ArgumentNullException(nameof(gameCode));
            if (username is null) throw new ArgumentNullException(nameof(username));

            await mediator.Send(new JoinGameCommand(gameCode, username, Context.ConnectionId));
            connectionUserMapping.Add(Context.ConnectionId, new GameCodeAndUsername(gameCode, username));
        }

        public Task StartRound()
            => mediator.Send(new StartRoundCommand(GameCode!));

        public Task PlayAnswer(IEnumerable<AnswerCard> answerCards)
            => mediator.Send(new PlayAnswerCommand(GameCode!, Username!, answerCards));

        public Task PickWinningAnswer(IEnumerable<AnswerCard> answerCards)
            => mediator.Send(new PickWinningAnswerCommand(GameCode!, answerCards));

        public async Task LeaveGame()
        {
            await mediator.Send(new LeaveGameCommand(GameCode!, Username!));
            connectionUserMapping.Remove(Context.ConnectionId);
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            if (GameCode is not null && Username is not null)
            {
                await mediator.Send(new LeaveGameCommand(GameCode!, Username!));
                connectionUserMapping.Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
