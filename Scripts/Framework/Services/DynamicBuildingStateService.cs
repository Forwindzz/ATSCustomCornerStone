using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Buildings;
using Eremite.Model.Orders;
using Eremite.Services;
using Eremite.Services.Orders;
using Forwindz.Framework.Utils;
using HarmonyLib;
using Newtonsoft.Json;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UniRx;

namespace Forwindz.Framework.Services
{

    internal class DecorationModelInfo
    {
        public DecorationTier tier;
        public int decorationScore;

        public DecorationModelInfo(DecorationModel decorationModel)
        {
            decorationScore = decorationModel.decorationScore;
            tier = decorationModel.tier;
        }

        public void Apply(DecorationModel decorationModel)
        {
            decorationModel.decorationScore = decorationScore;
            decorationModel.tier = tier;
        }
    }

    public class DynamicDecorationStateInfo
    {
        public float decorationPercent = 1.0f;
    }

    internal class DynamicBuildingState
    {
        [JsonIgnore]
        public Dictionary<string, DecorationModelInfo> originalDecorations = new();

        public Dictionary<string, DynamicDecorationStateInfo> decorationStates = new();

        public void InitGame()
        {
            List<DecorationModel> decorationModels = MB.Settings.Buildings.FilterCast<DecorationModel>().ToList();
            foreach (var decorationModel in decorationModels)
            {
                originalDecorations[decorationModel.Name] = new DecorationModelInfo(decorationModel);
            }
            ApplyStates();
        }

        public void AddDecorationPercent(string decoName, float percent)
        {
            if(decorationStates.TryGetValue(decoName, out DynamicDecorationStateInfo info))
            {
                info.decorationPercent += percent;
            }
            else
            {
                info = new DynamicDecorationStateInfo();
                info.decorationPercent += percent;
                decorationStates[decoName] = info;
            }
        }

        public float GetDecorationPercent(string decoName)
        {
            if (decorationStates.TryGetValue(decoName, out DynamicDecorationStateInfo info))
            {
                return info.decorationPercent;
            }
            return 1.0f;
        }

        public void ApplyStates()
        {
            foreach(var decoName in decorationStates.Keys)
            {
                ApplyDecorationState(decoName);
            }
        }

        public void ApplyDecorationState(string decoName)
        {
            DynamicDecorationStateInfo dynamicDecoInfo = decorationStates[decoName];
            DecorationModelInfo originalDecoInfo = originalDecorations[decoName];
            DecorationModel currentDecoModel = (DecorationModel)MB.Settings.GetBuilding(decoName);
            currentDecoModel.decorationScore = (int)(originalDecoInfo.decorationScore * dynamicDecoInfo.decorationPercent);
        }

        public void DestoryRestore()
        {
            foreach (var decoName in decorationStates.Keys)
            {
                DecorationModelInfo originalDecoInfo = originalDecorations[decoName];
                DecorationModel currentDecoModel = (DecorationModel)MB.Settings.GetBuilding(decoName);
                currentDecoModel.decorationScore = originalDecoInfo.decorationScore;
            }
        }
    }

    public class DynamicBuildingStateService : GameService, IDynamicBuildingStateService, IService
    {
        [ModSerializedField]
        private DynamicBuildingState state = new();
        private FieldInfo fieldInfo_decorationTypeOwning = null;
        private FieldInfo fieldInfo_monitor = null;

        public readonly Subject<Dictionary<string, DynamicDecorationStateInfo>> decorationValueChangeSubject = new();
        public IObservable<Dictionary<string, DynamicDecorationStateInfo>> OnDecorationValueChange => decorationValueChangeSubject;


        static DynamicBuildingStateService()
        {
            CustomServiceManager.RegGameService<DynamicBuildingStateService>();
        }

        public override IService[] GetDependencies()
        {
            return new IService[] {
                Serviceable.BuildingsService,
                Serviceable.RecipesService,
                Serviceable.OrdersService,
                CustomServiceManager.GetAsIService<IExtraStateService>()
            };
        }

        public override void OnDestroy()
        {
            state.DestoryRestore();
            base.OnDestroy();
        }

        public OrdersMonitor Reflect_OrderService_monitor => (OrdersMonitor)fieldInfo_monitor?.GetValue(Serviceable.OrdersService);
        public Dictionary<ObjectiveState, DecorationTypeOwningLogic> Reflect_OrderMonitor_decorationTypeOwning
        {
            get
            {
                OrdersMonitor monitor = Reflect_OrderService_monitor;
                if (monitor == null) return null;
                return (Dictionary<ObjectiveState, DecorationTypeOwningLogic>)(fieldInfo_decorationTypeOwning?.GetValue(monitor));
            }
        }
            
            
        public override UniTask OnLoading()
        {
            fieldInfo_monitor = ReflectUtils.GetDeclaredField<OrdersService>("monitor");
            fieldInfo_decorationTypeOwning = ReflectUtils.GetDeclaredField<OrdersMonitor>("decorationTypeOwning");
            state.InitGame();
            UpdateDecorationOrder();
            return UniTask.CompletedTask;
        }

        private void UpdateDecorationOrder()
        {
            Dictionary<ObjectiveState, DecorationTypeOwningLogic> decoOrderDict = Reflect_OrderMonitor_decorationTypeOwning;
            if (decoOrderDict == null)
            {
                FLog.Error("Failed to get decoration orders, the decoration order info cannot be refreshed!");
                return;
            }
            foreach(var pair in decoOrderDict)
            {
                ObjectiveState objectiveState = pair.Key;
                DecorationTypeOwningLogic logic = pair.Value;
                logic.InitState(objectiveState);
            }

        }

        public void AddDecorationPercent(string decorationName, float percent)
        {
            AddDecorationPercentWithoutUpdate(decorationName, percent);
            decorationValueChangeSubject.OnNext(state.decorationStates);
            UpdateDecorationInfo();
        }

        private void AddDecorationPercentWithoutUpdate(string decorationName, float percent)
        {
            state.AddDecorationPercent(decorationName, percent);
            state.ApplyDecorationState(decorationName);
        }

        public void AddAllDecorationPercent(float percent)
        {
            foreach (var decoName in state.originalDecorations.Keys)
            {
                AddDecorationPercentWithoutUpdate(decoName, percent);
                state.ApplyDecorationState(decoName);
            }
            UpdateDecorationInfo();
        }

        private void UpdateDecorationInfo()
        {
            decorationValueChangeSubject.OnNext(state.decorationStates);
            UpdateDecorationOrder();
        }

        public float GetDecorationPercent(string decorationName)
        {
            return state.GetDecorationPercent(decorationName);
        }

        public int GetDecorationAmount(DecorationTier tier)
        {
            return BuildingsService.Decorations.Values
                .Sum(int(Decoration d)=>{
                    if (d.IsFinished() && d.model.hasDecorationTier && d.model.tier == tier)
                    {
                        return d.model.decorationScore;
                    }

                    return 0;
                });
        }
    }
}
