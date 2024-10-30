using Cysharp.Threading.Tasks;
using Eremite.Services;
using Forwindz.Framework.Services;
using Forwindz.Framework.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Scripts.Framework.Utils
{
    /// <summary>
    /// Only for debugging
    /// </summary>
    public static class Debug
    {
        static Debug()
        {
            PatchesManager.RegPatch(typeof(Debug));
        }

        /*
        [HarmonyPatch(typeof(Service), nameof(Service.Load))]
        [HarmonyPostfix]
        private static void Service_Load_PostPatch(Service __instance, ref UniTask __result)
        {
            FLog.Info($"Load Finish: {__instance.GetType().Name}");
        }

        [HarmonyPatch(typeof(Service), nameof(Service.Load))]
        [HarmonyPrefix]
        private static void Service_Load_PrePatch(Service __instance, ref UniTask __result)
        {
            FLog.Info($"Try Load   : {__instance.GetType().Name}");
        }
        */
    }
}
