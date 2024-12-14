using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Buildings;
using Eremite.Services;
using Forwindz.Framework.Utils.Extend;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UnityEngine;

namespace Forwindz.Framework.Services
{
    internal class DynamicHearthState
    {
        [JsonIgnore]
        public HubTier[] originalHubTiers = null;
        public float hubPopRequirePercent = 1.0f;
        public int hubPopRequireCount = 0;

        public void InitGame()
        {
            originalHubTiers = MB.Settings.hubsTiers.DeepClone();
        }

        public void ApplyStates()
        {
            HubTier[] hubTiers = MB.Settings.hubsTiers;
            for (int i = 0; i < hubTiers.Length; i++)
            {
                hubTiers[i].minPopulation = (int)Mathf.Max(originalHubTiers[i].minPopulation * hubPopRequirePercent + hubPopRequireCount, 0);
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
            HubTier[] hubTiers = MB.Settings.hubsTiers;
            for (int i = 0; i < hubTiers.Length; i++)
            {
                hubTiers[i].minPopulation = originalHubTiers[i].minPopulation;
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
