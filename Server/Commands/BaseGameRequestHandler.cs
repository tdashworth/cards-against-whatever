using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Commands
{
    abstract class BaseGameRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly IGameRepositoy gameRepositoy;
        protected readonly IHubContextFascade<IGameClient> hubContextFascade;

        public BaseGameRequestHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
        {
            this.gameRepositoy = gameRepositoy;
            this.hubContextFascade = hubContextFascade;
        }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    abstract class BaseGameRequestHandler<TRequest> : BaseGameRequestHandler<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        protected BaseGameRequestHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade) : base(gameRepositoy, hubContextFascade)
        {
        }
    }
}
