using CardsAgainstWhatever.Server.Models;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;
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
        public JoinGameHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade, ILogger<IRequestHandler<JoinGameCommand>> logger)
            : base(gameRepositoy, hubContextFascade, logger) { }

        public async override Task HandleVoid(JoinGameCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var allPlayersClient = hubContextFascade.GetGroup(request.GameCode);
            var callingPlayerClient = hubContextFascade.GetClient(request.ConnectionId);

            var player = game.Players.FirstOrDefault(player => player.Username == request.Username);

            if (player is not null && player.ConnectionId is not null)
            {
                throw new Exception($"Username {request.Username} has been taken in the game {request.GameCode}.");
            }

            if (player is null)
            {
                player = new ServerPlayer(request.Username, request.ConnectionId);
                game.Players.Add(player);
            }
            else
            {
                player.Status = PlayerStatus.Lobby;
                player.ConnectionId = request.ConnectionId;
            }

            await callingPlayerClient.GameJoined(
                gameStatus: game.Status,
                existingPlayersInGame: game.Players.Cast<Player>().ToList(),
                currentRoundNumber: game.RoundNumber,
                currentQuestion: game.CurrentQuestion,
                currentCardCzar: game.CurrentCardCzar);

            await allPlayersClient.PlayerJoined((Player)player);

            await hubContextFascade.JoinGroup(request.GameCode, request.ConnectionId);
        }
    }
}
