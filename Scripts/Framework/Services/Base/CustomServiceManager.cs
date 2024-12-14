using Eremite.Controller;
using Eremite.Services;
using Eremite.Services.World;
using Forwindz.Framework.Utils;
using HarmonyLib;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using static Forwindz.Framework.Utils.FLog;
namespace Forwindz.Framework.Services
{
    /// <summary>
    /// Manage All customized Services
    /// To add service, you need to invoke `CustomServiceManager.RegXXXXService<T>()` before the game fully initialized
    /// The service should contain a non-parameter constructor
    /// MetaServices and WorldServices will be created and loaded when the player click "play" in the main menu
    /// GameServices will be created and loaded when the player load or start a new settlement game
    /// </summary>
    public static class CustomServiceManager
    {
        private readonly static List<Type> appExtraServiceTypes = new();
        private readonly static List<Type> gameExtraServiceTypes = new();
        private readonly static List<Type> metaExtraServiceTypes = new();
        private readonly static List<Type> worldExtraServiceTypes = new();

        private static Dictionary<Type, WeakReference<IService>> customServices = new();
        private static WeakReference<List<IService>> gameServices = null;
        private static WeakReference<List<IService>> metaServices = null;
        private static WeakReference<List<IService>> appServices = null;
        private static WeakReference<List<IService>> worldServices = null;

        /// <summary>
        /// All services in GameServices
        /// Might be null if GameServices is not created
        /// </summary>
        public static IReadOnlyList<IService> GameServicesList => gameServices.GetValue();
        public static IReadOnlyList<IService> MetaServicesList => metaServices.GetValue();
        public static IReadOnlyList<IService> AppServicesList => appServices.GetValue();
        public static IReadOnlyList<IService> WorldServicesList => worldServices.GetValue();

        static CustomServiceManager()
        {
            PatchesManager.RegPatch(typeof(CustomServiceManager));
        }

        public static void RegAppService<T>() where T : IService
        {
            appExtraServiceTypes.Add(typeof(T));
        }

        public static void RegGameService<T>() where T : IService
        {
            gameExtraServiceTypes.Add(typeof(T));
        }

        public static void RegMetaService<T>() where T : IService
        {
            metaExtraServiceTypes.Add(typeof(T));
        }

        public static void RegWorldService<T>() where T : IService
        {
            worldExtraServiceTypes.Add(typeof(T));
        }

        /// <summary>
        /// Get the existing custom services
        /// Might through error or return null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            if(customServices[typeof(T)].TryGetTarget(out IService service))
            {
                return (T)service;
            }
            else
            {
                return default;
            }    
        }

        public static IService GetAsIService<T>()
        {
            if (customServices[typeof(T)].TryGetTarget(out IService service))
            {
                return service;
            }
            else
            {
                return default;
            }
        }

        public static IService GetService(Type type)
        {
            if (customServices[type].TryGetTarget(out IService service))
            {
                return service;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Safely get service, will return null if the service instance not exists
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IService GetServiceSafe(Type type)
        {
            if(customServices.TryGetValue(type,out WeakReference<IService> refService))
            {
                if(refService.TryGetTarget(out IService result))
                {
                    return result;
                }
            }
            return null;
        }

        #region Patch
        //-------------------------------
        // Patch
        //-------------------------------

        [HarmonyPatch(typeof(WorldServices), nameof(WorldServices.CreateServices))]
        [HarmonyPrefix]
        private static void WorldServicesCreate_PrePatch(
            WorldServices __instance)
        {
            foreach (Type serviceType in worldExtraServiceTypes)
            {
                CreateServiceToList(__instance.allServices, serviceType);
            }
            FLog.Info($"Try to create {worldExtraServiceTypes.Count} services in WorldServices");
            worldServices = new WeakReference<List<IService>>(__instance.allServices);
        }

        [HarmonyPatch(typeof(AppServices), nameof(AppServices.CreateServices))]
        [HarmonyPrefix]
        private static void AppServicesCreate_PrePatch(
            AppServices __instance)
        {
            foreach (Type serviceType in appExtraServiceTypes)
            {
                CreateServiceToList(__instance.allServices, serviceType);
            }
            FLog.Info($"Try to create {appExtraServiceTypes.Count} services in AppServices");
            appServices = new WeakReference<List<IService>>(__instance.allServices);
        }

        [HarmonyPatch(typeof(GameServices), nameof(GameServices.CreateServices))]
        [HarmonyPostfix]
        private static void GameServicesCreate_PostPatch(
            GameServices __instance)
        {
            foreach (Type serviceType in gameExtraServiceTypes)
            {
                CreateServiceToList(__instance.allServices, serviceType);
            }
            FLog.Info($"Try to create {gameExtraServiceTypes.Count} services in GameServices");
            gameServices = new WeakReference<List<IService>>(__instance.allServices);
        }

        [HarmonyPatch(typeof(MetaServices), nameof(MetaServices.CreateServices))]
        [HarmonyPrefix]
        private static void MetaServicesCreate_PrePatch(
            MetaServices __instance)
        {
            foreach (Type serviceType in metaExtraServiceTypes)
            {
                CreateServiceToList(__instance.allServices, serviceType);
            }
            FLog.Info($"Try to create {metaExtraServiceTypes.Count} services in MetaServices");
            metaServices = new WeakReference<List<IService>>(__instance.allServices);
        }

        private static void CreateServiceToList(List<IService> allServices, Type serviceType)
        {
            FLog.Info($"Try to create service {serviceType.FullName}");
            FLog.AssertNotNull(allServices);
            FLog.AssertNotNull(customServices);
            FLog.AssertNotNull(serviceType);
            ConstructorInfo ctorInfo = ReflectUtils.GetNonParameterConstructor(serviceType);
            FLog.AssertNotNull(ctorInfo);
            IService service = null;
            try
            {
                service = (IService)ctorInfo.Invoke(null);
            }
            catch (Exception ex)
            {
                FLog.Error("Failed to create service!");
                FLog.Error(ex);
            }
            
            FLog.AssertNotNull(service);
            List<Type> types = [.. serviceType.GetInterfaces(), serviceType, ..serviceType.GetBaseTypes()];
            foreach (Type type in types)
            {
                if (type == null) 
                {
                    continue;
                }
                if (GetServiceSafe(type)!=null)
                {
                    continue;
                }
                customServices[type] = new WeakReference<IService>(service);
                FLog.Info($"Add Service {serviceType.Name} as {type.Name}");
            }
            allServices.Add(service);
        }

        #endregion
    }


}
