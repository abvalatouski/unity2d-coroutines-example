using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using UnityEngine;

public class ColorProvider : IProvider<Color32>
{
    private readonly HttpClient httpClient;
    private readonly Uri uri;
    private readonly JsonSerializer jsonSerializer;
    private readonly JsonConverter jsonConverter;

    public ColorProvider()
    {
        httpClient = new HttpClient();
        uri = new Uri("https://www.colr.org/json/color/random");
        jsonSerializer = new JsonSerializer();
        jsonConverter = new ColorJsonConverter();
    }

    public async Task<Color32> ProvideAsync(CancellationToken cancellationToken)
    {
        byte[] bytes = await Task.Run(() => httpClient.GetByteArrayAsync(uri), cancellationToken);
        using (var jsonReader = new JsonTextReader(new StreamReader(new MemoryStream(bytes))))
        {
            var color = (Color32)jsonConverter.ReadJson(jsonReader, typeof(Color32), null, jsonSerializer);
            return color;
        }
    }
}
