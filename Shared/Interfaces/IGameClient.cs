using CardsAgainstWhatever.Shared.Dtos.Events;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameClient
    {
        Task GameJoined(GameJoinedEvent gameJoinedEvent);
        Task PlayerJoined(PlayerJoinedEvent playerJoinedEvent);

        Task RoundStarted(RoundStartedEvent roundStartedEvent);

        Task PlayerMoved(PlayerMovedEvent playerMovedEvent);

        Task RoundClosed(RoundClosedEvent roundClosedEvent); 
        Task RoundEnded(RoundEndedEvent roundEndedEvent);
    }
}
