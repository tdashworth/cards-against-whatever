﻿using CardsAgainstWhatever.Client.Extensions;
using CardsAgainstWhatever.Shared.Models;
using Fluxor;
using System.Collections.Generic;
using System.Linq;

namespace CardsAgainstWhatever.Client.Stores.Game
{
    public record JoinGameAction(string GameCode, string Username)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, JoinGameAction action)
            => state with
            {
                IsLoading = true,
                CurrentErrorMessage = null,
                GameCode = action.GameCode.ToUpper(),
                Username = action.Username.Trim()
            };
    }

    public record GameJoinedEvent(GameStatus gameStatus, IEnumerable<Player> ExistingPlayersInGame, IEnumerable<AnswerCard> cardsInHand, IEnumerable<IReadOnlyList<AnswerCard>> cardsOnTable, int? CurrentRoundNumber, QuestionCard? CurrentQuestion, Player? CurrentCardCzar)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, GameJoinedEvent action)
            => state with
            {
                IsLoading = false,
                Status = action.gameStatus,
                Players = action.ExistingPlayersInGame.ToList(),
                CurrentRoundNumber = action.CurrentRoundNumber,
                CurrentQuestion = action.CurrentQuestion,
                CurrentCardCzar = action.CurrentCardCzar?.Username is not null ? state.Players.FindByUsername(action.CurrentCardCzar.Username) : null,
                CardsInHand = action.cardsInHand.ToList(),
                CardsOnTable = action.cardsOnTable.ToList(),
                SelectedCardsInHand = new List<AnswerCard>(),
                SelectedCardsOnTable = null,
            };
    }

    public record GameJoinedFailedEvent(string ErrorMessage)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, GameJoinedFailedEvent action)
            => state with
            {
                IsLoading = false,
                CurrentErrorMessage = action.ErrorMessage
            };
    }

    public record PlayerJoinedEvent(Player Player)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, PlayerJoinedEvent action)
            => state with
            {
                Players = state.Players!.CopyAndUpdate(players =>
                {
                    players.RemoveAll(player => player.Username == action.Player.Username);
                    players.Add(action.Player);
                })
            };
    }

    public record PlayerLeftEvent(Player Player)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, PlayerLeftEvent action)
            => state with
            {
                Players = state.Players!.CopyAndUpdate(players => players
                    .FindByUsername(action.Player.Username)
                    .Update(player => player!.Status = PlayerStatus.Left))
            };
    }

    public record StartRoundAction()
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, StartRoundAction action)
            => state with
            {
                IsLoading = true,
            };
    }

    public record RoundStartedEvent(int CurrentRoundNumber, QuestionCard CurrentQuestion, string CurrentCardCzarUsername, IEnumerable<AnswerCard> DealtCards)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, RoundStartedEvent action)
            => state with
            {
                IsLoading = false,
                Status = GameStatus.CollectingAnswers,
                Players = state.Players!.UpdateEach(player =>
                {
                    if (player.Status != PlayerStatus.Left)
                    {
                        player.Status = PlayerStatus.PlayingAnswer;
                    }
                }).ToList(),
                CurrentRoundNumber = action.CurrentRoundNumber,
                CurrentQuestion = action.CurrentQuestion,
                CurrentCardCzar = state.Players
                                            .FindByUsername(action.CurrentCardCzarUsername)
                                            .Update(player => player!.Status = PlayerStatus.AwatingAnswers),
                CardsOnTable = new List<IReadOnlyList<AnswerCard>>(),
                CardsInHand = state.CardsInHand!.CopyAndUpdate(cards => cards.AddRange(action.DealtCards)),
                SelectedCardsInHand = new List<AnswerCard>(),
                SelectedCardsOnTable = null,
                PlayersSelections = null,
            };
    }

    public record AnswerSelectedAction(AnswerCard Answer)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, AnswerSelectedAction action)
            => state with
            {
                SelectedCardsInHand = state.SelectedCardsInHand is null || state.CurrentQuestion!.Picks == 1
                ? new List<AnswerCard> { action.Answer }
                : state.SelectedCardsInHand.CopyAndUpdate(cards =>
                {
                    if (cards.Contains(action.Answer))
                    {
                        cards.Remove(action.Answer);
                    } else
                    {
                        cards.Add(action.Answer);
                    }
                })
            };
    }

    public record PlayAnswerAction()
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, PlayAnswerAction action)
            => state with
            {
                CardsInHand = state.CardsInHand!.CopyAndUpdate(cards => { 
                    foreach (var card in state.SelectedCardsInHand!)
                    {
                        cards.Remove(card);
                    }
                })
            };
    }

    public record AnswerPlayedEvent(string Username)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, AnswerPlayedEvent action)
            => state.Status != GameStatus.CollectingAnswers
            ? state
            : state with
            {
                Players = state.Players!.CopyAndUpdate(players =>
                {
                    var player = players.FindByUsername(action.Username);
                    if (player is not null) player.Status = PlayerStatus.AnswerPlayed;
                }),
                CardsOnTable = state.CardsOnTable!.CopyAndUpdate(cards =>
                {
                    var blankCards = Enumerable.Range(1, state.CurrentQuestion!.Picks).Select(_ => new AnswerCard("")).ToList();
                    cards.Add(blankCards);
                })
            };
    }

    public record WinnerSelectedAction(IReadOnlyList<AnswerCard> WinningAnswer)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, WinnerSelectedAction action)
            => state with
            {
                SelectedCardsOnTable = action.WinningAnswer
            };
    }

    public record PickWinnerAction()
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, PickWinnerAction action)
            => state;
    }

    public record RoundClosedEvent(IEnumerable<IEnumerable<AnswerCard>> PlayedCardsGroupedPerPlayer)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, RoundClosedEvent action)
            => state with
            {
                Status = GameStatus.SelectingWinner,
                CurrentCardCzar = state.CurrentCardCzar.Update(player => player!.Status = PlayerStatus.SelectingWinner),
                CardsOnTable = action.PlayedCardsGroupedPerPlayer.Select(x => x.ToList()).ToList()
            };
    }

    public record RoundEndedEvent(string Username, Dictionary<string, IReadOnlyList<AnswerCard>> PlayersSelections)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, RoundEndedEvent action)
            => state with
            {
                Status = GameStatus.Lobby,
                Players = state.Players!.CopyAndUpdate(players =>
                {
                    var winningPlayer = players.FindByUsername(action.Username);
                    if (winningPlayer is not null) winningPlayer.Score++;

                    foreach (var player in players)
                    {
                        if (player.Status != PlayerStatus.Left)
                        {
                            player.Status = PlayerStatus.Lobby;
                        }
                    }
                }),
                SelectedCardsInHand = null,
                SelectedCardsOnTable = null,
                PlayersSelections = action.PlayersSelections
            };
    }

    public record LeaveGameAction()
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, LeaveGameAction action)
            => state with
            {
                GameCode = null,
                Username = null,
                Status = null
            };
    }
}
