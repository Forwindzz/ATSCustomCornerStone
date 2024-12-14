using Eremite.Model.Trade;
using ForwindzCustomPerks.Framework.Services;

namespace Forwindz.Framework.Services
{
    public interface IDynamicTraderInfoService
    {
        public DynamicTraderInfoData DynamicTraderState { get; }
        TraderModel GetForceTrader();
        public void AddForceTrader(string name);
        public void RemoveForceTrader(string name);
        public void RemoveAssaultTrader(string name);
        public void ResetNextTrader();
        public void SetEffectFreeChance(string traderName, float chance);
        public void AddEffectFreeChance(string traderName, float chance);
    }
}
