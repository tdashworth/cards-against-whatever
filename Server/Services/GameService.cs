using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepositoy gameRepositoy;

        public GameService(IGameRepositoy gameRepositoy)
        {
            this.gameRepositoy = gameRepositoy;
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
            var player = new ServerPlayer { Username = username, ConnectionId = connectionId };

            game.Players.Add(player);

            return player;
        }

        public async Task Leave(string gameCode, string username)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            var player = game.Players.Find(p => p.Username == username);

            game.Players.Remove(player);
        }

        public async Task<Dictionary<ServerPlayer, List<AnswerCard>>> StartRound(string gameCode)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            var cardsToDeal = new Dictionary<ServerPlayer, List<AnswerCard>>();

            foreach (var player in game.Players)
            {
                cardsToDeal.Add(player, game.CardDeck.PickUpAnswers(10 - player.CardsInHand.Count));
            }

            return cardsToDeal;
        }
    }
}
