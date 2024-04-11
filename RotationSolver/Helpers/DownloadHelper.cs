using ECommons.DalamudServices;
using RotationSolver.UI;
using XIVConfigUI;

namespace RotationSolver.Helpers;

public static class DownloadHelper
{
    /// <summary>
    /// <see href="https://garlandtools.org/db/#status/1093"><strong>HP and MP Boost</strong></see> ↑ (All Classes)
    /// <para>Maximum HP and MP are increased.</para>
    /// </summary>
    public static string[] LinkLibraries { get; private set; } = [];
    public static string[] UsersHash { get; private set; } = [];
    public static string[] Supporters { get; private set; } = [];
    public static IncompatiblePlugin[] IncompatiblePlugins { get; private set; } = [];

    public static async Task DownloadAsync()
    {
        LinkLibraries = await DownloadOneAsync<string[]>($"https://raw.githubusercontent.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/main/Resources/downloadList.json") ?? [];
        IncompatiblePlugins = await DownloadOneAsync<IncompatiblePlugin[]>($"https://raw.githubusercontent.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/main/Resources/IncompatiblePlugins.json") ?? [];

        DataCenter.ContributorsHash = await DownloadOneAsync<string[]>($"https://raw.githubusercontent.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/main/Resources/ContributorsHash.json") ?? [];

        UsersHash = await DownloadOneAsync<string[]>($"https://raw.githubusercontent.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/main/Resources/UsersHash.json") ?? [];

        Supporters = await DownloadOneAsync<string[]>($"https://raw.githubusercontent.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/main/Resources/Supporters.json") ?? [];
    }

    private static async Task<T?> DownloadOneAsync<T>(string url)
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
