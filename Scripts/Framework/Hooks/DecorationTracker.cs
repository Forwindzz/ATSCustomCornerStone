using Eremite.Buildings;
using Eremite.Controller.Effects;
using Eremite.Model.Effects;
using Forwindz.Framework.Utils;
using Forwindz.Scripts.Framework.Utils;

namespace Forwindz.Framework.Hooks
{
    public class DecorationTracker : HookTracker<DecorationHook>
    {
        public DecorationTracker(HookState hookState, DecorationHook model, HookedEffectModel effectModel, HookedEffectState effectState) : base(hookState, model, effectModel, effectState)
        {
            Recalculate();
        }

        public void OnAddBuilding(Building building)
        {
            Decoration decoration = building as Decoration;
            if(decoration==null)
            {
                return;
            }
            if(decoration.model.hasDecorationTier && decoration.model.tier == model.decorationTier)
            {
                Recalculate();
                //Update(decoration.model.decorationScore);
            }
        }

        public void OnRemoveBuilding(Building building)
        {
            Decoration decoration = building as Decoration;
            if (decoration == null)
            {
                return;
            }
            if (decoration.model.hasDecorationTier && decoration.model.tier == model.decorationTier)
            {
                Recalculate();
                //Update(-decoration.model.decorationScore);
            }
        }

        public void Recalculate()
        {
            int num = BuildingHelper.CountDecorationValue(model.decorationTier);
            FLog.Info($"Decoration Tracker: {model.decorationTier.name} > CurNum={hookState.totalAmount} > {hookState.currentAmount} | UpdateToNum={num} | curFireCount={hookState.firedAmount}");
            UpdateTo(num);
        }

        private void UpdateTo(int amount)
        {
            Update(amount - hookState.totalAmount);
        }

        private void Update(int amount)
        {
            hookState.totalAmount += amount;
            hookState.currentAmount += amount;
            while (hookState.currentAmount >= model.amount)
            {
                Fire();
                hookState.currentAmount -= model.amount;
            }
            while (hookState.currentAmount < 0)
            {
                Revert();
                hookState.currentAmount += model.amount;
            }
        }
    }
}
