using CardsAgainstWhatever.Server.Models;
using CardsAgainstWhatever.Server.Services.Interfaces;
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
    public record PickWinningAnswerCommand(
        string GameCode,
        IEnumerable<AnswerCard> SelectedWinningAnswerCards)

        : IRequest;

    class PickWinningAnswerHandler : BaseGameRequestHandler<PickWinningAnswerCommand>
    {
        public PickWinningAnswerHandler(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade, ILogger<IRequestHandler<PickWinningAnswerCommand>> logger)
            : base(gameRepositoy, hubContextFascade, logger) { }

        public async override Task HandleVoid(PickWinningAnswerCommand request, CancellationToken cancellationToken)
        {
            var game = await gameRepositoy.GetByCode(request.GameCode);
            var gameGroupClient = hubContextFascade.GetGroup(request.GameCode);

            if (game.Status != GameStatus.SelectingWinner)
            {
                logger.LogWarning($"Invalid action. You can only select a winner when the game status is {GameStatus.SelectingWinner}.");
                throw new Exception($"Invalid action. You can only select a winner when the game status is {GameStatus.SelectingWinner}.");
            }

            var winner = DetermineWinnerFromPlayedCards(game.Players, request.SelectedWinningAnswerCards);

            if (winner == null)
            {
                throw new Exception($"Winner could not be determined in game {request.GameCode}.");
            }

            winner.WonCards.Add(game.CurrentQuestion!);
            winner.Score++;

            ResetPlayers(game.Players);

            game.Status = GameStatus.Lobby;

            await gameGroupClient.RoundEnded((Player)winner);
        }

        private static void ResetPlayers(IEnumerable<ServerPlayer> players)
        {
            foreach (var player in players)
            {
                if (player.Status != PlayerStatus.Left)
                {
                    player.Status = PlayerStatus.Lobby;
                }
                player.PlayedCards.Clear();
            }
        }

        private static ServerPlayer? DetermineWinnerFromPlayedCards(IEnumerable<ServerPlayer> players, IEnumerable<AnswerCard> winningCards)
        {
            return players.FirstOrDefault(player => winningCards.All(card => player.PlayedCards.Contains(card)));
        }
    }
}
