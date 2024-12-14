using Eremite.Buildings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Services
{
    public interface IDynamicBuildingStateService
    {
        public void AddDecorationPercent(string decorationName, float percent);
        public void AddAllDecorationPercent(float percent);
        public float GetDecorationPercent(string decorationName);
        public int GetDecorationAmount(DecorationTier tier);
    }
}
