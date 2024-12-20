using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Model;
using Eremite.Model.State;
using Eremite.Model.Trade;
using Eremite.Services;
using Forwindz.Framework.Services;
using Forwindz.Framework.Utils;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniRx;

namespace ForwindzCustomPerks.Framework.Services
{

    public class DynamicTraderExtraState
    {
        public float chanceToFreeEffect = 0.0f;
    }

    public class DynamicTraderInfoData
    {
        public List<string> forceTraderNameList = new();

        // global effects, work for the whole settlement
        [JsonProperty]
        private Dictionary<string, DynamicTraderExtraState> traderGlobalStates = new();

        // current visit state, only work for each merchant visit
        /// <summary>
        /// This will modify the original effect price, temporarily.
        /// The prices are still affected by effects
        /// </summary>
        public Dictionary<string, float> effectsSellValue = new();

        public DynamicTraderExtraState GetTraderGlobalEffects(string traderName)
        {
            if(!traderGlobalStates.TryGetValue(traderName, out DynamicTraderExtraState extraState))
            {
                FLog.Info($"[{traderName}] does not have extra states, create it");
                extraState = new DynamicTraderExtraState();
                traderGlobalStates[traderName] = extraState;
            }
            return extraState;
        }

        public bool GetEffectModifiedSellValue(string effectName, out float modifiedValue)
        {
            if(effectsSellValue.TryGetValue(effectName, out modifiedValue))
            {
                return true;
            }
            else
            {
                modifiedValue = 0.0f;
                return false;
            }
        }

    }

    public class DynamicTraderInfoService : GameService, IDynamicTraderInfoService, IService
    {
        [ModSerializedField]
        internal DynamicTraderInfoData stateData = new();
        private IDisposable traderArriveListener = null;

        public DynamicTraderInfoData DynamicTraderState => stateData;

        private MethodInfo methodUpdateNextVisit = null;



        static DynamicTraderInfoService()
        {
            CustomServiceManager.RegGameService<DynamicTraderInfoService>();
            PatchesManager.RegPatch<DynamicTraderInfoService>();
        }

        public override IService[] GetDependencies()
        {
            return new IService[] {
                Serviceable.TradeService,
                CustomServiceManager.GetAsIService<IExtraStateService>()
            };
        }
        
        public void UpdateNextVisit()
        {
            if(methodUpdateNextVisit==null)
            {
                FLog.Error("Cannot invoke UpdateNextVisit()! The trader state cannot be update!");
                return;
            }
            methodUpdateNextVisit.Invoke(Serviceable.TradeService, null);
        }

        public override void OnDestroy()
        {
            traderArriveListener.Dispose();
            base.OnDestroy();
        }

        public override UniTask OnLoading()
        {
            methodUpdateNextVisit = AccessTools.Method(typeof(Eremite.Services.TradeService), "UpdateNextVisit");
            traderArriveListener = Serviceable.TradeService.OnTraderArrived.Subscribe(
                new Action<TraderVisitState>(OnTraderArrive));
            RestoreData();
            return UniTask.CompletedTask;
        }

        private void RestoreData()
        {
            FLog.Info($"Restored DynamicTraderStateInfo > ");
            FLog.Info($"Sell Perks Count: {stateData.effectsSellValue.Count} | Force List: {stateData.forceTraderNameList.Count}");
        }

        private void OnTraderArrive(TraderVisitState visitState)
        {
            FLog.Info($"Trader [{visitState.trader}] Arrived! Process its effect price!");
            this.ApplyEffectsToVisitState(visitState);
        }

