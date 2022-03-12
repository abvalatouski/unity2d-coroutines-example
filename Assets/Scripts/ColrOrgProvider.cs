using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using UnityEngine;

public class ColrOrgProvider : IProvider<Color32>
{
    private readonly HttpClient httpClient;
    private readonly Uri uri;
    private readonly int numberOfRetries;

    public ColrOrgProvider()
    {
        httpClient = new HttpClient();
        uri = new Uri("https://www.colr.org/json/color/random");
        numberOfRetries = 5;
    }

    public async Task<Color32> ProvideAsync(CancellationToken cancellationToken)
    {
        for (var i = numberOfRetries; ; i--)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (numberOfRetries == 0)
            {
                throw new Exception("<color=#F00000>Bruh. The API is broken. (._.)</color>");
            }

            byte[] bytes = await Task.Run(() => httpClient.GetByteArrayAsync(uri), cancellationToken);
            using var jsonReader = new JsonTextReader(new StreamReader(new MemoryStream(bytes)));
            
            MoveToJsonHexValue(jsonReader);
            if (TryParseHexColor(jsonReader.ReadAsString(), out var color))
            {
                return color;
            }

            Debug.Log("Pending...");
        }
    }

    private void MoveToJsonHexValue(JsonReader jsonReader)
    {
        const int TokensToSkip = 7;
        for (var i = 0; i < TokensToSkip; i++)
        {
            jsonReader.Read();
        }
    }

    private bool TryParseHexColor(string hex, out Color32 color)
    {
        if (hex is null || hex.Length != 6)
        {
            color = default;
            return false;
        }

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
        color = new Color32(r, g, b, a);
        return true;
    }
}
