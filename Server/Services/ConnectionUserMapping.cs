using CardsAgainstWhatever.Server.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace CardsAgainstWhatever.Server.Services
{
    public class ConnectionUserMapping : Dictionary<string, GameCodeAndUsername>, IConnectionUserMapping
    {
    }
}
