using Dalamud.Logging;
using ECommons.DalamudServices;
using RotationSolver.UI;
using System.Text;

namespace RotationSolver.Helpers;

public static class DownloadHelper
{
    public static string[] LinkLibraries { get; private set; } = Array.Empty<string>();
    public static string[] ContributorsHash { get; private set; } = Array.Empty<string>();
    public static string[] UsersHash { get; private set; } = Array.Empty<string>();
    public static string[] Supporters { get; private set; } = Array.Empty<string>();
    public static IncompatiblePlugin[] IncompatiblePlugins { get; private set; } = Array.Empty<IncompatiblePlugin>();

    public static async Task DownloadAsync()
    {
        LinkLibraries = await DownloadOneAsync<string[]>($"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Resources/downloadList.json") ?? Array.Empty<string>();
        IncompatiblePlugins = await DownloadOneAsync<IncompatiblePlugin[]>($"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Resources/IncompatiblePlugins.json") ?? Array.Empty<IncompatiblePlugin>();

        ContributorsHash = await DownloadOneAsync<string[]>($"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Resources/ContributorsHash.json") ?? Array.Empty<string>();

        UsersHash = await DownloadOneAsync<string[]>($"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Resources/UsersHash.json") ?? Array.Empty<string>();

        Supporters = await DownloadOneAsync<string[]>($"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Resources/Supporters.json") ?? Array.Empty<string>();
    }

    private static async Task<T> DownloadOneAsync<T>(string url)
    {
        using var client = new HttpClient();
        try
        {
            var str = await client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<T>(str);
        }
        catch (Exception ex)
        {
            Svc.Log.Information(ex, "Failed to load downloading List.");
            return default;
        }
    }
}
