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
        private readonly IGameRepositoy gameRepositoy;
        private readonly IHubContextFascade<IGameClient> hubContextFascade;

        public GameService(IGameRepositoy gameRepositoy, IHubContextFascade<IGameClient> hubContextFascade)
        {
            this.gameRepositoy = gameRepositoy;
            this.hubContextFascade = hubContextFascade;
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
            await hubContextFascade.JoinGroup(gameCode, connectionId);
            await hubContextFascade.GetGroup(gameCode).PlayerJoined(new PlayerJoinedEvent { NewPlayer = player });

            return player;
        }

        public async Task Leave(string gameCode, string username)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            var player = game.Players.Find(p => p.Username == username);

            game.Players.Remove(player);
            await hubContextFascade.LeaveGroup(gameCode, player.ConnectionId);
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

                return hubContextFascade.GetClient(player.ConnectionId).RoundStarted(new RoundStartedEvent
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
            var gameGroupClient = hubContextFascade.GetGroup(gameCode);

            if (player == null)
            {
                throw new Exception($"Player {username} not found in game {gameCode}.");
            }

            player.PlayedCards = playedCards;
            player.State = PlayerState.MovePlayed;

            await gameGroupClient.PlayerMoved(new PlayerMovedEvent { Username = username });

            var allPlayersMadeMove = game.Players
                .Where(player => player != game.CurrentCardCzar)
                .All(player => player.PlayedCards.Any());

            if (allPlayersMadeMove)
            {
                await gameGroupClient.RoundClosed(new RoundClosedEvent
                {
                    PlayedCardsGroupedPerPlayer = game.Players
                        .Where(player => player != game.CurrentCardCzar)
                        .Select(player => player.PlayedCards).ToList()
                });
            }
        }

        public async Task PickWinner(string gameCode, List<AnswerCard> winningCards)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            var gameGroupClient = hubContextFascade.GetGroup(gameCode);
            var winner = game.Players
                .Where(player => player != game.CurrentCardCzar)
                .FirstOrDefault(player => player.PlayedCards.All(card => winningCards.Contains(card)));

            if (winner == null)
            {
                throw new Exception($"Winner could not be determined in game {gameCode}.");
            }

            winner.WonCards.Add(game.CurrentQuestion);

            await gameGroupClient.RoundEnded(new RoundEndedEvent
            {
                Winner = winner
            });
        }
    }
}
