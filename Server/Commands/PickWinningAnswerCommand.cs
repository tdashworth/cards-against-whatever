using CardsAgainstWhatever.Server.Models;
using CardsAgainstWhatever.Server.Services.Interfaces;
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
    public class PickWinningAnswerCommand : IRequest
    {
        public string GameCode { get; set; }
        public IEnumerable<AnswerCard> SelectedWinningAnswerCards { get; set; }
    }

    class PickWinningAnswerHandler : BaseGameRequestHandler<PickWinningAnswerCommand>
    {
        public PickWinningAnswerHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
            : base(gameRepositoy, hubContextFascade) { }

        public async override Task<Unit> Handle(PickWinningAnswerCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var gameGroupClient = hubContextFascade.GetGroup(request.GameCode);
            var winner = game.Players
                .Where(player => player != game.CurrentCardCzar)
                .FirstOrDefault(player => player.PlayedCards.All(card => request.SelectedWinningAnswerCards.Contains(card)));

            if (winner == null)
            {
                throw new Exception($"Winner could not be determined in game {request.GameCode}.");
            }

            winner.WonCards.Add(game.CurrentQuestion);

            await gameGroupClient.RoundEnded(new RoundEndedEvent
            {
                Winner = winner
            });

            return Unit.Value;
        }
    }
}
