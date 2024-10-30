using ATS_API.Helpers;
using Eremite;
using Eremite.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Extend
{
    public static class GoodModelExt
    {
        public static bool ContainTag(this GoodModel goodModel, string name)
        {
            foreach(ModelTag tag in goodModel.tags)
            {
                if(tag.Name.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        public static GoodModel GetGoodModel(GoodsTypes goodType)
        {
            return MB.Settings.GetGood(goodType.ToName());
        }
    }
}
