using Eremite;
using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Forwindz.Framework.Services;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    public class NoQueenCornerstoneEffectModel : EffectModel
    {
        public bool removeCornerstoneFlag = true;

        public override bool IsPerk => true;

        public override bool IsPositive => !removeCornerstoneFlag;

        public override string GetAmountText()
        {
            return removeCornerstoneFlag?"v":"x";
        }

        public override Sprite GetDefaultIcon()
        {
            return overrideIcon;
        }

        public override Color GetTypeColor()
        {
            return MB.Settings.RewardColorNegativeResolveEffect;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return false;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            IDynamicCornerstoneService service = CustomServiceManager.GetService<IDynamicCornerstoneService>();
            service.SetNoCornerstone(removeCornerstoneFlag);
        }

        public override void OnRemove(EffectContextType contextType, string contextModel, int contextId)
        {
            IDynamicCornerstoneService service = CustomServiceManager.GetService<IDynamicCornerstoneService>();
            service.SetNoCornerstone(!removeCornerstoneFlag);
        }
    }
}
