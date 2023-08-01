using Dalamud.Logging;
using RotationSolver.UI;
using System.Text;

namespace RotationSolver.Helpers;

public static class DownloadHelper
{
    public static string[] LinkLibraries { get; private set; }
    public static IncompatiblePlugin[] IncompatiblePlugins { get; private set; }

    public static async Task DownloadAsync()
    {
        LinkLibraries = await DownloadOneAsync<string[]>("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Resources/downloadList.json");
        IncompatiblePlugins = await DownloadOneAsync<IncompatiblePlugin[]>("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Resources/IncompatiblePlugins.json");
    }

    private static async Task<T> DownloadOneAsync<T>(string url)
    {
        using var client = new HttpClient();
        try
        {
            var bts = await client.GetByteArrayAsync(url);
            return JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(bts));
        }
        catch (Exception ex)
        {
            PluginLog.Log(ex, "Failed to load downloading List.");
            return default;
        }
    }
}
