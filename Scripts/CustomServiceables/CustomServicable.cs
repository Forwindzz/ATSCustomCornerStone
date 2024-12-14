using Eremite.Buildings;
using Forwindz.Framework.Utils;
using System;
using UniRx;

namespace Forwindz.Content.CustomServiceables
{
    /*
    public static class CustomServicable
    {
        public static int buildingsCompleted = 0;
        public static int decorationBuildingsCompleted = 0;
        public static int roadsCompleted = 0;
        public static readonly Subject<Building> buildingConstructionFinishedSubject = new Subject<Building>();

        public static IObservable<Building> OnBuildingConstructionFinished
        {
            get
            {
                return buildingConstructionFinishedSubject;
            }
        }

        public static void OnBuildingCompleted(Building building) 
        {
            buildingsCompleted++;

            if (Utils.IsDecorationBuilding(building))
            {
                decorationBuildingsCompleted++;
            }

            if (Utils.IsRoad(building))
            {
                roadsCompleted++;
            }
            
            buildingConstructionFinishedSubject.OnNext(building);
        }
    }
    */
}
