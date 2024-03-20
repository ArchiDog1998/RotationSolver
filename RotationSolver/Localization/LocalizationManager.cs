using Dalamud.Utility;
using ECommons.DalamudServices;
using System.ComponentModel;

namespace RotationSolver.Localization;

internal static class LocalizationManager
{
    private static Dictionary<string, string> _rightLang = [];

    private static readonly Dictionary<string, Dictionary<string, string>> _translations = [];
    public static string Local(this Enum @enum)
    {
        var key = (@enum.GetType().FullName ?? string.Empty) + "." + @enum.ToString();
        return key.Local(@enum.GetAttribute<DescriptionAttribute>()?.Description ?? @enum.ToString());
    }

    public static string Local(this MemberInfo member)
    {
        var key = (member.DeclaringType?.FullName ?? string.Empty) + "." + member.ToString();
        return key.Local(member.GetCustomAttribute<DescriptionAttribute>()?.Description ?? member.ToString()!);
    }

    public static string Local(this Type type)
    {
        return (type.FullName ?? type.Name).Local(type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? type.ToString()!);
    }

    internal static string Local(this string key, string @default)
    {
#if DEBUG
        _rightLang[key] = @default;
#else
        if (_rightLang.TryGetValue(key, out var value)) return value;
#endif
        return @default;
    }


    public static void InIt()
    {
#if DEBUG
        
        var dirInfo = Svc.PluginInterface.AssemblyLocation.Directory;
        dirInfo = dirInfo?.Parent!.Parent!.Parent!.Parent!;


        var directory = dirInfo.FullName + @"\Localization";
        if (!Directory.Exists(directory)) return;

        if (Svc.PluginInterface.UiLanguage != "en") return;

        //Default values.
        var path = Path.Combine(directory, "Localization.json");
        _rightLang = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path)) ?? [];

        if( _rightLang == null)
        {
            Svc.Log.Error("Load translations failed");
        }
#else
        SetLanguage(Svc.PluginInterface.UiLanguage);
#endif
        Svc.PluginInterface.LanguageChanged += OnLanguageChange;
    }

    private static async void SetLanguage(string lang)
    {
        if (_translations.TryGetValue(lang, out var value))
        {
            _rightLang = value;
        }
        else
        {
            try
            {
                var url = $"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/RotationSolver/Localization/{lang}.json";
                using var client = new HttpClient();
                _rightLang = _translations[lang] = JsonConvert.DeserializeObject<Dictionary<string, string>>(await client.GetStringAsync(url))!;
            }
            catch (HttpRequestException ex) when (ex?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Svc.Log.Information(ex, $"No language {lang}");
                _rightLang = [];
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, $"Failed to download the language {lang}");
                _rightLang = [];
            }
        }

        RotationSolverPlugin.ChangeUITranslation();
    }

#if DEBUG
    private static void ExportLocalization()
    {
        var dirInfo = Svc.PluginInterface.AssemblyLocation.Directory;
        dirInfo = dirInfo?.Parent!.Parent!.Parent!.Parent!;


        var directory = dirInfo.FullName + @"\Localization";
        if (!Directory.Exists(directory)) return;

        if (Svc.PluginInterface.UiLanguage != "en") return;

        //Default values.
        var path = Path.Combine(directory, "Localization.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(_rightLang, Formatting.Indented));

        Svc.Log.Info("Exported the json file");
    }
#endif

    public static void Dispose()
    {
        Svc.PluginInterface.LanguageChanged -= OnLanguageChange;
#if DEBUG
        ExportLocalization();
#endif
    }

    private static void OnLanguageChange(string languageCode)
    {
#if DEBUG
#else
        try
        {
            Svc.Log.Information($"Loading Localization for {languageCode}");
            SetLanguage(languageCode);
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Unable to Load Localization");
        }
#endif
    }
}
