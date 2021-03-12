﻿using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Commands
{
    public class PlayAnswerCommand : IRequest
    {
        public string GameCode { get; set; }
        public string Username { get; set; }
        public List<AnswerCard> SelectedAnswerCards { get; set; }
    }

    class PlayAnswerHandler : BaseGameRequestHandler<PlayAnswerCommand>
    {
        public PlayAnswerHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
            : base(gameRepositoy, hubContextFascade) { }

        public async override Task<Unit> Handle(PlayAnswerCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var player = game.Players.FirstOrDefault(player => player.Username == request.Username);
            var gameGroupClient = hubContextFascade.GetGroup(request.GameCode);

            if (player == null)
            {
                throw new Exception($"Player {request.Username} not found in game {request.GameCode}.");
            }

            player.PlayedCards = request.SelectedAnswerCards;
            player.State = PlayerState.MovePlayed;

            await gameGroupClient.PlayerMoved(new PlayerMovedEvent { Username = request.Username });

            var allPlayersMadeMove = game.Players
                .Where(player => player != game.CurrentCardCzar)
                .All(player => player.PlayedCards.Any());

            if (allPlayersMadeMove)
            {
                await gameGroupClient.RoundClosed(new RoundClosedEvent
                {
                    PlayedCardsGroupedPerPlayer = game.Players
                        .Where(player => player != game.CurrentCardCzar)
                        .Select(player => player.PlayedCards).ToList()
                });
            }

            return Unit.Value;
        }
    }
}