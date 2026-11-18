using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace TwitchLeecher.Core;

public class ImageHelper
{
    // shortcut: fixed limit of 4 parallel thumbnail downloads, make configurable if slow links need it
    private static readonly SemaphoreSlim Gate = new SemaphoreSlim(4);
    // shared: a client per thumbnail opened a new connection (and socket) for every search result
    private static readonly HttpClient Client = new HttpClient();

    public static async Task<Bitmap> LoadFromWeb(Uri url)
    {
        await Gate.WaitAsync();
        try
        {
            using var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
        finally
        {
            Gate.Release();
        }
    }
}