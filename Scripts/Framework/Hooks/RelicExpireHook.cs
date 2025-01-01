using Eremite.Buildings;
using Eremite.Model.Effects;
using Forwindz.Framework.Utils;

namespace Forwindz.Framework.Hooks
{
    public class RelicExpireHook : HookLogic
    {
        public static readonly HookLogicType HookLogicTypeEnum =
            GUIDManagerExtend.Get<HookLogicType>(PluginInfo.PLUGIN_GUID, "RelicExpireHook");
        public override HookLogicType Type => HookLogicTypeEnum;

        public DangerLevel dangerLevel;
        public int amount = 1;
        // satisfy one of two conditions:
        public int expireLoopLeast = 1; // the current effect tier is >= this
        public int expireTierLeast = 1; // the current loop is >= this (when reach max effect tier, it will continue loop from last tier)

        public override string GetAmountText()
        {
            return amount.ToString();
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            if(building is RelicModel relic)
            {
                return relic.dangerLevel == dangerLevel &&
                    (relic.effectsTiers?.Length ?? 0) >= expireTierLeast;
            }
            return false;
        }

        public override int GetIntAmount()
        {
            return amount;
        }
    }
}
