using ATS_API.Effects;
using Eremite.Buildings;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Forwindz.Framework.Services;
using Forwindz.Scripts.Framework.Services;
using Forwindz.Scripts.Framework.Utils;
using System;
using System.Collections.Generic;
using UniRx;

namespace Forwindz.Framework.Hooks
{
    public class DecorationMonitor : HookMonitor<DecorationHook, DecorationTracker>
    {
        static DecorationMonitor()
        {
            CustomHookedEffectManager.NewHookLogic<DecorationHook>(
                DecorationHook.HookLogicTypeEnum, new DecorationMonitor());
        }


        public override void AddHandle(DecorationTracker tracker)
        {
            var buildingMonitorService = CustomServiceManager.GetService<BuildingMonitorService>();
            tracker.handle.Add(
                buildingMonitorService.OnBuildingConstructionFinished.Subscribe(new Action<Building>(tracker.OnAddBuilding))
                );

            tracker.handle.Add(
                buildingMonitorService.OnBuildingRemovedFinished.Subscribe(new Action<Building>(tracker.OnRemoveBuilding))
                );

            var dynamicBuildingStateService = CustomServiceManager.GetService<DynamicBuildingStateService>();
            tracker.handle.Add(
                dynamicBuildingStateService.OnDecorationValueChange.Subscribe(
                    (Dictionary<string, DynamicDecorationStateInfo> info) => tracker.Recalculate())
                );
        }

        public override DecorationTracker CreateTracker(HookState state, DecorationHook model, HookedEffectModel effectModel, HookedEffectState effectState)
        {
            return new DecorationTracker(state, model, effectModel, effectState);
        }

        public override void InitValue(DecorationTracker tracker)
        {
            tracker.Recalculate();
        }

        public override int GetInitValueFor(DecorationHook model)
        {
            return BuildingHelper.CountDecorationValue(model.decorationTier);
        }

        public override int GetInitProgressFor(DecorationHook model)
        {
            return GetInitValueFor(model) % model.amount;
        }

        public override int GetFiredAmountPreviewFor(DecorationHook model)
        {
            return GetInitValueFor(model) / model.amount;
        }
    }
}
