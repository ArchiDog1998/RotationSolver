using ECommons.DalamudServices;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace RotationSolver.Basic.Data;
internal enum TimelineType : byte
{
    Unknown,
    InCombat,
    GameLog,
    Ability,
    StartsUsing,
    SystemLogMessage,
    ActorControl,
}

internal readonly struct TimelineItem(float time, string name, TimelineType type, string[] ids, JObject? obj, RaidLangs langs)
{
    private RaidLangs.Lang Lang
    {
        get
        {
            var key = Svc.ClientState.ClientLanguage switch
            {
                Dalamud.ClientLanguage.English => "",
                Dalamud.ClientLanguage.Japanese => "ja",
                Dalamud.ClientLanguage.German => "de",
                Dalamud.ClientLanguage.French => "fr",
                (Dalamud.ClientLanguage)4 => "cn", 
                _ => "ko",
            };

            if (langs.langs.TryGetValue(key, out var lang)) return lang;
            return new RaidLangs.Lang();
        }
    }
    public TimelineType Type => type;

    public float Time => time;

    public JObject? Object => obj;

    public string Name
    {
        get
        {
            if (Lang.replaceText.TryGetValue(name, out var lang)) return lang;
            return name;
        }
    }

    public bool IsShown => Name is not "--Reset--" and not "--sync--";

    public string this[string propertyName]
    {
        get
        {
            var prop = Object?[propertyName]?.ToString() ?? string.Empty;
            foreach (var pair in Lang.replaceSync)
            {
                prop = prop.Replace(pair.Key, pair.Value);
            }
            return prop;
        }
    }

    public TimelineItem(float time, string name, string type, string[] ids, JObject? obj, RaidLangs langs)
        : this(time, name, GetTypeFromName(type), ids, obj, langs)
    {
        
    }

    private static TimelineType GetTypeFromName(string type)
    {
        switch(type)
        {
            case "":
                return TimelineType.Unknown;

            case "ActorControl":
                return TimelineType.ActorControl; //TODO: how to check that command.

            case "InCombat":
                return TimelineType.InCombat;

            case "GameLog":
                return TimelineType.GameLog;

            case "#Ability":
            case "Ability":
                return TimelineType.Ability;

            case "StartsUsing":
                return TimelineType.StartsUsing;

            case "SystemLogMessage":
                return TimelineType.SystemLogMessage;

            default:
#if DEBUG
                Svc.Log.Warning($"New timelinetype: {type}");
#endif
                return TimelineType.Unknown;
        }
    }

    public void UpdateRaidTimeOffset()
    {
        if (Name == "--Reset--")
        {
            DataCenter.RaidTimeRaw = -1;
            Svc.Log.Debug($"Reset the {nameof(DataCenter.RaidTimeRaw)}.");
        }
        else
        {
            DataCenter.RaidTimeRaw = Time;
            Svc.Log.Debug($"Reset the {nameof(DataCenter.RaidTimeRaw)} to {DataCenter.RaidTimeRaw}.");
        }
    }

    public bool IsIdMatched(uint id)
    {
        return ids.Any(i => new Regex(i).IsMatch(id.ToString("X")));
    }

    public override string ToString()
    {
        return $"""
            IsShown: {IsShown},
            Time: {Time},
            Name: {Name},
            Type: {Type},
            Ids: {string.Join(", ", ids)}
            """;
    }
}
