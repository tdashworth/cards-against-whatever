using CardsAgainstWhatever.Server.Models;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Commands
{
    public record JoinGameCommand(
        string GameCode,
        string Username,
        string ConnectionId)

        : IRequest;

    class JoinGameHandler : BaseGameRequestHandler<JoinGameCommand>
    {
        public JoinGameHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
            : base(gameRepositoy, hubContextFascade) { }

        public async override Task<Unit> Handle(JoinGameCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var allPlayersClient = hubContextFascade.GetGroup(request.GameCode);
            var callingPlayerClient = hubContextFascade.GetClient(request.ConnectionId);

            var player = game.Players.FirstOrDefault(player => player.Username == request.Username);

            if (player is not null && player.ConnectionId is not null)
            {
                throw new Exception("Username taken.");
            }

            if (player is null)
            {
                player = new ServerPlayer(request.Username, request.ConnectionId);
                game.Players.Add(player);
            }
            else
            {
                player.State = PlayerState.InLobby;
                player.ConnectionId = request.ConnectionId;
            }

            await callingPlayerClient.GameJoined(
                existingPlayersInGame: game.Players.Cast<Player>().ToList(),
                currentRoundNumber: game.RoundNumber,
                currentQuestion: game.CurrentQuestion,
                currentCardCzar: game.CurrentCardCzar);

            await allPlayersClient.PlayerJoined((Player)player);

            await hubContextFascade.JoinGroup(request.GameCode, request.ConnectionId);

            return Unit.Value;
        }
    }
}
