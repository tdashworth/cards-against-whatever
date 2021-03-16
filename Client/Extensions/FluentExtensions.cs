using System;
using System.Collections.Generic;
using System.Linq;

namespace CardsAgainstWhatever.Client.Extensions
{
    public static class FluentExtensions
    {
        public static T Update<T>(this T obj, Action<T> update)
        {
            update(obj);
            return obj;
        }

        public static IEnumerable<T> UpdateEach<T>(this IEnumerable<T> list, Action<T> update)
            => list.Select(item => item.Update(update));
    }
}
