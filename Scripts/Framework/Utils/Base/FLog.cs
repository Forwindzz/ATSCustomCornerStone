using System;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Logging;


namespace Forwindz.Framework.Utils
{
    /// <summary>
    /// My Logger
    /// </summary>
    public static class FLog
    {
        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("f9");

        public static void Debug(params object[] objs)
        {
            logger.LogDebug(objs);
        }

        public static void Debug(object obj)
        {
            logger.LogDebug(obj);
        }

        public static void Info(params object[] objs)
        {
            logger.LogInfo(objs);
        }

        public static void Info(object obj)
        {
            logger.LogInfo(obj);
        }

        public static void Warning(params object[] objs)
        {
            logger.LogWarning(objs);
        }

        public static void Warning(object obj)
        {
            logger.LogWarning(obj);
        }

        public static void Error(params object[] objs)
        {
            logger.LogError(objs);
        }

        public static void Error(object obj)
        {
            logger.LogError(obj);
        }

        public static void ErrorAndThrow(object obj)
        {
            logger.LogError(obj);
            throw new Exception("Throw Exception:" + obj.ToString());
        }

        public static void Fatal(params object[] objs)
        {
            logger.LogFatal(objs);
        }

        public static void Fatal(object obj)
        {
            logger.LogFatal(obj);
        }

        public static void FatalAndThrow(object obj)
        {
            logger.LogFatal(obj);
            throw new Exception("Throw Exception:" + obj.ToString());
        }

        public static void Message(params object[] objs)
        {
            logger.LogMessage(objs);
        }

        public static void Message(object obj)
        {
            logger.LogMessage(obj);
        }

        public static void Assert(bool result, object message)
        {
            if(!result)
            {
                logger.LogError("Assert failed!");
                logger.LogError(message);
                throw new Exception("Assert Failed :" + message.ToString());
            }
        }

        public static void AssertNotNull<T>(T obj) where T : class
        {
            if (obj == null) 
            {
                logger.LogError($"Not null Assert failed! {typeof(T).FullName}");
                throw new Exception($"Not null Assert Failed for {typeof(T).FullName}");
            }
        }

        public static void AssertType<T>(object obj)
        {
            if (!obj.GetType().Equals(typeof(T)))
            {
                logger.LogError($"Type Assert failed! Get <{obj.GetType().FullName}> Expect <{typeof(T).FullName}>");
                string info = obj?.ToString();
                if(info==null) info = "null";
                throw new Exception("Type Assert Failed :" + info);
            }
        }
    }
}
