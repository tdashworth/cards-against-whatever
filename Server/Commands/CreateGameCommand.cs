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
    public class CreateGameCommand : IRequest<GameCreatedEvent>
    {
        public List<QuestionCard> QuestionCards { get; set; }
        public List<AnswerCard> AnswerCards { get; set; }
    }

    class CreateGameHandler : BaseGameRequestHandler<CreateGameCommand, GameCreatedEvent>
    {
        public CreateGameHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
            : base(gameRepositoy, hubContextFascade) { }

        public async override Task<GameCreatedEvent> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            var gameCoee = await gameRepositoy.Create(request.QuestionCards, request.AnswerCards);

            return new GameCreatedEvent
            {
                GameCode = gameCoee
            };
        }
    }
}
