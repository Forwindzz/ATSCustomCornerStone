using Eremite;
using Eremite.Buildings;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Scripts.Framework.Utils
{
    public static class BuildingHelper
    {
        public static int CountDecorationValue(DecorationTier tier)
        {
            int num = 0;
            foreach (Decoration decoration in GameMB.BuildingsService.Decorations.Values)
            {
                if (decoration.IsFinished() && decoration.model.hasDecorationTier && decoration.model.tier == tier)
                {
                    num += decoration.model.decorationScore;
                }
            }
            return num;
        }

        public static int CountProductionBuildingArea()
        {
            int num = 0;
            foreach (ProductionBuilding building in GameMB.BuildingsService.ProductionBuildings)
            {
                if (building.IsFinished())
                {
                    Vector2Int size = building.Size;
                    num += size.x * size.y;
                }
            }
            return num;
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
