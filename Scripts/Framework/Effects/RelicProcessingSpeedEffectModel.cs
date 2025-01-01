using Eremite.Buildings;
using Eremite;
using Eremite.Model;
using Eremite.Model.Effects;
using Forwindz.Framework.Services;
using Forwindz.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    public class RelicProcessingSpeedEffectModel : EffectModel
    {
        public string relicName;
        public float amount;

        public override bool IsPerk => true;

        public override bool IsPositive => amount >= 0;

        public override bool Fits()
        {
            return base.Fits() && (MB.Settings.GetRelic(relicName) != null);
        }

        public override string GetAmountText()
        {
            return MB.TextsService.GetPercentage((int)(amount * 100.0f), true, true);
        }

        public override Sprite GetDefaultIcon()
        {
            return overrideIcon;
        }

        public override Color GetTypeColor()
        {
            return MB.Settings.DangerousThreatGladeColor;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return building.Name == relicName;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            IDynamicRelicService service = CustomServiceManager.GetService<IDynamicRelicService>();
            if (service != null)
            {
                service.AddProcessSpeed(relicName, amount);
            }
            else
            {
                FLog.Error("Cannot find IDynamicRelicService!");
            }
        }

        public override void OnRemove(EffectContextType contextType, string contextModel, int contextId)
        {
            IDynamicRelicService service = CustomServiceManager.GetService<IDynamicRelicService>();
            if (service != null)
            {
                service.AddProcessSpeed(relicName, -amount);
            }
            else
            {
                FLog.Error("Cannot find IDynamicRelicService!");
            }
        }

        public override float GetFloatAmount()
        {
            return amount;
        }
    }
}
