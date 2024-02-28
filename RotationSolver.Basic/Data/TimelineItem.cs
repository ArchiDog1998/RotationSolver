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

internal readonly struct TimelineItem(float time, string name, TimelineType type, string[] ids, JObject? obj)
{
    public TimelineType Type => type;

    public float Time => time;

    public JObject? Object => obj;

    public string Name => name;

    public bool IsShown => name is not "--Reset--" and not "--sync--";

    public string this[string propertyName] => Object?[propertyName]?.ToString() ?? string.Empty;

    public TimelineItem(float time, string name, string type, string[] ids, JObject? obj)
        : this(time, name, GetTypeFromName(type), ids, obj)
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

            case "Ability":
                return TimelineType.Ability;

            case "StartsUsing":
                return TimelineType.StartsUsing;

            case "SystemLogMessage":
                return TimelineType.SystemLogMessage; //TODO: a6s for testing

            default:
                Svc.Log.Warning($"New timelinetype: {type}");
                return TimelineType.Unknown;
        }
    }

    public void UpdateRaidTimeOffset()
    {
        DataCenter.RaidTimeRaw = Time;
        Svc.Log.Debug($"Reset the {nameof(DataCenter.RaidTimeRaw)} to {DataCenter.RaidTimeRaw}.");
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
