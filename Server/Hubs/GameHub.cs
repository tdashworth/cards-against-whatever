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

        private string GameCode => this.Context?.GetHttpContext()?.Request?.Query["GameCode"];
        private string Username => this.Context?.UserIdentifier;

        public override async Task OnConnectedAsync()
        {
            // TODO: Validate GameCode and Username
            
            await mediator.Send(new JoinGameCommand
            {
                ConnectionId = Context.ConnectionId,
                Username = Username,
                GameCode = GameCode
            });

            await base.OnConnectedAsync();
        }

        public Task StartRound() => mediator.Send(new StartRoundCommand
        {
            GameCode = GameCode
        });

        public Task PlayAnswer(IEnumerable<AnswerCard> answerCards) => mediator.Send(new PlayAnswerCommand
        {
            GameCode = GameCode,
            Username = Username,
            SelectedAnswerCards = answerCards
        });

        public Task PickWinningAnswer(IEnumerable<AnswerCard> answerCards) => mediator.Send(new PickWinningAnswerCommand
        {
            GameCode = GameCode,
            SelectedWinningAnswerCards = answerCards
        });

    }
}
