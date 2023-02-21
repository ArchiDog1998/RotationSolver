using Dalamud.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RotationSolver.Localization;

internal class LocalizationManager : IDisposable
{
    public static Strings RightLang { get; private set; } = new Strings();

    private readonly Dictionary<string, Strings> _translations = new Dictionary<string, Strings>();
    public LocalizationManager()
    {
        var assembly = Assembly.GetCallingAssembly();

        foreach (var lang in Dalamud.Localization.ApplicableLangCodes)
        {
            ReadFile(lang, assembly);
        }

        SetLanguage(Service.Interface.UiLanguage);
        Service.Interface.LanguageChanged += OnLanguageChange;
    }

    private void ReadFile(string lang, Assembly assembly)
    {
        Stream manifestResourceStream = assembly.GetManifestResourceStream("RotationSolver.Localization." + lang + ".json");
        if (manifestResourceStream == null) return;
        using StreamReader streamReader = new StreamReader(manifestResourceStream);
        _translations[lang] = JsonConvert.DeserializeObject<Strings>(streamReader.ReadToEnd());
#if DEBUG
        Service.ChatGui.Print($"Load {lang} succeessfully!");
#endif
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
    public void ExportLocalization()
    {
        var directory = @"D:\OneDrive - stu.zafu.edu.cn\PartTime\FFXIV\RotationSolver\RotationSolver\Localization";
        if (!Directory.Exists(directory)) return;

        //Default values.
        var path = Path.Combine(directory, "Localization.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(new Strings(), Formatting.Indented));
    }
#endif

    public void Dispose()
    {
        Service.Interface.LanguageChanged -= OnLanguageChange;
    }

    private void OnLanguageChange(string languageCode)
    {
        try
        {
            PluginLog.Information($"Loading Localization for {languageCode}");
            SetLanguage(languageCode);
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to Load Localization");
        }
    }
}
