using ECommons.DalamudServices;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using Dalamud.Game;

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

internal class TimelineItem(float time, string name, TimelineType type, JObject? obj, RaidLangs langs, float? jumpTime, float windowMin, float windowMax)
{
    private uint[]? ids = null;

    public uint LastActionID { get; set; }
    public uint[] ActionIDs => ids ??= GetActionIds();
    private uint[] GetActionIds()
    {
        if (Type is not TimelineType.Ability and not TimelineType.StartsUsing) return [];
        var idsRaw = this["id"];

        if (idsRaw == null || idsRaw.Length == 0) return [];

        List<uint> reuslt = [];
        List<Regex> regexes = [];

        foreach ( var id in idsRaw)
        {
            if (uint.TryParse(id, NumberStyles.HexNumber, null, out var i))
            {
                reuslt.Add(i);
            }
            else
            {
                regexes.Add(new(id));
            }
        }

        if (regexes.Count == 0) return [.. reuslt];

#if DEBUG
        Svc.Log.Debug("Need to cast the regex, with " + string.Join(", ", regexes));
#endif
        var count = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()?.RowCount ?? ushort.MaxValue;

        for (uint i = 0; i < count; i++)
        {
            var text = i.ToString("X");
            if (regexes.Any(i => i.IsMatch(text)))
            {
                reuslt.Add(i);
            }
        }
        return [.. reuslt];
    }

    private RaidLangs.Lang Lang
    {
        get
        {
            var key = Svc.ClientState.ClientLanguage switch
            {
                ClientLanguage.Japanese => "ja",
                ClientLanguage.German => "de",
                ClientLanguage.French => "fr",
                (ClientLanguage)4 => "cn", 
                (ClientLanguage)5 => "ko",
                _ => "",
            };

            if (langs.langs.TryGetValue(key, out var lang)) return lang;
            return new RaidLangs.Lang();
        }
    }
    public TimelineType Type => type;

    public float Time => time;

    public float WindowMin => windowMin;
    public float WindowMax => windowMax;
    public JObject? Object => obj;

    public string Name
    {
        get
        {
            if (Lang.replaceText.TryGetValue(name, out var lang)) return lang;
            return name;
        }
    }

    public bool IsInWindow => DataCenter.RaidTimeRaw >= Time - WindowMin && DataCenter.RaidTimeRaw <= Time + WindowMax;

    public bool IsShown => Name is not "--Reset--" and not "--sync--";

    public bool this[string propertyName, uint matchValue]
        => this[propertyName, matchValue.ToString("X")];

    public bool this[string propertyName, string matchString]
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
            DataCenter.RaidTimeRaw = jumpTime ?? Time;
        }
    }

    public override string ToString()
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
