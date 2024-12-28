using ATS_API.Effects;
using ATS_API.Helpers;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Model.Effects.Hooked;
using Forwindz.Framework.Utils;

using HarmonyLib;
using Forwindz.Framework.Hooks;
using static Eremite.Model.Effects.HookedStateTextArg;
using System.Collections.Generic;
using System.Linq;
using System;
using Forwindz.Framework.Effects;
using UnityEngine;
using Eremite.Model.ViewsConfigurations;
using Eremite;

using HookedTextArgType = Eremite.Model.Effects.Hooked.TextArgType;
using CompositeTextArgType = Eremite.Model.Effects.TextArgType;

namespace Forwindz.Content
{
    internal class CustomCornerstones
    {
        static CustomCornerstones()
        {
            PatchesManager.RegPatch<CustomCornerstones>();
        }

        public static void CreateNewCornerstones()
        {
            UnityEngine.Debug.Log("Starting to creating custom cornerstones");
            CreateForwindzCornerStones();
            UnityEngine.Debug.Log("All custom cornerstones created successfully!");
        }

        private static HookedEffectBuilder CreateBaseHookEffect(
            string cornerstoneName, 
            string cornerstoneIconPath,
            EffectRarity rarity,
            bool isPositive=true,
            bool addAsCornerStone = true)
        {
            HookedEffectBuilder builder = new(PluginInfo.PLUGIN_GUID, cornerstoneName, cornerstoneIconPath);
            builder.SetPositive(isPositive);
            builder.SetRarity(rarity);
            if(addAsCornerStone)
            {
                builder.SetObtainedAsCornerstone();
                //TODO: replace this with an elegant ATS_API's API
                //builder.SetAvailableInAllBiomesAndSeasons();
                EffectAvailability.RegularCornerstones.Add(builder);
            }
            builder.SetLabel("Mod - Forwindz");
            builder.SetDescriptionKey($"{PluginInfo.PLUGIN_GUID}_{cornerstoneName}_description");
            builder.SetDisplayNameKey($"{PluginInfo.PLUGIN_GUID}_{cornerstoneName}_displayName");
            return builder;
        }

        private static CompositeEffectBuilder CreateBaseCompositeEffect(
            string cornerstoneName,
            string cornerstoneIconPath,
            EffectRarity rarity,
            bool isPositive = true)
        {
            CompositeEffectBuilder builder = new(PluginInfo.PLUGIN_GUID, cornerstoneName, cornerstoneIconPath);
            builder.SetPositive(isPositive);
            builder.SetRarity(rarity);
            builder.SetObtainedAsCornerstone();

            //TODO: replace this with an elegant ATS_API's API
            //builder.SetAvailableInAllBiomesAndSeasons();
            EffectAvailability.RegularCornerstones.Add(builder);

            builder.SetLabel("Mod - Forwindz");
            builder.SetDescriptionKey($"{PluginInfo.PLUGIN_GUID}_{cornerstoneName}_description");
            builder.SetDisplayNameKey($"{PluginInfo.PLUGIN_GUID}_{cornerstoneName}_displayName");
            return builder;
        }

        private static HookedEffectBuilder HookedEffectAddPreviewKey(HookedEffectBuilder builder)
        {
            builder.SetPreviewDescriptionKey($"{PluginInfo.PLUGIN_GUID}_{builder.Name}_preview");
            return builder;
        }

        private static HookedEffectBuilder HookedEffectAddRetroactiveKey(HookedEffectBuilder builder)
        {
            builder.SetRetroactiveDescriptionKey($"{PluginInfo.PLUGIN_GUID}_{builder.Name}_retroactive");
            return builder;
        }

        private static void CreateForwindzCornerStones()
        {
            CreateSaladRecipe();
            CreateFoolhardyGambler();
            CreateGardenDesign();
            CreateUsabilityDesign();
            CreateOverdraftTechnicalContract();
            CreateAmberFluctuation();
        }

        private static void CreateAmberFluctuation()
        {
            //TODO:...
        }

