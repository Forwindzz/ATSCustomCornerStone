using Eremite;
using Eremite.Buildings;
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
    public class ModifyRelicExpireEffectModel : EffectModel
    {
        public string relicName;
        public int tierIndex;
        public string effectName;
        public RelicArrayOperation operation;

        public override bool IsPerk => true;

        public override bool IsPositive
        {
            get
            {
                bool effectIsPositive = MB.Settings.GetEffect(effectName)?.IsPositive ?? true;
                return
                    effectIsPositive && operation == RelicArrayOperation.Add ||
                    !effectIsPositive && operation == RelicArrayOperation.Remove;
            }
        }

        public override bool Fits()
        {
            return base.Fits() &&
                (MB.Settings.GetEffect(effectName)?.Fits() ?? false) &&
                (MB.Settings.GetRelic(relicName) != null)
                ;
        }

        public override string GetAmountText()
        {
            return MB.Settings.GetRelic(relicName)?.displayName?.GetText();
        }

        public override Sprite GetDefaultIcon()
        {
            return MB.Settings.GetEffect(effectName)?.GetIcon() ?? overrideIcon;
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
            if(service!=null)
            {
                service.ModifyExpireEffect(relicName, tierIndex, effectName, operation);
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
                service.RemoveModifyExpireEffect(relicName, tierIndex, effectName, operation);
            }
            else
            {
                FLog.Error("Cannot find IDynamicRelicService!");
            }
        }
    }
}
