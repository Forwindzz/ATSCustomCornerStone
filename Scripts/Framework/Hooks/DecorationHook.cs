using Eremite.Buildings;
using Eremite.Model.Effects;
using Forwindz.Framework.Utils;

namespace Forwindz.Framework.Hooks
{
    public class DecorationHook : HookLogic
    {
        public static readonly HookLogicType HookLogicTypeEnum =
            GUIDManagerExtend.Get<HookLogicType>(PluginInfo.PLUGIN_GUID, "DecorationHook");

        public override HookLogicType Type => HookLogicTypeEnum;

        public override string FormatedDescription => 
            base.TryFormat(this.description.Text, decorationTier.displayName.Text, amount);

        public override string GetDescriptionInfo => "{0} - decoration tier, {1} - value";

        public DecorationTier decorationTier;
        public int amount;

        public override string GetAmountText()
        {
            return amount.ToString();
        }

        public override int GetIntAmount()
        {
            return amount;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return false;
        }

        public override bool CanBeDrawn()
        {
            return true;//consider check decoration building
        }
    }
}
