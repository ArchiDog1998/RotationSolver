using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Localization;

internal class LocalizationManager : IDisposable
{
    private readonly Dalamud.Localization localization;

    public LocalizationManager()
    {
        var assemblyLocation = Service.Interface.AssemblyLocation.DirectoryName;
        var filePath = Path.Combine(assemblyLocation, "translations");

        localization = new Dalamud.Localization(filePath, "XIVAutoAttack_");
        localization.SetupWithLangCode(Service.Interface.UiLanguage);

        Service.Interface.LanguageChanged += OnLanguageChange;
    }

#if DEBUG
    public void ExportLocalization()
    {
        try
        {
            localization.ExportLocalizable();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Error exporting localization files");
        }
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
            localization.SetupWithLangCode(languageCode);
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to Load Localization");
        }
    }
}
