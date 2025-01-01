using Eremite;
using Eremite.Buildings;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Services.Monitors;
using Forwindz.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Forwindz.Framework.Effects
{
    public class ObtainRelicRewardEffectModel : EffectModel
    {
        public static string newsKey = "ObtainRelicRewardEffectModel_Info_Bonus";

        [Flags]
        public enum FilterType
        {
            None = 0,
            Perks = 1,
            Goods = 2,
            Others = 4
        }

        public FilterType filterType;
        public float getExtraChance = 0.0f;
        public int decisionIndex = -1; //-1 means all

        public override bool IsPerk => true;

        public override string GetAmountText()
        {
            return Services.TextsService.GetPercentage((int)(getExtraChance * 100.0f));
        }

        public override Sprite GetDefaultIcon()
        {
            return overrideIcon;
        }

        public override Color GetTypeColor()
        {
            return Settings.DangerousThreatGladeColor;
        }

        public override bool HasImpactOn(BuildingModel building)
        {
            if (building is RelicModel relic)
            {
                return
                    (relic.rewardsTiers?.Length ?? 0) > 0 ||
                    (relic.decisionsRewards?.Length ?? 0) > 0;
            }
            return false;
        }

        public override void OnApply(EffectContextType contextType, string contextModel, int contextId)
        {
            if (contextType != EffectContextType.Building)
            {
                FLog.Warning("Cannot trigger effect, require Building context");
                return;
            }
            Building building = GameServices.BuildingsService.GetBuilding(contextId);
            Relic relic = building as Relic;
            if (relic == null)
            {
                FLog.Warning("This effect can only apply to Relic, Cannot detect the relic building");
                return;
            }
            if (
                relic.model.hasDynamicRewards &&
                relic.model.rewardsTiers != null &&
                relic.model.rewardsTiers.Length > 0)
            {
                EffectModel[] effects = relic.GetCurrentDynamicRewards();
                ApplyEffects(effects, relic);
            }
            if (
                relic.model.decisionsRewards != null &&
                relic.model.decisionsRewards.Length > 0
                )
            {
                if (decisionIndex < 0)
                {
                    IEnumerable<EffectModel> effects =
                        relic.state.rewardsSets
                        .SelectMany(x => x)
                        .Select(GameMB.GameModelService.GetEffect);
                    ApplyEffects(effects, relic);
                }
                else if (decisionIndex < relic.state.rewardsSets.Length)
                {
                    var rewardEffectStrings = relic.state.rewardsSets[decisionIndex].Select(GameMB.GameModelService.GetEffect);
                    ApplyEffects(rewardEffectStrings, relic);
                }
            }
        }

        private void ApplyEffects(IEnumerable<EffectModel> effects, Relic relic)
        {
            FLog.Info($"Apply Reward from Relic {relic.ModelName}. Candidate Count {effects.Count()}. Condition {filterType}");
            float baseAdditional =
                GameServices.EffectsService.GetRelicExtraRewardsChance(relic) + getExtraChance + 1.0f;

            int times = StableRNG.StableCritialTimes(baseAdditional, 3.0f);

            string newsDescription = "";
            int totalEffectCount = 0;
            foreach (EffectModel effect in effects)
            {
                if (effect == null)
                {
                    continue;
                }
                bool execute = false;
                if (filterType.HasFlag(FilterType.Goods) && effect is GoodsEffectModel)
                {
                    execute = true;
                }
                if (filterType.HasFlag(FilterType.Perks) && effect.IsPerk)
                {
                    execute = true;
                }
                if (filterType.HasFlag(FilterType.Others) &&
                    !(effect is GoodsEffectModel || effect.IsPerk)
                    )
                {;
                    execute = true;
                }

                if (execute)
                {
                    for (int i = 0; i < times; i++)
                    {
                        effect.Apply();
                    }
                    totalEffectCount++;
                    if (effect is GoodsEffectModel goodsEffect)
                    {
                        newsDescription +=
                            GetText("Effect_StatePreview_Generic_Gained",
                                TryFormat(
                                    goodsEffect.description.Text,
                                    goodsEffect.good.GetNameWithIcon(),
                                    times * goodsEffect.GetScaledAmount())) 
                            + "\n";
                    }
                    else
                    {
                        newsDescription +=
                            GetText("Effect_StatePreview_Generic_Gained",
                                effect.DisplayName)
                            + " x " + times + "\n";
                    }
                }
            }

            if (totalEffectCount <= 0 || times <= 0) 
            {
                return;
            }

            BroadcastCallbackTranslateCameraToPos broadcastCallback = new BroadcastCallbackTranslateCameraToPos(relic.Field);
            SO.NewsService.PublishNews(
                            GetText(newsKey, relic.DisplayName, totalEffectCount),
                            newsDescription,
                            AlertSeverity.Info,
                            null,
                            broadcastCallback);
        }

        public override int GetIntAmount()
        {
            return 1;
        }
    }
}
