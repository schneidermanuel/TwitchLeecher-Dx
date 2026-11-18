using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace TwitchLeecher.Core;

public class ImageHelper
{
    // shared: a client per thumbnail opened a new connection (and socket) for every search result
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<Bitmap> LoadFromWeb(Uri url)
    {
        try
        {
            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }
}