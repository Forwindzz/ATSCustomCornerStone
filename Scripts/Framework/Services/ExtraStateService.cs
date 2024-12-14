using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Services;
using Forwindz.Framework.Utils;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Forwindz.Framework.Services
{
        

    public class ExtraStateService : GameService, IExtraStateService, IService
    {
        private class SaveInformation
        {
            [JsonProperty]
            public object obj;
            public Type type;
            public string name;

            public SaveInformation(object obj, Type type, string name)
            {
                this.obj = obj;
                this.type = type;
                this.name = name;
            }

            public void TryRecover()
            {
                if(obj==null)
                {
                    return;
                }
                FLog.AssertType<JObject>(obj);
                obj = ((JObject)obj).ToObject(type);
            }
        }

        private class SaveInformations
        {
            public string versionName = "";
            public Dictionary<string, SaveInformation> extraStates = new();

            public void OnSave()
            {
                versionName = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            public void OnDestory()
            {
                extraStates.Clear();
                versionName = "";
            }

            public static SaveInformations CreateNew()
            {
                return new SaveInformations();
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
        public static string SAVE_PATH = "f9_mod\\ModGameSave.save";
        public const string SAVE_KEY_INFO = "MOD_FORWINDZ_SAVE_GAME";
        private SaveInformations saveInformations;

        private List<TrackedField> trackFieldList = new();

        static ExtraStateService()
        {
            CustomServiceManager.RegGameService<ExtraStateService>();
            PatchesManager.RegPatch(typeof(ExtraStateService));
        }

        public ExtraStateService() { }

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
            ScanAttributes();
            await LoadAllStates();
            return;
        }

        public override void OnDestroy()
        {
            trackFieldList.Clear();
            saveInformations.OnDestory();
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

        public async UniTask LoadAllStates()
        {
            if(MB.GameSaveService.IsNewGame())
            {
                FLog.Info("ExtraStateService: New Game, do not load!");
                saveInformations = SaveInformations.CreateNew();
                return;
            }
            string fullFilePath = Path.Combine(Serviceable.ProfilesService.GetProfileFolderPath(), SAVE_PATH);
            if(!File.Exists(fullFilePath))
            {
                FLog.Error("The Save not exists : " + fullFilePath);
                FLog.Warning("We generate a new one instead");
                
                saveInformations = SaveInformations.CreateNew();
                return;
            }

            try
            {
                saveInformations = await Serviceable.SavesIOService
                                .Get<SaveInformations>(
                                    Path.Combine(Serviceable.ProfilesService.GetProfileFolderPath(), SAVE_PATH),
                                    SAVE_KEY_INFO);
            }
            catch(Exception e)
            {
                Log.Error($"Failed to load extra state");
                Log.Error(e);
                Log.Warning($"We create a new one instead!");
                saveInformations = SaveInformations.CreateNew();
                return;
            }
            
            foreach (SaveInformation saveInfo in saveInformations.extraStates.Values)
            {
                try
                {
                    saveInfo.TryRecover();
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to load extra state {saveInfo.name}");
                    Log.Error(e);
                }
            }
            foreach(TrackedField trackedField in trackFieldList)
            {
                try
                {
                    trackedField.ApplyValue(saveInformations.extraStates[trackedField.saveName].obj);
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
            FLog.Info("Try to save extra states");
            saveInformations.OnSave();
            foreach (TrackedField trackedField in trackFieldList)
            {
                saveInformations.extraStates[trackedField.saveName] =
                        new SaveInformation(trackedField.GetValue(), trackedField.field.FieldType, trackedField.saveName);
            }
            Serviceable.SavesIOService.SaveToFile(saveInformations, SAVE_PATH, SaveLocation.Profile);
            FLog.Info("Saved extra states!");
        }



        public T GetStateSafe<T>(string name)
        {
            if (saveInformations.extraStates.TryGetValue(name, out SaveInformation saveInformation))
            {
                if(saveInformation.obj is T result)
                {
                    return result;
                }
                else
                {
                    FLog.Warning($"SafeGetState: Unable to convert {saveInformation.obj.GetType().Name} {name} to {typeof(T).Name}");
                }
                
            }
            else
            {
                FLog.Warning($"SafeGetState: Unable to find {name}, type <{typeof(T).Name}>");
            }
            return default;
        }


        public T GetState<T>(string name)
        {
            return (T)saveInformations.extraStates[name].obj;
        }



        #region Patch

        // GameSaveService

        // save
        [HarmonyPatch(typeof(GameSaveService), nameof(Eremite.Services.GameSaveService.SaveState))]
        [HarmonyPostfix]
        private static void GameSaveService_SaveState_PostPatch(GameService __instance, ref UniTask __result)
        {
            IExtraStateService service = CustomServiceManager.GetService<IExtraStateService>();
            service.SaveAllStates();
        }

        [HarmonyPatch(typeof(GameSaveService), nameof(Eremite.Services.GameSaveService.ForceSaveState))]
        [HarmonyPostfix]
        private static void GameSaveService_ForceSaveState_PostPatch(GameService __instance)
        {
            IExtraStateService service = CustomServiceManager.GetService<IExtraStateService>();
            service.SaveAllStates();
        }

        /*
        // load
        [HarmonyPatch(typeof(GameSaveService), nameof(Eremite.Services.GameSaveService.LoadRegularSave))]
        [HarmonyPrefix]
        private static async void GameSaveService_LoadRegularSave_PrePatch(GameService __instance, UniTask<GameState> __result)
        {
            IExtraStateService service = CustomServiceManager.GetService<IExtraStateService>();
        }*/

        //TODO: inject save/load from devSavesService, new game
        #endregion
    }
}
