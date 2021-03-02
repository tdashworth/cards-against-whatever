using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared
{
    public interface IGameClient
    {
        Task NewPlayer(Player player);

        Task NewRound(List<AnswerCard> answerCards);
    }
}
