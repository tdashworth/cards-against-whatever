using CardsAgainstWhatever.Server.Hubs;
using CardsAgainstWhatever.Server.Models;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared;
using CardsAgainstWhatever.Shared.Dtos;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services
{
    public class GameService : IGameService
    {
        private readonly IHubContext<GameHub, IGameClient> hubContext;
        private readonly IGameRepositoy gameRepositoy;

        public GameService(IGameRepositoy gameRepositoy, IHubContext<GameHub, IGameClient> hubContext)
        {
            this.gameRepositoy = gameRepositoy;
            this.hubContext = hubContext;
        }

        public Task<string> Create(IEnumerable<QuestionCard> questionCards, IEnumerable<AnswerCard> answerCards)
        {
            return gameRepositoy.Create(questionCards, answerCards);
        }

        public async Task<List<Player>> GetPlayers(string gameCode)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            return game.Players.Cast<Player>().ToList();
        }

        public async Task<Player> Join(string gameCode, string username, string connectionId)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            var player = new ServerPlayer { Username = username, ConnectionId = connectionId, State = PlayerState.InLobby };

            game.Players.Add(player);
            await hubContext.Groups.AddToGroupAsync(connectionId, gameCode);
            await hubContext.Clients.GroupExcept(gameCode, new[] { connectionId }).NewPlayer(new PlayerJoinedEvent { NewPlayer = player });

            return player;
        }

        public async Task Leave(string gameCode, string username)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            var player = game.Players.Find(p => p.Username == username);

            game.Players.Remove(player);
            await hubContext.Groups.RemoveFromGroupAsync(player.ConnectionId, gameCode);
            // TODO PlayerLeftEvent
        }

        public async Task StartRound(string gameCode)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            game.IncrementRoundNumber();
            game.SelectNextCardCzar();
            game.SelectNextQuestion();

            await Task.WhenAll(game.Players.Select(player =>
            {
                var newCards = game.CardDeck.PickUpAnswers(5 - player.CardsInHand.Count);
                player.CardsInHand.AddRange(newCards);
                player.PlayedCards.Clear();
                player.State = PlayerState.PlayingMove;

                return hubContext.Clients.Client(player.ConnectionId).NewRound(new RoundStartedEvent
                {
                    DealtCards = newCards,
                    QuestionCard = game.CurrentQuestion,
                    RoundNumber = game.RoundNumber,
                    CardCzar = game.CurrentCardCzar,
                });
            }));
        }

        public async Task PlayCards(string gameCode, string username, List<AnswerCard> playedCards)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            var player = game.Players.Find(player => player.Username == username);
            var gameGroupClient = hubContext.Clients.Group(gameCode);

            if (player == null)
            {
                throw new Exception($"Player {username} not found in game {gameCode}.");
            }

            player.PlayedCards = playedCards;
            player.State = PlayerState.MovePlayed;

            await gameGroupClient.NewMovePlayed(new PlayerMovedEvent { Username = username });

            var allPlayersMadeMove = game.Players
                .Where(player => player != game.CurrentCardCzar)
                .All(player => player.PlayedCards.Any());

            if (allPlayersMadeMove)
            {
                await gameGroupClient.AllMovesPlayed(new RoundClosedEvent
                {
                    PlayedCardsGroupedPerPlayer = game.Players.Select(player => player.PlayedCards).ToList()
                });
            }
        }
    }
}
