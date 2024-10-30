using Eremite.Buildings;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Forwindz.Content.CustomServiceables;
using System;
using UniRx;

namespace Forwindz.Content.CustomHooks
{
    public class BuildingCompletedMonitor : HookMonitor<BuildingCompletedHook, BuildingCompletedTracker>
    {
        public override void AddHandle(BuildingCompletedTracker tracker)
        {
            tracker.handle.Add(CustomServicable.OnBuildingConstructionFinished.Subscribe(new Action<Building>(tracker.Update)));
        }

        public override BuildingCompletedTracker CreateTracker(HookState state, BuildingCompletedHook model, HookedEffectModel effectModel, HookedEffectState effectState)
        {
            return new BuildingCompletedTracker(state, model, effectModel, effectState);
        }

        public override void InitValue(BuildingCompletedTracker tracker)
        {
            tracker.SetAmount(this.GetInitValueFor(tracker.model));
        }
        
        public override int GetInitValueFor(BuildingCompletedHook model)
        {
            int countedBuildings = CustomServicable.buildingsCompleted;
            if (model.ignoreDecorationBuildings)
            {
                countedBuildings -= CustomServicable.decorationBuildingsCompleted;
            }

            if (model.ignoreRoads)
            {
                countedBuildings -= CustomServicable.roadsCompleted;
            }

            return countedBuildings;
        }
        
        public override int GetInitProgressFor(BuildingCompletedHook model)
        {
            return this.GetInitValueFor(model) % model.amount;
        }
        
        public override int GetFiredAmountPreviewFor(BuildingCompletedHook model)
        {
            return this.GetInitValueFor(model) / model.amount;
        }
    }
}
