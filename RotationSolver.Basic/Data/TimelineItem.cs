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
    AddedCombatant,
}

internal struct TimelineItem(float time, string name, TimelineType type, JObject? obj, RaidLangs langs, float? jumpTime, float windowMin, float windowMax)
{
    private uint[]? ids = null;
    public uint[] ActionIDs => ids ??= GetActionIds();
    private readonly uint[] GetActionIds()
    {
        if (Type is not TimelineType.Ability and not TimelineType.StartsUsing) return [];
        var idsRaw = this["id"];

        if (idsRaw == null || idsRaw.Length == 0) return [];

        var regex = idsRaw.Select(id => new Regex(id));

        var count = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()?.RowCount ?? ushort.MaxValue;

        List<uint> reuslt = [];
        for (uint i = 0; i < count; i++)
        {
            var text = i.ToString("X");
            if (regex.Any(i => i.IsMatch(text)))
            {
                reuslt.Add(i);
            }
        }
        return [.. reuslt];
    }

    private readonly RaidLangs.Lang Lang
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
    public readonly TimelineType Type => type;

    public readonly float Time => time;

    public readonly float WindowMin => windowMin;
    public readonly float WindowMax => windowMax;
    public readonly JObject? Object => obj;

    public readonly string Name
    {
        get
        {
            if (Lang.replaceText.TryGetValue(name, out var lang)) return lang;
            return name;
        }
    }

    public readonly bool IsInWindow => DataCenter.RaidTimeRaw >= Time - WindowMin && DataCenter.RaidTimeRaw <= Time + WindowMax;

    public readonly bool IsShown => Name is not "--Reset--" and not "--sync--";

    public readonly bool this[string propertyName, uint matchValue]
        => this[propertyName, matchValue.ToString("X")];

    public readonly bool this[string propertyName, string matchString]
    {
        get
        {
            var properties = this[propertyName];

            if (properties.Length == 0) return true;

            foreach (var str in properties)
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

    public readonly string[] this[string propertyName]
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

    public TimelineItem(float time, string name, string type, JObject? obj, RaidLangs langs, float? jumpTime, float windowMin, float windowMax)
        : this(time, name, GetTypeFromName(type), obj, langs, jumpTime, windowMin, windowMax)
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

            case "AddedCombatant":
                return TimelineType.AddedCombatant;

            default:
#if DEBUG
                Svc.Log.Warning($"New timelinetype: {type}");
#endif
                return TimelineType.Unknown;
        }
    }

    public readonly void UpdateRaidTimeOffset()
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
            DataCenter.RaidTimeRaw = jumpTime ?? Time;
        }
    }

    public readonly override string ToString()
    {
        return $"""
            IsShown: {IsShown},
            Time: {Time}, JumpTime: {jumpTime ?? -1},
            Name: {Name}, Type: {Type},
            Window: {windowMin}, {windowMax},
            Ids: {string.Join(", ", this["id"])}
            """;
    }
}
