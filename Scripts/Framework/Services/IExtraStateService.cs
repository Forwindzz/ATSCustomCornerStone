using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Forwindz.Framework.Services
{
    public interface IExtraStateService
    {
        public T GetState<T>(string name);
        public T GetStateSafe<T>(string name);
        public void SaveAllStates();
        public UniTask LoadAllStates();

    }

    public enum SaveLoadType
    {
        Game,
        App,
        Meta,
        World,
        Unknown
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ModSerializedFieldAttribute : Attribute
    {
        public SaveLoadType SLType { get; }
        public string SaveName { get; }

        public ModSerializedFieldAttribute(SaveLoadType type = SaveLoadType.Unknown, string saveName = "")
        {
            SLType = type;
            SaveName = saveName;
        }
    }
}
