using System.Collections.Generic;

namespace Forwindz.Content
{
    public static class ProfessionExtension
    {
        internal static readonly Dictionary<Professions, string> TypeToInternalName = new()
        {
            {
                    Professions.BlightFighter,
                    "BlightFighter"
            },

            {
                    Professions.Builder,
                    "Builder"
            },

            {
                    Professions.Farmer,
                    "Farmer"
            },

            {
                    Professions.FireKeeper,
                    "FireKeeper"
            },

            {
                    Professions.Forager,
                    "Forager"
            },

            {
                    Professions.Geologist,
                    "Geologist"
            },

            {
                    Professions.Harvester,
                    "Harvester"
            },

            {
                    Professions.Herbalist,
                    "Herbalist"
            },

            {
                    Professions.Miner,
                    "Miner"
            },

            {
                    Professions.RainCatcherWorker,
                    "Rain Catcher Worker"
            },

            {
                    Professions.Scavenger,
                    "Scavenger"
            },

            {
                    Professions.Scout,
                    "Scout"
            },

            {
                    Professions.Stonecutter,
                    "Stonecutter"
            },

            {
                    Professions.Trapper,
                    "Trapper"
            },

            {
                    Professions.Woodcutter,
                    "Woodcutter"
            },

            {
                    Professions.BathHouseWorker,
                    "Bath House worker"
            },

            {
                    Professions.ClanMamber,
                    "Clan Mamber"
            },

            {
                    Professions.Explorer,
                    "Explorer"
            },

            {
                    Professions.Guildmember,
                    "Guild member"
            },

            {
                    Professions.Librarian,
                    "Librarian"
            },

            {
                    Professions.Monk,
                    "Monk"
            },

            {
                    Professions.Priest,
                    "Priest"
            },

            {
                    Professions.Seller,
                    "Seller"
            },

            {
                    Professions.Speaker,
                    "Speaker"
            },

            {
                    Professions.Teadoctor,
                    "Teadoctor"
            },

            {
                    Professions.Waiter,
                    "Waiter"
            },

            {
                    Professions.ClaypitDigger,
                    "Claypit Digger"
            },

            {
                    Professions.GreenhouseWorker,
                    "Greenhouse Worker"
            },

            {
                    Professions.RainCollectorWorker,
                    "Rain Collector Worker"
            },

            {
                    Professions.Rancher,
                    "Rancher"
            },

            {
                    Professions.Alchemist,
                    "Alchemist"
            },

            {
                    Professions.Apothecary,
                    "Apothecary"
            },

            {
                    Professions.Artisan,
                    "Artisan"
            },

            {
                    Professions.Baker,
                    "Baker"
            },

            {
                    Professions.BeaneryWorker,
                    "Beanery Worker"
            },

            {
                    Professions.BreweryWorker,
                    "Brewery Worker"
            },

            {
                    Professions.BrickyardWorker,
                    "Brickyard Worker"
            },

            {
                    Professions.Butcher,
                    "Butcher"
            },

            {
                    Professions.Carpenter,
                    "Carpenter"
            },

            {
                    Professions.Cellarworker,
                    "Cellar worker"
            },

            {
                    Professions.ClayPitWorkshopWorker,
                    "Clay Pit Workshop Worker"
            },

            {
                    Professions.Cook,
                    "Cook"
            },

            {
                    Professions.Cooper,
                    "Cooper"
            },

            {
                    Professions.Craftsman,
                    "Craftsman"
            },

            {
                    Professions.DistilleryWorker,
                    "Distillery Worker"
            },

            {
                    Professions.Druid,
                    "Druid"
            },

            {
                    Professions.FactoryWorker,
                    "Factory Worker"
            },

            {
                    Professions.Finesmith,
                    "Finesmith"
            },

            {
                    Professions.FurnaceWorker,
                    "Furnace worker"
            },

            {
                    Professions.GranaryWorker,
                    "Granary worker"
            },

            {
                    Professions.GreenhouseWorkshopWorker,
                    "Greenhouse Workshop Worker"
            },

            {
                    Professions.GrillWorker,
                    "Grill Worker"
            },

            {
                    Professions.Hauler,
                    "Hauler"
            },

            {
                    Professions.KilnWorker,
                    "Kiln worker"
            },

            {
                    Professions.Leatherworker,
                    "Leatherworker"
            },

            {
                    Professions.LumbermillWorker,
                    "Lumbermill worker"
            },

            {
                    Professions.Manufacturer,
                    "Manufacturer"
            },

            {
                    Professions.MillWorker,
                    "Mill worker"
            },

            {
                    Professions.Outfitter,
                    "Outfitter"
            },

            {
                    Professions.OvenWorker,
                    "Oven worker"
            },

            {
                    Professions.PantryWorker,
                    "Pantry worker"
            },

            {
                    Professions.PressWorker,
                    "Press Worker"
            },

            {
                    Professions.Provisioner,
                    "Provisioner"
            },

            {
                    Professions.Scribe,
                    "Scribe"
            },

            {
                    Professions.Sewer,
                    "Sewer"
            },

            {
                    Professions.Smelter,
                    "Smelter"
            },

            {
                    Professions.Smith,
                    "Smith"
            },

            {
                    Professions.SmokehouseWorker,
                    "Smokehouse worker"
            },

            {
                    Professions.StampingMillWorker,
                    "Stamping Mill Worker"
            },

            {
                    Professions.Supplier,
                    "Supplier"
            },

            {
                    Professions.TeahouseWorker,
                    "Teahouse Worker"
            },

            {
                    Professions.TincturyWorker,
                    "Tinctury Worker"
            },

            {
                    Professions.Tinkerer,
                    "Tinkerer"
            },

            {
                    Professions.ToolshopWorker,
                    "Toolshop Worker"
            },

            {
                    Professions.Weaver,
                    "Weaver"
            },
        };

        public static string ToName(this Professions type)
        {
            if (TypeToInternalName.TryGetValue(type, out var value))
            {
                return value;
            }

            UnityEngine.Debug.LogError("Cannot find name of type: " + type);
            return Professions.Alchemist.ToString();
        }
    }
}