        private static void CreateOverdraftTechnicalContract()
        {
            CompositeEffectBuilder compositeBuilder = CreateBaseCompositeEffect(
                "OverdraftTechnicalContract",
                "OverdraftTechnicalContract.jpeg", //TODO: replace
                EffectRarity.Legendary);

            var noCornerstoneEffect = EffectFactoryExtend.CreateEffect<NoQueenCornerstoneEffectModel>(compositeBuilder);
            noCornerstoneEffect.removeCornerstoneFlag = true;

            var extraPickEffect = EffectFactoryExtend.CreateEffect<MultiCurrentCornerstonePickEffectModel>(compositeBuilder);
            extraPickEffect.restrictYears = [2, 4, 6];
            extraPickEffect.times = 2;
            CornerstonesViewConfiguration defaultCornerstoneViewConfig = MB.Settings.GetCornerstonesViewConfiguration("Cornerstones View Default");
            extraPickEffect.viewConfiguration = ScriptableObject.CreateInstance<CornerstonesViewConfiguration>();
            extraPickEffect.viewConfiguration.npcDialogue = new LocaText();
            extraPickEffect.viewConfiguration.npcDialogue.key = "OverdraftTechnicalContract_npcDialogue";
            extraPickEffect.viewConfiguration.npcName = defaultCornerstoneViewConfig.npcName;
            extraPickEffect.viewConfiguration.npcIcon = defaultCornerstoneViewConfig.npcIcon;
            compositeBuilder.AddEffects(
                [
                    noCornerstoneEffect,
                    extraPickEffect
                ]
                );

            compositeBuilder.SetNestedAmountIndex(1);
            compositeBuilder.SetDescriptionArgs(
                [
                    (CompositeTextArgType.Amount, 1)
                ]
                );
        }

        private static void CreateSaladRecipe()
        {
            HookedEffectBuilder builder = CreateBaseHookEffect(
                "SaladRecipe",
                "SaladRecipe.jpeg",
                EffectRarity.Epic
                );
            builder.SetDrawLimit(1);
            //builder.AddInstantEffect(EffectFactoryExtend.AddHookedEffect_ChanceForNoConsumptionEffectModel(builder,0.15f));
            builder.AddInstantEffect(EffectFactoryExtend.AddHookedEffect_GoodEdibleEffectModel(
                builder, GoodsTypes.Food_Raw_Herbs.ToGoodModel(), true));
            builder.AddInstantEffect(EffectFactoryExtend.AddHookedEffect_GoodEdibleEffectModel(
                builder, GoodsTypes.Food_Raw_Grain.ToGoodModel(), true));
            builder.AddInstantEffect(EffectFactoryExtend.AddHookedEffect_GoodEdibleEffectModel(
                builder, GoodsTypes.Mat_Raw_Algae.ToGoodModel(), true));
        }

        private static void CreateFoolhardyGambler()
        {
            HookedEffectBuilder builder = CreateBaseHookEffect(
                "FoolhardyGambler",
                "FoolhardyGambler.jpeg",
                EffectRarity.Legendary
                );
            builder.SetDrawLimit(1);
            builder.AddInstantEffect(
                EffectFactoryExtend.AddHookedEffect_OnlyTraderEffectModel(
                    builder, 
                    TraderTypes.Trader_7_Trickster.ToTraderModel()
                    ));
            builder.AddInstantEffect(
                EffectFactoryExtend.AddHookedEffect_TraderIntervalEffectModel(
                    builder,
                    0.85f
                    ));
            builder.AddInstantEffect(
                EffectFactoryExtend.AddHookedEffect_AddTraderFreeEffectChanceEffectModel(
                    builder,
                    TraderTypes.Trader_7_Trickster.ToTraderModel(),
                    0.15f
                    ));
            builder.SetDescriptionArgs(
                (SourceType.InstantEffect, HookedTextArgType.Amount, 0),
                (SourceType.InstantEffect, HookedTextArgType.Amount, 1),
                (SourceType.InstantEffect, HookedTextArgType.Amount, 2)
                );
        }

