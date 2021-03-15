using CardsAgainstWhatever.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Extensions
{
    public static class CollectionExtensions
    {
        public static List<T> CopyAndUpdate<T>(this IReadOnlyList<T> orginalList, Action<List<T>> updateAction)
        {
            var newList = new List<T>();
            newList.AddRange(orginalList);
            updateAction(newList);
            return newList;
        }

        public static Player? FindByUsername(this IEnumerable<Player>? players, string username)
            => players?.FirstOrDefault(player => player.Username == username);
    }
}
