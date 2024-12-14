using Eremite.Buildings;
using Eremite.Model.Effects;
using Forwindz.Framework.Utils;
using UnityEngine;

namespace Forwindz.Framework.Hooks
{
    public class BuildingCompletedHook : HookLogic
    {
        [NewCustomEnum]
        public static readonly HookLogicType EnumBuildingComplete;

        [Min(0f)]
        public int amount = 1;
        public bool ignoreDecorationBuildings = true;
        public bool ignoreRoads = true;

        public override HookLogicType Type => EnumBuildingComplete;
        static BuildingCompletedHook()
        {
            CustomIntEnumManager<HookLogicType>.ScanAndAssignEnumValues<BuildingCompletedHook>();
        }

        public override bool CanBeDrawn()
        {
            return true;
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
