#define DEBUG

using BepInEx;
using Eremite;
using Eremite.Controller;
using Eremite.Model;
using Eremite.Model.State;
using Eremite.Services;
using Forwindz.Content;
using Forwindz.Framework.Utils;
using HarmonyLib;
using System.Collections.Generic;


namespace Forwindz
{

    public static class PluginInfo
    {
        public const string PLUGIN_GUID= "Forwindz.CustomCornerstones";
        public const string PLUGIN_NAME = "f9's cornerstones";
        public const string PLUGIN_VERSION = "1.0";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;

        private void Awake()
        {
            Instance = this;
            ReflectUtils.InitializeAllStatics(typeof(Plugin).Assembly);
            PatchesManager.RegPatch<Plugin>();
            PatchesManager.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");            
        }             

        [HarmonyPatch(typeof(MainController), nameof(MainController.InitReferences))]
        [HarmonyPostfix]
        private static void PostSetupMainController()
        {
            Instance.Logger.LogInfo("Initializing post Init References in MainController");
            LocalizationLoader.LoadLocalization("assets/lang");
            CustomCornerstones.CreateNewCornerstones();
            //Utils.SetSeasonsTime(20f, 20f, 10f, SeasonQuarter.First); // makes cornerstones appear immediately with game start
        }

        [HarmonyPatch(typeof(CornerstonesService), nameof(CornerstonesService.GenerateRewardsFor))]
        [HarmonyPrefix]
        private static bool GenerateRewardsFor_PrePatch(CornerstonesService __instance, SeasonRewardModel model, ref string viewConfiguration, ref bool isExtra)
        {
#if DEBUG
            Log.Info(string.Format("[Cor] Generate{0} cornerstones for {1} {2} {3} with model {4} {5} {6}",
            [
                __instance.GetExtraLogSufix(isExtra),
                Serviceable.CalendarService.Year,
                Serviceable.CalendarService.Season,
                Serviceable.CalendarService.Quarter,
                model.year,
                model.season,
                model.quarter
            ]), null);
            //TODO: this is debug code
            List<string> effects = 
                [$"{PluginInfo.PLUGIN_GUID}_GardenDesign", $"{PluginInfo.PLUGIN_GUID}_UsabilityDesign", $"{PluginInfo.PLUGIN_GUID}_FoolhardyGambler", $"{PluginInfo.PLUGIN_GUID}_SaladRecipe"];
            RewardPickState reward = new()
            {
                seed = 1,
                id = Serviceable.TwitchService.GetUniqueTwitchId(),
                options = effects,
                isExtra = isExtra,
                viewConfiguration = viewConfiguration,
                date = new GameDate
                {
                    year = model.year,
                    season = model.season,
                    quarter = model.quarter
                }
            };

            __instance.Picks.Add(reward);
            __instance.OnPicksChanged.OnNext();
#endif
            return false;
        }

        [HarmonyPatch(typeof(MainController), nameof(MainController.OnServicesReady))]
        [HarmonyPostfix]
        private static void HookMainControllerSetup()
        {
            // This method will run after game load (Roughly on entering the main menu)
            // At this point a lot of the game's data will be available.
            // Your main entry point to access this data will be `Serviceable.Settings` or `MainController.Instance.Settings`
            Instance.Logger.LogInfo($"Performing game initialization on behalf of {PluginInfo.PLUGIN_GUID}.");
            Instance.Logger.LogInfo($"The game has loaded {MainController.Instance.Settings.effects.Length} effects.");
            /*
            BuildingModel shelterModel = MB.Settings.GetBuilding("Shelter");
            GoodRef woodRef = shelterModel.requiredGoods[0];
            woodRef.amount = 5;
            */
        }
        
        [HarmonyPatch(typeof(GameController), nameof(GameController.StartGame))]
        [HarmonyPostfix]
        private static void HookEveryGameStart()
        {
            // Too difficult to predict when GameController will exist and I can hook observers to it
            // So just use Harmony and save us all some time. This method will run after every game start
            var isNewGame = MB.GameSaveService.IsNewGame();
            Instance.Logger.LogInfo($"Entered a game. Is this a new game: {isNewGame}.");

            if (isNewGame)
            {   
                /*
                SO.EffectsService.GrantWildcardPick(1);
                Instance.Logger.LogInfo("New wildcard pick granted!");

                EffectModel resolveEffect = SO.Settings.GetEffect("Resolve for Glade");
                resolveEffect.AddAsPerk();
                Instance.Logger.LogInfo("Got the Resolve for Glade cornerstone.");
                */
            }
        }
        /*
        [HarmonyPatch(typeof(HookedEffectModel), nameof(HookedEffectModel.GetAmountText))]
        [HarmonyPostfix]
        private static void HookedEffectModel_GetAmountText_Postfix(ref string __result, HookedEffectModel __instance)
        {
            // If we are not using nested amounts, then it will use amount texts instead. No further handling needed.
            if (!__instance.hasNestedAmount) return;

            // If amount text has any value, then prefix it to the result.
            if (__instance.amountText != null)
            {
                __result = __instance.amountText + __result;
            }
        }
        */

    }
}
