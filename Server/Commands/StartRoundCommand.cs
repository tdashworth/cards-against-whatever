using CardsAgainstWhatever.Server.Extensions;
using CardsAgainstWhatever.Server.Models;
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
    public record StartRoundCommand(
        string GameCode)

        : IRequest;

    class StartRoundHandler : BaseGameRequestHandler<StartRoundCommand>
    {
        public StartRoundHandler(IGameRepository gameRepository, IHubContextFacade<IGameClient> hubContextFacade, ILogger<IRequestHandler<StartRoundCommand>> logger)
            : base(gameRepository, hubContextFacade, logger) { }

        public async override Task HandleVoid(StartRoundCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepository.GetByCode(request.GameCode);

            if (game.Status != GameStatus.Lobby)
            {
                logger.LogWarning($"Invalid action. You can only start a game when the game status is {GameStatus.Lobby}.");
                throw new Exception($"Invalid action. You can only start a game when the game status is {GameStatus.Lobby}.");
            }

            StartRoundForGame(game);

            await game.Players
                .Where(player => player.Status != PlayerStatus.Left)
                .Select(player => StartRoundForPlayer(game, player))
                .AwaitAll();

            game.CurrentCardCzar!.Status = PlayerStatus.AwatingAnswers;
        }

        private static void StartRoundForGame(ServerGame game)
        {
            game.IncrementRoundNumber();
            game.SelectNextCardCzar();
            game.SelectNextQuestion();
            game.Status = GameStatus.CollectingAnswers;
        }

        private Task StartRoundForPlayer(ServerGame game, ServerPlayer player)
        {
            var playerClient = hubContextFacade.GetClient(player.ConnectionId!);
            var newCards = game.CardDeck.PickUpAnswers(10 - player.CardsInHand.Count);
            player.CardsInHand.AddRange(newCards);
            player.Status = PlayerStatus.PlayingAnswer;

            return playerClient.RoundStarted(
                    currentRoundNumber: (game.RoundNumber ?? 0),
                    currentQuestion: game.CurrentQuestion!,
                    currentCardCzar: game.CurrentCardCzar!,
                    dealtCards: newCards);
        }
    }
}
