using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Utils
{
    /*
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            float x = obj["x"].ToObject<float>();
            float y = obj["y"].ToObject<float>();
            return new Vector2(x, y);
        }
    }

    public class SerializeAdjust
    {
        static SerializeAdjust()
        {
            if(JsonConvert.DefaultSettings==null)
            {
                FLog.Info("Add customized JsonConvert.DefaultSettings");
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new Vector2Converter() }
                };
                return;
            }
            else
            {
                FLog.Info("Wrap existing customized JsonConvert.DefaultSettings:");
                Func<JsonSerializerSettings> existsSettings = JsonConvert.DefaultSettings;
                //wrap it!
                JsonConvert.DefaultSettings = () =>
                {
                    JsonSerializerSettings settings = existsSettings();
                    CheckAndAssignConverter<Vector2>(settings, new Vector2Converter());
                    return settings;
                };
            }
        }

        internal static void CheckAndAssignConverter<T>(JsonSerializerSettings settings, JsonConverter newConverter)
        {
            if (settings.Converters==null)
            {
                settings.Converters = new List<JsonConverter>();
            }
            foreach (var converter in settings.Converters)
            {
                if(converter.CanConvert(typeof(T)))
                {
                    FLog.Warning($"Already exist converter {typeof(Convert).FullName} for {typeof(T).FullName}");
                    return;
                }
            }
            settings.Converters.Add(newConverter);
            FLog.Info($"Add converter {typeof(Convert).FullName} for {typeof(T).FullName}");
        }
    }
    */
}
