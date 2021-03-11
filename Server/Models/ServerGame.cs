using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Models
{
    public class ServerGame
    {
        public readonly string Code;
        public readonly CardDeck CardDeck;
        public readonly List<ServerPlayer> Players;
        
        public QuestionCard CurrentQuestion { get; private set; }
        public ServerPlayer CurrentCardCzar { get; private set; }
        public int RoundNumber { get; private set; }

        public ServerGame(string code, CardDeck cardDeck)
        {
            Code = code;
            CardDeck = cardDeck;
            Players = new();
        }

        public void IncrementRoundNumber() => RoundNumber++;

        public void SelectNextCardCzar()
        {
            if (Players is null || Players.Count < 2)
            {
                throw new Exception("Not enough players!");
            }

            if (CurrentCardCzar == null)
            {
                CurrentCardCzar = Players.First();
                return;
            }

            var currentPlayerIndex = Players.FindIndex(player => player == CurrentCardCzar);
            var nextPlayerIndex = (currentPlayerIndex + 1) % Players.Count;
            CurrentCardCzar = Players[nextPlayerIndex];
        }

        public void SelectNextQuestion()
        {
            CurrentQuestion = CardDeck.PickUpQuestion();
        }
    }
}
