using ATS_API.Effects;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Forwindz.Framework.Services;
using ForwindzCustomPerks.Framework.Services;
using UniRx;

namespace Forwindz.Framework.Hooks
{
    public class TradeDealMonitor : HookMonitor<TradeDealHook,TradeDealTracker>
    {
        static TradeDealMonitor()
        {
            CustomHookedEffectManager.NewHookLogic<TradeDealHook>(
                TradeDealHook.TradeDeal, new TradeDealMonitor());
        }

        public override void AddHandle(TradeDealTracker tracker)
        {
            DynamicTraderInfoService service = CustomServiceManager.GetService<DynamicTraderInfoService>();
            tracker.handle.Add(service.OnTradeCompleteEvent.Subscribe(
                (TradeDealInfo info) =>
                    tracker.UpdateDealInfo(info.visit, info.villageOffer, info.traderOffer)
                ));
        }

        public override TradeDealTracker CreateTracker(HookState state, TradeDealHook model, HookedEffectModel effectModel, HookedEffectState effectState)
        {
            return new TradeDealTracker(state, model, effectModel, effectState);
        }

        public override void InitValue(TradeDealTracker tracker)
        {
            tracker.SetAmount(this.GetInitFloatValueFor(tracker.model));
        }


    }
}
