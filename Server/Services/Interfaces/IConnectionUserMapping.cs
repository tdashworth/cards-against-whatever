using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services.Interfaces
{
    public record GameCodeAndUsername(string GameCode, string Username);
    public interface IConnectionUserMapping : IDictionary<string, GameCodeAndUsername>
    {
    }
}
