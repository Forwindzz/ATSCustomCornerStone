using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Forwindz.Framework.Services;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    public class DecorationPercentEffectModel : EffectModel
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
            return building is DecorationModel;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            CustomServiceManager.GetService<IDynamicBuildingStateService>().AddAllDecorationPercent(percent);
        }

        public override void OnRemove(EffectContextType contextType, string contextModel, int contextId)
        {
            CustomServiceManager.GetService<IDynamicBuildingStateService>().AddAllDecorationPercent(-percent);
        }

        public override bool IsPositive => percent >= 0.0f;

    }
}
