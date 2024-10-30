using Eremite;
using Eremite.Buildings;
using Eremite.Model;
using Eremite.WorldMap;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Forwindz.Framework.Utils
{
    public static class Utils
    {
        public static string GetMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder hashString = new StringBuilder();

                foreach (byte b in hashBytes)
                {
                    hashString.Append(b.ToString("x2")); // 转为16进制字符串
                }

                return hashString.ToString();
            }
        }

        public static T GetValue<T>(this WeakReference<T> instance) where T : class
        {
            if(instance.TryGetTarget(out T value))
            {
                return value;
            }
            else
            {
                return null;    
            }
        }

        public static void SetSeasonsTime(float drizzleTime, float clearanceTime, float stormTime, SeasonQuarter firstCornerstoneTime = SeasonQuarter.Second)
        {
            foreach (BiomeModel biome in MB.Settings.biomes)
            {
                biome.seasons.DrizzleTime = drizzleTime;
                biome.seasons.ClearanceTime = clearanceTime;
                biome.seasons.StormTime = stormTime;
                biome.seasons.SeasonRewards[0].quarter = firstCornerstoneTime;
            }
        }
        public static string GetGoodIconAndName(GoodModel good)
        {

            string goodName = good.Name;
            int indexOfClosingParentheses = goodName.LastIndexOf("]");
            if (indexOfClosingParentheses != -1)
            {
                goodName = goodName.Substring(indexOfClosingParentheses + 1);
            }
            else if (goodName.Contains("_Meta"))
            {
                goodName = goodName.Substring("_Meta".Length + 1);
            }

            goodName = goodName.Trim();

            return $"<sprite name=\"{good.Name.ToLowerInvariant()}\"> {goodName}";
        }

        public static bool IsDecorationBuilding(Building building)
        {
            string buildingCategory = building.BuildingModel.category.name;
            return buildingCategory.Equals("Decorations");
        }

        public static bool IsRoad(Building building)
        {
            string buildingCategory = building.BuildingModel.category.name;
            return buildingCategory.Equals("Roads");
        }
    }
}
