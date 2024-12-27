#define NOT_USE_API_SAVE
using ATS_API.SaveLoading;
using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Services;
using Forwindz.Framework.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

#if !NOT_USE_API_SAVE
namespace Forwindz.Framework.Services
{
    public class ExtraStateAPIService : GameService, IExtraStateService, IService
    {
        [Serializable]
        private class SaveInformation
        {
            [JsonProperty]
            public object obj;
            [JsonProperty]
            public Type type;

            public SaveInformation() { }

            public SaveInformation(object obj)
            {
                this.obj = obj;
                this.type = obj.GetType();
            }

            public SaveInformation(object obj, Type type)
            {
                this.obj = obj;
                this.type = type;
            }

            public void TryRecover()
            {
                if (obj == null)
                {
                    return;
                }
                if(obj is JObject jobj)
                {
                    obj = jobj.ToObject(type);
                }
            }
        }

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
        private static ModSaveData modSaveData = null;
        private static SaveFileState modSaveState = SaveFileState.LoadedFile;
        private static bool finishAPILoading = false;

        private List<TrackedField> trackFieldList = new();

        static ExtraStateAPIService()
        {
            CustomServiceManager.RegGameService<ExtraStateAPIService>();
            PatchesManager.RegPatch(typeof(ExtraStateAPIService));
            // The design of API save system is independent with game services...
            // This will mess up the life cycle of everything... hope everything goes well...
            // I think it is necessary to have life cycle concept in API, otherwise it is impossible to handle any procedural logic.
            ModdedSaveManager.ListenForLoadedSaveData(
                PluginInfo.PLUGIN_GUID, OnAPILoadSaveDataStatic);
            ModdedSaveManager.ListenForResetSettlementSaveData(
                PluginInfo.PLUGIN_GUID, OnAPIResetSaveDataStatic);
        }

        // Invoke when create new settlement (usually in the init)
        private static void OnAPIResetSaveDataStatic(ModSaveData data)
        {
            ExtraStateAPIService service = CustomServiceManager.GetServiceSafe<ExtraStateAPIService>();
            if (service == null)
            {
                return;
            }
            FLog.Info("reset API settlment data callback!");
            service.ScanAttributes();
            service.OnNewSave(data);
            finishAPILoading = true;
        }

        // Invoke when switch the profile...
        private static void OnAPILoadSaveDataStatic(ModSaveData data, SaveFileState state)
        {
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
            ScanAttributes();
            if (MB.GameSaveService.IsNewGame())
            {
                FLog.Info("Start wait api reset save data!");
                await AsyncUtils.WaitForConditionAsync(() => finishAPILoading, 50);
                FLog.Info("Wait api reset save data finished!");
                return;
            }
            FLog.Info("Load Extra States!");
            await LoadAllStates();
            return;
        }

        private void OnAPILoadSaveData(ModSaveData data, SaveFileState state)
        {
            var settlementData = data.CurrentSettlement;
            switch (state)
            {
                case SaveFileState.NewFile:
                    OnNewSave(data);
                    break;
                case SaveFileState.LoadedFile:
                    FLog.Info($"Setup after loading, elements count: {trackFieldList.Count}");
                    foreach (TrackedField trackedField in trackFieldList)
                    {
                        try
                        {
                            SaveInformation saveInformation = settlementData.GetValueAsObject<SaveInformation>(trackedField.saveName);
                            saveInformation.TryRecover();
                            trackedField.ApplyValue(saveInformation.obj);
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
            trackFieldList.Clear();
            finishAPILoading = false;
            base.OnDestroy();
        }

        private void ScanAttributes()
        {
            trackFieldList.Clear();
            var services = CustomServiceManager.GameServicesList;
            foreach (var service in services)
            {
                var fields = service.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    ModSerializedFieldAttribute attribute = field.GetCustomAttribute<ModSerializedFieldAttribute>();
                    if (attribute != null &&
                        (attribute.SLType == SaveLoadType.Game || attribute.SLType == SaveLoadType.Unknown)
                        )
                    {
                        var value = field.GetValue(service);
                        string name = attribute.SaveName;
                        if (name == null || name.Length == 0)
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
        }

        private void OnNewSave(ModSaveData data)
        {
            SaveData settlementData = data.CurrentSettlement;
            FLog.Info($"Setup save for new file, elements count: {trackFieldList.Count}");
            foreach (TrackedField trackedField in trackFieldList)
            {
                try
                {
                    settlementData.SetValue(trackedField.saveName, new SaveInformation(trackedField.GetValue()));
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to load & apply extra state {trackedField.saveName} to field");
                    Log.Error(e);
                }
            }
        }

        public void SaveAllStates()
        {
            ModdedSaveManager.SaveAllModdedData();
        }

        public T GetStateSafe<T>(string name)
        {
            return (T)ModdedSaveManager.GetSaveData(PluginInfo.PLUGIN_GUID).CurrentSettlement.GetValue(name);
        }


        public T GetState<T>(string name)
        {
            return (T)ModdedSaveManager.GetSaveData(PluginInfo.PLUGIN_GUID).CurrentSettlement.GetValue(name);
        }
    }
}
#endif