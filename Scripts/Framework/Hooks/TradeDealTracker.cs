using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Eremite.Model.State;
using Eremite.Model.Trade;
using UnityEngine;

namespace Forwindz.Framework.Hooks
{
    public class TradeDealTracker : HookTracker<TradeDealHook>
    {
        public TradeDealTracker(HookState hookState, TradeDealHook model, HookedEffectModel effectModel, HookedEffectState effectState) : base(hookState, model, effectModel, effectState)
        {
        }

        public void UpdateDealInfo(TraderVisitState visit, TradingOffer villageOffer, TradingOffer traderOffer)
        {
            float v = 0.0f;
            switch (model.valueType)
            {
                case TradeDealHook.ValueType.SellValue:
                    v = TradeService.GetValueInCurrency(villageOffer);
                    break;
                case TradeDealHook.ValueType.BuyValue:
                    v = TradeService.GetValueInCurrency(traderOffer);
                    break;
                case TradeDealHook.ValueType.BuyExceedValue:
                    v = TradeService.GetValueInCurrency(villageOffer) - TradeService.GetValueInCurrency(traderOffer);
                    break;
            }

            bool canFire = false;

            switch (model.judgeType)
            {
                case TradeDealHook.JudgeType.Higher:
                    canFire = v > model.amount;
                    break;
                case TradeDealHook.JudgeType.Lower:
                    canFire = v < model.amount;
                    break;
                case TradeDealHook.JudgeType.Equal:
                    canFire = Mathf.Abs(v - model.amount) < 0.01f;
                    break;
            }

            if(canFire)
            {
                Fire();
            }
        }

        public void SetAmount(float amount)
        {
            this.UpdateTo(amount - this.hookState.totalFloatAmount);
        }

        private void UpdateTo(float value)
        {
            this.hookState.totalFloatAmount += value;
            this.hookState.currentFloatAmount += value;
            while (this.hookState.currentFloatAmount >= (float)this.model.amount)
            {
                base.Fire();
                this.hookState.currentFloatAmount -= (float)this.model.amount;
            }
        }


    }
}
