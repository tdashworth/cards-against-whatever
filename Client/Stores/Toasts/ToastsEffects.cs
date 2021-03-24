using Fluxor;
using System;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Stores.Toasts
{
    public class ToastsEffects
    {
        [EffectMethod]
        public async Task Handle(AddToast action, IDispatcher dispatcher)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            dispatcher.Dispatch(new RemoveToast(action.toast));
        }
    }
}
