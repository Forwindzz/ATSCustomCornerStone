using ATS_API.Effects;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Forwindz.Framework.Services;
using UniRx;

namespace Forwindz.Framework.Hooks
{
    public class RelicExpireMonitor : HookMonitor<RelicExpireHook,RelicExpireTracker>
    {
        static RelicExpireMonitor()
        {
            CustomHookedEffectManager.NewHookLogic<RelicExpireHook>(
                RelicExpireHook.HookLogicTypeEnum, new RelicExpireMonitor());
        }

        public override void AddHandle(RelicExpireTracker tracker)
        {
            IDynamicRelicService service = CustomServiceManager.GetService<IDynamicRelicService>();
            tracker.handle.Add(
                service.OnRelicExpire.Subscribe(
                tracker.OnRelicExpireUpdate)
                );
        }

        public override RelicExpireTracker CreateTracker(HookState state, RelicExpireHook model, HookedEffectModel effectModel, HookedEffectState effectState)
        {
            return new RelicExpireTracker(state, model, effectModel, effectState);
        }

        public override void InitValue(RelicExpireTracker tracker)
        {
            tracker.SetAmount(this.GetInitFloatValueFor(tracker.model));
        }
    }
}
