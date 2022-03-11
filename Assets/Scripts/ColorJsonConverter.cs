using System;

using Newtonsoft.Json;
using UnityEngine;

public class ColorJsonConverter : JsonConverter<Color32>
{
    public override Color32 ReadJson(
        JsonReader reader,
        Type objectType,
        Color32 existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        try
        {
            const int TokensToRead = 7;
            for (var i = 0; i < TokensToRead; i++)
            {
                reader.Read();
            }

            var hex = reader.ReadAsString();
            var r1 = hex[0] - '0';
            var r2 = hex[1] - '0';
            var g1 = hex[2] - '0';
            var g2 = hex[3] - '0';
            var b1 = hex[4] - '0';
            var b2 = hex[5] - '0';
            
            var r = (byte)(r1 * 16 + r2);
            var g = (byte)(g1 * 16 + g2);
            var b = (byte)(b1 * 16 + b2);
            var a = (byte)255;

            return hasExistingValue
                ? existingValue
                : new Color32(r, g, b, a);
        }
        catch (Exception)
        {
            throw new Exception(
                "Can't parse data from the API. Ignore that! :P");
        }
    }

    public override void WriteJson(
        JsonWriter writer,
        Color32 value,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
