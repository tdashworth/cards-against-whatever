using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
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

        public LeaveGameHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade, IMediator mediator)
            : base(gameRepositoy, hubContextFascade)
        {
            this.mediator = mediator;
        }

        public async override Task<Unit> Handle(LeaveGameCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var allPlayersClient = hubContextFascade.GetGroup(request.GameCode);

            var player = game.Players.FirstOrDefault(player => player.Username == request.Username);

            if (player is null)
            {
                throw new Exception($"Player {request.Username} could not be found in game {request.GameCode}");
            }

            player.Status = PlayerStatus.Left;

            if (!string.IsNullOrEmpty(player.ConnectionId))
            {
                await hubContextFascade.LeaveGroup(request.GameCode, player.ConnectionId);
                player.ConnectionId = null;
            }

            await allPlayersClient.PlayerLeft((Player)player);

            await mediator.Send(new CloseRoundCommand(request.GameCode));

            return Unit.Value;
        }
    }
}
