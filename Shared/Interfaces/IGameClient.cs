using CardsAgainstWhatever.Shared.Dtos;
using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared.Interfaces
{
    public interface IGameClient
    {
        Task NewPlayer(NewPlayerEvent newPlayer);

        Task NewRound(NewRoundEvent newRound);

        Task NewMovePlayed(NewMovePlayedEvent newMovePlayedEvent);

        Task AllMovesPlayed(AllMovesPlayedEvent allMovesPlayedEvent);
    }
}
