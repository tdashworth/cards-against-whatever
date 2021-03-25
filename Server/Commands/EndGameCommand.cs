using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Commands
{
    public record EndGameCommand(
        string GameCode)

        : IRequest;

    class EndGameHandler : BaseGameRequestHandler<EndGameCommand>
    {
        public EndGameHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade, ILogger<IRequestHandler<EndGameCommand>> logger)
            : base(gameRepositoy, hubContextFascade, logger) { }

        public async override Task HandleVoid(EndGameCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var activePlayers = game.Players.Where(player => player.Status != PlayerStatus.Left);

            if (activePlayers.Any())
            {
                return;
            }

            await gameRepositoy.Delete(request.GameCode);
        }
    }
}
