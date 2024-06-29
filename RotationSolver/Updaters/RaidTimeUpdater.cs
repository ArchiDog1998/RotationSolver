using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration.Timeline;
using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using RotationSolver.UI;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace RotationSolver.Updaters;
internal static partial class RaidTimeUpdater
{
    internal static readonly Dictionary<uint, string> PathForRaids = [];

    private static readonly Dictionary<uint, TimelineItem[]> _savedTimeLines = [];

    public static string GetLink(uint id)
    {
        if (!PathForRaids.TryGetValue(id, out var path)) return string.Empty;
        return "https://github.com/xpdota/event-trigger/tree/master/timelines/src/main/resources/timeline/" + path;
    }

    internal static void UpdateTimeline()
    {
        UpdateTimelineEvent();
        UpdateTimelineAddCombat();
    }

    static readonly Dictionary<uint, bool> _isInCombat = [];
    private static void UpdateTimelineAddCombat()
    {
        if (DataCenter.TimelineItems.Length == 0) return;
        foreach (var obj in DataCenter.AllTargets)
        {
            if (obj is PlayerCharacter) continue;
            var id = obj.ObjectId;
            var newInCombat = obj.InCombat();

            if (_isInCombat.TryGetValue(id, out var inCombat)
                && !inCombat && newInCombat)
            {
                var name = GetNameFromObjectId(id);

                foreach (var item in DataCenter.TimelineItems)
                {
                    if (!item.IsInWindow) continue;
                    if (item.Type is not TimelineType.AddedCombatant) continue;

                    if (!item["name", name]) continue;

                    item.UpdateRaidTimeOffset();
                    break;
                }
            }
            _isInCombat[id] = newInCombat;
        }
    }

    private static void UpdateTimelineEvent()
    {
        if (!Service.Config.Timeline.TryGetValue(Svc.ClientState.TerritoryType, out var timeline))
        {
            DownloadTerritory(Svc.ClientState.TerritoryType);
            return;
        }
        if (timeline.Sum(i => i.Value.Count) == 0)
        {
            DownloadTerritory(Svc.ClientState.TerritoryType);
        }

        foreach (var item in DataCenter.TimelineItems)
        {
            if (!timeline.TryGetValue(item.Time, out var items)) continue;

            var time = item.Time - DataCenter.RaidTimeRaw;

            foreach (var i in items)
            {
                i.Enable = i.InPeriod(item);
            }
        }
    }

    private static readonly List<uint> _downloadingList = [];
    public static void DownloadTerritory(uint id)
    {
        if (_downloadingList.Contains(id)) return;
        _downloadingList.Add(id);

        Task.Run(() =>
        {
            DownloadTerritoryPrivate(id);
        });
    }

    private static void DownloadTerritoryPrivate(uint id)
    {
        try
        {
            using var client = new HttpClient();
            var str = client.GetStringAsync($"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Resources/Timelines/{id}.json").Result;

            Service.Config.Timeline[id] = JsonConvert.DeserializeObject<Dictionary<float, List<BaseTimelineItem>>>(str,
                    new BaseTimelineItemConverter(), new ITimelineConditionConverter())!;
        }
        catch (Exception ex)
        {
#if DEBUG
            Svc.Log.Error(ex, $"Failed to download the timeline {id}.");
#endif
            return;
        }
        _downloadingList.Remove(id);
    }

    internal static async void EnableAsync()
    {
        Svc.DutyState.DutyWiped += DutyState_DutyWiped;
        Svc.DutyState.DutyCompleted += DutyState_DutyWiped;

        try
        {
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
        }
        catch(Exception ex)
        {
            Svc.Log.Warning(ex, "Failed to download the timelines!");
        }


        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        ClientState_TerritoryChanged(Svc.ClientState.TerritoryType);

        Svc.GameNetwork.NetworkMessage += GameNetwork_NetworkMessage;
        Svc.Chat.ChatMessage += Chat_ChatMessage;
    }

    internal static void Disable() 
    {
        Svc.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
        Svc.DutyState.DutyWiped -= DutyState_DutyWiped;
        Svc.DutyState.DutyCompleted -= DutyState_DutyWiped;
        Svc.GameNetwork.NetworkMessage -= GameNetwork_NetworkMessage;
        Svc.Chat.ChatMessage -= Chat_ChatMessage;
    }

    private static void DutyState_DutyWiped(object? _, ushort e)
    {
        DataCenter.RaidTimeRaw = -1;
    }

    private static void Chat_ChatMessage(Dalamud.Game.Text.XivChatType type, uint senderId, ref Dalamud.Game.Text.SeStringHandling.SeString sender, ref Dalamud.Game.Text.SeStringHandling.SeString message, ref bool isHandled)
    {
        var name = GetNameFromObjectId(senderId);

#if DEBUG
        //Svc.Log.Debug(sender.TextValue.ToString());
#endif

        foreach (var item in DataCenter.TimelineItems)
        {
            if (!item.IsInWindow) continue;
            if (item.Type is not TimelineType.GameLog) continue;

            var typeString = ((uint)type).ToString("X4");
            if (!item["code", typeString]) continue;
            if (!item["name", name]) continue;
            if (!item["line", message.TextValue]) continue;

            item.UpdateRaidTimeOffset();
            break;
        }
    }

