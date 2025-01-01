using Eremite.Buildings;
using Eremite.Model.Effects;
using Eremite.Services;
using Forwindz.Framework.Utils;

namespace Forwindz.Framework.Hooks
{
    public class TradeDealHook : HookLogic
    {
        public static readonly HookLogicType HookLogicTypeEnum =
            GUIDManagerExtend.Get<HookLogicType>(PluginInfo.PLUGIN_GUID, "TradeDeal");
        public override HookLogicType Type => HookLogicTypeEnum;

        public float amount;
        public ValueType valueType = ValueType.SellValue;
        public JudgeType judgeType = JudgeType.Higher;


        public enum ValueType
        {
            SellValue,
            BuyValue,
            BuyExceedValue
        }

        public enum JudgeType
        {
            Higher,
            Lower,
            Equal
        }

        public override string GetAmountText()
        {
            return amount.ToString();
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return false;
        }

        public override bool CanBeDrawn()
        {
            return base.CanBeDrawn() && !Serviceable.TradeService.IsTradingBlocked();
        }

        public override bool IsNoRevertImplemented()
        {
            return true;
        }

        public override int GetIntAmount()
        {
            return (int)amount;
        }

        public override float GetFloatAmount()
        {
            return amount;
        }
    }
}
