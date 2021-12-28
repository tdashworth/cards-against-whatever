using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardsAgainstWhatever.Server.Models
{
    public class ServerGame
    {
        public readonly string Code;
        public readonly CardDeck CardDeck;
        public readonly List<ServerPlayer> Players;

        public GameStatus Status { get; set; }
        public QuestionCard? CurrentQuestion { get; private set; }
        public ServerPlayer? CurrentCardCzar { get; private set; }
        public int? RoundNumber { get; private set; }

        public ServerGame(string code, CardDeck cardDeck)
        {
            Code = code;
            CardDeck = cardDeck;
            Players = new();
            Status = GameStatus.Lobby;
        }

        public void IncrementRoundNumber()
        {
            if (RoundNumber is null)
            {
                RoundNumber = 0;
            }

            RoundNumber++;
        }

        public void SelectNextCardCzar()
        {
            if (Players.Count(player => player.Status != PlayerStatus.Left) < 2)
            {
                throw new Exception("Not enough active players!");
            }

            var nextPlayerIndex = CurrentCardCzar is null
                ? 0
                : (Players.FindIndex(player => player == CurrentCardCzar) + 1) % Players.Count;

            while (Players[nextPlayerIndex].Status == PlayerStatus.Left)
            {
                nextPlayerIndex = (nextPlayerIndex + 1) % Players.Count;
            }

            CurrentCardCzar = Players[nextPlayerIndex];
        }

        public void SelectNextQuestion()
        {
            CurrentQuestion = CardDeck.PickUpQuestion();
        }

        public IEnumerable<IReadOnlyList<AnswerCard>> CardsOnTable => Status == GameStatus.CollectingAnswers
            ? Players
                .Where(player => player.PlayedCards is not null && player.PlayedCards.Any())
                .Select(player => player.PlayedCards!.Select(card => new AnswerCard("")).ToList())
            : Players
                .Where(player => player.PlayedCards is not null && player.PlayedCards.Any())
                .Select(player => player.PlayedCards!);
    }
}
