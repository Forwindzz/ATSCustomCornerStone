using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Model.Trade;
using Forwindz.Framework.Services;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    /// <summary>
    /// Only this trader will come to your settlement
    /// If this effect is applied several times with different traders, then randomly choose one trader who is marked as "Only"
    /// </summary>
    public class OnlyTraderEffectModel : EffectModel
    {
        //TODO: maybe deal with text?
        public TraderModel trader;

        public override bool IsPerk => true;

        public override string GetAmountText()
        {
            return trader.displayName.Text;
        }

        public override Sprite GetDefaultIcon()
        {
            return trader?.icon;
        }

        public override Color GetTypeColor()
        {
            return Settings.RewardColorMerchantsReproach;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return false;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            CustomServiceManager.GetService<IDynamicTraderInfoService>().AddForceTrader(trader.Name);
        }

        public override void OnRemove(EffectContextType contextType, string contextModel, int contextId)
        {
            CustomServiceManager.GetService<IDynamicTraderInfoService>().RemoveForceTrader(trader.Name);
        }


    }
}
