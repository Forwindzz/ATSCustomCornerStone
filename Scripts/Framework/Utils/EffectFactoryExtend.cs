using ATS_API.Effects;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Model.Trade;
using Forwindz.Framework.Effects;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Utils
{
    public static class EffectFactoryExtend
    {

        public static T CreateEffect<T>() where T:EffectModel
        {
            T effect = ScriptableObject.CreateInstance<T>();
            effect.blockedBy = Array.Empty<EffectModel>();
            effect.usabilityTags = Array.Empty<ModelTag>();
            return effect;
        }

        public static T CreateEffect<T>(CompositeEffectBuilder builder) where T : EffectModel
        {
            T effect = ScriptableObject.CreateInstance<T>();
            effect.description = builder.Model.description;
            effect.displayName = builder.Model.displayName;
            effect.label = builder.Model.label;
            effect.blockedBy = Array.Empty<EffectModel>();
            effect.usabilityTags = Array.Empty<ModelTag>();
            return effect;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">0.15 means 15%</param>
        /// <returns></returns>
        public static 
            ChanceForNoConsumptionEffectModel
            AddHookedEffect_ChanceForNoConsumptionEffectModel(
            IEffectBuilder effectBuilder,
            float amount
            )
        {
            var effect = 
                EffectFactory.NewHookedEffect<ChanceForNoConsumptionEffectModel>(effectBuilder);
            effect.amount = amount;
            return effect;
        }


        public static
            GoodEdibleEffectModel
            AddHookedEffect_GoodEdibleEffectModel(
            IEffectBuilder effectBuilder,
            GoodModel goodModel,
            bool ediable
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<GoodEdibleEffectModel>(effectBuilder);
            effect.good = goodModel;
            effect.eatable = ediable;
            return effect;
        }

        public static
            AddTraderFreeEffectChanceEffectModel
            AddHookedEffect_AddTraderFreeEffectChanceEffectModel(
            IEffectBuilder effectBuilder,
            TraderModel traderModel,
            float chance
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<AddTraderFreeEffectChanceEffectModel>(effectBuilder);
            effect.trader = traderModel;
            effect.chance = chance;
            return effect;
        }

        public static
            OnlyTraderEffectModel
            AddHookedEffect_OnlyTraderEffectModel(
            IEffectBuilder effectBuilder,
            TraderModel traderModel
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<OnlyTraderEffectModel>(effectBuilder);
            effect.trader = traderModel;
            return effect;
        }

        public static
            TraderIntervalEffectModel
            AddHookedEffect_TraderIntervalEffectModel(
            IEffectBuilder effectBuilder,
            float amount
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<TraderIntervalEffectModel>(effectBuilder);
            effect.amount = amount;
            return effect;
        }

        public static
            HearthPopPercentEffectModel
            AddHookedEffect_HearthPopPercentEffectModel(
            IEffectBuilder effectBuilder,
            float percent
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<HearthPopPercentEffectModel>(effectBuilder);
            effect.percent = percent;
            return effect;
        }

        public static
            DecorationPercentEffectModel
            AddHookedEffect_DecorationPercentEffectModel(
            IEffectBuilder effectBuilder,
            float percent
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<DecorationPercentEffectModel>(effectBuilder);
            effect.percent = percent;
            return effect;
        }

        public static
            VillagersBreakTimeRateEffectModel
            AddHookedEffect_VillagersBreakTimeRateEffectModel(
            IEffectBuilder effectBuilder,
            float percent
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<VillagersBreakTimeRateEffectModel>(effectBuilder);
            effect.amount = percent;
            return effect;
        }

        public static
            GlobalProductionRateEffectModel
            AddHookedEffect_GlobalProductionRateEffectModel(
            IEffectBuilder effectBuilder,
            float percent
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<GlobalProductionRateEffectModel>(effectBuilder);
            effect.amount = percent;
            return effect;
        }

        public static
            HarvestingRateEffectModel
            AddHookedEffect_HarvestingRateEffectModel(
            IEffectBuilder effectBuilder,
            float percent
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<HarvestingRateEffectModel>(effectBuilder);
            effect.amount = percent;
            return effect;
        }

        public static
            VillagersSpeedEffectModel
            AddHookedEffect_VillagersSpeedEffectModel(
            IEffectBuilder effectBuilder,
            VillagerSpeedRewardType type,
            float percent
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<VillagersSpeedEffectModel>(effectBuilder);
            effect.type = type;
            effect.amount = percent;
            return effect;
        }

        public static
            GlobalExtraProductionChanceEffectModel
            AddHookedEffect_GlobalExtraProductionChanceEffectModel(
            IEffectBuilder effectBuilder,
            float percent
            )
        {
            var effect =
                EffectFactory.NewHookedEffect<GlobalExtraProductionChanceEffectModel>(effectBuilder);
            effect.amount = percent;
            return effect;
        }


    }
}
