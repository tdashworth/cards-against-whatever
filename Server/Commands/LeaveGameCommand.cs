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
    public record LeaveGameCommand(
        string GameCode,
        string Username)

        : IRequest;


    class LeaveGameHandler : BaseGameRequestHandler<LeaveGameCommand>
    {
        private readonly IMediator mediator;

        public LeaveGameHandler(IGameRepository gameRepository, IHubContextFacade<IGameClient> hubContextFacade, ILogger<IRequestHandler<LeaveGameCommand>> logger, IMediator mediator)
            : base(gameRepository, hubContextFacade, logger)
        {
            this.mediator = mediator;
        }

        public async override Task HandleVoid(LeaveGameCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepository.GetByCode(request.GameCode);
            var allPlayersClient = hubContextFacade.GetGroup(request.GameCode);

            var player = game.Players.FirstOrDefault(player => player.Username == request.Username);

            if (player is null)
            {
                throw new Exception($"Player {request.Username} could not be found in game {request.GameCode}");
            }

            player.Status = PlayerStatus.Left;

            if (!string.IsNullOrEmpty(player.ConnectionId))
            {
                await hubContextFacade.LeaveGroup(request.GameCode, player.ConnectionId);
                player.ConnectionId = null;
            }

            await allPlayersClient.PlayerLeft((Player)player);

            await mediator.Send(new CloseRoundCommand(request.GameCode));
        }
    }
}
