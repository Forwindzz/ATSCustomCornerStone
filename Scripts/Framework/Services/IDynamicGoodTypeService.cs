using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Services
{
    public interface IDynamicGoodTypeService
    {
        public void GoodRemoveTag(string goodName, string tagName);
        public void GoodAddTag(string goodName, string tagName);
        public void GoodSetEatable(string goodName, bool state);
        public ModifyGoodTypeStatesTracker GetStateTracker { get; }
    }
}
