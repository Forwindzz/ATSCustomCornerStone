using Cysharp.Threading.Tasks;
using Eremite.Model;
using Eremite.Services;
using Forwindz.Framework.Utils;
using Forwindz.Framework.Utils.Extend;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Forwindz.Framework.Services
{

    public class ModifyGoodTypeState
    {
        public enum OperationType
        {
            SetEdiable = 0,
            SetBurnable = 1,
            SetTag = 2,
        }
        [JsonProperty]
        public readonly OperationType op;
        /// <summary>
        /// Tag name
        /// </summary>
        [JsonProperty]
        public readonly string content;
        /// <summary>
        /// For tag:
        /// true -> add tag
        /// false -> remove tag
        /// For other:
        /// set the state to the corresponding value
        /// </summary>
        [JsonProperty]
        public readonly bool boolState;

        private ModifyGoodTypeState() { }

        private ModifyGoodTypeState(OperationType op, string content, bool boolState)
        {
            this.op = op;
            this.content = content;
            this.boolState = boolState;
        }

        public static ModifyGoodTypeState GenAddTag(string tagName)
        {
            return new ModifyGoodTypeState(OperationType.SetTag, tagName, true);
        }

        public static ModifyGoodTypeState GenRemoveTag(string tagName)
        {
            return new ModifyGoodTypeState(OperationType.SetTag, tagName, false);
        }

        public static ModifyGoodTypeState GenSetEdiable(bool ediable)
        {
            return new ModifyGoodTypeState(OperationType.SetEdiable, null, ediable);
        }

        public static ModifyGoodTypeState GenSetBurnable(bool burnable)
        {
            return new ModifyGoodTypeState(OperationType.SetBurnable, null, burnable);
        }

        public bool IsAlreadyApplied(GoodModel goodModel)
        {
            switch (op)
            {
                case OperationType.SetEdiable:
                    return goodModel.eatable == boolState;
                case OperationType.SetBurnable:
                    return goodModel.canBeBurned = boolState;
                case OperationType.SetTag:
                    if (boolState) // add operation
                    {
                        return goodModel.ContainTag(content);
                    }
                    else // remove operation
                    {
                        return !goodModel.ContainTag(content);
                    }
                default:
                    FLog.Warning($"Unknown enum values {op} in ModifyGoodTypeState");
                    return false;
            }
        }


        public void ApplyState(GoodModel goodModel)
        {
            switch (op)
            {
                case OperationType.SetEdiable:
                    goodModel.eatable = boolState;
                    if(boolState)
                    {
                        Serviceable.StateService.Actors.rawFoodConsumptionPermits[goodModel.Name] = true;
                    }
                    else
                    {
                        Serviceable.StateService.Actors.rawFoodConsumptionPermits.Remove(goodModel.Name);
                    }
                    break;
                case OperationType.SetBurnable:
                    goodModel.canBeBurned = boolState;
                    Serviceable.HearthService.SetCanBeBurned(goodModel.Name, boolState);
                    //TODO: Will this affect Hearth?
                    break;
                case OperationType.SetTag:
                    if (boolState) // add operation
                    {
                        ArrayExt.ForceAdd(ref goodModel.tags, Serviceable.Settings.GetTag(content));
                    }
                    else // remove operation
                    {
                        ArrayExt.ForceRemove(ref goodModel.tags, Serviceable.Settings.GetTag(content));
                    }
                    break;
                default:
                    FLog.Warning($"Unknown enum values {op} in ModifyGoodTypeState");
                    break;
            }
        }


        public void RemoveState(GoodModel goodModel)
        {
            switch (op)
            {
                case OperationType.SetEdiable:
                    goodModel.eatable = !boolState;
                    break;
                case OperationType.SetBurnable:
                    goodModel.canBeBurned = !boolState;
                    break;
                case OperationType.SetTag:
                    if (!boolState) // add operation
                    {
                        ArrayExt.ForceAdd(ref goodModel.tags, Serviceable.Settings.GetTag(content));
                    }
                    else // remove operation
                    {
                        ArrayExt.ForceRemove(ref goodModel.tags, Serviceable.Settings.GetTag(content));
                    }
                    break;
                default:
                    FLog.Warning($"Unknown enum values {op} in ModifyGoodTypeState");
                    break;
            }
        }

        public ModifyGoodTypeState GenerateInverseState()
        {
            return new ModifyGoodTypeState(op, content, !boolState);
        }

        public override bool Equals(object obj)
        {
            if (obj is ModifyGoodTypeState m)
            {
                if (m.op == op)
                {
                    switch (op)
                    {
                        case OperationType.SetEdiable:
                        case OperationType.SetBurnable:
                            return m.boolState == boolState;
                        case OperationType.SetTag:
                            return m.boolState == boolState && m.content.Equals(content);
                    }
                }
                return false;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            uint hashCodeBool = boolState ? 1u : 0u;
            uint hashCodeString = op == OperationType.SetTag ? (uint)EqualityComparer<string>.Default.GetHashCode(content) : 0u;
            uint hashCodeOp = (uint)op.GetHashCode();
            // bool op string
            // BOOSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
            // 31 29
            uint hashCode = (hashCodeBool << 31) | ((hashCodeOp & 0x03) << 29) | (hashCodeString & 0xFFFFFFFF);
            return (int)hashCode;
        }

        public override string ToString()
        {
            int boolV = boolState ? 1 : 0;
            string result = $"{boolV}_{op.ToString()}";
            if (op == OperationType.SetTag)
            {
                result += "_" + content;
            }
            return result;
        }

        public string ToInvString()
        {
            int boolV = boolState ? 0 : 1;
            string result = $"{boolV}_{op.ToString()}";
            if (op == OperationType.SetTag)
            {
                result += "_" + content;
            }
            return result;
        }
    }

    public class ModifyGoodTypeInfo
    {
        [JsonProperty]
        private string goodName;
        [JsonProperty]
        private ModifyGoodTypeState state;
        [JsonProperty]
        private readonly bool isOriginalState;
        [JsonProperty]
        internal int duplicateTimes = 0;

        [JsonIgnore]
        public string GoodName => goodName;
        [JsonIgnore]
        public ModifyGoodTypeState State => state;
        [JsonIgnore]
        public bool IsOriginalState => isOriginalState;

        public ModifyGoodTypeInfo()
        {

        }

        public ModifyGoodTypeInfo(string goodName, ModifyGoodTypeState state)
        {
            this.goodName = goodName;
            this.state = state;
            isOriginalState = state.IsAlreadyApplied(GetGoodModel());
        }

        public GoodModel GetGoodModel()
        {
            return Serviceable.Settings.GetGood(goodName);
        }

        public void ApplyState()
        {
            FLog.Info($"+State {ToString()}");
            state.ApplyState(GetGoodModel());
        }


        public void RemoveState()
        {
            if(isOriginalState)
            {
                return;
            }
            FLog.Info($"-State {ToString()}");
            state.RemoveState(GetGoodModel());
        }

        public ModifyGoodTypeInfo GenerateInverseInfo()
        {
            return new ModifyGoodTypeInfo(goodName, state.GenerateInverseState());
        }

        public override string ToString()
        {
            return state.ToString() + "_" + goodName;
        }

        public string ToInvString()
        {
            return state.ToInvString() + "_" + goodName;
        }
    }

    public class ModifyGoodTypeStatesTracker
    {
        /// <summary>
        /// Same modification will have same ToString() value, we use this as key to detect duplications of modification
        /// </summary>
        [JsonProperty]
        private Dictionary<string,ModifyGoodTypeInfo> duplicateTracker = new();

        public ModifyGoodTypeInfo GetExistingInfo(ModifyGoodTypeInfo info)
        {
            ModifyGoodTypeInfo storedInfo = null;
            string key = info.ToString();
            if (!duplicateTracker.TryGetValue(key, out storedInfo))
            {
                storedInfo = info;
                duplicateTracker[key] = storedInfo;
            }
            return storedInfo;
        }

        public ModifyGoodTypeInfo GetExistingInvInfo(ModifyGoodTypeInfo info)
        {
            ModifyGoodTypeInfo storedInfo = null;
            string key = info.ToInvString();
            if (!duplicateTracker.TryGetValue(key, out storedInfo))
            {
                storedInfo = info;
                duplicateTracker[key] = storedInfo.GenerateInverseInfo();
            }
            return storedInfo;
        }

        /// <summary>
        /// Add state
        /// If the state already exists, then this do nothing
        /// If the inverse state exists...
        /// - the inverse state counteract the state
        /// </summary>
        /// <param name="info"></param>
        public void AddState(ModifyGoodTypeInfo stateInfo)
        {
            ModifyGoodTypeInfo info = GetExistingInfo(stateInfo);
            ModifyGoodTypeInfo infoInv = GetExistingInvInfo(stateInfo);
            // balanced state -> become unbalance, apply state!
            if (info.duplicateTimes == infoInv.duplicateTimes)
            {
                FLog.Info($"Apply state {stateInfo}");
                info.ApplyState();
            }

            info.duplicateTimes++;
        }

        public void RemoveState(ModifyGoodTypeInfo stateInfo)
        {
            ModifyGoodTypeInfo info = GetExistingInfo(stateInfo);
            ModifyGoodTypeInfo infoInv = GetExistingInvInfo(stateInfo);
            // balanced state -> become unbalance, apply state!
            if (info.duplicateTimes == infoInv.duplicateTimes)
            {
                FLog.Info($"Remove state {stateInfo}");
                info.RemoveState();
            }

            info.duplicateTimes--;
        }

        public void RemoveAllState()
        {
            FLog.Info("remove all dynamic good states");
            foreach (ModifyGoodTypeInfo info in duplicateTracker.Values)
            {
                ModifyGoodTypeInfo invInfo = duplicateTracker[info.ToInvString()];
                if (info.duplicateTimes > invInfo.duplicateTimes) 
                {
                    info.RemoveState();
                }
            }
            duplicateTracker.Clear();
        }

        public void RestoreState()
        {
            try
            {
                FLog.Info($"restore dynamic good states, total states type: {duplicateTracker.Count}");
                foreach (ModifyGoodTypeInfo info in duplicateTracker.Values)
                {
                    ModifyGoodTypeInfo invInfo = duplicateTracker[info.ToInvString()];
                    if (info.duplicateTimes > invInfo.duplicateTimes)
                    {
                        info.ApplyState();
                    }
                }
                FLog.Info($"finish restore dynamic good states");
            }catch(Exception e)
            {
                FLog.Error(e);
                FLog.Error("Failed to restore :(");
            }
        }
    }

    /// <summary>
    /// Allow to add or remove good type dynamically.
    /// Note: The original good model will be changed.
    /// </summary>
    public class DynamicGoodTypeService : GameService, IDynamicGoodTypeService, IService
    {
        [ModSerializedField]
        protected ModifyGoodTypeStatesTracker modifyGoodState = new();
        private MethodInfo cacheMethod;

        static DynamicGoodTypeService()
        {
            CustomServiceManager.RegGameService<DynamicGoodTypeService>();
        }

        public DynamicGoodTypeService() { }

        private void RecomputeCache()
        {
            cacheMethod.Invoke(GoodsService, null);
        }

        public override UniTask OnLoading()
        {
            FLog.Info("Load Dynamic Good State!");
            modifyGoodState.RestoreState();
            cacheMethod = AccessTools.Method(GoodsService.GetType(),"Cache");
            FLog.AssertNotNull(cacheMethod);
            RecomputeCache();
            return UniTask.CompletedTask;
        }

        public override void OnDestroy()
        {
            modifyGoodState.RemoveAllState();
            base.OnDestroy();
        }

        public override IService[] GetDependencies()
        {
            return new IService[] { 
                Serviceable.GoodsService,
                CustomServiceManager.GetAsIService<IExtraStateService>()
            };
        }

        public ModifyGoodTypeStatesTracker GetStateTracker => modifyGoodState;

        public void GoodSetEatable(string goodName, bool state)
        {
            FLog.Info($"Set Ediable {goodName} {state}");
            modifyGoodState.AddState(
                new ModifyGoodTypeInfo(
                    goodName,
                    ModifyGoodTypeState.GenSetEdiable(state)
                ));
            RecomputeCache();
        }

        public void GoodAddTag(string goodName, string tagName)
        {
            FLog.Info($"Add Tag {goodName} {tagName}");
            modifyGoodState.AddState(
                new ModifyGoodTypeInfo(
                    goodName,
                    ModifyGoodTypeState.GenAddTag(tagName)
                ));
            RecomputeCache();
        }

        public void GoodRemoveTag(string goodName, string tagName)
        {
            FLog.Info($"Remove Tag {goodName} {tagName}");
            modifyGoodState.AddState(
                new ModifyGoodTypeInfo(
                    goodName,
                    ModifyGoodTypeState.GenRemoveTag(tagName)
                ));
            RecomputeCache();
        }

    }
}
