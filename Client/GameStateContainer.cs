using CardsAgainstWhatever.Shared;
using CardsAgainstWhatever.Shared.Dtos;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
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
        public ActionButtonState ActionButton { get; private set; } = new();
        public string Code { get; private set; }
        public string Username { get; private set; }
        public int RoundNumber { get; private set; }
        public List<Player> Players { get; private set; } = new();
        public List<AnswerCard> CardsInHand { get; private set; } = new();
        public List<AnswerCard> CardsSelected { get; private set; } = new();
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
            CardCzar = Players.Find(player => player.Username == cardCzar.Username);
            QuestionCard = question;
            CardsInHand.AddRange(cards);
            State = CardCzar.Username == Username ? GameState.AwaitAnswerCards : GameState.PickAnswerCards;
            Players.Where(player => player != CardCzar).Select(player => player.State = PlayerState.PlayingMove);
            CardCzar.State = PlayerState.CardCzarAwaitingMoves;
            ActionButton.ButtonText = "Play selected cards";
            NotifyStateChanged();
        }

        public void PlayMove(List<AnswerCard> playedCards)
        {
            CardsSelected = playedCards;
            State = GameState.AwaitAnswerCards;
            NotifyStateChanged();
        }

        public void MovePlayed(string username)
        {
            var player = Players.Find(player => player.Username == username);
            player.State = PlayerState.MovePlayed;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public class ActionButtonState
        {
            public string ButtonText { get; set; }
            public bool IsEnabled { get; set; }
            public string ButtonMessage { get; set; }
            public Action OnClick { get; set; }
        }
    }
}
