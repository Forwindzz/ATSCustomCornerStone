using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Forwindz.Framework.Services;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    public class GoodPriceFluctuationEffectModel : EffectModel
    {
        public Vector2 range;
        public string goodName;

        public override bool IsPerk => true;

        public override bool IsPositive => (range.x + range.y) >= 0.0f;

        public override string GetAmountText()
        {
            return
                Services.TextsService.GetPercentage((int)(range.x * 100.0f), true, true) + "~" +
                Services.TextsService.GetPercentage((int)(range.y * 100.0f), true, true);
        }

        public override Sprite GetDefaultIcon()
        {
            return Settings.GetGoodIcon(goodName) ?? overrideIcon;
        }

        public override float GetFloatAmount()
        {
            IDynamicGoodTypeService service = CustomServiceManager.GetService<IDynamicGoodTypeService>();
            if (service == null)
            {
                return 0.0f;
            }
            return service.GetGoodPriceFluctuation(goodName, range);
        }

        public override Color GetTypeColor()
        {
            return Settings.RewardColorRawGood;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return false;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            IDynamicGoodTypeService service = CustomServiceManager.GetService<IDynamicGoodTypeService>();
            service.TriggerGoodPriceFluctuation(goodName, range);
        }

        public override void OnRemove(EffectContextType contextType, string contextModel, int contextId)
        {
            IDynamicGoodTypeService service = CustomServiceManager.GetService<IDynamicGoodTypeService>();
            service.RemoveGoodPriceFluctuationDefine(goodName, range);
        }


    }
}
