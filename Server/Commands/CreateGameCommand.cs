using CardsAgainstWhatever.Server.Extensions;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Commands
{
    public record CreateGameCommand(
        List<QuestionCard> QuestionCards,
        List<AnswerCard> AnswerCards)

        : IRequest<GameCreatedEvent>;

    class CreateGameHandler : BaseGameRequestHandler<CreateGameCommand, GameCreatedEvent>
    {
        public CreateGameHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
            : base(gameRepositoy, hubContextFascade) { }

        public async override Task<GameCreatedEvent> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            var random = new Random();

            var gameCode = await gameRepositoy.Create(
                request.QuestionCards.Shuffle(random),
                request.AnswerCards.Shuffle(random));

            return new GameCreatedEvent(gameCode);
        }
    }
}