        private static void CreateGardenDesign()
        {
            HookedEffectBuilder builder = CreateBaseHookEffect(
                "GardenDesign",
                "GardenDesign.jpeg",
                EffectRarity.Legendary
                );
            builder.SetDrawLimit(1);
            builder.AddInstantEffect(
                EffectFactoryExtend.AddHookedEffect_HearthPopPercentEffectModel(
                    builder,
                    -0.9f
                ));
            builder.AddInstantEffect(
                EffectFactoryExtend.AddHookedEffect_DecorationPercentEffectModel(
                    builder,
                    1.0f
                ));
            builder.AddInstantEffect(
                EffectFactoryExtend.AddHookedEffect_VillagersBreakTimeRateEffectModel(
                    builder,
                    1.0f
                ));
            builder.SetDescriptionArgs(
                (SourceType.InstantEffect, HookedTextArgType.Amount, 0),
                (SourceType.InstantEffect, HookedTextArgType.Amount, 1),
                (SourceType.InstantEffect, HookedTextArgType.Amount, 2)
                );
        }

        private static void CreateUsabilityDesign()
        {
            const float BASE_VALUE = 0.01f;
            CompositeEffectBuilder compositeBuilder = CreateBaseCompositeEffect(
                "UsabilityDesign",
                "UsabilityDesign.jpeg",
                EffectRarity.Epic);

            // for all sub effect
            HookedStateTextArg hookArg0 = new HookedStateTextArg()
            {
                source = HookedStateTextSource.TotalGainFloatFromHooked,
                sourceIndex = 0,
                asPercentage = true
            };

            HookedEffectBuilder comfortBuilder = CreateBaseHookEffect(
                "UsabilityDesign_Comfort",
                "UsabilityDesign.jpeg",
                EffectRarity.None,
                true, false
                );
            DecorationHook comfortHook = HookFactoryExtend.Create_DecorationHook(
                    DecorationTierTypes.Comfort.ToDecorationTier(),
                    1);
            comfortBuilder.AddHook(comfortHook);
            comfortBuilder.AddHookedEffect(
                EffectFactoryExtend.AddHookedEffect_HarvestingRateEffectModel(comfortBuilder, BASE_VALUE)
                );
            comfortBuilder.SetNestedAmount(SourceType.HookedEffect, HookedTextArgType.Amount, 0);
            comfortBuilder.SetDescriptionArgs([(SourceType.HookedEffect, HookedTextArgType.Amount, 0)]);
            HookedEffectAddPreviewKey(comfortBuilder);
            comfortBuilder.SetPreviewDescriptionArgs(hookArg0);
            HookedEffectAddRetroactiveKey(comfortBuilder);
            comfortBuilder.SetRetroactiveDescriptionArgs(hookArg0);

            HookedEffectBuilder aestheticsBuilder = CreateBaseHookEffect(
                "UsabilityDesign_Aesthetics",
                "UsabilityDesign.jpeg",
                EffectRarity.None,
                true, false
                );
            DecorationHook aestheticsHook = HookFactoryExtend.Create_DecorationHook(
                    DecorationTierTypes.Aesthetics.ToDecorationTier(),
                    1);
            aestheticsBuilder.AddHook(aestheticsHook);
            aestheticsBuilder.AddHookedEffect(
                EffectFactoryExtend.AddHookedEffect_VillagersSpeedEffectModel(
                    aestheticsBuilder,
                    VillagerSpeedRewardType.Global,
                    BASE_VALUE)
                );
            aestheticsBuilder.SetNestedAmount(SourceType.HookedEffect, HookedTextArgType.Amount, 0);
            aestheticsBuilder.SetDescriptionArgs([(SourceType.HookedEffect, HookedTextArgType.Amount, 0)]);
            HookedEffectAddPreviewKey(aestheticsBuilder);
            aestheticsBuilder.SetPreviewDescriptionArgs(hookArg0);
            HookedEffectAddRetroactiveKey(aestheticsBuilder);
            aestheticsBuilder.SetRetroactiveDescriptionArgs(hookArg0);

            HookedEffectBuilder harmonyBuilder = CreateBaseHookEffect(
                "UsabilityDesign_Harmony",
                "UsabilityDesign.jpeg",
                EffectRarity.None,
                true,false
                );
            DecorationHook harmonyHook = HookFactoryExtend.Create_DecorationHook(
                    DecorationTierTypes.Harmony.ToDecorationTier(),
                    1);
            harmonyBuilder.AddHook(harmonyHook);
            harmonyBuilder.AddHookedEffect(
                EffectFactoryExtend.AddHookedEffect_GlobalExtraProductionChanceEffectModel(harmonyBuilder, BASE_VALUE)
                );
            harmonyBuilder.SetDescriptionArgs(
                [
                    (SourceType.Hook, HookedTextArgType.Amount, 0),
                ]
                );
            harmonyBuilder.SetNestedAmount(SourceType.HookedEffect, HookedTextArgType.Amount, 0);
            harmonyBuilder.SetDescriptionArgs([(SourceType.HookedEffect, HookedTextArgType.Amount, 0)]);
            HookedEffectAddPreviewKey(harmonyBuilder);
            harmonyBuilder.SetPreviewDescriptionArgs(hookArg0);
            HookedEffectAddRetroactiveKey(harmonyBuilder);
            harmonyBuilder.SetRetroactiveDescriptionArgs(hookArg0);
            compositeBuilder.AddEffects([harmonyBuilder.EffectModel, aestheticsBuilder.EffectModel, comfortBuilder.EffectModel]);

            compositeBuilder.SetNestedFloatAmountIndex(0);
            compositeBuilder.SetNestedAmountIndex(0);
            //compositeBuilder.SetNestedPreviewIndex(0);
            compositeBuilder.SetNestedRetroactiveIndex(0);

            compositeBuilder.SetDescriptionArgs(
                [
                    (CompositeTextArgType.Amount, 0)
                ]
                );
            compositeBuilder.SetShowEffectAsPerks(true);

        }

