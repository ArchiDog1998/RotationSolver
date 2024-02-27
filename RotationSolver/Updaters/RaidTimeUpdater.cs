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

    private static void ClientState_TerritoryChanged(ushort obj)
    {
        try
        {
            UpdateRaidTime("06-ew/raid/p9s.txt");
        }
        catch (Exception e)
        {
            Svc.Log.Warning(e, "Failed to update the raid timeline!");
        }
    }
    static async void UpdateRaidTime(string path)
    {
        using var client = new HttpClient();
        var message = await client.GetAsync("https://raw.githubusercontent.com/OverlayPlugin/cactbot/main/ui/raidboss/data/" + path);

        if (!message.IsSuccessStatusCode) return;

        var str = await message.Content.ReadAsStringAsync();

        var result = new List<TimelineItem>();
        foreach (var timelineItem in TimeLineItem().Matches(str).Cast<Match>())
        {
            try
            {
                var header = TimeHeader().Match(timelineItem.Value);
                var action = JObject.Parse(ActionGetter().Match(timelineItem.Value).Value)["id"];
                var time = float.Parse(Time().Match(header.Value).Value);
                var name = Name().Match(header.Value).Value[1..^1];

                string[] ids = [];
                if (action is JArray array)
                {
                    ids = [..array.Select(i => i.ToString())];
                }
                else
                {
                    ids = [action?.ToString() ?? string.Empty];
                }

                result.Add(new (time, name, ids));
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "sth wrong with matching!");
            }
        }
        DataCenter.TimelineItems = [..result];
    }

    [GeneratedRegex("\\d+\\.\\d.*Ability.*")]
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
