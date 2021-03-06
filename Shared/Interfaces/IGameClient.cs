using CardsAgainstWhatever.Shared.Dtos.Events;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameClient
    {
        Task NewPlayer(PlayerJoinedEvent newPlayer);

        Task NewRound(RoundStartedEvent newRound);

        Task NewMovePlayed(PlayerMovedEvent newMovePlayedEvent);

        Task AllMovesPlayed(RoundClosedEvent allMovesPlayedEvent);
    }
}
