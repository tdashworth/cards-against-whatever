using CardsAgainstWhatever.Client.Extensions;
using CardsAgainstWhatever.Shared.Models;
using Fluxor;
using System.Collections.Generic;

namespace CardsAgainstWhatever.Client.Stores.Game
{
    public record GameState(
        bool IsLoading,
        string? CurrentErrorMessage,
        string? GameCode,
        string? Username,
        GameStatus? Status,
        IReadOnlyList<Player>? Players,
        int? CurrentRoundNumber,
        QuestionCard? CurrentQuestion,
        Player? CurrentCardCzar,
        IReadOnlyList<List<AnswerCard>>? CardsOnTable,
        IReadOnlyList<AnswerCard>? SelectedCardsOnTable,
        IReadOnlyList<AnswerCard>? CardsInHand,
        IReadOnlyList<AnswerCard>? SelectedCardsInHand)
    {
        public bool IsCardCzar => CurrentCardCzar?.Username == Username;
        public Player? Me => Players?.FindByUsername(Username ?? "");
    }

    public class GameFeature : Feature<GameState>
    {
        public override string GetName() => "Game";

        protected override GameState GetInitialState() => new GameState(false, null, null, null, null, null, null, null, null, null, null, null, null);
    }
}
