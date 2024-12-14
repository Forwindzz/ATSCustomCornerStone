using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Utils
{
    /// <summary>
    /// Load Patches by classes
    /// </summary>
    public static class PatchesManager
    {
        private static Dictionary<Type, Harmony> patchTypes = new();
        public static Harmony harmony = new Harmony("Forwindz");
        private static bool dirty = true;

        public static void RegPatch<T>()
        {
            patchTypes[typeof(T)] = null;
            dirty = true;
        }

        public static void RegPatch(Type type)
        {
            patchTypes[type] = null;
            dirty = true;
        }

        public static void PatchAll()
        {
            if(!dirty)
            {
                return;
            }
            Dictionary<Type, Harmony> patches = new();
            foreach (var pair in patchTypes)
            {
                Type type = pair.Key;
                Harmony harmonyInstance = pair.Value;
                if (harmonyInstance==null)
                {
                    FLog.Info($"Try to Patch {type.Namespace} > {type.Name}");
                    patches[type] = Harmony.CreateAndPatchAll(type);
                }
                else
                {
                    patches[type] = harmonyInstance;
                }
            }
            patchTypes = patches;
            dirty = false;
        }
    }
}
