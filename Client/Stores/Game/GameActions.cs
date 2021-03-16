using CardsAgainstWhatever.Client.Extensions;
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
                GameCode = action.GameCode,
                Username = action.Username
            };
    }

    public record GameJoinedEvent(List<Player> ExistingPlayersInGame, int? CurrentRoundNumber, QuestionCard? CurrentQuestion, Player? CurrentCardCzar)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, GameJoinedEvent action)
            => state with
            {
                IsLoading = false,
                Players = action.ExistingPlayersInGame,
                CardsInHand = new List<AnswerCard>(),
                CurrentRoundNumber = action.CurrentRoundNumber,
                CurrentQuestion = action.CurrentQuestion,
                CurrentCardCzar = action.CurrentCardCzar?.Username is not null ? state.Players.FindByUsername(action.CurrentCardCzar.Username) : null
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
                    .Update(player => player!.State = PlayerState.Left))
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

    public record RoundStartedEvent(int CurrentRoundNumber, QuestionCard CurrentQuestion, string CurrentCardCzarUsername, List<AnswerCard> DealtCards)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, RoundStartedEvent action)
            => state with
            {
                IsLoading = false,
                Status = GameStatus.CollectingAnswers,
                Players = state.Players!.UpdateEach(player =>
                {
                    if (player.State != PlayerState.Left)
                    {
                        player.State = PlayerState.PlayingAnswer;
                    }
                }).ToList(),
                CurrentRoundNumber = action.CurrentRoundNumber,
                CurrentQuestion = action.CurrentQuestion,
                CurrentCardCzar = state.Players
                                            .FindByUsername(action.CurrentCardCzarUsername)
                                            .Update(player => player!.State = PlayerState.AwatingAnswers),
                CardsOnTable = new List<List<AnswerCard>>(),
                CardsInHand = state.CardsInHand!.CopyAndUpdate(cards => cards.AddRange(action.DealtCards)),
                SelectedCardsInHand = new List<AnswerCard>(),
                SelectedCardsOnTable = new List<AnswerCard>()
            };
    }

    public record AnswerSelectedAction(AnswerCard Answer)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, AnswerSelectedAction action)
            => state with
            {
                SelectedCardsInHand = state.SelectedCardsInHand!.CopyAndUpdate(cards =>
                {
                    if (state.CurrentQuestion!.Picks == 1)
                    {
                        cards.Clear();
                        cards.Add(action.Answer);
                    }
                    else
                    {
                        if (cards.Contains(action.Answer))
                        {
                            cards.Remove(action.Answer);
                        }
                        else if (cards.Count < state.CurrentQuestion!.Picks)
                        {
                            cards.Add(action.Answer);
                        }
                    }
                })
            };
    }

    public record PlayAnswerAction()
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, StartRoundAction action)
            => state with { };
    }

    public record AnswerPlayedEvent(string Username)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, AnswerPlayedEvent action)
            => state with
            {
                Players = state.Players!.CopyAndUpdate(players =>
                {
                    var player = players.FindByUsername(action.Username);
                    if (player is not null) player.State = PlayerState.AnswerPlayed;
                }),
                CardsOnTable = state.CardsOnTable!.CopyAndUpdate(cards =>
                {
                    var blankCards = Enumerable.Range(1, state.CurrentQuestion!.Picks).Select(_ => new AnswerCard("")).ToList();
                    cards.Add(blankCards);
                })
            };
    }

    public record WinnerSelectedAction(List<AnswerCard> WinningAnswer)
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
            => state with { };
    }

    public record RoundClosedEvent(List<List<AnswerCard>> PlayedCardsGroupedPerPlayer)
    {
        [ReducerMethod]
        public static GameState Reduce(GameState state, RoundClosedEvent action)
            => state with
            {
                Status = GameStatus.SelectingWinner,
                CurrentCardCzar = state.CurrentCardCzar.Update(player => player!.State = PlayerState.PickingWinner),
                CardsOnTable = action.PlayedCardsGroupedPerPlayer
            };
    }

    public record RoundEndedEvent(string Username)
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
                        if (player.State != PlayerState.Left)
                        {
                            player.State = PlayerState.InLobby;
                        }
                    }
                }),
            };
    }
}
