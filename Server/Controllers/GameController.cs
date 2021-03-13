using CardsAgainstWhatever.Server.Commands;
using CardsAgainstWhatever.Shared.Dtos.Actions;
using CardsAgainstWhatever.Shared.Dtos.Events;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IMediator mediator;

        public GameController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public Task<GameCreatedEvent> Create(CreateGameAction request)
        {
            return mediator.Send(new CreateGameCommand
            {
                QuestionCards = request.QuestionCards,
                AnswerCards = request.AnswerCards
            });
        }
    }
}
