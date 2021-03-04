using CardsAgainstWhatever.Shared;
using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client
{
    public enum GameState
    {
        New,
        Joined,
        PickAnswerCards,
        AwaitAnswerCards
    }

    public class GameStateContainer
    {
        public GameState State { get; private set; } = GameState.New;
        public string Code { get; private set; }
        public string Username { get; private set; }
        public int RoundNumber { get; private set; }
        public List<Player> Players { get; private set; } = new();
        public List<AnswerCard> CardsInHand { get; private set; } = new();
        public QuestionCard QuestionCard { get; private set; }
        public Player CardCzar { get; private set; }

        public event Action OnChange;

        public void JoinGame(string code, string username, List<Player> players)
        {
            Code = code;
            Username = username;
            State = GameState.Joined;
            Players.AddRange(players);
            NotifyStateChanged();
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
            NotifyStateChanged();
        }

        public void NewRound(int roundNumber, Player cardCzar, QuestionCard question, List<AnswerCard> cards)
        {
            RoundNumber = roundNumber;
            CardCzar = cardCzar;
            QuestionCard = question;
            CardsInHand.AddRange(cards);
            State = CardCzar.Username == Username ? GameState.AwaitAnswerCards : GameState.PickAnswerCards;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
