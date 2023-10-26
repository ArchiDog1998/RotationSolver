using Dalamud.Logging;
using ECommons.DalamudServices;

namespace RotationSolver.Localization;

internal class LocalizationManager : IDisposable
{
    public static Strings RightLang { get; private set; } = new Strings();

    private readonly Dictionary<string, Strings> _translations = new();
    public LocalizationManager()
    {
        SetLanguage(Svc.PluginInterface.UiLanguage);
        Svc.PluginInterface.LanguageChanged += OnLanguageChange;
#if DEBUG
        ExportLocalization();
#endif
    }

    private async void SetLanguage(string lang)
    {
        if (_translations.TryGetValue(lang, out var value))
        {
            RightLang = value;
        }
        else
        {
            try
            {
                var url = $"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/RotationSolver/Localization/{lang}.json";
                using var client = new HttpClient();
                RightLang = _translations[lang] = JsonConvert.DeserializeObject<Strings>(await client.GetStringAsync(url));
            }
            catch (HttpRequestException ex) when (ex?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Svc.Log.Information(ex, $"No language {lang}");
                RightLang = new Strings();
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, $"Failed to download the language {lang}");
                RightLang = new Strings();
            }
        }

        RotationSolverPlugin.ChangeUITranslation();
    }

#if DEBUG
    private static void ExportLocalization()
    {
        var directory = @"E:\OneDrive - stu.zafu.edu.cn\PartTime\FFXIV\RotationSolver\RotationSolver\Localization";
        if (!Directory.Exists(directory)) return;

        //Default values.
        var path = Path.Combine(directory, "Localization.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(new Strings(), Formatting.Indented));
    }
#endif

    public void Dispose()
    {
        Svc.PluginInterface.LanguageChanged -= OnLanguageChange;
    }

    private void OnLanguageChange(string languageCode)
    {
        try
        {
            Svc.Log.Information($"Loading Localization for {languageCode}");
            SetLanguage(languageCode);
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Unable to Load Localization");
        }
    }
}
