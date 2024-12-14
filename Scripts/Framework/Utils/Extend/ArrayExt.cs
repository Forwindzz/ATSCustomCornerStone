using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Utils.Extend
{
    /// <summary>
    /// Operate Array like List
    /// This will cause lots of copy operations,
    /// Do not use them frequently!
    /// </summary>
    public static class ArrayExt
    {
        public static T[] ForceAdd<T>(this T[] source, T item)
        {
            T[] result = new T[source.Length + 1];
            source.CopyTo(result, 0);
            result[result.Length-1] = item; 
            return result;
        }

        public static void ForceAdd<T>(ref T[] source, T item)
        {
            source = source.ForceAdd(item);
        }

        public static T[] ForceRemove<T>(this T[] source, T item)
        {
            int index = Array.IndexOf(source, item);
            if (index < 0) return source;
            T[] result = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, result, 0, index);
            if (index < source.Length - 1)
                Array.Copy(source, index + 1, result, index, source.Length - index - 1);
            return result;
        }

        public static void ForceRemove<T>(ref T[] source, T item)
        {
            source = source.ForceRemove(item);
        }
    }
}
