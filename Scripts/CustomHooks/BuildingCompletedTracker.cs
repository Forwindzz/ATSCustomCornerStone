using Eremite.Buildings;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Forwindz.Content.CustomHooks;
using Forwindz.Framework.Utils;

namespace Forwindz.Content.CustomHooks
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

            if (model.ignoreDecorationBuildings && Utils.IsDecorationBuilding(building))
            {                
                return;
            }

            if (model.ignoreRoads && Utils.IsRoad(building))
            {                
                return;
            }

            this.Update(1);
        }
        
        public void SetAmount(int amount)
        {
            this.Update(amount - this.hookState.totalAmount);
        }

        private void Update(int amount)
        {
            this.hookState.totalAmount += amount;
            this.hookState.currentAmount += amount;
            while (this.hookState.currentAmount >= this.model.amount)
            {
                base.Fire();
                this.hookState.currentAmount -= this.model.amount;
            }
        }
    }
}
