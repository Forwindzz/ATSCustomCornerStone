using Eremite;
using Eremite.Buildings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Utils
{
    //API has already done this!

    /*
    public enum DecorationTierTypes
    {
        Comfort = 1,
        Aesthetics = 2,
        Harmony = 3
    }

    public static class DecorationTierExtensions
    {
        public static DecorationTier ToDecorationTier(this DecorationTierTypes decorationTypes)
        {
            if(decorationTierTypeToInternalName.TryGetValue(decorationTypes,out string name))
            {
                return GameMB.Settings.GetDecorationTier(name);
            }
            return null;
        }

        public static string ToInternalName(this DecorationTierTypes decorationTypes)
        {
            if (decorationTierTypeToInternalName.TryGetValue(decorationTypes, out string name))
            {
                return name;
            }
            return null;
        }

        internal static readonly Dictionary<DecorationTierTypes, string> decorationTierTypeToInternalName = new()
            {
            { DecorationTierTypes.Comfort, "DecorationTier 1"},
            { DecorationTierTypes.Aesthetics, "DecorationTier 2"},
            { DecorationTierTypes.Harmony, "DecorationTier 3"}
            };
    }
    */
}
