using CardsAgainstWhatever.Client.Stores.Game;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Interfaces;
using Fluxor;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Services
{
    public class GameClientListener : IGameClient
    {
        private readonly IDispatcher dispatcher;

        public GameClientListener(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task GameJoined(CardsAgainstWhatever.Shared.Dtos.Events.GameJoinedEvent gameJoinedEvent) => Task.Run(() =>
        {
            dispatcher.Dispatch(new Stores.Game.GameJoinedEvent(gameJoinedEvent.ExistingPlayersInGame));
        });

        public Task PlayerJoined(CardsAgainstWhatever.Shared.Dtos.Events.PlayerJoinedEvent playerJoinedEvent) => Task.Run(() =>
        {
            dispatcher.Dispatch(new Stores.Game.PlayerJoinedEvent(playerJoinedEvent.NewPlayer));
        });

        public Task RoundStarted(CardsAgainstWhatever.Shared.Dtos.Events.RoundStartedEvent roundStartedEvent) => Task.Run(() =>
        {
            dispatcher.Dispatch(new Stores.Game.RoundStartedEvent(
                roundStartedEvent.RoundNumber,
                roundStartedEvent.QuestionCard, 
                roundStartedEvent.CardCzar.Username,
                roundStartedEvent.DealtCards));
        });

        public Task PlayerMoved(PlayerMovedEvent playerMovedEvent) => Task.Run(() =>
        {
            dispatcher.Dispatch(new AnswerPlayedEvent(playerMovedEvent.Username));
        });

        public Task RoundClosed(CardsAgainstWhatever.Shared.Dtos.Events.RoundClosedEvent roundClosedEvent) => Task.Run(() =>
        {
            dispatcher.Dispatch(new Stores.Game.RoundClosedEvent(roundClosedEvent.PlayedCardsGroupedPerPlayer));
        });

        public Task RoundEnded(CardsAgainstWhatever.Shared.Dtos.Events.RoundEndedEvent roundEndedEvent) => Task.Run(() =>
        {
            dispatcher.Dispatch(new Stores.Game.RoundEndedEvent(roundEndedEvent.Winner.Username));
        });
    }
}
