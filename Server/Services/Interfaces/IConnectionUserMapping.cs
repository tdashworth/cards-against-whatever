using System.Collections.Generic;

namespace CardsAgainstWhatever.Server.Services.Interfaces
{
    public record GameCodeAndUsername(string GameCode, string Username);
    public interface IConnectionUserMapping : IDictionary<string, GameCodeAndUsername>
    {
    }
}
