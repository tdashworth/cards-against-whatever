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
        private readonly IMediator mediator;

        public PlayAnswerHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade, IMediator mediator)
            : base(gameRepositoy, hubContextFascade)
        {
            this.mediator = mediator;
        }

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

            await mediator.Send(new CloseRoundCommand(request.GameCode));

            return Unit.Value;
        }
    }
}
