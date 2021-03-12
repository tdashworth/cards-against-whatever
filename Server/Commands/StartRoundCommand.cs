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
    public class StartRoundCommand : IRequest
    {
        public string GameCode { get; set; }
    }

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
                var playerClient = hubContextFascade.GetClient(player.ConnectionId);
                var newCards = game.CardDeck.PickUpAnswers(5 - player.CardsInHand.Count);
                player.CardsInHand.AddRange(newCards);
                player.PlayedCards.Clear();
                player.State = PlayerState.PlayingMove;

                return playerClient.RoundStarted(new RoundStartedEvent
                {
                    RoundNumber = game.RoundNumber,
                    QuestionCard = game.CurrentQuestion,
                    CardCzar = game.CurrentCardCzar,
                    DealtCards = newCards,
                });
            }));

            game.CurrentCardCzar.State = PlayerState.CardCzarAwaitingMoves;

            return Unit.Value;
        }
    }
}
