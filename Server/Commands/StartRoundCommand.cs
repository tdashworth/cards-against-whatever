using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
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
        public StartRoundHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
            : base(gameRepositoy, hubContextFascade) { }

        public async override Task<Unit> Handle(StartRoundCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            game.IncrementRoundNumber();
            game.SelectNextCardCzar();
            game.SelectNextQuestion();

            await Task.WhenAll(game.Players.Select(player =>
            {
                var playerClient = hubContextFascade.GetClient(player.ConnectionId!);
                var newCards = game.CardDeck.PickUpAnswers(5 - player.CardsInHand.Count);
                player.CardsInHand.AddRange(newCards);
                player.PlayedCards.Clear();
                player.State = PlayerState.PlayingAnswer;

                return playerClient.RoundStarted(
                        currentRoundNumber: (game.RoundNumber ?? 0),
                        currentQuestion: game.CurrentQuestion!,
                        currentCardCzar: game.CurrentCardCzar!,
                        dealtCards: newCards);
            }));

            game.CurrentCardCzar!.State = PlayerState.AwatingAnswers;

            return Unit.Value;
        }
    }
}
