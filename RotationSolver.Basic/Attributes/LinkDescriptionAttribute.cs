using Dalamud.Utility;
using System.Net;
using System;
using ImGuiScene;
using Dalamud.Interface.Internal;

namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class LinkDescriptionAttribute : Attribute
{
    public TextureWrap Texture { get; private set; }
    public string Path { get; }
    public LinkDescriptionAttribute(string path)
    {
        Path = path;
        LoadTexture(path);
    }

    private void LoadTexture(string path)
    {
        try
        {
            Task.Run(async () =>
            {
                var bytes = await LoadBytes(path);
                Texture = TryLoadImage(bytes);
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
        return Service.Interface.UiBuilder.LoadImage(bytes);
    }
}
