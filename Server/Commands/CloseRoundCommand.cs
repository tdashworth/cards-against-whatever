using CardsAgainstWhatever.Server.Extensions;
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

            if (game.Status != GameStatus.CollectingAnswers)
            {
                return Unit.Value;
            }

            var haveAllPlayersMadeMove = game.Players.All(player => player.Status != PlayerStatus.PlayingAnswer);

            if (!haveAllPlayersMadeMove)
            {
                return Unit.Value;
            }

            Thread.Sleep(100);
            game.Status = GameStatus.SelectingWinner;

            var playedCardsGroupedPerPlayer = game.Players
                    .Where(player => player.PlayedCards.Any())
                    .Select(player => player.PlayedCards)
                    .ToList()
                    .Shuffle(new Random());

            await gameGroupClient.RoundClosed(playedCardsGroupedPerPlayer);

            return Unit.Value;
        }
    }
}
