using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Services;
using Forwindz.Framework.Utils;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Forwindz.Framework.Services
{
    public enum RelicArrayOperation
    {
        Add,
        Remove,
    }

    [Serializable]
    internal class RelicExpireEffectInfo
    {
        public string effectName;
        public int tierIndex;
        public RelicArrayOperation operation;
        
        public override bool Equals(object obj)
        {
            if(obj is RelicExpireEffectInfo other)
            {
                return 
                    effectName.Equals(other.effectName) &&
                    tierIndex == other.tierIndex &&
                    operation == other.operation;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return 
                (effectName.GetHashCode()*97+233337) ^ 
                (tierIndex.GetHashCode()*71+2) ^ 
                (operation.GetHashCode()*37+37) ;
        }
    }

    [Serializable]
    internal class ExtraRelicInfo
    {
        public string relicName;
        public float extraProcessSpeed = 0.0f;
        public List<RelicExpireEffectInfo> expireEffectList = new();
    }

    [Serializable]
    internal class DynamicRelicModel
    {
        [JsonIgnore]
        public RelicModel currentModel;

        [JsonProperty]
        public ExtraRelicInfo modifyInfo = new();

        private List<DynamicValueArray<EffectModel>> effectStep_effectsWrapper = new();

        public DynamicRelicModel() { }

        public DynamicRelicModel(string name)
        {
            modifyInfo.relicName = name;
            RestoreFromSave();
        }

        public void RestoreFromSave()
        {
            currentModel = MB.Settings.GetRelic(modifyInfo.relicName);
            foreach (var expireEffectInfo in currentModel.effectsTiers)
            {
                effectStep_effectsWrapper.Add(new DynamicValueArray<EffectModel>(
                        () => expireEffectInfo.effect,
                        (EffectModel[] value) => expireEffectInfo.effect = value
                    ));
            }
            ApplyModification();
        }

        public void ApplyModification()
        {
            int count = modifyInfo.expireEffectList.Count;
            for (int i = 0; i < count; i++) 
            {
                var expireEffectInfo = modifyInfo.expireEffectList[i];
                TriggerModifyExpireEffect(expireEffectInfo);
            }
        }

        public void AddModifyExpireEffect(RelicExpireEffectInfo newExpireInfo)
        {
            modifyInfo.expireEffectList.Add(newExpireInfo);
            TriggerModifyExpireEffect(newExpireInfo);
            
        }

        public void RemoveModifyExpireEffect(RelicExpireEffectInfo expireInfo)
        {
            if(modifyInfo.expireEffectList.Remove(expireInfo))
            {
                foreach(var expireEffectInfo in modifyInfo.expireEffectList)
                {
                    if(expireEffectInfo.Equals(expireInfo) &&
                        expireInfo.operation == RelicArrayOperation.Remove)
                    {
                        return; // is removed many times, so just keep it removed
                    }
                }
                RelicArrayOperation oldOperation = expireInfo.operation;
                switch (expireInfo.operation)
                {
                    case RelicArrayOperation.Add:
                        expireInfo.operation = RelicArrayOperation.Remove;
                        break;
                    case RelicArrayOperation.Remove:
                        expireInfo.operation = RelicArrayOperation.Add;
                        break;
                }
                TriggerModifyExpireEffect(expireInfo);
                expireInfo.operation = oldOperation;
            }
        }

        private void TriggerModifyExpireEffect(RelicExpireEffectInfo expireEffectInfo)
        {
            FLog.Info($"Trigger: {modifyInfo.relicName} | Tier {expireEffectInfo.tierIndex} | {expireEffectInfo.effectName} | {expireEffectInfo.operation}");
            if (expireEffectInfo.tierIndex >= effectStep_effectsWrapper.Count)
            {
                FLog.Error($"Out of index {expireEffectInfo.tierIndex}, count: {effectStep_effectsWrapper.Count}.");
                return;
            }
            var effectsWrapper = effectStep_effectsWrapper[expireEffectInfo.tierIndex];
            EffectModel targetEffectModel = null;
            switch (expireEffectInfo.operation)
            {
                case RelicArrayOperation.Add:
                    targetEffectModel = MB.Settings.GetEffect(expireEffectInfo.effectName);
                    effectsWrapper.AddNewValue(targetEffectModel);
                    break;
                case RelicArrayOperation.Remove:
                    EffectModel[] currentValue = effectsWrapper.CurrentValue;
                    for (int j = 0; j < currentValue.Length; j++)
                    {
                        if (currentValue[j].Name == expireEffectInfo.effectName)
                        {
                            targetEffectModel = currentValue[j];
                            effectsWrapper.Remove(j);
                            break;
                        }
                    }
                    break;
            }

            if(targetEffectModel==null)
            {
                FLog.Warning($"Cannot Remove expired effect {expireEffectInfo.effectName} in {modifyInfo.relicName}.effectTiers[{expireEffectInfo.tierIndex}]");
                return;
            }

            // current existing expired relic
            IEnumerable<Relic> filteredRelic = Serviceable.BuildingsService.Relics.Values.Where<Relic>(
                (Relic relic) =>
                    relic.ModelName == modifyInfo.relicName &&
                    (
                        (relic.state.currentDynamicEffect >= expireEffectInfo.tierIndex) || //reach this tier
                        relic.state.continuousTicks > 0 // already reach this tier, but loop back
                    ));

            FLog.Info($"Find {filteredRelic.Count()} relics to be modified");
            switch (expireEffectInfo.operation)
            {
                case RelicArrayOperation.Add:
                    foreach (var relic in filteredRelic)
                    {
                        targetEffectModel.Apply(EffectContextType.Building, relic.ModelName, relic.Id);
                    }
                    break;
                case RelicArrayOperation.Remove:
                    foreach (var relic in filteredRelic)
                    {
                        targetEffectModel.Remove(EffectContextType.Building, relic.ModelName, relic.Id);
                    }
                    break;
            }

        }

        public void RecoverOriginalState()
        {
            foreach (var effectWrapper in effectStep_effectsWrapper)
            {
                effectWrapper.RestoreOriginalValue();
            }
        }
    }

    [Serializable]
    internal class DynamicRelicState
    {
        [JsonProperty]
        private Dictionary<string, DynamicRelicModel> relicInfos = new();

        public void OnLoad()
        {
            foreach(var pair in relicInfos)
            {
                DynamicRelicModel model = pair.Value;
                model.RestoreFromSave();
            }
        }

        public void OnDestroy()
        {
            foreach (var pair in relicInfos)
            {
                DynamicRelicModel model = pair.Value;
                model.RestoreFromSave();
            }
        }

        public DynamicRelicModel GetDynamicRelicModel(string relicName)
        {
            if(!relicInfos.TryGetValue(relicName,out DynamicRelicModel model))
            {
                var orgModel = MB.Settings.GetRelic(relicName);
                if(orgModel.Name==null)
                {
                    FLog.Error($"Cannot find Relic {relicName}");
                    return null;
                }
                model = new DynamicRelicModel(relicName);
                relicInfos[relicName] = model;
            }
            return model;
        }

        public DynamicRelicModel TryGetDynamicRelicModel(string relicName)
        {
            if (relicInfos.TryGetValue(relicName, out DynamicRelicModel model))
            {
                return model;
            }
            return null;
        }
    }

    public class DynamicRelicService: GameService, IDynamicRelicService, IService
    {
        [ModSerializedField]
        private DynamicRelicState state = new();

        private Subject<Relic> onRelicExpireSubject = new();
        public IObservable<Relic> OnRelicExpire => onRelicExpireSubject;

        static DynamicRelicService()
        {
            CustomServiceManager.RegGameService<DynamicRelicService>();
            PatchesManager.RegPatch<DynamicRelicService>();
        }

        public override IService[] GetDependencies()
        {
            return new IService[] {
                Serviceable.BuildingsService,
                CustomServiceManager.GetAsIService<IExtraStateService>()
            };
        }

        public override void OnDestroy()
        {
            state.OnDestroy();
            base.OnDestroy();
        }

        public override UniTask OnLoading()
        {
            state.OnLoad();
            return UniTask.CompletedTask;
        }

        //interface...

        public void RemoveExpireEffect(string relicName, int tierIndex, string effectName)
        {
            ModifyExpireEffect(relicName, tierIndex, effectName, RelicArrayOperation.Remove);
        }

        public void AddExpireEffect(string relicName, int tierIndex, string effectName)
        {
            ModifyExpireEffect(relicName, tierIndex, effectName, RelicArrayOperation.Add);
        }

        public void AddProcessSpeed(string relicName, float speed)
        {
            var dynamicModel = state.GetDynamicRelicModel(relicName);
            if (dynamicModel == null)
            {
                return;
            }
            dynamicModel.modifyInfo.extraProcessSpeed += speed;
        }

        public void SetProcessSpeed(string relicName, float speed)
        {
            var dynamicModel = state.GetDynamicRelicModel(relicName);
            if (dynamicModel == null)
            {
                return;
            }
            dynamicModel.modifyInfo.extraProcessSpeed -= speed;
        }

        public float GetProcessSpeed(string relicName)
        {
            var dynamicModel = state.GetDynamicRelicModel(relicName);
            if (dynamicModel == null)
            {
                return 0.0f;
            }
            return dynamicModel.modifyInfo.extraProcessSpeed;
        }

        public void ModifyExpireEffect(string relicName, int tierIndex, string effectName, RelicArrayOperation op)
        {
            var dynamicModel = state.GetDynamicRelicModel(relicName);
            if (dynamicModel == null)
            {
                return;
            }
            dynamicModel.AddModifyExpireEffect(new RelicExpireEffectInfo()
            {
                effectName = effectName,
                tierIndex = tierIndex,
                operation = op
            });
        }

        public void RemoveModifyExpireEffect(string relicName, int tierIndex, string effectName, RelicArrayOperation op)
        {
            var dynamicModel = state.GetDynamicRelicModel(relicName);
            if (dynamicModel == null)
            {
                return;
            }
            dynamicModel.RemoveModifyExpireEffect(new RelicExpireEffectInfo()
            {
                effectName = effectName,
                tierIndex = tierIndex,
                operation = op
            });
        }

        #region Patch

        // Apply extra processing speed
        [HarmonyPatch(
            typeof(Eremite.Services.EffectsService),
            nameof(Eremite.Services.EffectsService.GetRelicsWorkingRate))]
        [HarmonyPostfix]
        private static void EffectsService_GetRelicsWorkingRate_PostPatch(
            EffectsService __instance,
            ref float __result,
            Relic relic)
        {
            DynamicRelicService service = CustomServiceManager.GetService<DynamicRelicService>();
            if(service==null)
            {
                return;
            }
            DynamicRelicModel dynamicRelicModel = service.state.TryGetDynamicRelicModel(relic.ModelName);
            if(dynamicRelicModel==null)
            {
                return;
            }
            __result += dynamicRelicModel.modifyInfo.extraProcessSpeed;
        }

        [HarmonyPatch(
            typeof(Relic),
            nameof(Relic.ChangeDynamicEffects))]
        [HarmonyPostfix]
        private static void Relic_ChangeDynamicEffects_PostPatch(
            Relic __instance)
        {
            DynamicRelicService service = CustomServiceManager.GetService<DynamicRelicService>();
            if (service == null)
            {
                return;
            }
            service.onRelicExpireSubject.OnNext(__instance);
        }

        #endregion
    }
}