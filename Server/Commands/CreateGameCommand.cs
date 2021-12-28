using CardsAgainstWhatever.Server.Extensions;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Dtos.Events;
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
    public record CreateGameCommand(
        List<QuestionCard> QuestionCards,
        List<AnswerCard> AnswerCards)

        : IRequest<GameCreatedEvent>;

    class CreateGameHandler : BaseGameRequestHandler<CreateGameCommand, GameCreatedEvent>
    {
        public CreateGameHandler(IGameRepository gameRepository, IHubContextFacade<IGameClient> hubContextFacade, ILogger<IRequestHandler<CreateGameCommand, GameCreatedEvent>> logger)
            : base(gameRepository, hubContextFacade, logger) { }

        public async override Task<GameCreatedEvent> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            if (request.QuestionCards is null || !request.QuestionCards.Any())
            {
                throw new ArgumentException(nameof(request.QuestionCards), "must not be null or empty");
            }

            if (request.AnswerCards is null || !request.AnswerCards.Any())
            {
                throw new ArgumentException(nameof(request.AnswerCards), "must not be null or empty");
            }

            var random = new Random();

            var gameCode = await gameRepository.Create(
                request.QuestionCards.Shuffle(random),
                request.AnswerCards.Shuffle(random));

            return new GameCreatedEvent(gameCode);
        }
    }
}
