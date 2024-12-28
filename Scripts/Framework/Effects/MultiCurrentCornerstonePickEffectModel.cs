using Eremite;
using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Model.ViewsConfigurations;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    public class MultiCurrentCornerstonePickEffectModel : EffectModel
    {
        public CornerstonesViewConfiguration viewConfiguration;
        public int times = 1;
        public List<int> restrictYears = new List<int>();
        public override bool IsPerk => true;

        public override bool IsPositive => true;

        public override string GetAmountText()
        {
            return times.ToString();
        }

        public override Sprite GetDefaultIcon()
        {
            return overrideIcon;
        }

        public override int GetIntAmount()
        {
            return times;
        }

        public override Color GetTypeColor()
        {
            return MB.Settings.RewardColorComposite;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            return false;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            int thisYear = SO.CalendarService.Year;
            int minDeltaYears = int.MaxValue;
            int bestYear = thisYear;
            foreach (int restrictYear in restrictYears)
            {
                int deltaYear = Mathf.Abs(restrictYear - thisYear);
                if (deltaYear < minDeltaYears)
                {
                    minDeltaYears = deltaYear;
                    bestYear = restrictYear;
                }
            }
            for (int i = 0; i < times; i++)
            {
                SO.CornerstonesService.GrantExtraPick(bestYear, viewConfiguration.Name);
            }
        }
    }
}
