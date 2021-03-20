﻿using CardsAgainstWhatever.Server.Extensions;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Commands
{
    public record CloseRoundCommand(
        string GameCode)

        : IRequest;

    class CloseRoundHandler : BaseGameRequestHandler<CloseRoundCommand>
    {
        public CloseRoundHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade, ILogger<IRequestHandler<CloseRoundCommand>> logger)
            : base(gameRepositoy, hubContextFascade, logger) { }


        public async override Task HandleVoid(CloseRoundCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var gameGroupClient = hubContextFascade.GetGroup(request.GameCode);

            if (game.Status != GameStatus.CollectingAnswers)
            {
                logger.LogWarning($"{request.GameCode}: Close round not possible in status {game.Status}.");
                return;
            }

            var haveAllPlayersMadeMove = game.Players.All(player => player.Status != PlayerStatus.PlayingAnswer);

            if (!haveAllPlayersMadeMove)
            {
                logger.LogDebug($"{request.GameCode}: Close round not possible as some players need to play their answer.");
                return;
            }

            Thread.Sleep(100);
            game.Status = GameStatus.SelectingWinner;

            var playedCardsGroupedPerPlayer = game.Players
                    .Where(player => player.PlayedCards.Any())
                    .Select(player => player.PlayedCards)
                    .ToList()
                    .Shuffle(new Random());

            await gameGroupClient.RoundClosed(playedCardsGroupedPerPlayer);
        }
    }
}
