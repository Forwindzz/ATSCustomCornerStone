using Cysharp.Threading.Tasks;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Eremite.Services;
using Forwindz.Framework.Services;
using Forwindz.Framework.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Hooks
{

    public class CustomHookedEffectsController
    {
        static CustomHookedEffectsController()
        {
            PatchesManager.RegPatch<CustomHookedEffectsController>();
        }

        private static CustomHookedEffectsController instance = new();
        public static CustomHookedEffectsController Instance => instance;

        private Dictionary<HookLogicType, IHookMonitor> typeMonitorMapping = new();

        public static T Register<T>(HookLogicType hookLogicType) where T : IHookMonitor, new()
        {
            T monitor = new();
            Register(hookLogicType, monitor);
            return monitor;
        }

        public static void Register(HookLogicType hookLogicType, IHookMonitor monitor)
        {
            Dictionary<HookLogicType, IHookMonitor> typeMonitorMapping = Instance.typeMonitorMapping;
            if (typeMonitorMapping.ContainsKey(hookLogicType))
            {
                FLog.Error($"Already Registered HookLogicType {hookLogicType}! This will overwrite the previous reg!");
            }
            typeMonitorMapping[hookLogicType] = monitor;
            FLog.Info($"Reg enum {hookLogicType} = {monitor.GetType().Name}");
        }


        #region Patches

        // init
        /*
        [HarmonyPatch(typeof(HookedEffectsController), nameof(HookedEffectsController.SetUp))]
        [HarmonyPrefix]
        private static void HookedEffectsController_SetUp_PrePatch(HookedEffectsController __instance)
        {
        }
        */

        // destory
        [HarmonyPatch(typeof(HookedEffectsController), nameof(HookedEffectsController.OnDestroy))]
        [HarmonyPostfix]
        private static void HookedEffectsController_OnDestroy_PostPatch(HookedEffectsController __instance)
        {
            var typeMonitorMapping = Instance.typeMonitorMapping;
            foreach(IHookMonitor monitor in typeMonitorMapping.Values )
            {
                monitor.Dispose();
            }
        }

        // HookLogicType -> IHookMonitor
        [HarmonyPatch(typeof(HookedEffectsController), nameof(HookedEffectsController.GetMonitorFor))]
        [HarmonyPrefix]
        private static bool HookedEffectsController_GetMonitorFor_PrePatch(HookLogicType type, ref IHookMonitor __result)
        {
            var typeMonitorMapping = Instance.typeMonitorMapping;
            if(typeMonitorMapping.TryGetValue(type,out IHookMonitor monitor))
            {
                __result = monitor;
                return false;
            }
            return true;
        }

        #endregion
    }

}
