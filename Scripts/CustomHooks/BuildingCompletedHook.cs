using Eremite.Buildings;
using Eremite.Model.Effects;
using UnityEngine;

namespace Forwindz.Content.CustomHooks
{
    public class BuildingCompletedHook : HookLogic
    {
        [Min(0f)]
        public int amount = 1;
        public bool ignoreDecorationBuildings = true;
        public bool ignoreRoads = true;

        public override HookLogicType Type => (HookLogicType)CustomHookType.BuildingCompleted;

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
