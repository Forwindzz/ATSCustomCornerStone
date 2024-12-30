using Cysharp.Threading.Tasks;
using Eremite.Services;
using Forwindz.Framework.Utils;
using HarmonyLib;

namespace Forwindz.Framework.Services
{
    internal class DynamicCornerstoneState
    {
        // if stack>0, yearly cornerstone is banned
        public int noYearlyCornerstone = 0;
    }

    public class DynamicCornerstoneService : GameService, IDynamicCornerstoneService, IService
    {
        [ModSerializedField]
        internal DynamicCornerstoneState state = new();

        static DynamicCornerstoneService()
        {
            CustomServiceManager.RegGameService<DynamicCornerstoneService>();
            PatchesManager.RegPatch<DynamicCornerstoneService>();
        }

        public override IService[] GetDependencies()
        {
            return new IService[]
            {
                CustomServiceManager.GetAsIService<IExtraStateService>()
            };
        }

        public override UniTask OnLoading()
        {
            return base.OnLoading();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void SetNoCornerstone(bool v)
        {
            state.noYearlyCornerstone += v ? 1 : -1;
        }

        public bool GetNoCornerstone()
        {
            return state.noYearlyCornerstone > 0;
        }

        #region patch

        // disable cornerstone reward check
        [HarmonyPatch(typeof(Eremite.Services.CornerstonesService), nameof(Eremite.Services.CornerstonesService.CheckForPick))]
        [HarmonyPrefix]
        private static bool CornerstonesService_CheckForPick_PrePatch(CornerstonesService __instance)
        {
            DynamicCornerstoneService service = CustomServiceManager.GetService<DynamicCornerstoneService>();
            if(service==null)
            {
                return true;
            }
            if(service.GetNoCornerstone())
            {
                //FLog.Info("Cancel yearly cornerstone reward!");
                return false;
            }
            return true;
        }

        #endregion

    }
}
