using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Extensions
{
    public static class CollectionExtensions
    {
        public static IList<T> Shuffle<T>(this IList<T> list, Random random)
        {
            List<T> randomizedList = new List<T>();
            while (list.Count > 0)
            {
                int index = random.Next(0, list.Count);
                randomizedList.Add(list[index]);
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        public static Task<T[]> AwaitAll<T>(this IEnumerable<Task<T>> list)
            => Task.WhenAll(list);

        public static Task AwaitAll(this IEnumerable<Task> list)
            => Task.WhenAll(list);
    }
}
