using System;
using System.Collections.Generic;

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
    }
}
