using ECommons.DalamudServices;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RotationSolver.Basic.Data;

/// <summary>
/// The timeline type.
/// </summary>
public enum TimelineType : byte
{
    /// <summary/>
    Unknown,

    /// <summary/>
    InCombat,

    /// <summary/>
    GameLog,

    /// <summary/>
    Ability,

    /// <summary/>
    StartsUsing,

    /// <summary/>
    SystemLogMessage,

    /// <summary/>
    ActorControl,

    /// <summary/>
    AddedCombatant,
}

/// <summary>
/// The timeline item on cactbot.
/// </summary>
public class TimelineItem
{
    private uint[]? ids = null;
    private readonly RaidLangs _langs;
    private readonly float? _jumpTime;
    private readonly JObject? _object;

    /// <summary>
    /// The last used action id.
    /// </summary>
    public uint LastActionID { get; internal set; }

    /// <summary>
    /// The action Ids in this item.
    /// </summary>
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
                Dalamud.ClientLanguage.Japanese => "ja",
                Dalamud.ClientLanguage.German => "de",
                Dalamud.ClientLanguage.French => "fr",
                (Dalamud.ClientLanguage)4 => "cn", 
                (Dalamud.ClientLanguage)5 => "ko",
                _ => "",
            };

            if (_langs.langs.TryGetValue(key, out var lang)) return lang;
            return new RaidLangs.Lang();
        }
    }

    /// <summary>
    /// The type of this timeline.
    /// </summary>
    public TimelineType Type { get; }

    /// <summary>
    /// The time it set.
    /// </summary>
    public float Time { get; }

    /// <summary>
    /// The window value min.
    /// </summary>
    public float WindowMin { get; }

    /// <summary>
    /// The window value max.
    /// </summary>
    public float WindowMax { get; }

    private readonly string _name;
    /// <summary>
    /// The display name.
    /// </summary>
    public string Name
    {
        get
        {
            if (Lang.replaceText.TryGetValue(_name, out var lang)) return lang;
            return _name;
        }
    }

    /// <summary>
    /// Is now in the window.
    /// </summary>
    public bool IsInWindow => DataCenter.RaidTimeRaw >= Time - WindowMin && DataCenter.RaidTimeRaw <= Time + WindowMax;

    internal bool IsShown => Name is not "--Reset--" and not "--sync--";

    /// <summary>
    /// Is property matched.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="matchValue"></param>
    /// <returns></returns>
    public bool this[string propertyName, uint matchValue]
        => this[propertyName, matchValue.ToString("X")];

    /// <summary>
    /// Is property matched.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="matchString"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get the properties.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public string[] this[string propertyName]
    {
        get
        {
            string[] strings = [];

            var strRelay = _object?[propertyName];

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

    internal TimelineItem(float time, string name, TimelineType type, JObject? obj, RaidLangs langs, float? jumpTime, float windowMin, float windowMax)
    {
        _name = name;
        Type = type;
        Time = time;
        WindowMax = windowMax;
        WindowMin = windowMin;
        _object = obj;
        _langs = langs;
        _jumpTime = jumpTime;
    }

    internal TimelineItem(float time, string name, string type, JObject? obj, RaidLangs langs, float? jumpTime, float windowMin, float windowMax)
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

    internal void UpdateRaidTimeOffset()
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
            DataCenter.RaidTimeRaw = _jumpTime ?? Time;
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"""
            IsShown: {IsShown},
            Time: {Time}, JumpTime: {_jumpTime ?? -1},
            Name: {Name}, Type: {Type},
            Window: {WindowMin}, {WindowMax},
            Ids: {string.Join(", ", this["id"])}
            """;
    }
}
