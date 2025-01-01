using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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

        public static void Debug(object obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.LogDebug(ProcessMetaInfo(memberName, filePath, lineNumber) + obj);
        }

        public static void Info(object obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.LogInfo(ProcessMetaInfo(memberName, filePath, lineNumber) + obj);
        }

        public static void Warning(object obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.LogWarning(ProcessMetaInfo(memberName, filePath, lineNumber) + obj);
        }

        public static void Error(
            object obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.LogError(ProcessMetaInfo(memberName, filePath, lineNumber) + obj);
        }

        public static void ErrorAndThrow(object obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.LogError(ProcessMetaInfo(memberName, filePath, lineNumber) + obj);
            throw new Exception("Throw Exception:" + obj.ToString());
        }

        public static void Fatal(
            object obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.LogFatal(ProcessMetaInfo(memberName, filePath, lineNumber) + obj);
        }

        public static void FatalAndThrow(object obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.LogFatal(ProcessMetaInfo(memberName, filePath, lineNumber) + obj);
            throw new Exception("Throw Exception:" + obj.ToString());
        }

        public static void Message(object obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            logger.LogMessage(ProcessMetaInfo(memberName, filePath, lineNumber)+obj);
        }

        public static void Assert(bool result, object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if(!result)
            {
                logger.LogError($"{ProcessMetaInfo(memberName, filePath, lineNumber)}Assert failed!");
                logger.LogError(message);
                throw new Exception("Assert Failed :" + message.ToString());
            }
        }

        public static void AssertNotNull<T>(T obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0) where T : class
        {
            if (obj == null)
            {
                logger.LogError($"{ProcessMetaInfo(memberName, filePath, lineNumber)}Not null Assert failed! {typeof(T).FullName}");
                throw new Exception($"Not null Assert Failed for {typeof(T).FullName}");
            }
        }

        public static void AssertType<T>(object obj,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!obj.GetType().Equals(typeof(T)))
            {
                logger.LogError($"{ProcessMetaInfo(memberName, filePath, lineNumber)} Type Assert failed! Get <{obj.GetType().FullName}> Expect <{typeof(T).FullName}>");
                string info = obj?.ToString();
                if(info==null) info = "null";
                throw new Exception("Type Assert Failed :" + info);
            }
        }

        private static string ProcessMetaInfo(
            string memberName, string filePath, int lineNumber)
        {
            return $"[@{Path.GetFileNameWithoutExtension(filePath)}.{memberName}:{lineNumber}] ";
        }
    }
}
