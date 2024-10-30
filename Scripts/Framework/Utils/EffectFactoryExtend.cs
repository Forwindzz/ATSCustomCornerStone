using ATS_API.Effects;
using Eremite.Model;
using Eremite.Model.Effects;
using Forwindz.Framework.Effects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Utils
{
    public static class EffectFactoryExtend
    {
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

    }
}