    private static void GameNetwork_NetworkMessage(nint dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, Dalamud.Game.Network.NetworkMessageDirection direction)
    {
        if (direction != Dalamud.Game.Network.NetworkMessageDirection.ZoneDown) return;
        OpCode op = (OpCode)opCode;

        switch (op)
        {
            case OpCode.SystemLogMessage:
                OnSystemLogMessage(dataPtr);
                break;
            case OpCode.ActorControlTarget:
            case OpCode.ActorControlSelf:
            case OpCode.ActorControl:
                OnActorControl(dataPtr);
                break;

            case OpCode.Effect:
                OnEffect(dataPtr, targetActorId);
                break;

            case OpCode.ActorCast:
                OnCast(dataPtr, targetActorId);
                break;
        }
    }

    private static void OnCast(IntPtr dataPtr, uint targetActorId)
    {
        var name = GetNameFromObjectId(targetActorId);
        var actionId = ReadUshort(dataPtr, 0);
        var castingTime = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()?
            .GetRow(actionId)?.Cast100ms / 10f;

        for (int i = 0; i < DataCenter.TimelineItems.Length; i++)
        {
            var item = DataCenter.TimelineItems[i];

            if (item.Type is TimelineType.Ability
                && item["id", actionId]
                && item["source", name])
            {
                var t = DataCenter.RaidTimeRaw + castingTime;

                if (t >= item.Time - item.WindowMin && t <= item.Time + item.WindowMax)
                {
                    item.LastActionID = actionId;
#if DEBUG
                    Svc.Log.Debug($"Set the timeline {item.Time} action to {actionId}");
#endif
                }
            }

            if (!item.IsInWindow) continue;
            if (item.Type is not TimelineType.StartsUsing) continue;

            if (!item["id", actionId]) continue;
            if (!item["source", name]) continue;

            item.LastActionID = actionId;
            item.UpdateRaidTimeOffset();
            break;
        }
    }
#if DEBUG
    private static unsafe int[] DataIndex<T>(IntPtr dataPtr, T subData, int size, Func<T, T, bool> equal)
        where T : unmanaged
    {
        var length = sizeof(T);

        List<int> indexes = [];
        for (int i = 0; i < size - length; i++)
        {
            var v = *(T*)(dataPtr + i);
            if(equal(v, subData))
            {
                indexes.Add(i);
            }
        }
        return [..indexes];
    }

    private static bool IsTheSame(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }
#endif
    static DateTime _actionUpdateTime = DateTime.MinValue;
    private static void OnEffect(IntPtr dataPtr, uint targetActorId)
    {
        var name = GetNameFromObjectId(targetActorId);

        for (int i = 0; i < DataCenter.TimelineItems.Length; i++)
        {
            var item = DataCenter.TimelineItems[i];
            if (!item.IsInWindow) continue;
            if (item.Type is not TimelineType.Ability) continue;

            var actionId = ReadUint(dataPtr, 28);
            if (!item["id", actionId]) continue;
            if (!item["source", name]) continue;

            if (DateTime.Now - _actionUpdateTime < TimeSpan.FromSeconds(0.5)) continue;

            item.LastActionID = actionId;
            item.UpdateRaidTimeOffset();
            _actionUpdateTime = DateTime.Now;
            break;
        }
    }

    private static void OnActorControl(IntPtr dataPtr)
    {
        foreach (var item in DataCenter.TimelineItems)
        {
            if (!item.IsInWindow) continue;
            if (item.Type is not TimelineType.ActorControl) continue;

            if (!item["command", ReadUint(dataPtr, 8)]) continue;
            if (!item["data0", ReadUshort(dataPtr, 12)]) continue;
            if (!item["data1", ReadUshort(dataPtr, 14)]) continue;
            if (!item["data2", ReadUshort(dataPtr, 16)]) continue;
            if (!item["data3", ReadUshort(dataPtr, 18)]) continue;

            item.UpdateRaidTimeOffset();
            break;
        }
    }

    private static void OnSystemLogMessage(IntPtr dataPtr)
    {
        Svc.Log.Debug($"Message: {ReadUint(dataPtr, 4):X}, {ReadUint(dataPtr, 8):X}, {ReadUint(dataPtr, 12):X}, {ReadUint(dataPtr, 16):X}");
        foreach (var item in DataCenter.TimelineItems)
        {
            if (!item.IsInWindow) continue;
            if (item.Type is not TimelineType.SystemLogMessage) continue;

            if (!item["id", ReadUint(dataPtr, 4)]) continue;
            if (!item["param0", ReadUint(dataPtr, 8)]) continue;
            if (!item["param1", ReadUint(dataPtr, 12)]) continue;
            if (!item["param2", ReadUint(dataPtr, 16)]) continue;

            item.UpdateRaidTimeOffset();
            break;
        }
    }

