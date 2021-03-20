using CardsAgainstWhatever.Client.Extensions;
using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Stores.Toasts
{
    public record AddToast(Toast toast)
    {
        [ReducerMethod]
        public static ToastsState Reduce(ToastsState state, AddToast action)
            => state with
            {
                Toasts = state.Toasts.CopyAndUpdate(list => list.Add(action.toast))
            };
    }

    public record RemoveToast(Toast toast)
    {
        [ReducerMethod]
        public static ToastsState Reduce(ToastsState state, RemoveToast action)
            => state with
            {
                Toasts = state.Toasts.CopyAndUpdate(list => list.Remove(action.toast))
            };
    }
}
