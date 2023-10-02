using Dalamud.Logging;
using ECommons.DalamudServices;

namespace RotationSolver.Localization;

internal class LocalizationManager : IDisposable
{
    public static Strings RightLang { get; private set; } = new Strings();

    private readonly Dictionary<string, Strings> _translations = new();
    public LocalizationManager()
    {
        var assembly = Assembly.GetCallingAssembly();

        foreach (var lang in Dalamud.Localization.ApplicableLangCodes)
        {
            ReadFile(lang, assembly);
        }

        SetLanguage(Svc.PluginInterface.UiLanguage);
        Svc.PluginInterface.LanguageChanged += OnLanguageChange;
    }

    private void ReadFile(string lang, Assembly assembly)
    {
        Stream manifestResourceStream = assembly.GetManifestResourceStream("RotationSolver.Localization." + lang + ".json");
        if (manifestResourceStream == null) return;
        using StreamReader streamReader = new(manifestResourceStream);
        _translations[lang] = JsonConvert.DeserializeObject<Strings>(streamReader.ReadToEnd());
    }

    private void SetLanguage(string lang)
    {
        if (_translations.TryGetValue(lang, out var value))
        {
            RightLang = value;
        }
        else
        {
            RightLang = new Strings();
        }

        RotationSolverPlugin.ChangeUITranslation();
    }

#if DEBUG
    public static void ExportLocalization()
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
