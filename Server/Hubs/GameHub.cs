﻿using CardsAgainstWhatever.Server.Commands;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Dtos;
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

        public Task<Response> JoinGame(string gameCode, string username) => TryOrReturnException(async () =>
        {
            gameCode = gameCode.ToUpper();
            username = username.Trim();

            if (gameCode is null) throw new ArgumentNullException(nameof(gameCode));
            if (username is null) throw new ArgumentNullException(nameof(username));

            await mediator.Send(new JoinGameCommand(gameCode, username, Context.ConnectionId));
            connectionUserMapping.Add(Context.ConnectionId, new GameCodeAndUsername(gameCode, username));
        });

        public Task<Response> StartRound() => TryOrReturnException(()
            => mediator.Send(new StartRoundCommand(GameCode!))
        );

        public Task<Response> PlayAnswer(IReadOnlyList<AnswerCard> answerCards) => TryOrReturnException(()
            => mediator.Send(new PlayAnswerCommand(GameCode!, Username!, answerCards))
        );

        public Task<Response> PickWinningAnswer(IReadOnlyList<AnswerCard> answerCards) => TryOrReturnException(()
            => mediator.Send(new PickWinningAnswerCommand(GameCode!, answerCards))
        );

        public Task<Response> LeaveGame() => TryOrReturnException(async () =>
        {
            await mediator.Send(new LeaveGameCommand(GameCode!, Username!));
            connectionUserMapping.Remove(Context.ConnectionId);
        });

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            if (GameCode is not null && Username is not null)
            {
                await mediator.Send(new LeaveGameCommand(GameCode!, Username!));
                connectionUserMapping.Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        private async Task<Response> TryOrReturnException(Func<Task> func)
        {
            try
            {
                await func();
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response { ErrorMessage = ex.Message };
            }
        }
    }
}
