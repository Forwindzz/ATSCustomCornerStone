using Eremite.Buildings;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;

namespace Forwindz.Framework.Hooks
{
    public class RelicExpireTracker : HookTracker<RelicExpireHook>
    {
        public RelicExpireTracker(HookState hookState, RelicExpireHook model, HookedEffectModel effectModel, HookedEffectState effectState) : base(hookState, model, effectModel, effectState)
        {
        }

        public void OnRelicExpireUpdate(Relic relic)
        {
            if (
                relic.model.dangerLevel == model.dangerLevel &&
                (
                    relic.state.currentDynamicEffect >= model.expireTierLeast ||
                    relic.state.continuousTicks >= model.expireLoopLeast
                )
                )
            {
                FireWithRelicInfo(relic);
            }
        }

        public void SetAmount(float amount)
        {
            this.UpdateAdd(amount - this.hookState.totalFloatAmount);
        }

        private void UpdateAdd(float value)
        {
            this.hookState.totalFloatAmount += value;
            this.hookState.currentFloatAmount += value;
            while (this.hookState.currentFloatAmount >= (float)this.model.amount)
            {
                base.Fire(EffectContextType.None, "", 0);
                this.hookState.currentFloatAmount -= (float)this.model.amount;
            }
        }

        private void FireWithRelicInfo(Relic relic, int value = 1)
        {
            this.hookState.totalFloatAmount += value;
            this.hookState.currentFloatAmount += value;
            while (this.hookState.currentFloatAmount >= (float)this.model.amount)
            {
                base.Fire(EffectContextType.Building, relic.ModelName, relic.Id);
                this.hookState.currentFloatAmount -= (float)this.model.amount;
            }
        }
    }
}
