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
    public record PlayAnswerCommand(
        string GameCode,
        string Username,
        IEnumerable<AnswerCard> SelectedAnswerCards)

        : IRequest;

    class PlayAnswerHandler : BaseGameRequestHandler<PlayAnswerCommand>
    {
        private readonly IMediator mediator;

        public PlayAnswerHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade, ILogger<IRequestHandler<PlayAnswerCommand>> logger, IMediator mediator)
            : base(gameRepositoy, hubContextFascade, logger)
        {
            this.mediator = mediator;
        }

        public async override Task HandleVoid(PlayAnswerCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var gameGroupClient = hubContextFascade.GetGroup(request.GameCode);

            if (game.Status == GameStatus.CollectingAnswers)
            {
                logger.LogWarning($"Invalid action. You can only play answers when the game status is {GameStatus.CollectingAnswers}.");
                throw new Exception($"Invalid action. You can only play answers when the game status is {GameStatus.CollectingAnswers}.");
            }

            var player = game.Players.FirstOrDefault(player => player.Username == request.Username);

            if (player == null)
            {
                logger.LogError($"Player {request.Username} not found in game {request.GameCode}.");
                throw new Exception($"Player {request.Username} not found in game {request.GameCode}.");
            }

            player.PlayedCards = request.SelectedAnswerCards.ToList();
            player.Status = PlayerStatus.AnswerPlayed;

            await gameGroupClient.PlayerMoved(player);

            await mediator.Send(new CloseRoundCommand(request.GameCode));
        }
    }
}
