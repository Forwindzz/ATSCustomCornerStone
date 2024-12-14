using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Services
{
    public interface IDynamicHearthService
    {
        public void AddHearthRequirePopPercent(float v);
        public void AddHearthRequirePopCount(int v);
    }
}
