using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Forwindz.Framework.Services;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    public class HearthPopPercentEffectModel : EffectModel
    {
        public float percent = 0.0f;

        public override bool IsPerk => true;

        public override string GetAmountText()
        {
            return this.GetPercentage(percent);
        }

        public override Sprite GetDefaultIcon()
        {
            throw null;
        }

        public override Color GetTypeColor()
        {
            return Settings.RewardColorBuildingProduction;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return building is HearthModel;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            CustomServiceManager.GetService<IDynamicHearthService>().AddHearthRequirePopPercent(percent);
        }

        public override void OnRemove(EffectContextType contextType, string contextModel, int contextId)
        {
            CustomServiceManager.GetService<IDynamicHearthService>().AddHearthRequirePopPercent(-percent);
        }

        public override bool IsPositive => percent >= 0.0f;
    }
}