    private unsafe static ushort ReadUshort(IntPtr dataPtr, int offset)
    {
        return *(ushort*)(dataPtr + offset);
    }

    private unsafe static uint ReadUint(IntPtr dataPtr, int offset)
    {
        return *(uint*)(dataPtr + offset);
    }

    private unsafe static float ReadFloat(IntPtr dataPtr, int offset)
    {
        return *(float*)(dataPtr + offset);
    }

    private static string GetNameFromObjectId(uint id)
    {
        var obj = Svc.Objects.SearchById(id);
        var nameId = obj is BattleChara battle ? battle.NameId : 0;
        var name = Svc.Data.GetExcelSheet<BNpcName>()?.GetRow(nameId)?.Singular.RawString;

        if (!string.IsNullOrEmpty(name)) return name;

        return obj?.Name.TextValue ?? string.Empty;
    }

    private static async void ClientState_TerritoryChanged(ushort id)
    {
        _isInCombat.Clear();
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
        var matches = str.Split('\n').Select(s => TimeLineItem().Match(s)).Cast<Match>();
        var dict = new Dictionary<string, float>();

        foreach (var timelineItem in matches)
        {
            try
            {
                var timeline = timelineItem.Value;
                if (!LabelHeader().IsMatch(timeline)) continue;

                var time = float.Parse(Time().Match(timeline).Value);
                var name = Name().Match(timeline).Value[1..^1];

                dict[name] = time;
            }
            catch (Exception e)
            {
                Svc.Log.Warning(e, "Failed to get the time label");
            }
        }

        foreach (var timelineItem in matches)
        {
            try
            {
                var timeline = timelineItem.Value;

                var header = TimeHeader().Match(timeline).Value;

                if (string.IsNullOrEmpty(header)) continue;

                var time = float.Parse(Time().Match(header).Value, CultureInfo.InvariantCulture);
                var name = Name().Match(header).Value[1..^1];

                var timelineStr = ActionGetter().Match(timeline).Value;

                var item = string.IsNullOrEmpty(timelineStr) ? null : JObject.Parse(timelineStr);

                var rest = timeline[header.Length..];
                var type = Type().Match(rest)?.Value ?? string.Empty;
                if (type.Length > 3)
                {
                    type = type[1..^2].Split(' ').LastOrDefault() ?? string.Empty;
                }

                var jumpTimeStr = JumpTime().Match(timeline).Value;
                float? jumpTime = null;
                if (jumpTimeStr.Length > 5)
                {
                    jumpTime = float.Parse(jumpTimeStr[5..], CultureInfo.InvariantCulture);
                }
                else
                {
                    jumpTimeStr = JumpName().Match(timeline).Value;
                    if (jumpTimeStr.Length > 7)
                    {
                        if (dict.TryGetValue(jumpTimeStr[6..^1], out var t))
                        {
                            jumpTime = t;
                        }
                        else
                        {
                            Svc.Log.Warning($"Failed to parse the jump: {jumpTimeStr}");
                        }
                    }
                }

                var windowStr = WindowTime().Match(timeline).Value;
                float windowMin = 2.5f, windowMax = 2.5f;
                if (windowStr.Length > 7)
                {
                    var windowStrs = windowStr[7..].Split(',');
                    windowMin = float.Parse(windowStrs.First(), CultureInfo.InvariantCulture);
                    windowMax = float.Parse(windowStrs.Last(), CultureInfo.InvariantCulture);
                }

                result.Add(new (time, name, type, item, lang, jumpTime, windowMin, windowMax));
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "sth wrong with matching!");
            }
        }

#if DEBUG
        //foreach (var item in result)
        //{
        //    Svc.Log.Debug(item.ToString());
        //}
#endif
        return [..result.OrderBy(i => i.Time)];
    }

    [GeneratedRegex("jump [\\d\\.]+")]
    private static partial Regex JumpTime();

    [GeneratedRegex("jump \".*?\"")]
    private static partial Regex JumpName();

    [GeneratedRegex("window [\\d\\.,]+")]
    private static partial Regex WindowTime();

    [GeneratedRegex(" .*? {")]
    private static partial Regex Type();

    [GeneratedRegex("^[\\d\\.]+.*")]
    private static partial Regex TimeLineItem();

    [GeneratedRegex("^[\\d\\.]+")]
    private static partial Regex Time();

    [GeneratedRegex("\".*?\"")]
    private static partial Regex Name();

    [GeneratedRegex("^[\\d\\.]+ \".*?\"")]
    private static partial Regex TimeHeader();

    [GeneratedRegex("^[\\d\\.]+ label \".*?\"")]
    private static partial Regex LabelHeader();

    [GeneratedRegex("{.*}")]
    private static partial Regex ActionGetter();
}
