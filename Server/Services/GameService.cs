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

        public async Task Join(string gameCode, string username)
        {
            var game = await gameRepositoy.GetByCode(gameCode);

            game.Players.Add(new Player(username));
        }

        public async Task Leave(string gameCode, string username)
        {
            var game = await gameRepositoy.GetByCode(gameCode);
            var player = game.Players.Find(p => p.Username == username);

            game.Players.Remove(player);
        }
    }
}
