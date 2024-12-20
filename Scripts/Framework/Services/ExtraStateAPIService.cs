#define NOT_USE_API_SAVE
using ATS_API.SaveLoading;
using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Services;
using Forwindz.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

//TODO: wait for finishing API save system
#if !NOT_USE_API_SAVE
namespace Forwindz.Framework.Services
{
        

    public class ExtraStateAPIService : GameService, IExtraStateService, IService
    {

        private class TrackedField
        {
            public string saveName;
            public object owner;
            public FieldInfo field;

            public TrackedField(string saveName, object owner, FieldInfo field)
            {
                this.saveName = saveName;
                this.owner = owner;
                this.field = field;
            }

            public void ApplyValue(object obj)
            {
                field.SetValue(owner, obj);
            }

            public object GetValue()
            {
                return field.GetValue(owner);
            }
        }
        public static string SAVE_PATH = "f9_mod\\ModGameSave.save";
        public const string SAVE_KEY_INFO = "MOD_FORWINDZ_SAVE_GAME";
        private static bool loadedFromAPI = false;
        private static ModSaveData modSaveData = null;
        private static SaveFileState modSaveState = SaveFileState.LoadedFile;

        private List<TrackedField> trackFieldList = new();

        static ExtraStateAPIService()
        {
            CustomServiceManager.RegGameService<ExtraStateAPIService>();
            PatchesManager.RegPatch(typeof(ExtraStateAPIService));
            ModdedSaveManager.ListenForLoadedSaveData(
                PluginInfo.PLUGIN_GUID, OnAPILoadSaveDataStatic);
            //ModdedSaveManager.AddErrorHandler(
            //    PluginInfo.PLUGIN_GUID, OnAPILoadErrorStatic);
        }

        //private static UniTask<ModSaveData> OnAPILoadErrorStatic(ErrorData data)
        //{
        //    return UniTask.CompletedTask;
        //}

        private static void OnAPILoadSaveDataStatic(ModSaveData data, SaveFileState state)
        {
            ExtraStateAPIService service = CustomServiceManager.GetServiceSafe<ExtraStateAPIService>();
            if(service==null)
            {
                return;
            }
            service.ScanAttributes();
            modSaveData = data;
            modSaveState = state;
        }

        public ExtraStateAPIService() { }

        public override IService[] GetDependencies()
        {
            return new IService[] { 
                Serviceable.StateService, 
                Serviceable.GameSaveService,
                Serviceable.SavesIOService
            };
        }

        public async override UniTask OnLoading()
        {
            FLog.Info("Load Extra States!");
            await LoadAllStates();
            return;
        }

        private void OnAPILoadSaveData(ModSaveData data, SaveFileState state)
        {
            loadedFromAPI = true;
            var settlementData = data.CurrentSettlement;
            switch (state)
            {
                case SaveFileState.NewFile:
                    FLog.Info($"Setup save for new file, elements count: {trackFieldList.Count}");
                    foreach (TrackedField trackedField in trackFieldList)
                    {
                        try
                        {
                            settlementData.SetValue(trackedField.saveName, trackedField.GetValue());
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Failed to load & apply extra state {trackedField.saveName} to field");
                            Log.Error(e);
                        }
                    }
                    break;
                case SaveFileState.LoadedFile:
                    FLog.Info($"Setup after loading, elements count: {trackFieldList.Count}");
                    foreach (TrackedField trackedField in trackFieldList)
                    {
                        try
                        {
                            trackedField.ApplyValue(settlementData.GetValue(trackedField.saveName));
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Failed to load & apply extra state {trackedField.saveName} to field");
                            Log.Error(e);
                        }
                    }
                    break;
            }
        }

        public override void OnDestroy()
        {
            loadedFromAPI = false;
            trackFieldList.Clear();
            base.OnDestroy();
        }

        private void ScanAttributes()
        {
            var services = CustomServiceManager.GameServicesList;
            foreach (var service in services)
            {
                var fields = service.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    ModSerializedFieldAttribute attribute = field.GetCustomAttribute<ModSerializedFieldAttribute>();
                    if (attribute != null && 
                        (attribute.SLType==SaveLoadType.Game || attribute.SLType==SaveLoadType.Unknown)
                        )
                    {
                        var value = field.GetValue(service);
                        string name = attribute.SaveName;
                        if(name==null || name.Length==0)
                        {
                            string typeInfo = $"{service.GetType().FullName}#{field.FieldType.FullName}#{field.Name}";
                            name = $"{service.GetType().Name}.{field.Name}_{Utils.Utils.GetMd5Hash(typeInfo)}";
                        }
                        trackFieldList.Add(new TrackedField(name, service, field));
                    }
                }
            }
        }

        public UniTask LoadAllStates()
        {
            OnAPILoadSaveData(modSaveData, modSaveState);
            return UniTask.CompletedTask;
            //await AsyncUtils.WaitForConditionAsync(() => loadedFromAPI);
        }

        public void SaveAllStates()
        {
            ModdedSaveManager.SaveAllModdedData();
        }

        public T GetStateSafe<T>(string name)
        {
            try
            {
                return (T)ModdedSaveManager.GetSaveData(PluginInfo.PLUGIN_GUID).CurrentSettlement.GetValue(name);
            }
            catch (Exception ex)
            {
            }
            return default;
        }


        public T GetState<T>(string name)
        {
            return (T)ModdedSaveManager.GetSaveData(PluginInfo.PLUGIN_GUID).CurrentSettlement.GetValue(name);
        }
    }
}
#endif