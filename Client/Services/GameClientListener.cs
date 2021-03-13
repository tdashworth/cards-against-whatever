using BlazorComponentBus;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Services
{
    public class GameClientListener : IGameClient
    {
        private readonly BlazorComponentBus.ComponentBus Bus;

        public GameClientListener(ComponentBus bus)
        {
            Bus = bus;
        }

        public Task GameJoined(GameJoinedEvent gameJoinedEvent) => Bus.Publish(gameJoinedEvent);

        public Task PlayerJoined(PlayerJoinedEvent playerJoinedEvent) => Bus.Publish(playerJoinedEvent);

        public Task RoundStarted(RoundStartedEvent roundStartedEvent) => Bus.Publish(roundStartedEvent);

        public Task PlayerMoved(PlayerMovedEvent playerMovedEvent) => Bus.Publish(playerMovedEvent);

        public Task RoundClosed(RoundClosedEvent roundClosedEvent) => Bus.Publish(roundClosedEvent);

        public Task RoundEnded(RoundEndedEvent roundEndedEvent) => Bus.Publish(roundEndedEvent);
    }
}
