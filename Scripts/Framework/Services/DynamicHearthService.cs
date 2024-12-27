using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Buildings;
using Eremite.Services;
using Forwindz.Framework.Utils;
using Newtonsoft.Json;
using Sirenix.Utilities;
using System.Collections.Generic;

namespace Forwindz.Framework.Services
{
    public class HubTierDelegate
    {
        public DynamicValueInt hubPop;

        public HubTierDelegate(HubTier hub) 
        {
            hubPop = new DynamicValueInt(
                () => hub.minPopulation,
                (int delta) => hub.minPopulation += delta
                );
        }

        public void RestoreToOriginal()
        {
            hubPop.RestoreOriginalValue();
        }
    }

    internal class DynamicHearthState
    {
        [JsonIgnore]
        public List<HubTierDelegate> hubTierDelegates = new();
        public float hubPopRequirePercent = 1.0f;
        public int hubPopRequireCount = 0;

        public void InitGame()
        {
            foreach(var hubTier in MB.Settings.hubsTiers)
            {
                hubTierDelegates.Add(new HubTierDelegate(hubTier));
            }
        }

        public void ApplyStates()
        {
            HubTier[] hubTiers = MB.Settings.hubsTiers;
            foreach(HubTierDelegate hubTierDelegate in hubTierDelegates)
            {
                hubTierDelegate.hubPop.SetNewValue(
                    (int)(hubTierDelegate.hubPop.BaseValue * hubPopRequirePercent + hubPopRequireCount)
                    );
            }
            // refresh hearth
            var hearthes = Serviceable.BuildingsService.Buildings.Values.FilterCast<Hearth>();
            foreach (Hearth hearth in hearthes)
            {
                hearth.SlowUpdate();
            }
        }

        public void DestoryRestore()
        {
            foreach (HubTierDelegate hubTierDelegate in hubTierDelegates)
            {
                hubTierDelegate.RestoreToOriginal();
            }
        }
    }

    public class DynamicHearthService : GameService, IDynamicHearthService, IService
    {


        [ModSerializedField]
        private DynamicHearthState state = new();

        static DynamicHearthService()
        {
            CustomServiceManager.RegGameService<DynamicHearthService>();
        }

        public override IService[] GetDependencies()
        {
            return new IService[] {
                Serviceable.HearthService,
                CustomServiceManager.GetAsIService<IExtraStateService>()
            };
        }

        public override void OnDestroy()
        {
            state.DestoryRestore();
            base.OnDestroy();
        }

        public override UniTask OnLoading()
        {
            state.InitGame();
            state.ApplyStates();
            return UniTask.CompletedTask;
        }

        public void AddHearthRequirePopPercent(float v)
        {
            state.hubPopRequirePercent += v;
            state.ApplyStates();
        }

        public void AddHearthRequirePopCount(int v)
        {
            state.hubPopRequireCount += v;
            state.ApplyStates();
        }

    }
}
