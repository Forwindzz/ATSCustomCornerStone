using Eremite.Buildings;
using Eremite.Model.Effects;
using Forwindz.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Hooks
{
    public class DecorationHook : HookLogic
    {
        [NewCustomEnum]
        public static readonly HookLogicType EnumDecorationPoints;

        public override HookLogicType Type => EnumDecorationPoints;

        public override string FormatedDescription => 
            base.TryFormat(this.description.Text, decorationTier.displayName.Text, amount);

        public override string GetDescriptionInfo => "{0} - decoration tier, {1} - value";

        public DecorationTier decorationTier;
        public int amount;

        static DecorationHook()
        {
            CustomIntEnumManager<HookLogicType>.ScanAndAssignEnumValues<DecorationHook>();
        }

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


    }
}
