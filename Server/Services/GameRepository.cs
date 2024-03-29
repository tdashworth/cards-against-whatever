﻿using CardsAgainstWhatever.Server.Models;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services
{
    public class GameRepository : IGameRepository
    {
        private static IDictionary<string, ServerGame> GameStore = new Dictionary<string, ServerGame>();

        public Task<string> Create(IEnumerable<QuestionCard> questionCards, IEnumerable<AnswerCard> answerCards)
        {
            var game = new ServerGame(
                GeneratorUniqueCode(new Random(), GameStore.Keys),
                new CardDeck(questionCards, answerCards));

            GameStore.Add(game.Code.ToUpper(), game);

            return Task.FromResult(game.Code);
        }

        public Task<ServerGame> GetByCode(string code)
        {
            if (!GameStore.ContainsKey(code.ToUpper()))
            {
                throw new Exception($"Game {code} doesn't exist.");
            }

            return Task.FromResult(GameStore[code.ToUpper()]);
        }

        public Task Delete(string code)
        {
            GameStore.Remove(code);

            return Task.CompletedTask;
        }

        private string GeneratorUniqueCode(Random random, IEnumerable<string> existingCodes)
        {
            string code;

            do
            {
                code = GenerateCode(new Random());
            } while (existingCodes.Contains(code));

            return code;
        }

        private string GenerateCode(Random random)
        {
            int length = 5;
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }
    }
}
