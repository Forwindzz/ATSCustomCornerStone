using Eremite;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Utils.Extend
{
    public static class SOExtend
    {
        public static List<T> DeepClone<T>(this List<T> objs) where T : SO
        {
            List<T> result = new List<T>();
            result.Capacity = objs.Count;
            for (int i = 0; i < objs.Count; i++)
            {
                result.Add(objs[i].DeepClone<T>());
            }
            return result;
        }

        public static T[] DeepClone<T>(this T[] objs) where T : SO
        {
            T[] result = new T[objs.Length];
            for (int i = 0;i<objs.Length;i++)
            {
                result[i] = objs[i].DeepClone<T>();
            }
            return result;
        }

        public static T Clone<T>(this T original) where T : SO
        {
            if (original == null)
            {
                return null;
            }
            T copy = UnityEngine.Object.Instantiate(original);
            return copy;
        }

        public static SO DeepClone(this SO original)
        {
            if (original == null)
            {
                return null;
            }
            SO copy = UnityEngine.Object.Instantiate(original);
            SODeepCopyFields(original, copy);

            return copy;
        }

        public static T DeepClone<T>(this SO original) where T : SO
        {
            return (T)DeepClone(original);
        }

        private static void SODeepCopyFields(object source, object destination)
        {
            var fields = source.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (field.IsNotSerialized || 
                    !field.FieldType.IsSubclassOf(typeof(SO))
                    )
                {
                    continue;
                }
                SO orgValue = (SO)field.GetValue(source);
                SO copyValue = orgValue != null ? orgValue.DeepClone() : null;
                field.SetValue(destination, copyValue);

                /*
            // ref type
            if (value != null && !field.FieldType.IsValueType && field.FieldType != typeof(string))
            {
                object deepCopyValue;

                if (field.FieldType.IsSubclassOf(typeof(SO)))
                {
                    var scriptableObjectValue = value as SO;
                    deepCopyValue = scriptableObjectValue != null ? scriptableObjectValue.DeepClone() : null;

                }
                else if (field.FieldType.IsArray)
                {
                    Array orgArray = (Array)value;
                    Array copyArray = Array.CreateInstance(field.FieldType, orgArray.Length);
                    deepCopyValue = copyArray;
                    SODeepCopyFields(orgArray, deepCopyValue);
                }
                else
                {
                    deepCopyValue = Activator.CreateInstance(field.FieldType);
                    SODeepCopyFields(value, deepCopyValue);
                }

                field.SetValue(destination, deepCopyValue);
                
            }
                else // value type
                {
                    field.SetValue(destination, value);
                }*/
            }
        }

    }
}
