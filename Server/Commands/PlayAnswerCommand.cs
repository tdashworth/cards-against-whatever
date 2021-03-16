using CardsAgainstWhatever.Server.Services.Interfaces;
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
    public record PlayAnswerCommand(
        string GameCode,
        string Username,
        IEnumerable<AnswerCard> SelectedAnswerCards)

        : IRequest;

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

            player.PlayedCards = request.SelectedAnswerCards.ToList();
            player.State = PlayerState.AnswerPlayed;

            await gameGroupClient.PlayerMoved(player);

            var haveAllPlayersMadeMove = game.Players.All(player => player.State != PlayerState.PlayingAnswer);

            Thread.Sleep(1000);

            if (haveAllPlayersMadeMove)
            {
                var playedCardsGroupedPerPlayer = game.Players
                        .Where(player => player.State == PlayerState.AnswerPlayed)
                        .Select(player => player.PlayedCards).ToList();

                await gameGroupClient.RoundClosed(playedCardsGroupedPerPlayer);
            }

            return Unit.Value;
        }
    }
}
