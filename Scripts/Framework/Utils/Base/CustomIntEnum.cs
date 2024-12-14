using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Forwindz.Framework.Utils
{
    // C# do not support number constrain and Type inference :(
    // (The newest .Net support it, but we cannot move to newest .Net)
    // You can create other enum underlying types (like long, short, or other valid underlying type)
    // by manually modify the file: 
    // 1. Copy this code file and rename it
    // 2. modify `using EnumUnderlyingType = your_type`
    // 3. Rename CustomIntEnum<T> and CustomIntEnumManager<T> with your IDE, ex. CustomShortEnum<T>
    using EnumUnderlyingType = int;

    /// <summary>
    /// A single base element of customized enum object
    /// </summary>
    /// <typeparam name="T">The target enum class</typeparam>
    public struct CustomIntEnum<T>
        where T : Enum
    {
        public EnumUnderlyingType value;

        public CustomIntEnum(EnumUnderlyingType value)
        {
            this.value = value;
        }

        public static implicit operator T(CustomIntEnum<T> enumObj) => (T)Enum.ToObject(typeof(T), enumObj.value);
        public static implicit operator CustomIntEnum<T>(T enumObjOrg) => new CustomIntEnum<T>((EnumUnderlyingType)(object)enumObjOrg);

        public static T GetMaxOriginalEnum()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Max();
        }

        public static EnumUnderlyingType GetMaxOriginalEnumValue()
        {
            return (EnumUnderlyingType)(object)(Enum.GetValues(typeof(T)).Cast<T>().Max());
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class NewCustomEnumAttribute : Attribute
    {
        public string Name { get; }
        public object PredefinedValue { get; }

        public NewCustomEnumAttribute(string name = null, object predefinedValue = null)
        {
            Name = name;
            PredefinedValue = predefinedValue;
        }
    }

    public class CustomIntEnumManager<T>
        where T : Enum
    {
        private static Dictionary<string, CustomIntEnum<T>> nameEnumMapping = new();
        private static EnumUnderlyingType curMaxIndex = CustomIntEnum<T>.GetMaxOriginalEnumValue();
        private static Dictionary<EnumUnderlyingType, string> valueNameMapping = new();

        public static EnumUnderlyingType CurrentMaxIndex => curMaxIndex;
        public static IReadOnlyDictionary<string, CustomIntEnum<T>> NameEnumMapping => nameEnumMapping;
        public static IReadOnlyDictionary<EnumUnderlyingType, string> ValueNameMapping => valueNameMapping;

        public static CustomIntEnum<T> AddEnum(string name)
        {
            do
            {
                curMaxIndex++;
            }
            while (valueNameMapping.ContainsKey(curMaxIndex));
            CustomIntEnum<T> newEnum = new CustomIntEnum<T>((EnumUnderlyingType)curMaxIndex);
            nameEnumMapping.Add(name, newEnum);
            valueNameMapping.Add(newEnum.value, name);
            FLog.Info($"Add enum {name}={newEnum.value} to {typeof(T).Name}");
            return newEnum;
        }

        public static CustomIntEnum<T> AddPredefinedEnum(string name, EnumUnderlyingType value)
        {
            if (value < curMaxIndex) 
            {
                FLog.Error($"Try to add enum {name}={value} to {typeof(T).Name}, but the predefined value is already used. This may overwrite the previous enum!");
            }
            else if(valueNameMapping.ContainsKey(value))
            {
                FLog.Error($"Try to add enum {name}={value} to {typeof(T).Name}, but the predefined value is already used by other predefinitions. This will overwrite the previous enum!");
            }
            CustomIntEnum<T> newEnum = new CustomIntEnum<T>(value);
            nameEnumMapping.Add(name, newEnum);
            valueNameMapping.Add(newEnum.value, name);
            FLog.Info($"Add predefined enum {name}={newEnum.value} to {typeof(T).Name}");
            return newEnum;
        }

        public static void ScanAndAssignEnumValues<C>()
        {
            Type typeC = typeof(C);
            var fields = typeC.GetFields();
            FLog.Info($"{typeof(CustomIntEnum<T>).Name}: Scan enum values for {typeof(C).Name}, static fields count {fields.Length}");
            foreach (var field in fields)
            {
                NewCustomEnumAttribute attribute = field.GetCustomAttribute<NewCustomEnumAttribute>();
                if (attribute==null)
                {
                    continue;
                }
                string enumName = attribute.Name ?? field.Name;
                if(!field.IsInitOnly)
                {
                    FLog.Warning($"Enum field {typeC.Name}.{field.Name} is not readonly. New enum should be inmutable.");
                }
                FLog.Info($"Find enum field: {field.FieldType.Name} {field.Name}");
                Type fieldType = field.FieldType;
                // check type to avoid problem
                if (fieldType == typeof(CustomIntEnum<T>))
                {
                    if (attribute.PredefinedValue == null)
                    {
                        field.SetValue(null, AddEnum(enumName));
                    }
                    else
                    {
                        field.SetValue(null, AddPredefinedEnum(enumName, (EnumUnderlyingType)attribute.PredefinedValue));
                    }
                }
                else if (fieldType == typeof(T))
                {
                    if (attribute.PredefinedValue == null)
                    {
                        field.SetValue(null, (T)AddEnum(enumName));
                    }
                    else
                    {
                        field.SetValue(null, (T)AddPredefinedEnum(enumName, (EnumUnderlyingType)attribute.PredefinedValue));
                    }
                }
                else
                {
                    FLog.Error($"{typeC.Name}.{field.Name} has incorrect type, it should be CustomEnum<{typeof(T).Name},{typeC.Name}>");
                }
            }
        }

    }
}
