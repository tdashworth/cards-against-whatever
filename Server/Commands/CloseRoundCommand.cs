using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
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
        public CloseRoundHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
            : base(gameRepositoy, hubContextFascade) { }

        public async override Task<Unit> Handle(CloseRoundCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var gameGroupClient = hubContextFascade.GetGroup(request.GameCode);

            var haveAllPlayersMadeMove = game.Players.All(player => player.State != PlayerState.PlayingAnswer);

            if (haveAllPlayersMadeMove)
            {
                Thread.Sleep(1000);
                var playedCardsGroupedPerPlayer = game.Players
                        .Where(player => player.PlayedCards.Any())
                        .Select(player => player.PlayedCards).ToList();

                await gameGroupClient.RoundClosed(playedCardsGroupedPerPlayer);
            }

            return Unit.Value;
        }
    }
}
