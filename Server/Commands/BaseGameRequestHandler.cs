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
        protected readonly IGameRepositoy gameRepositoy;
        protected readonly IHubContextFascade<IGameClient> hubContextFascade;
        protected readonly ILogger logger;

        public BaseGameRequestHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade, ILogger<IRequestHandler<TRequest, TResponse>> logger)
        {
            this.gameRepositoy = gameRepositoy;
            this.hubContextFascade = hubContextFascade;
            this.logger = logger;
        }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    abstract class BaseGameRequestHandler<TRequest> : BaseGameRequestHandler<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        protected BaseGameRequestHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade, ILogger<IRequestHandler<TRequest>> logger)
            : base(gameRepositoy, hubContextFascade, logger) { }

        public async override Task<Unit> Handle(TRequest request, CancellationToken cancellationToken)
        {
            await HandleVoid(request, cancellationToken);
            return Unit.Value;
        }

        public abstract Task HandleVoid(TRequest request, CancellationToken cancellationToken);
    }
}
