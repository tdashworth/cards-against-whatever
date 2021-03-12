using CardsAgainstWhatever.Server.Models;
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
    public class JoinGameCommand : IRequest<GameJoinedEvent>
    {
        public string GameCode { get; set; }
        public string Username { get; set; }
        public string ConnectionId { get; set; }
    }

    class JoinGameHandler : BaseGameRequestHandler<JoinGameCommand, GameJoinedEvent>
    {
        public  JoinGameHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
            : base(gameRepositoy, hubContextFascade) { }

        public async override Task<GameJoinedEvent> Handle(JoinGameCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var allPlayersClient = hubContextFascade.GetGroup(request.GameCode);

            var player = new ServerPlayer
            {
                Username = request.Username,
                ConnectionId = request.ConnectionId,
                State = PlayerState.InLobby
            };

            game.Players.Add(player);
            await hubContextFascade.JoinGroup(request.GameCode, request.ConnectionId);

            await allPlayersClient.PlayerJoined(new PlayerJoinedEvent
            {
                NewPlayer = player
            });

            return new GameJoinedEvent
            {
                Code = request.GameCode,
                Username = request.Username,
                ExistingPlayersInGame = game.Players.Cast<Player>().ToList()
            };
        }
    }
}
