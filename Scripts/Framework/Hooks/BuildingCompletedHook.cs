using ATS_API.Effects;
using ATS_API.Helpers;
using Eremite.Buildings;
using Eremite.Model.Effects;
using Forwindz.Framework.Utils;
using System.Xml.Linq;
using System;
using UnityEngine;

namespace Forwindz.Framework.Hooks
{
    public class BuildingCompletedHook : HookLogic
    {
        public static readonly HookLogicType HookLogicTypeEnum = 
            GUIDManager.Get<HookLogicType>(PluginInfo.PLUGIN_GUID, "BuildingCompletedHook");

        [Min(0f)]
        public int amount = 1;
        public bool ignoreDecorationBuildings = true;
        public bool ignoreRoads = true;

        public override HookLogicType Type => HookLogicTypeEnum;

        public override bool CanBeDrawn()
        {
            return true;
        }

        public override string GetAmountText()
        {
            return amount.ToString();
        }

        public override int GetIntAmount()
        {
            return amount;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return false;
        }
    }
}
