using ECommons.DalamudServices;
using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration.Timeline;
using RotationSolver.UI;
using System.Text.RegularExpressions;

namespace RotationSolver.Updaters;
internal static partial class RaidTimeUpdater
{
    internal static readonly Dictionary<uint, string> PathForRaids = [];

    private static readonly Dictionary<uint, TimelineItem[]> _savedTimeLines = [];
    private static readonly Queue<(DateTime, ActionTimelineItem)> _addedItems = new();

    public static string GetLink(uint id)
    {
        if (!PathForRaids.TryGetValue(id, out var path)) return string.Empty;
        return "https://github.com/xpdota/event-trigger/tree/master/timelines/src/main/resources/timeline/" + path;
    }

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

    internal static async void EnableAsync()
    {
        Svc.DutyState.DutyWiped += DutyState_DutyWiped;
        Svc.DutyState.DutyCompleted += DutyState_DutyWiped;

        using var client = new HttpClient();
        var message = await client.GetAsync("https://raw.githubusercontent.com/xpdota/event-trigger/master/timelines/src/main/resources/timelines.csv");

        if (!message.IsSuccessStatusCode)
        {
            return;
        }

        var str = await message.Content.ReadAsStringAsync();

        foreach (var pair in str.Split('\n'))
        {
            var items = pair.Split(',');
            if (items.Length < 2) continue;

            if (!uint.TryParse(items[0], out var id)) continue;
            var name = items[1][1..^1];
            PathForRaids[id] = name;
        }

        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        ClientState_TerritoryChanged(Svc.ClientState.TerritoryType);
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
        if (PathForRaids.ContainsKey(id))
        {
            RotationConfigWindow._territoryId = id;
        }
        try
        {
            DataCenter.TimelineItems = await GetRaidAsync(id) ?? [];
        }
        catch (Exception e)
        {
            Svc.Log.Warning(e, "Failed to update the raid timeline!");
        }
    }

    private static readonly List<ushort> _downloading = [];
    internal static TimelineItem[] GetRaidTime(ushort id)
    {
        if (_savedTimeLines.TryGetValue(id, out var value)) return value;
        if (!PathForRaids.TryGetValue(id, out var path)) return [];
        if (_downloading.Contains(id)) return [];

        _downloading.Add(id);
        Task.Run(async () =>
        {
            _savedTimeLines[id] = await DownloadRaidAsync(path);
            _downloading.Remove(id);
        });

        return [];
    }

    static async Task<TimelineItem[]> DownloadRaidAsync(string path)
    {
        var langs = await DownloadRaidLangsAsync(path);

        return await DownloadRaidTimeAsync(path, langs);
    }
    static async Task<TimelineItem[]> GetRaidAsync(ushort id)
    {
        if (_savedTimeLines.TryGetValue(id, out var value)) return value;
        if (PathForRaids.TryGetValue(id, out var path))
        {
            return _savedTimeLines[id] = await DownloadRaidAsync(path);
        }
        return [];
    }

    static async Task<RaidLangs> DownloadRaidLangsAsync(string path)
    {
        using var client = new HttpClient();
        var message = await client.GetAsync("https://raw.githubusercontent.com/xpdota/event-trigger/master/timelines/src/main/resources/timeline/translations/" + path + ".json");

        if (!message.IsSuccessStatusCode)
        {
            return new();
        }

        var str = await message.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<RaidLangs>(str) ?? new();
    }

    static async Task<TimelineItem[]> DownloadRaidTimeAsync(string path, RaidLangs lang)
    {
        using var client = new HttpClient();
        var message = await client.GetAsync("https://raw.githubusercontent.com/xpdota/event-trigger/master/timelines/src/main/resources/timeline/" + path);

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

                result.Add(new (time, name, type, ids, item, lang));
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
        return [..result.OrderBy(i => i.Time)];
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
