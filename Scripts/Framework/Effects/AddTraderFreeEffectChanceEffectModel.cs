using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Model.Trade;
using Forwindz.Framework.Services;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    public class AddTraderFreeEffectChanceEffectModel : EffectModel
    {
        public TraderModel trader;
        public float chance;
        public override bool IsPerk => true;

        public override string GetAmountText()
        {
            return this.GetPercentage(chance);
        }

        public override Sprite GetDefaultIcon()
        {
            return trader?.icon ?? overrideIcon;
        }

        public override Color GetTypeColor()
        {
            return Settings.RewardColorMerchantsReproach;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return false;
        }

        public override float GetFloatAmount()
        {
            return chance;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            CustomServiceManager.GetService<IDynamicTraderInfoService>().AddEffectFreeChance(trader.Name, chance);
        }

        public override void OnRemove(EffectContextType contextType, string contextModel, int contextId)
        {
            CustomServiceManager.GetService<IDynamicTraderInfoService>().AddEffectFreeChance(trader.Name, -chance);
        }

    }
}
