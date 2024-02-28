using ECommons.DalamudServices;
using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration.Timeline;
using System.Text.RegularExpressions;

namespace RotationSolver.Updaters;
internal static partial class RaidTimeUpdater
{
    internal static readonly Dictionary<uint, string> _pathForRaids = new()
    {
#if DEBUG
        { 296, "02-arr/trial/titan-ex.txt" },
        { 530, "03-hw/raid/a6s.txt" }, //For System log
        { 748, "04-sb/raid/o5n.txt" }, //For Actor Control
#endif
        { 1148, "06-ew/raid/p9s.txt" },
        { 1150, "06-ew/raid/p10s.txt" },
        { 1152, "06-ew/raid/p11s.txt" },
        { 1154, "06-ew/raid/p12s.txt" },
    };

    private static readonly Dictionary<uint, TimelineItem[]> _savedTimeLines = [];
    private static readonly Queue<(DateTime, ActionTimelineItem)> _addedItems = new();

    internal static void UpdateTimeline()
    {
        if (!Service.Config.Timeline.TryGetValue(Svc.ClientState.TerritoryType, out var timeline)) return;

        while (_addedItems.TryPeek(out var addedItem))
        {
            if (DateTime.Now - addedItem.Item1 < TimeSpan.FromSeconds(5)) break;
            _addedItems.Dequeue();
        }

        foreach (var item in DataCenter.TimelineItems)
        {
            var time = item.Time - DataCenter.RaidTimeRaw;

            if (time < 0) continue;
            if (!timeline.TryGetValue(item.Time, out var items)) continue;

            foreach (var item2 in items.OfType<ActionTimelineItem>()
                .Where(i => !_addedItems.Any(added => added.Item2 == i)))
            {
                if (!item2.InPeriod(item)) continue;

                var act = DataCenter.RightNowRotation?.AllBaseActions.FirstOrDefault(a => (ActionID)a.ID == item2.ID);

                if (act == null) continue;

                DataCenter.AddCommandAction(act, item2.Duration);
                _addedItems.Enqueue((DateTime.Now, item2));

#if DEBUG
                Svc.Log.Debug($"Added the action{act} to timeline.");
#endif
            }
        }
    }

    internal static void Enable()
    {
        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        ClientState_TerritoryChanged(Svc.ClientState.TerritoryType);
        Svc.DutyState.DutyWiped += DutyState_DutyWiped;
        Svc.DutyState.DutyCompleted += DutyState_DutyWiped;
    }

    private static void DutyState_DutyWiped(object? sender, ushort e)
    {
        DataCenter.RaidTimeRaw = -1;
    }

    internal static void Disable() 
    {
        Svc.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
        Svc.DutyState.DutyWiped -= DutyState_DutyWiped;
        Svc.DutyState.DutyCompleted -= DutyState_DutyWiped;
    }

    private static async void ClientState_TerritoryChanged(ushort id)
    {
        try
        {
            DataCenter.TimelineItems = await GetRaidTimeAsync(id);
        }
        catch (Exception e)
        {
            Svc.Log.Warning(e, "Failed to update the raid timeline!");
        }
    }

    private static List<ushort> _downloading = [];
    public static TimelineItem[] GetRaidTime(ushort id)
    {
        if (_savedTimeLines.TryGetValue(id, out var value)) return value;
        if (!_pathForRaids.TryGetValue(id, out var path)) return [];
        if (_downloading.Contains(id)) return [];

        _downloading.Add(id);
        Task.Run(async () =>
        {
            _savedTimeLines[id] = await DownloadRaidTimeAsync(path);
            _downloading.Remove(id);
        });

        return [];
    }

    static async Task<TimelineItem[]> GetRaidTimeAsync(ushort id)
    {
        if (_savedTimeLines.TryGetValue(id, out var value)) return value;
        if (_pathForRaids.TryGetValue(id, out var path))
        {
            return _savedTimeLines[id] = await DownloadRaidTimeAsync(path);
        }
        return [];
    }

    static async Task<TimelineItem[]> DownloadRaidTimeAsync(string path)
    {
        using var client = new HttpClient();
        var message = await client.GetAsync("https://raw.githubusercontent.com/OverlayPlugin/cactbot/main/ui/raidboss/data/" + path);

        if (!message.IsSuccessStatusCode)
        {
            return [];
        }

        var str = await message.Content.ReadAsStringAsync();

        var result = new List<TimelineItem>();
        foreach (var timelineItem in TimeLineItem().Matches(str).Cast<Match>())
        {
            try
            {
                var timeline = timelineItem.Value;

                var header = TimeHeader().Match(timeline).Value;

                if (string.IsNullOrEmpty(header)) continue;

                var time = float.Parse(Time().Match(header).Value);
                var name = Name().Match(header).Value[1..^1];

                var timelineStr = ActionGetter().Match(timeline).Value;

                JObject? item = null;
                string[] ids = [];
                if (!string.IsNullOrEmpty(timelineStr))
                {
                    item = JObject.Parse(timelineStr);

                    var id = item["id"];
                    if (id is JArray array)
                    {
                        ids = [.. array.Select(i => i.ToString())];
                    }
                    else
                    {
                        ids = [id?.ToString() ?? string.Empty];
                    }
                }

                var rest = timeline[header.Length..];
                var type = Type().Match(rest)?.Value ?? string.Empty;
                if (type.Length > 3)
                {
                    type = type[1..^2].Split(' ').LastOrDefault() ?? string.Empty;
                }

                result.Add(new (time, name, type, ids, item));
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "sth wrong with matching!");
            }
        }

#if DEBUG
        foreach (var item in result)
        {
            Svc.Log.Debug(item.ToString());
        }
#endif
        return [..result];
    }

    [GeneratedRegex(" .*? {")]
    private static partial Regex Type();

    [GeneratedRegex("[\\d\\.]+.*")]
    private static partial Regex TimeLineItem();

    [GeneratedRegex("^[\\d\\.]+")]
    private static partial Regex Time();

    [GeneratedRegex("\".*?\"")]
    private static partial Regex Name();

    [GeneratedRegex("^[\\d\\.]+ \".*?\"")]
    private static partial Regex TimeHeader();

    [GeneratedRegex("{.*}")]
    private static partial Regex ActionGetter();
}
