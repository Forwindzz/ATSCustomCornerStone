using ATS_API.Effects;
using Eremite.Model;
using Eremite.WorldMap;
using Eremite;
using System.Collections.Generic;
using HarmonyLib;
using ATS_API.Biomes;

namespace Forwindz.Framework.Utils
{
    // this is a tempoary fix!
    // TODO: Move the code to API mod, not here!
    public class EffectAvailability
    {
        static EffectAvailability()
        {
            PatchesManager.RegPatch<EffectAvailability>();
        }

        public static List<IEffectBuilder> RegularCornerstones = new List<IEffectBuilder>();

        private static void SetAvailableBasedOnRarity(List<IEffectBuilder> effectModelBuilders)
        {
            Settings settings = SO.Settings;
            BiomeModel[] biomes = settings.biomes;
            HashSet<EffectsTable> usedEffectTables = new HashSet<EffectsTable>();
            foreach (BiomeModel biome in biomes)
            {
                //FLog.Info($"--- Process {biome.Name}");
                int rewardCount = biome.seasons.SeasonRewards.Count;
                for (int i = 0; i < rewardCount; i++)
                {
                    SeasonRewardModel seasonRewardModel = biome.seasons.SeasonRewards[i];
                    int year = seasonRewardModel.year;
                    bool isLegendaryYear = year == 2 || year == 4 || year == 6;
                    //FLog.Info($"- Year <{year}> | Season {seasonRewardModel.season} | {seasonRewardModel.quarter} | {seasonRewardModel.effectsTable.Name}");
                    EffectsTable effectsTable = seasonRewardModel.effectsTable;
                    if (usedEffectTables.Contains(effectsTable)) //already added? (different years may use same referenced object)
                    {
                        //FLog.Info("Already added!");
                        continue;
                    }
                    usedEffectTables.Add(effectsTable);
                    EffectsTableEntity[] baseEffects = seasonRewardModel.effectsTable.effects;
                    List<EffectsTableEntity> seasonEffects = new List<EffectsTableEntity>(baseEffects);
                    if(seasonRewardModel.effectsTable.effects.Length==0)
                    {
                        // no cornerstone, we should not add
                        continue;
                    }
                    foreach (IEffectBuilder effectBuilder in effectModelBuilders)
                    {
                        EffectModel effect = effectBuilder.Model;
                        switch (effect.rarity)
                        {
                            case EffectRarity.Epic:
                                if (!isLegendaryYear)
                                {
                                    var entity = new EffectsTableEntity();
                                    //TODO: cannot get weight since is private!
                                    //TODO: move this to API!
                                    entity.chance = 100;
                                    entity.effect = effect;
                                    seasonEffects.Add(entity);
                                    FLog.Info($"{biome.Name} Epic Availability: Add <{effect.Name}> to Year <{year}>, Weight={entity.chance} | {seasonEffects.Count}");
                                }
                                break;
                            case EffectRarity.Legendary:
                                if (isLegendaryYear)
                                {
                                    var entity = new EffectsTableEntity();
                                    entity.chance = 100;
                                    entity.effect = effect;
                                    seasonEffects.Add(entity);
                                    FLog.Info($"{biome.Name} Legendary Availability: Add <{effect.Name}> to Year <{year}>, Weight={entity.chance} | {seasonEffects.Count}");
                                }
                                break;
                        }
                    }
                    seasonRewardModel.effectsTable.effects = seasonEffects.ToArray();

                }
            }
        }



        [HarmonyPatch(typeof(ATS_API.Biomes.BiomeManager), nameof(BiomeManager.SyncBiomes))]
        [HarmonyPostfix]
        private static void API_BiomeManager_SyncBiomes_PostPatch()
        {
            SetAvailableBasedOnRarity(RegularCornerstones);
        }
    }
}
