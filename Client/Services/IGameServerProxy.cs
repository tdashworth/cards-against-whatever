using CardsAgainstWhatever.Shared.Dtos.Actions;
using CardsAgainstWhatever.Shared.Dtos.Events;
using CardsAgainstWhatever.Shared.Interfaces;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Services
{
    public interface IGameServerProxy : IGameServer
    {
        Task<GameCreatedEvent> CreateGame(CreateGameAction action);
        Task JoinGame(JoinGameAction action);
    }
}