using ATS_API.Localization;
using BepInEx;
using BepInEx.Logging;
using Eremite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Utils
{
    public class LocalizationLoader
    {
        /// <summary>
        /// Load localization files and add them to the game
        /// </summary>
        /// <param name="folderPath">The localization folder</param>
        public static void LoadLocalization(string folderPath = "lang")
        {
            string folderFullPath = Path.Combine(Paths.PluginPath, folderPath);
            try
            {
                string[] filePaths = Directory.GetFiles(folderFullPath, "*.json", SearchOption.AllDirectories);

                foreach (string filePath in filePaths) 
                {
                    try
                    {
                        string content = File.ReadAllText(filePath);
                        Dictionary<string, string> data = JSON.FromJson<Dictionary<string, string>>(content);
                        string baseName = Path.GetFileNameWithoutExtension(filePath);
                        SystemLanguage languageCode = LocalizationManager.CodeToLanguage(baseName);
                        foreach (KeyValuePair<string, string> entry in data)
                        {
                            LocalizationManager.AddString(entry.Key, entry.Value, languageCode);
                            Log.Info($"{languageCode} [{entry.Key}]:{entry.Value}");
                        }
                        Log.Info($"Load {data.Count} entries for {languageCode} language, from {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error while processing localization file: {filePath}");
                        Log.Error(ex);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
