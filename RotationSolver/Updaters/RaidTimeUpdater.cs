using ECommons.DalamudServices;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace RotationSolver.Updaters;
internal static partial class RaidTimeUpdater
{
    internal static void Enable()
    {
        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        ClientState_TerritoryChanged(Svc.ClientState.TerritoryType);
    }

    internal static void Disable() 
    {
        Svc.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
    }

    private static async void ClientState_TerritoryChanged(ushort obj)
    {
        try
        {
            DataCenter.TimelineItems = await UpdateRaidTime("06-ew/raid/p9s.txt");
        }
        catch (Exception e)
        {
            Svc.Log.Warning(e, "Failed to update the raid timeline!");
        }
    }
    static async Task<TimelineItem[]> UpdateRaidTime(string path)
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
                var time = float.Parse(Time().Match(header).Value);
                var name = Name().Match(header).Value[1..^1];

                var item = JObject.Parse(ActionGetter().Match(timeline).Value);

                string[] ids = [];
                if (item != null)
                {
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
                var type = Type().Match(rest)?.Value[1..^1] ?? string.Empty;

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

    [GeneratedRegex(" .*? ")]
    private static partial Regex Type();

    [GeneratedRegex("\\d+\\.\\d.*")]
    private static partial Regex TimeLineItem();

    [GeneratedRegex("^\\d+\\.\\d")]
    private static partial Regex Time();

    [GeneratedRegex("\".*?\"")]
    private static partial Regex Name();

    [GeneratedRegex("^\\d+\\.\\d \".*?\"")]
    private static partial Regex TimeHeader();

    [GeneratedRegex("{.*}")]
    private static partial Regex ActionGetter();
}
