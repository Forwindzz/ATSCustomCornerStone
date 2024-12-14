using ATS_API.Effects;
using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Forwindz.Framework.Hooks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Utils
{
    public static class HookFactoryExtend
    {
        public static GoodProducedHook Create_GoodProducedHook(
            GoodRef goodRef,
            bool cycles
            )
        {
            GoodProducedHook hook = Activator.CreateInstance<GoodProducedHook>();
            hook.good = goodRef;
            hook.cycles = cycles;
            return hook;
        }

        public static DecorationHook Create_DecorationHook(
            DecorationTier tier,
            int amount
            )
        {
            DecorationHook hook = Activator.CreateInstance<DecorationHook>();
            hook.decorationTier= tier;
            hook.amount= amount;
            return hook;
        }
    }
}
