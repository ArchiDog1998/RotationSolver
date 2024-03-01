using ECommons.DalamudServices;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using static Dalamud.Interface.Utility.Raii.ImRaii;

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

internal readonly struct TimelineItem(float time, string name, TimelineType type, JObject? obj, RaidLangs langs)
{
    private RaidLangs.Lang Lang
    {
        get
        {
            var key = Svc.ClientState.ClientLanguage switch
            {
                Dalamud.ClientLanguage.Japanese => "ja",
                Dalamud.ClientLanguage.German => "de",
                Dalamud.ClientLanguage.French => "fr",
                (Dalamud.ClientLanguage)4 => "cn", 
                (Dalamud.ClientLanguage)5 => "ko",
                _ => "",
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

    public bool this[string propertyName, uint matchValue]
        => this[propertyName, matchValue.ToString("X")];

    public bool this[string propertyName, string matchString]
    {
        get
        {
            foreach (var str in this[propertyName])
            {
                var prop = str;
                if (new Regex(prop).IsMatch(matchString))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public string[] this[string propertyName]
    {
        get
        {
            string[] strings = [];

            var strRelay = Object?[propertyName];

            if (strRelay == null) return [];

            if (strRelay is JArray array)
            {
                strings = [.. array.Select(i => i.ToString())];
            }
            else
            {
                strings = [strRelay?.ToString() ?? string.Empty];
            }

            var list = new List<string>(strings.Length);
            foreach (var str in strings)
            {
                if (string.IsNullOrEmpty(str)) continue;

                var prop = str;

                foreach (var pair in Lang.replaceSync)
                {
                    prop = prop.Replace(pair.Key, pair.Value);
                }
                list.Add(prop);
            }
            return [.. list];
        }
    }

    public TimelineItem(float time, string name, string type, JObject? obj, RaidLangs langs)
        : this(time, name, GetTypeFromName(type), obj, langs)
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
#if DEBUG
            Svc.Log.Debug($"Reset the {nameof(DataCenter.RaidTimeRaw)}.");
#endif
            DataCenter.RaidTimeRaw = -1;
        }
        else
        {
#if DEBUG
            var timeOffset = Time - DataCenter.RaidTimeRaw;
            Svc.Log.Debug($"Set the {nameof(DataCenter.RaidTimeRaw)} to {Time}, added {timeOffset}s");
#endif
            DataCenter.RaidTimeRaw = Time;
        }
    }

    public override string ToString()
    {
        return $"""
            IsShown: {IsShown},
            Time: {Time},
            Name: {Name},
            Type: {Type},
            Ids: {string.Join(", ", this["id"])}
            """;
    }
}
