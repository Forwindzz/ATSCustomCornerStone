using Eremite.Buildings;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Forwindz.Scripts.Framework.Utils;

namespace Forwindz.Framework.Hooks
{
    public class BuildingCompletedTracker : HookTracker<BuildingCompletedHook>
    {
        public BuildingCompletedTracker(HookState hookState, BuildingCompletedHook model, HookedEffectModel effectModel, HookedEffectState effectState)
            : base(hookState, model, effectModel, effectState)
        {
        }

        public void Update(Building building)
        {
            string buildingCategory = building.BuildingModel.category.name;
            UnityEngine.Debug.Log("Building has category " + buildingCategory);

            if (model.ignoreDecorationBuildings && BuildingHelper.IsDecorationBuilding(building))
            {
                return;
            }

            if (model.ignoreRoads && BuildingHelper.IsRoad(building))
            {
                return;
            }

            Update(1);
        }

        public void SetAmount(int amount)
        {
            Update(amount - hookState.totalAmount);
        }

        private void Update(int amount)
        {
            hookState.totalAmount += amount;
            hookState.currentAmount += amount;
            while (hookState.currentAmount >= model.amount)
            {
                Fire();
                hookState.currentAmount -= model.amount;
            }
        }
    }
}
