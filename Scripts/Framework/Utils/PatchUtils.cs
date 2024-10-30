using Eremite.Services;
using Forwindz.Content;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Forwindz.Framework.Utils
{
    public static class PatchUtils
    {
        public static void InjectConstructorPostPatch(Type injectClassType, MethodInfo methodInfo)
        {
            var constructors = injectClassType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var constructor in constructors)
            {
                PatchesManager.harmony.Patch(
                    constructor, postfix: new HarmonyMethod(methodInfo));
            }
        }

        public static void InjectConstructorPostPatch<T>(Action<T> action)
        {
            InjectConstructorPostPatch(typeof(T), ReflectUtils.GetMethodInfo(action));
        }
    }

    public static class ReflectUtils
    {
        public static MethodInfo GetMethodInfo(Action action) => action.Method;
        public static MethodInfo GetMethodInfo<T>(Action<T> action) => action.Method;
        public static MethodInfo GetMethodInfo<T, U>(Action<T, U> action) => action.Method;
        public static MethodInfo GetMethodInfo<TResult>(Func<TResult> fun) => fun.Method;
        public static MethodInfo GetMethodInfo<T, TResult>(Func<T, TResult> fun) => fun.Method;
        public static MethodInfo GetMethodInfo<T, U, TResult>(Func<T, U, TResult> fun) => fun.Method;

        public static MethodInfo GetMethodInfo(Delegate del) => del.Method;

        public static List<Type> GetAllBaseTypes(Type type)
        {
            List<Type> baseTypes = new List<Type>();
            Type currentType = type.BaseType;

            while (currentType != null)
            {
                baseTypes.Add(currentType);
                currentType = currentType.BaseType;
            }

            return baseTypes;
        }

        public static List<Type> GetAllBaseTypesAndInterfaces(Type type)
        {
            List<Type> types = new List<Type>();
            Type currentType = type.BaseType;

            while (currentType != null)
            {
                types.Add(currentType);
                currentType = currentType.BaseType;
            }

            types.AddRange(type.GetInterfaces());

            return types;
        }

        public static void InitializeAllStatics(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.TypeInitializer != null)
                {
                    //force to run static initializer
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
            }
        }

        public static ConstructorInfo GetNonParameterConstructor(Type type)
        {
            ConstructorInfo[] ctors = 
                type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            FLog.Info($"Try to search non parameter constructor {type.Name}");
            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] info = ctor.GetParameters();
                Type declaringType = ctor.DeclaringType;
                FLog.Info($"Detect {ctor.Name} > {info} > {declaringType.Name} > Private {ctor.IsPrivate}");
                if ((info == null || info.Length == 0) && !ctor.IsStatic && declaringType.Equals(type)) 
                {
                    return ctor;
                }
            }
            return null;
        }
    }
}
