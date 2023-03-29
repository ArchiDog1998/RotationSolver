using Dalamud.Utility;
using System.Net;
using System;
using ImGuiScene;
using Dalamud.Interface.Internal;

namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class LinkDescriptionAttribute : Attribute
{
    static readonly SortedDictionary<string, TextureWrap> _TextureCache = new SortedDictionary<string, TextureWrap>();

    static readonly List<string> _loadingTexture = new List<string>();

    public TextureWrap Texture { get; private set; }
    public string Description { get; private set; }
    public string Path { get; }
    public LinkDescriptionAttribute(string path, string description = "")
    {
        Path = path;
        LoadTexture(path);
        Description = description;
    }

    private void LoadTexture(string path)
    {
        if(_TextureCache.TryGetValue(path,out var t))
        {
            Texture = t;
            return;
        }
        if (_loadingTexture.Contains(path))
        {
            return;
        }
        _loadingTexture.Add(path);

        try
        {
            Task.Run(async () =>
            {
                var bytes = await LoadBytes(path);
                _TextureCache[path] = Texture = TryLoadImage(bytes);
            });
        }
        catch
        {
        }
    }

    private static async Task<byte[]> LoadBytes(string url)
    {
        var data = await Util.HttpClient.GetAsync(url);
        if (data.StatusCode == HttpStatusCode.NotFound)
            return null;

        data.EnsureSuccessStatusCode();
        return await data.Content.ReadAsByteArrayAsync();
    }

    private static TextureWrap TryLoadImage(byte[] bytes)
    {
        if (bytes == null)
            return null;

        try
        {
            return Service.Interface.UiBuilder.LoadImage(bytes);
        }
        catch
        {
            return null;
        }
    }
}
