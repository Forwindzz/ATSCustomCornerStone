using System;
using System.Runtime.CompilerServices;

namespace Forwindz.Framework.Utils
{
    /// <summary>
    /// To add a customized field:
    /// static CustomField<ClassToBeInjected, String> extraStringField = new(()=>"initString");
    /// This class is not tested yet :(
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    /// <typeparam name="FieldType"></typeparam>
    public class CustomField<ObjectType,FieldType> 
        where FieldType : class
        where ObjectType : class
    {
        private readonly ConditionalWeakTable<ObjectType, FieldType> customFields = new();
        private readonly Func<ObjectType, FieldType> initFieldFunc = (inst) => null;

        public FieldType GetField(ObjectType instance)
        {
            if (customFields.TryGetValue(instance, out FieldType field))
            {
                return field;
            }
            else
            {
                throw new Exception(
                    $"Try to get field {typeof(FieldType).FullName} from {typeof(ObjectType).FullName}, but instance {instance.GetHashCode()} haven't created field");
                //return default;
            }
        }

        public void SetField(ObjectType instance, FieldType fieldContent)
        {
            customFields.Add(instance, fieldContent);
        }

        /// <summary>
        /// This might invoked several times if the constructor invoke each other
        /// </summary>
        /// <param name="instance"></param>
        public void CreateField(ObjectType instance)
        {
            customFields.Add(instance, initFieldFunc(instance));
        }

        public void RemoveField(ObjectType instance)
        {
            customFields.Remove(instance);
        }

        public CustomField(Func<ObjectType, FieldType> initFieldFunc)
        {
            this.initFieldFunc = initFieldFunc;
            PatchUtils.InjectConstructorPostPatch<ObjectType>(CreateField);
        }

    }
}
