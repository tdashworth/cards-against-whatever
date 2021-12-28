using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Commands
{
    abstract class BaseGameRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly IGameRepository gameRepository;
        protected readonly IHubContextFacade<IGameClient> hubContextFacade;
        protected readonly ILogger logger;

        public BaseGameRequestHandler(IGameRepository gameRepository, IHubContextFacade<IGameClient> hubContextFacade, ILogger<IRequestHandler<TRequest, TResponse>> logger)
        {
            this.gameRepository = gameRepository;
            this.hubContextFacade = hubContextFacade;
            this.logger = logger;
        }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    abstract class BaseGameRequestHandler<TRequest> : BaseGameRequestHandler<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        protected BaseGameRequestHandler(IGameRepository gameRepository, IHubContextFacade<IGameClient> hubContextFacade, ILogger<IRequestHandler<TRequest>> logger)
            : base(gameRepository, hubContextFacade, logger) { }

        public async override Task<Unit> Handle(TRequest request, CancellationToken cancellationToken)
        {
            await HandleVoid(request, cancellationToken);
            return Unit.Value;
        }

        public abstract Task HandleVoid(TRequest request, CancellationToken cancellationToken);
    }
}