        /// <summary>
        /// Apply global state and effects (ex. free cornerstones)
        /// </summary>
        /// <param name="newVisitState"></param>
        private void ApplyEffectsToVisitState(TraderVisitState newVisitState)
        {
            stateData.effectsSellValue.Clear();
            DynamicTraderExtraState dynExtraState = stateData.GetTraderGlobalEffects(newVisitState.trader);

            // check free cornerstone
            if (dynExtraState.chanceToFreeEffect > 0.0f) 
            {
                FLog.Info($"{newVisitState.trader} has {dynExtraState.chanceToFreeEffect} free chance.");
                foreach (var effectPair in newVisitState.effects)
                {
                    FLog.Info($"Check {effectPair}");
                    string effectName = effectPair.Key;
                    bool alreadyBought = effectPair.Value;
                    if (!alreadyBought)
                    {
                        FLog.Info($"{dynExtraState.chanceToFreeEffect} free chance for {effectPair}");
                        if (RNG.Roll(dynExtraState.chanceToFreeEffect))
                        {
                            stateData.effectsSellValue.Add(effectName, 0.0f);
                            FLog.Info($"Set {effectName} as free");
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// The trading is restricted to the specific trader
        /// </summary>
        /// <returns></returns>
        public TraderModel GetForceTrader()
        {
            if (stateData.forceTraderNameList.Count == 0)
            {
                return null;
            }
            else
            {
                string traderName = stateData.forceTraderNameList
                    .Where(x => !StateService.Trade.assaultedTraders.Contains(x))
                    .FirstOrDefault();
                if (traderName == null || traderName.Length == 0) 
                {
                    return null;
                }
                return Serviceable.Settings.GetTrader(traderName);
            }
        }

        /// <summary>
        /// Allow to add multiple times.
        /// When add, remove it from assaulted list.
        /// </summary>
        /// <param name="trader"></param>
        public void AddForceTrader(string name)
        {
            FLog.Info($"Add Force Trader: {name}");
            RemoveAssaultTrader(name);
            stateData.forceTraderNameList.Add(name);
            ResetNextTrader();
        }

        /// <summary>
        /// Only remove one each time.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveForceTrader(string name)
        {
            if(!stateData.forceTraderNameList.Remove(name))
            {
                FLog.Warning("Cannot remove forced trader [" + name + "]. It not exists in forced list!");
            }
            else
            {
                FLog.Info($"Remove Force Trader: {name}");
                ResetNextTrader();
            }
        }

        public void RemoveAssaultTrader(string name)
        {
            FLog.Info($"Remove Assaulted Trader Record: {name}");
            Serviceable.StateService.Trade.assaultedTraders.Remove(name);
        }

        public void ResetNextTrader()
        {
            if(Serviceable.TradeService.IsMainTraderInTheVillage())
            {
                // if the merchant already arrive, do not reset the visit state
                // it will automatically reset after the merchant depart
                return;
            }
            FLog.Info($"Reset Next Trader");
            float travelStartTime = -1;
            bool hasOldtravel = false;
            if(Serviceable.StateService.Trade.visit!=null)
            {
                hasOldtravel = true;
                travelStartTime = Serviceable.StateService.Trade.visit.travelStartTime;
            }
            Serviceable.StateService.Trade.visit = null;
            UpdateNextVisit();
            if(Serviceable.StateService.Trade.visit != null)
            {
                if(hasOldtravel)
                {
                    // keep the travel start time, so that the arrival progress will not be reset
                    Serviceable.StateService.Trade.visit.travelStartTime = travelStartTime;
                }
            }
        }

        public void SetEffectFreeChance(string traderName, float chance)
        {
            DynamicTraderExtraState extraTraderState = stateData.GetTraderGlobalEffects(traderName);
            extraTraderState.chanceToFreeEffect = chance;
            FLog.Info($"{traderName} set {chance} free effects chance.");
        }

        public void AddEffectFreeChance(string traderName, float chance)
        {
            DynamicTraderExtraState extraTraderState = stateData.GetTraderGlobalEffects(traderName);
            extraTraderState.chanceToFreeEffect += chance;
            FLog.Info($"{traderName} add {chance} free effects chance, now it is {extraTraderState.chanceToFreeEffect}");
        }

        #region patch

        // next trader is always this trader:
        [HarmonyPatch(typeof(Eremite.Services.TradeService), nameof(Eremite.Services.TradeService.UpdateNextVisit))]
        [HarmonyPrefix]
        private static bool TradeService_UpdateNextVisit_PrePatch(TradeService __instance)
        {
            if(__instance.State.visit != null)
            {
                return true;
            }

            DynamicTraderInfoService service = CustomServiceManager.GetService<DynamicTraderInfoService>();
            TraderModel traderModel = service.GetForceTrader();
            if (traderModel != null)
            {
                // The next trader is always this trader!
                FLog.Info($"Force to set trader as {traderModel.Name}");
                __instance.State.visit = __instance.CreateVisit(traderModel, 0);
                return false;
            }
            return true;
        }
        /*
        [HarmonyPatch(typeof(Eremite.Services.TradeService), nameof(Eremite.Services.TradeService.CreateVisit))]
        [HarmonyPostfix]
        private static void TradeService_CreateVisit_PostPatch(TradeService __instance, ref TraderVisitState __result, TraderModel trader, int sourceId)
        {
            //CustomServiceManager.GetService<DynamicTraderInfoService>().CreateExtraSellEffectsInformation(__result);
        }*/
        /*
        /// <summary>
        /// Process buy cornerstone/effects behavior
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="effect"></param>
        /// <returns></returns>
        [HarmonyPatch(typeof(Eremite.Services.TradeService), nameof(Eremite.Services.TradeService.RemoveCurrencyFor))]
        [HarmonyPrefix]
        private static bool TradeService_RemoveCurrencyFor_PrePatch(TradeService __instance, EffectModel effect)
        {
            DynamicTraderInfoService service = CustomServiceManager.GetService<DynamicTraderInfoService>();
            if (service.stateData.GetEffectModifiedSellValue(effect.Name, out float modifiedValue))
            {
                if(modifiedValue>0) // if it is not free, pay it!
                {
                    // pay with modified value
                    Serviceable.StorageService.Remove(
                        service.PriceToCurrency(Serviceable.TradeService.GetValueInCurrency(effect)),
                        StorageOperationType.Trade);
                }
                return true;
            }
            return false;
        }*/

        [HarmonyPatch(
            typeof(Eremite.Services.TradeService), 
            nameof(Eremite.Services.TradeService.GetValueInCurrency),
            new Type[] { typeof(EffectModel)})]
        [HarmonyPostfix]
        private static void TradeService_GetValueInCurrency_Effect_PostPatch(
            TradeService __instance, 
            ref float __result,
            EffectModel effect)
        {
            DynamicTraderInfoService service = CustomServiceManager.GetService<DynamicTraderInfoService>();
            if (service.stateData.GetEffectModifiedSellValue(effect.name, out float modifiedValue))
            {
                if(effect.tradingBuyValue!=0)
                {
                    float finalValue = __result * modifiedValue / effect.tradingBuyValue;
                    FLog.Info($"Change effect price from {__result} to {finalValue}");
                    __result = finalValue;
                }
            }
        }
        #endregion
    }
}