        #region Patch
        /*
        // Allow to show UsabilityDesign preview, based on its rewards[i]'s hook
        [HarmonyPatch(
            typeof(Eremite.View.HUD.Perks.PerkTooltipStatePanel),
            nameof(Eremite.View.HUD.Perks.PerkTooltipStatePanel.UpdateStates))]
        [HarmonyPrefix]
        private static bool PerkTooltipStatePanel_UpdateStates_PrePatch(
            PerkTooltipStatePanel __instance)
        {
            if(__instance.effect==null)
            {
                return true;
            }
            if(__instance.effect.Name == $"{PluginInfo.PLUGIN_GUID}_UsabilityDesign")
            {
                CompositeEffectModel compositeEffect = __instance.effect as CompositeEffectModel;
                List<string> hookEffectNames = new();
                //collect all hook states
                foreach (var effect in compositeEffect.rewards)
                {
                    if(effect is HookedEffectModel hookedEffect)
                    {
                        hookEffectNames.Add(hookedEffect.Name);
                    }
                } 
                var hookStates = GameMB.StateService.HookedEffects.activeEffects.Where(
                    (HookedEffectState e) => hookEffectNames.Contains(e.model));
                __instance.SetUpHookedState(compositeEffect, hookStates);
                return false;
            }
            return true;
        }
        */

        // Allow to show UsabilityDesign retroative preview, based on its rewards[i]'s hook
        [HarmonyPatch(
            typeof(CompositeEffectModel),
            nameof(CompositeEffectModel.GetTooltipFootnote))]
        [HarmonyPrefix]
        private static bool CompositeEffectModel_GetTooltipFootnote_PrePatch(
            CompositeEffectModel __instance, ref string __result)
        {
            if (__instance.Name == $"{PluginInfo.PLUGIN_GUID}_UsabilityDesign")
            {
                List<string> footnotes = new();
                //collect all hook states
                foreach (var effect in __instance.rewards)
                {
                    string footnote = effect.GetTooltipFootnote();
                    if(footnote!=null)
                        footnotes.Add(footnote);
                }
                __result = footnotes.Join(null, "\n");
                return false;
            }
            return true;
        }

        /// <summary>
        /// This is for VillagersSpeedEffectModel,
        /// Force it to return its value in `GetFloatAmount()`.
        /// The original game does not implement it,
        /// and thus causing problem in retroactive and preview text... :(
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch(
            typeof(EffectModel),
            nameof(EffectModel.GetFloatAmount))]
        [HarmonyPrefix]
        private static bool EffectModel_GetFloatAmount_PrePatch(
            EffectModel __instance, ref float __result)
        {
            if(__instance is VillagersSpeedEffectModel villagersSpeedEffect)
            {
                __result = villagersSpeedEffect.amount;
                return false;
            }
            return true;
        }


        #endregion


    }
}
