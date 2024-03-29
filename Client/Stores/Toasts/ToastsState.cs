﻿using Fluxor;
using System.Collections.Generic;

namespace CardsAgainstWhatever.Client.Stores.Toasts
{
    public record Toast(string Title, string Message);

    public record ToastsState(IReadOnlyList<Toast> Toasts);

    public class ToastsFeature : Feature<ToastsState>
    {
        public override string GetName() => "Toasts";

        protected override ToastsState GetInitialState() => new ToastsState(new List<Toast>());
    }
}
