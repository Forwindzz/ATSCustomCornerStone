using ATS_API.Effects;
using Eremite.Buildings;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Forwindz.Framework.Services;
using Forwindz.Scripts.Framework.Services;
using System;
using UniRx;

namespace Forwindz.Framework.Hooks
{
    public class BuildingCompletedMonitor : HookMonitor<BuildingCompletedHook, BuildingCompletedTracker>
    {
        static BuildingCompletedMonitor()
        {
            CustomHookedEffectManager.NewHookLogic<BuildingCompletedHook>(
                BuildingCompletedHook.EnumBuildingComplete, new BuildingCompletedMonitor());
        }

        public override void AddHandle(BuildingCompletedTracker tracker)
        {
            tracker.handle.Add(
                CustomServiceManager.GetService<BuildingMonitorService>()
                .OnBuildingConstructionFinished.Subscribe(
                    new Action<Building>(tracker.Update)));
        }

        public override BuildingCompletedTracker CreateTracker(HookState state, BuildingCompletedHook model, HookedEffectModel effectModel, HookedEffectState effectState)
        {
            return new BuildingCompletedTracker(state, model, effectModel, effectState);
        }

        public override void InitValue(BuildingCompletedTracker tracker)
        {
            tracker.SetAmount(GetInitValueFor(tracker.model));
        }

        public override int GetInitValueFor(BuildingCompletedHook model)
        {
            var service = CustomServiceManager.GetService<BuildingMonitorService>();
            int countedBuildings = service.state.buildingsCompleted;
            if (model.ignoreDecorationBuildings)
            {
                countedBuildings -= service.state.decorationBuildingsCompleted;
            }

            if (model.ignoreRoads)
            {
                countedBuildings -= service.state.roadsCompleted;
            }

            return countedBuildings;
        }

        public override int GetInitProgressFor(BuildingCompletedHook model)
        {
            return GetInitValueFor(model) % model.amount;
        }

        public override int GetFiredAmountPreviewFor(BuildingCompletedHook model)
        {
            return GetInitValueFor(model) / model.amount;
        }
    }
}
