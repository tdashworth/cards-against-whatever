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
        AwaitAnswerCards,
        RoundClosed,
        PickWinner,
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
        public List<List<AnswerCard>> PlayedCards { get; private set; } = new();
        public QuestionCard QuestionCard { get; private set; }
        public Player CardCzar { get; private set; }

        public bool IsCardCzar
        {
            get { return CardCzar?.Username == Username; }
        }

        public event Action OnChange;

        public void JoinGame(string code, string username, List<Player> players)
        {
            Code = code;
            Username = username;
            Players.AddRange(players.Where(p => p.Username != Username));

            State = GameState.Joined;
            ActionButton.ButtonText = "Start new round";
            ActionButton.ButtonMessage = string.Empty;
            ActionButton.IsEnabled = true;

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

            ActionButton.ButtonText = !IsCardCzar ? "Play selected cards" : string.Empty;

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

        public void RoundClosed(List<List<AnswerCard>> allAnswerCardSets)
        {
            PlayedCards = allAnswerCardSets;

            State = CardCzar.Username == Username ? GameState.PickWinner : GameState.RoundClosed;
            ActionButton.ButtonText = IsCardCzar ? "Select winner" : string.Empty;
            ActionButton.ButtonMessage = string.Empty;
            ActionButton.IsEnabled = true;

            NotifyStateChanged();
        }

        public void RoundEnded(Player winner)
        {
            var player = Players.Find(p => p.Username == winner.Username);
            player.Score++;

            State = GameState.Joined;
            ActionButton.ButtonText = "Start new round";
            ActionButton.ButtonMessage = string.Empty;
            ActionButton.IsEnabled = true;

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
