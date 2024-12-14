using Cysharp.Threading.Tasks;
using Eremite.Buildings;
using Eremite.Services;
using Forwindz.Framework.Services;
using Forwindz.Framework.Utils;
using Forwindz.Scripts.Framework.Utils;
using HarmonyLib;
using System;
using UniRx;

namespace Forwindz.Scripts.Framework.Services
{

    public class BuildingMonitorState
    {
        public int buildingsCompleted = 0;
        public int decorationBuildingsCompleted = 0;
        public int roadsCompleted = 0;

        public int buildingsRemoved = 0;
        public int decorationBuildingsRemoved = 0;
        public int roadsRemoved = 0;
    }


    public class BuildingMonitorService : GameService, IService
    {
        [ModSerializedField]
        public BuildingMonitorState state = new();

        public readonly Subject<Building> buildingConstructionFinishedSubject = new Subject<Building>();
        public readonly Subject<Building> buildingRemovedSubject = new Subject<Building>();
        public IObservable<Building> OnBuildingConstructionFinished => buildingConstructionFinishedSubject;
        public IObservable<Building> OnBuildingRemovedFinished => buildingRemovedSubject;



        static BuildingMonitorService()
        {
            CustomServiceManager.RegGameService<BuildingMonitorService>();
            PatchesManager.RegPatch<BuildingMonitorService>();
        }

        public override IService[] GetDependencies()
        {
            return new IService[] {
                Serviceable.BuildingsService,
                CustomServiceManager.GetAsIService<IExtraStateService>(),
                CustomServiceManager.GetAsIService<IDynamicBuildingStateService>(),
                CustomServiceManager.GetAsIService<IDynamicHearthService>()
            };
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override UniTask OnLoading()
        {
            return UniTask.CompletedTask;
        }

        protected void OnBuildingCompleted(Building building)
        {

            state.buildingsCompleted++;

            if (BuildingHelper.IsDecorationBuilding(building))
            {
                state.decorationBuildingsCompleted++;
            }

            if (BuildingHelper.IsRoad(building))
            {
                state.roadsCompleted++;
            }

            buildingConstructionFinishedSubject.OnNext(building);
        }

        protected void OnBuildingRemoved(Building building)
        {
            state.buildingsRemoved++;

            if (BuildingHelper.IsDecorationBuilding(building))
            {
                state.decorationBuildingsRemoved++;
            }

            if (BuildingHelper.IsRoad(building))
            {
                state.roadsRemoved++;
            }

            buildingRemovedSubject.OnNext(building);
        }

        #region Patch

        [HarmonyPatch(typeof(Building), nameof(Building.FinishConstruction))]
        [HarmonyPostfix]
        private static void Building_FinishConstruction_Postfix(Building __instance)
        {
            FLog.Info($"Building finish: {__instance.name} ||\t Category: {__instance.BuildingModel.category.name}");
            CustomServiceManager.GetService<BuildingMonitorService>().OnBuildingCompleted(__instance);
        }

        [HarmonyPatch(typeof(Building), nameof(Building.Remove))]
        [HarmonyPostfix]
        private static void Building_Remove_Postfix(Building __instance)
        {
            FLog.Info($"Building remove: {__instance.name} ||\t Category: {__instance.BuildingModel.category.name}");
            CustomServiceManager.GetService<BuildingMonitorService>().OnBuildingCompleted(__instance);
        }

        #endregion
    }
}
