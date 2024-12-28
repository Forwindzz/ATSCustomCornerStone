using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Services
{
    public interface IDynamicGoodTypeService
    {
        public void GoodRemoveTag(string goodName, string tagName);
        public void GoodAddTag(string goodName, string tagName);
        public void GoodSetEatable(string goodName, bool state);

        public void AddGoodPriceFluctuationDefine(string goodName, Vector2 range);
        public void TriggerGoodPriceFluctuation(string goodName, Vector2 range, int times = 1);
        public void RemoveGoodPriceFluctuationDefine(string goodName, Vector2 range);
        public float GetGoodPriceFluctuation(string goodName, Vector2 range);

        public ModifyGoodTypeStatesTracker GetStateTracker { get; }

    }
}
