using ATS_API.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Utils
{
    public static class GUIDManagerExtend
    {
        public static T Get<T>(string guid, string name)
        {
            return GUIDManager.Get<T>(PluginInfo.PLUGIN_GUID, $"{typeof(T).FullName}.{name}");
        }
    }
}
