﻿using Eremite;
using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Services;
using Forwindz.Framework.Services;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    /// <summary>
    /// Make the good edible
    /// If you apply multiple times ediable and not ediable, 
    /// ediable + not ediable = no effect (keep the original eatable attribue)
    /// ediable + not ediable*2 = not ediable (not ediable is more than ediable)
    /// ediable*2 + not ediable = ediable (ediable is more than not ediable)
    /// 
    /// </summary>
    public class GoodEdibleEffectModel : EffectModel
    {
        //TODO: maybe deal with text?
        public GoodModel good;
        public bool eatable;

        public override bool IsPerk => true;

        public override string GetAmountText()
        {
            return $"<sprite name=\"{good.Name}\">";
        }


        public override Sprite GetDefaultIcon()
        {
            return good?.icon ?? overrideIcon;
        }

        public override Color GetTypeColor()
        {
            return Settings.RewardColorRawGood;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return false;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            IDynamicGoodTypeService service = CustomServiceManager.GetService<IDynamicGoodTypeService>();
            service.GoodSetEatable(good.Name, eatable);
        }

        public override void OnRemove(EffectContextType contextType, string contextModel, int contextId)
        {
            IDynamicGoodTypeService service = CustomServiceManager.GetService<IDynamicGoodTypeService>();
            service.GoodSetEatable(good.Name, !eatable);
        }
    }
}
