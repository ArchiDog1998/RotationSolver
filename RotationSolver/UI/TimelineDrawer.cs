using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration.Timeline;
using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;
using RotationSolver.Updaters;
using XIVConfigUI;
using XIVDrawer;
using XIVDrawer.Vfx;

namespace RotationSolver.UI;
internal static class TimelineDrawer
{
    private static float Scale => ImGuiHelpers.GlobalScale;

    private static readonly CollapsingHeaderGroup _timelineActionsList = new()
    {
        HeaderSize = FontSize.Fifth,
    };
    internal static uint _territoryId = 0;
    private static string _territorySearch = string.Empty;
    public static void DrawTimeline()
    {
        static string GetName(TerritoryType? territory)
        {
            var str = territory?.ContentFinderCondition?.Value?.Name?.RawString;
            if (str == null || string.IsNullOrEmpty(str)) return "Unnamed Duty";
            return str;
        }

        var territory = Svc.Data.GetExcelSheet<TerritoryType>();
        if (territory == null) return;

        var territories = RaidTimeUpdater.PathForRaids.Keys.OrderByDescending(i => i).Select(territory.GetRow).ToArray();

        var rightTerritory = territory?.GetRow(_territoryId);
        var name = GetName(rightTerritory);

        var imFont = DrawingExtensions.GetFont(21);
        float width = 0;
        using (var font = ImRaii.PushFont(imFont))
        {
            width = ImGui.CalcTextSize(name).X + ImGui.GetStyle().ItemSpacing.X * 2;
        }

        ImGuiHelper.DrawItemMiddle(() =>
        {
            ImGuiHelperRS.SearchCombo("##Choice the specific dungeon", name, ref _territorySearch, territories, GetName, t =>
            {
                _territoryId = t?.RowId ?? 0;
            }, UiString.ConfigWindow_Condition_DutyName.Local(), imFont, ImGuiColors.DalamudYellow);
        }, ImGui.GetWindowWidth(), width);

        RotationConfigWindow.DrawContentFinder(rightTerritory?.ContentFinderCondition.Value);

        if (!Service.Config.Timeline.TryGetValue(_territoryId, out var timeLine))
        {
            RaidTimeUpdater.DownloadTerritory(_territoryId);
            Service.Config.Timeline[_territoryId] = timeLine = [];
        }
        if (timeLine.Sum(i => i.Value.Count) == 0)
        {
            RaidTimeUpdater.DownloadTerritory(_territoryId);
        }

        ImGui.Separator();

        if (ImGui.Button(UiString.ConfigWindow_Actions_Copy.Local()))
        {
            var str = JsonConvert.SerializeObject(timeLine, Formatting.Indented);
            ImGui.SetClipboardText(str);
        }

        ImGui.SameLine();

        if (ImGui.Button(UiString.ActionSequencer_FromClipboard.Local()))
        {
            var str = ImGui.GetClipboardText();
            try
            {
                var set = JsonConvert.DeserializeObject<Dictionary<float, List<BaseTimelineItem>>>(str,
                    new BaseTimelineItemConverter(), new BaseDrawingGetterConverter(), new ITimelineConditionConverter())!;
                Service.Config.Timeline[_territoryId] = timeLine = set;
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "Failed to load the condition.");
            }
        }

        var link = RaidTimeUpdater.GetLink(_territoryId);
        if (!string.IsNullOrEmpty(link))
        {
            ImGui.SameLine();
            if (ImGui.Button(UiString.Timeline_OpenLink.Local()))
            {
                Util.OpenLink(link);
            }
        }

        ImGui.SameLine();
        ImGui.Text(UiString.TimelineRaidTime.Local() + ": " + TimeSpan.FromSeconds(DataCenter.RaidTimeRaw).ToString("hh\\:mm\\:ss\\.f"));

        RotationSolverPlugin._rotationConfigWindow?.Collection.DrawItems((int)UiString.TimelineRaidTime);

        using var table = ImRaii.Table("Rotation Solver List Timeline", 3, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.ScrollY);
        if (table)
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_Timeline_Time.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_Timeline_Name.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_Timeline_Actions.Local());

            ImGui.TableNextRow();

            foreach (var item in RaidTimeUpdater.GetRaidTime((ushort)_territoryId))
            {
                if (!item.IsShown) continue;

                if (!timeLine.TryGetValue(item.Time, out var timeLineItems))
                {
                    timeLine[item.Time] = timeLineItems = [];
                }

                ImGui.TableNextColumn();

                var color = item.IsInWindow ? ImRaii.PushColor(ImGuiCol.Text,
                    ImGuiColors.HealerGreen) : null;

                ImGui.Text(TimeSpan.FromSeconds(item.Time).ToString("hh\\:mm\\:ss\\.f"));

                ImGui.TableNextColumn();
                AddButton();
                ImGui.SameLine();

                var itemName = item.Name;
#if DEBUG
                itemName += $" ({item.WindowMin}, {item.WindowMax})";
#endif
                ImGui.TextWrapped(itemName);
                color?.Dispose();

                ImGui.TableNextColumn();

                for (int i = 0; i < timeLineItems.Count; i++)
                {
                    if (i != 0)
                    {
                        ImGui.Separator();
                    }
                    var timeLineItem = timeLineItems[i];

                    void Delete()
                    {
                        timeLineItems.RemoveAt(i);
                    };

                    void Up()
                    {
                        timeLineItems.RemoveAt(i);
                        timeLineItems.Insert(Math.Max(0, i - 1), timeLineItem);
                    };

                    void Down()
                    {
                        timeLineItems.RemoveAt(i);
                        timeLineItems.Insert(Math.Min(timeLineItems.Count, i + 1), timeLineItem);
                    }

                    void Execute()
                    {
                        Task.Run(async () =>
                        {
                            timeLineItem.OnEnable();
                            await Task.Delay(3000);
                            timeLineItem.OnDisable();
                        });
                    }

                    var key = $"TimelineItem Pop Up: {timeLineItem.GetHashCode()}";

                    ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                        (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]),
                        (UiString.ConfigWindow_Actions_MoveUp.Local(), Up, ["↑"]),
                        (UiString.ConfigWindow_Actions_MoveDown.Local(), Down, ["↓"]),
                        (UiString.TimelineExecute.Local(), Execute, ["→"]));

                    ConditionDrawer.DrawCondition(timeLineItem.InPeriod(item));

                    ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                        (Delete, [VirtualKey.DELETE]),
                        (Up, [VirtualKey.UP]),
                        (Down, [VirtualKey.DOWN]),
                        (Execute, [VirtualKey.RIGHT]));

                    ImGui.SameLine();

                    using var grp = ImRaii.Group();

                    DrawTimelineItem(timeLineItem, item);
                }

                ImGui.TableNextRow();

                void AddButton()
                {
                    if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddTimelineButton" + item.Time))
                    {
                        ImGui.OpenPopup("PopupTimelineButton" + item.Time);
                    }
                    ImguiTooltips.HoveredTooltip(UiString.AddTimelineButton.Local());

                    using var popUp = ImRaii.Popup("PopupTimelineButton" + item.Time);
                    if (popUp)
                    {
                        AddOneCondition<ActionTimelineItem>();
                        AddOneCondition<StateTimelineItem>();
                        AddOneCondition<DrawingTimeline>();
                        AddOneCondition<MacroTimelineItem>();
                        AddOneCondition<MoveTimelineItem>();
                    }

                    void AddOneCondition<T>() where T : BaseTimelineItem
                    {
                        if (ImGui.Selectable(typeof(T).Local()))
                        {
                            timeLineItems.Add(Activator.CreateInstance<T>());
                            ImGui.CloseCurrentPopup();
                        }
                    }
                }
            }
        }
    }

    private static void DrawTimelineItem(BaseTimelineItem timeLineItem, TimelineItem item)
    {
        var isOpen = _openedTab == timeLineItem.GetHashCode();
        if (ImGuiEx.IconButton(FontAwesomeIcon.Cog, "Condition icon." + timeLineItem.GetHashCode()))
        {
            _openedTab = isOpen ? 0 : timeLineItem.GetHashCode();
        }
        ImguiTooltips.HoveredTooltip(UiString.TimelineItemCondition.Local());

        ImGui.SameLine();

        var time = timeLineItem.Time;
        if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Seconds, $" ##Time{timeLineItem.GetHashCode()}", ref time, UiString.TimelineItemTime.Local()))
        {
            timeLineItem.Time = time;
        }

        if (timeLineItem is ActionTimelineItem actionItem)
        {
            if (DataCenter.RightNowRotation != null)
            {
                var popUpKey = $"Action Finder{timeLineItem.GetHashCode()}";
                ConditionDrawer.ActionSelectorPopUp(popUpKey, _timelineActionsList, DataCenter.RightNowRotation, item => actionItem.ID = (ActionID)item.ID);

                if (actionItem.ID.GetTexture(out var icon) || ImageLoader.GetTexture(4, out icon))
                {
                    ImGui.SameLine();
                    var cursor = ImGui.GetCursorPos();
                    if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * ConditionDrawer.IconSize, timeLineItem.GetHashCode().ToString()))
                    {
                        if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
                    }
                    ImGuiHelper.DrawActionOverlay(cursor, ConditionDrawer.IconSize, 1);
                }
            }
        }
        else if (timeLineItem is StateTimelineItem stateItem)
        {
            var state = stateItem.State;
            ImGui.SameLine();
            if (ConditionDrawer.DrawByteEnum($"##AutoState{timeLineItem.GetHashCode()}", ref state))
            {
                stateItem.State = state;
            }
        }
        else if (timeLineItem is DrawingTimeline drawingItem)
        {
            DrawDrawingTimeline(drawingItem, item);
        }
        else if (timeLineItem is MacroTimelineItem macroItem)
        {
            ImGui.SameLine();

            var macro = macroItem.Macro;
            if (ImGui.InputTextMultiline(UiString.ConfigWindow_About_Macros.Local() + ": ##" + macroItem.GetHashCode(), ref macro, 500, new Vector2( -1, 50)))
            {
                macroItem.Macro = macro;
            }
        }
        else if (timeLineItem is MoveTimelineItem moveItem)
        {
            ImGui.SameLine();
            if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddPoint" + moveItem.GetHashCode())
                && Player.Available)
            {
                moveItem.Points.Add(Player.Object.Position);
            }

            for (int i = 0; i < moveItem.Points.Count; i++)
            {
                var point = moveItem.Points[i];

                void Delete()
                {
                    moveItem.Points.RemoveAt(i);
                };

                void Up()
                {
                    moveItem.Points.RemoveAt(i);
                    moveItem.Points.Insert(Math.Max(0, i - 1), point);
                };

                void Down()
                {
                    moveItem.Points.RemoveAt(i);
                    moveItem.Points.Insert(Math.Min(moveItem.Points.Count, i + 1), point);
                }

                var key = $"Point Pop Up: {moveItem.Points.GetHashCode()}";

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                    (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]),
                    (UiString.ConfigWindow_Actions_MoveUp.Local(), Up, ["↑"]),
                    (UiString.ConfigWindow_Actions_MoveDown.Local(), Down, ["↓"]));

                if (ImageLoader.GetTexture(10, out var texture))
                {
                    if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, ConditionDrawer.IconSize * Vector2.One, "Position " + moveItem.GetHashCode() + i))
                    {
                        if (_previewItems == null)
                        {
                            _previewItems = [new StaticVfx(GroundOmenFriendly.BasicCircle.Omen(), point, 0, Vector3.One)];
                        }
                        else
                        {
                            foreach (var preview in _previewItems)
                            {
                                preview.Dispose();
                            }
                            _previewItems = null;
                        }
                    }
                }

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                    (Delete, [VirtualKey.DELETE]),
                    (Up, [VirtualKey.UP]),
                    (Down, [VirtualKey.DOWN]));

                ImGui.SameLine();

                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePosition.Local(), ref point, "Pos" + moveItem.GetHashCode() + i, "X", "Y", "Z", () => Player.Object?.Position ?? default))
                {
                    moveItem.Points[i] = point;
                }
            }
        }

        if (!isOpen) return;

        ImGui.Spacing();
        ImGui.Spacing();

        TimelineConditionDraw(timeLineItem.Condition, item);
    }
    private static void DrawDrawingTimeline(DrawingTimeline drawingItem, TimelineItem timelineItem)
    {
        var duration = drawingItem.Duration;
        if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Seconds, $"{UiString.TimelineDuration.Local()}##Duration{drawingItem.GetHashCode()}", ref duration, UiString.TimelineItemDuration.Local()))
        {
            drawingItem.Duration = duration;
        }

        ImGui.SameLine();

        AddButton();

        for (int i = 0; i < drawingItem.DrawingGetters.Count; i++)
        {
            if(i != 0)
            {
                ImGui.Spacing();
            }
            var item = drawingItem.DrawingGetters[i];

            void Delete()
            {
                drawingItem.DrawingGetters.RemoveAt(i);
            };

            void Up()
            {
                drawingItem.DrawingGetters.RemoveAt(i);
                drawingItem.DrawingGetters.Insert(Math.Max(0, i - 1), item);
            };

            void Down()
            {
                drawingItem.DrawingGetters.RemoveAt(i);
                drawingItem.DrawingGetters.Insert(Math.Min(drawingItem.DrawingGetters.Count, i + 1), item);
            }

            void Copy()
            {
                var str = JsonConvert.SerializeObject(drawingItem.DrawingGetters[i], Formatting.Indented);
                ImGui.SetClipboardText(str);
            }

            var key = $"DrawingItem Pop Up: {item.GetHashCode()}";

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]),
                (UiString.ConfigWindow_Actions_MoveUp.Local(), Up, ["↑"]),
                (UiString.ConfigWindow_Actions_MoveDown.Local(), Down, ["↓"]),
                (UiString.ConfigWindow_Actions_Copy.Local(), Copy, ["Ctrl"]));

            if (ImageLoader.GetTexture(30, out var texture))
            {
                if (ImGuiHelper.SilenceImageButton(texture.ImGuiHandle, Vector2.One * ConditionDrawer.IconSize, false, $"Icon :{item.GetHashCode()}"))
                {
                    if (_previewItems == null)
                    {
                        _previewItems = item.GetDrawing();
                    }
                    else
                    {
                        foreach (var preview in _previewItems)
                        {
                            preview.Dispose();
                        }
                        _previewItems = null;
                    }
                }
            }

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                (Delete, [VirtualKey.DELETE]),
                (Up, [VirtualKey.UP]),
                (Down, [VirtualKey.DOWN]),
                (Copy, [VirtualKey.CONTROL]));

            ImGui.SameLine();

            using var grp = ImRaii.Group();

            DrawingGetterDraw(item, timelineItem.ActionIDs);
        }

        void AddButton()
        {
            if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddDrawingButton" + drawingItem.GetHashCode()))
            {
                ImGui.OpenPopup("PopupDrawingButton" + drawingItem.GetHashCode());
            }
            ImguiTooltips.HoveredTooltip(UiString.AddDrawingTimelineButton.Local());

            using var popUp = ImRaii.Popup("PopupDrawingButton" + drawingItem.GetHashCode());
            if (popUp)
            {
                AddOneCondition<StaticDrawingGetter>();
                AddOneCondition<ObjectDrawingGetter>();

                if (timelineItem.ActionIDs.Length > 0)
                {
                    AddOneCondition<ActionDrawingGetter>();
                }
                if (ImGui.Selectable(UiString.ActionSequencer_FromClipboard.Local()))
                {
                    var str = ImGui.GetClipboardText();
                    try
                    {
                        var s = JsonConvert.DeserializeObject<BaseDrawingGetter>
                            (str, new BaseDrawingGetterConverter())!;
                        drawingItem.DrawingGetters.Add(s);
                    }
                    catch (Exception ex)
                    {
                        Svc.Log.Warning(ex, "Failed to load the condition.");
                    }
                    ImGui.CloseCurrentPopup();
                }
            }

            void AddOneCondition<T>() where T : BaseDrawingGetter
            {
                if (ImGui.Selectable(typeof(T).Local()))
                {
                    drawingItem.DrawingGetters.Add(Activator.CreateInstance<T>());
                    ImGui.CloseCurrentPopup();
                }
            }
        }
    }

    private static void TimelineConditionDraw(ITimelineCondition con, TimelineItem timelineItem)
    {
        if (con is TimelineConditionSet set)
        {
            ConditionDrawer.DrawCondition(set.IsTrue(timelineItem));

            ImGui.SameLine();
            using var grp = ImRaii.Group();
            ConditionDrawer.DrawByteEnum($"##Rule{set.GetHashCode()}", ref set.Type);
            ImGui.SameLine();
            AddButton();

            for (int i = 0; i < set.Conditions.Count; i++)
            {
                var condition = set.Conditions[i];

                void Delete()
                {
                    set.Conditions.RemoveAt(i);
                };

                void Up()
                {
                    set.Conditions.RemoveAt(i);
                    set.Conditions.Insert(Math.Max(0, i - 1), condition);
                };

                void Down()
                {
                    set.Conditions.RemoveAt(i);
                    set.Conditions.Insert(Math.Min(set.Conditions.Count, i + 1), condition);
                }

                void Copy()
                {
                    var str = JsonConvert.SerializeObject(set.Conditions[i], Formatting.Indented);
                    ImGui.SetClipboardText(str);
                }

                var key = $"Condition Pop Up: {condition.GetHashCode()}";

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                    (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]),
                    (UiString.ConfigWindow_Actions_MoveUp.Local(), Up, ["↑"]),
                    (UiString.ConfigWindow_Actions_MoveDown.Local(), Down, ["↓"]),
                    (UiString.ConfigWindow_Actions_Copy.Local(), Copy, ["Ctrl"]));


                ConditionDrawer.DrawCondition(condition.IsTrue(timelineItem));

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                    (Delete, [VirtualKey.DELETE]),
                    (Up, [VirtualKey.UP]),
                    (Down, [VirtualKey.DOWN]),
                    (Copy, [VirtualKey.CONTROL]));

                ImGui.SameLine();
                using var g = ImRaii.Group();

                TimelineConditionDraw(condition, timelineItem);
            }

            void AddButton()
            {
                if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddButton" + set.GetHashCode().ToString()))
                {
                    ImGui.OpenPopup("Popup" + set.GetHashCode().ToString());
                }
                ImguiTooltips.HoveredTooltip(UiString.AddTimelineCondition.Local());

                using var popUp = ImRaii.Popup("Popup" + set.GetHashCode().ToString());
                if (popUp)
                {
                    AddOneCondition<TimelineConditionSet>();
                    AddOneCondition<TimelineConditionAction>();
                    AddOneCondition<TimelineConditionTargetCount>();

                    if (ImGui.Selectable(UiString.ActionSequencer_FromClipboard.Local()))
                    {
                        var str = ImGui.GetClipboardText();
                        try
                        {
                            var s = JsonConvert.DeserializeObject<ITimelineCondition>
                                (str, new ITimelineConditionConverter())!;
                            set.Conditions.Add(s);
                        }
                        catch (Exception ex)
                        {
                            Svc.Log.Warning(ex, "Failed to load the condition.");
                        }
                        ImGui.CloseCurrentPopup();
                    }
                }

                void AddOneCondition<T>() where T : ITimelineCondition
                {
                    if (ImGui.Selectable(typeof(T).Local()))
                    {
                        set.Conditions.Add(Activator.CreateInstance<T>());
                        ImGui.CloseCurrentPopup();
                    }
                }
            }
        }
        else if (con is TimelineConditionTargetCount target)
        {
            var count = target.Count;
            if (ConditionDrawer.DrawDragInt(UiString.TimelineTargetCount.Local() +
                ": ##" + target.GetHashCode(), ref count))
            {
                target.Count = count;
            }

            DrawObjectGetter(target.Getter, UiString.TimelineTargetGetter.Local());
        }
        else if (con is TimelineConditionAction action)
        {
            var index = Array.IndexOf(timelineItem.ActionIDs, action.ActionID);
            var actionNames = timelineItem.ActionIDs.Select(i => (Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()?.GetRow(i)?.Name.RawString ?? "Unnamed Action") + $" ({i})").ToArray();

            ImGui.SameLine();
            if (ImGuiHelperRS.SelectableCombo("Action ##Select Action" + action.GetHashCode(), actionNames, ref index))
            {
                action.ActionID = timelineItem.ActionIDs[index];
            }
        }
        else if (con is TimelineConditionMapEffect map)
        {
            var duration = map.TimeDuration;
            if (ConditionDrawer.DrawDragFloat2(ConfigUnitType.Seconds, UiString.TimelineEffectDuration.Local(), ref duration, "Effect duration" + map.GetHashCode(), "Start", "End"))
            {
                map.TimeDuration = duration;
            }

            var param = map.Position;
            if (ConditionDrawer.DrawDragInt(UiString.TimelinePosition.Local() + "##" + map.GetHashCode(), ref param))
            {
                map.Position = (ushort)param;
            }

            param = map.Param1;
            if (ConditionDrawer.DrawDragInt("Param1##" + map.GetHashCode(), ref param))
            {
                map.Param1 = (ushort)param;
            }

            param = map.Param2;
            if (ConditionDrawer.DrawDragInt("Param2##" + map.GetHashCode(), ref param))
            {
                map.Param2 = (ushort)param;
            }
        }
    }

    static readonly IEnumerable<FieldInfo> _omenInfo = typeof(GroundOmenHostile).GetRuntimeFields()
        .Concat(typeof(GroundOmenNone).GetRuntimeFields())
        .Concat(typeof(GroundOmenFriendly).GetRuntimeFields());

    static readonly string[] _omenNames = _omenInfo
        .Select(f => f.GetValue(null))
        .OfType<string>().ToArray();

    static readonly string[] _omenShowNames = _omenInfo
        .Select(f => f.Name).ToArray();

    static readonly IEnumerable<FieldInfo> _actorInfo = typeof(LockOnOmen).GetRuntimeFields().Union(typeof(ChannelingOmen).GetRuntimeFields());
    static readonly string[] _actorNames = _actorInfo
        .Select(f => f.GetValue(null))
        .OfType<string>().ToArray();

    static readonly string[] _actorShowNames = _actorInfo
        .Select(f => f.Name).ToArray();

    private static IDisposable[]? _previewItems = null;
    static int _openedTab = 0;
    private static void DrawingGetterDraw(BaseDrawingGetter drawing, uint[] actionIds)
    {       
        var enable = drawing.Enable;
        if(ImGui.Checkbox("##Enable" + drawing.GetHashCode(), ref enable))
        {
            drawing.Enable = enable;
        }

        ImGui.SameLine();

        var name = drawing.Name;
        ImGui.SetNextItemWidth(300 * Scale);
        if (ImGui.InputText("##" + drawing.GetHashCode(), ref name, 256))
        {
            drawing.Name = name;
        }

        ImGui.SameLine();

        var isOpen = _openedTab == drawing.GetHashCode();
        if (ImGuiEx.IconButton(FontAwesomeIcon.Cog, "Config icon." + drawing.GetHashCode()))
        {
            _openedTab = isOpen ? 0 : drawing.GetHashCode();
        }
        if (!isOpen) return;
        
        if (drawing is StaticDrawingGetter staticDrawing)
        {
            var index = Array.IndexOf(_omenNames, staticDrawing.Path.UnOmen());
            if (ImGuiHelperRS.SelectableCombo("##PathName" + drawing.GetHashCode(), _omenShowNames, ref index))
            {
                staticDrawing.Path = _omenNames[index].Omen();
            }

            ImGui.SameLine();

            var isTarget = staticDrawing.PlaceOnObject;
            if (ImGui.Checkbox(UiString.TimelinePlaceOnTarget.Local() + "##" + drawing.GetHashCode(), ref isTarget))
            {
                staticDrawing.PlaceOnObject = isTarget;
            }

            if (isTarget)
            {
                var pos = staticDrawing.Position;
                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePositionOffset.Local() + ":　", ref pos, drawing.GetHashCode().ToString() + "PositionOffset", "X", "Y", "Z"))
                {
                    staticDrawing.Position = pos;
                }
            }
            else
            {
                var pos = staticDrawing.Position;
                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePosition.Local() + ":　", ref pos, drawing.GetHashCode().ToString() + "Position", "X", "Y", "Z", () => Player.Object?.Position ?? default))
                {
                    staticDrawing.Position = pos;
                }
            }


            var rot = staticDrawing.Rotation / MathF.PI * 180f;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Degree, "Rotation: ##" + drawing.GetHashCode(), ref rot))
            {
                staticDrawing.Rotation = rot * MathF.PI / 180f;
            }

            var scale = staticDrawing.Scale;
            if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelineScale.Local() + ":　", ref scale, drawing.GetHashCode().ToString() + "Scale", "X", "Y", "Z"))
            {
                staticDrawing.Scale = scale;
            }

            if (isTarget)
            {
                DrawObjectGetter(staticDrawing.ObjectGetter, UiString.TimelineObjectGetter.Local());
            }

            DrawTextDrawing(staticDrawing.Text, UiString.TimelineShowText.Local() + ": ");
        }
        else if (drawing is ObjectDrawingGetter objectDrawing)
        {
            var index = objectDrawing.IsActorEffect ? 1 : 0;
            if (ImGuiHelperRS.SelectableCombo("##ActorType" + drawing.GetHashCode(), [UiString.TimelineGround.Local(), UiString.TimelineActor.Local()], ref index))
            {
                objectDrawing.IsActorEffect = index != 0;
            }

            ImGui.SameLine();
            if (objectDrawing.IsActorEffect)
            {
                index = Array.IndexOf(_actorNames.Select(n => n.StartsWith("chn_") ? n.Channeling() : n.LockOn()).ToArray(), objectDrawing.Path);
                if (ImGuiHelperRS.SelectableCombo("##PathName" + drawing.GetHashCode(), _actorShowNames, ref index))
                {
                    var actorName = _actorNames[index];
                    objectDrawing.Path = actorName.StartsWith("chn_") ? actorName.Channeling() : actorName.LockOn();
                }
            }
            else
            {
                index = Array.IndexOf(_omenNames, objectDrawing.Path.UnOmen());
                if (ImGuiHelperRS.SelectableCombo("##PathName" + drawing.GetHashCode(), _omenShowNames, ref index))
                {
                    objectDrawing.Path = _omenNames[index].Omen();
                }

                var rot = objectDrawing.Rotation / MathF.PI * 180f;
                if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Degree, UiString.TimelineRotation.Local() + ": ##" + drawing.GetHashCode(), ref rot))
                {
                    objectDrawing.Rotation = rot * MathF.PI / 180f;
                }

                var pos = objectDrawing.Position;
                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePosition.Local() + ":　", ref pos, drawing.GetHashCode().ToString() + "Position", "X", "Y", "Z", () => Player.Object?.Position ?? default))
                {
                    objectDrawing.Position = pos;
                }

                var scale = objectDrawing.Scale;
                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelineScale.Local() + ":　", ref scale, drawing.GetHashCode().ToString() + "Scale", "X", "Y", "Z"))
                {
                    objectDrawing.Scale = scale;
                }
            }

            DrawObjectGetter(objectDrawing.ObjectGetter, UiString.TimelineObjectGetter.Local());
            DrawTextDrawing(objectDrawing.ObjectText, UiString.TimelineShowText.Local());

            var check = objectDrawing.GetATarget;
            if (ImGui.Checkbox(UiString.TimelineNeedATarget.Local() + ": ##" + drawing.GetHashCode(), ref check))
            {
                objectDrawing.GetATarget = check;
            }

            if (!check) return;

            ImGui.SameLine();
            check = objectDrawing.IsTargetByTarget;
            if (ImGui.Checkbox(UiString.TimelineTargetByTarget.Local() + ": ##" + drawing.GetHashCode(), ref check))
            {
                objectDrawing.IsTargetByTarget = check;
            }

            if (!check)
            {
                DrawObjectGetter(objectDrawing.TargetGetter, UiString.TimelineTargetGetter.Local());
            }

            DrawTextDrawing(objectDrawing.TargetText, UiString.TimelineShowText.Local());
        }
        else if (drawing is ActionDrawingGetter actionDrawing)
        {
            var index = Array.IndexOf(_omenNames, actionDrawing.Path.UnOmen());
            if (ImGuiHelperRS.SelectableCombo("##PathName" + drawing.GetHashCode(), _omenShowNames, ref index))
            {
                actionDrawing.Path = _omenNames[index].Omen();
            }

            index = Array.IndexOf(actionIds, actionDrawing.ActionID);
            var actionNames = actionIds.Select(i => Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()?.GetRow(i)?.Name.RawString ?? "Unnamed Action").ToArray();

            ImGui.SameLine();
            if (ImGuiHelperRS.SelectableCombo("Action ##Select Action" + drawing.GetHashCode(), actionNames, ref index))
            {
                actionDrawing.ActionID = actionIds[index];
            }

            var rot = actionDrawing.Rotation / MathF.PI * 180f;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Degree, UiString.TimelineRotation.Local() + ": ##" + drawing.GetHashCode(), ref rot))
            {
                actionDrawing.Rotation = rot * MathF.PI / 180f;
            }

            var pos = actionDrawing.Position;
            if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePosition.Local() + ": ", ref pos, drawing.GetHashCode().ToString(), "X", "Y", "Z", () => Player.Object?.Position ?? default))
            {
                actionDrawing.Position = pos;
            }

            var scale = actionDrawing.X;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Yalms, UiString.TimelineScale.Local() + " X:　##" + drawing.GetHashCode(), ref scale))
            {
                actionDrawing.X = scale;
            }

            scale = actionDrawing.Y;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Yalms, UiString.TimelineScale.Local() + " Y:　##" + drawing.GetHashCode(), ref scale))
            {
                actionDrawing.Y = scale;
            }

            DrawObjectGetter(actionDrawing.ObjectGetter, UiString.TimelineObjectGetter.Local());
        }
    }

    private static void DrawObjectGetter(ObjectGetter getter, string getterName)
    {
        if (ImGui.Button(getterName + "##" + getter.GetHashCode()))
        {
            if (_previewItems == null)
            {
                _previewItems = [.. Svc.Objects.Where(getter.CanGet).Select(b => new StaticVfx(GroundOmenNone.Circle.Omen(), b, Vector3.One * b.HitboxRadius))];
            }
            else
            {
                foreach (var preview in _previewItems)
                {
                    preview.Dispose();
                }
                _previewItems = null;
            }
        }

        ImGui.SameLine();

        var check = getter.Type;

        if (ConditionDrawer.DrawByteEnum("Object Type: ##" + getter.GetHashCode(), ref check))
        {
            getter.Type = check;
        }

        using var indent = ImRaii.PushIndent();

        ImGui.SameLine();

        var key = "Status PopUp" + getter.GetHashCode();
        var status = Svc.Data.GetExcelSheet<Status>()?.GetRow(getter.Status);
        if (ImageLoader.GetTexture(getter.Status == 0 ? 16220 : status?.Icon ?? 10100, out var texture, 10100))
        {
            if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, new Vector2(24, 32) * Scale, "Status" + getter.Status.ToString()))
            {
                ImGui.OpenPopup(key);
            }

            ImGuiHelper.ExecuteHotKeysPopup(key + "Edit", string.Empty, $"{status?.Name ?? "Unknown"} ({getter.Status})", false,
                (() => getter.Status = 0, [VirtualKey.DELETE]));
        }

        switch (check)
        {
            case ObjectType.Myself:
                return;

            case ObjectType.PlayerCharactor:

                var size = Vector2.One * 24 * Scale;

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_tank.png", out var overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Tank, "Tank##" + getter.GetHashCode()))
                    {
                        getter.Tank = !getter.Tank;
                    }
                }

                ImGui.SameLine();

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_healer.png", out overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Healer, "Healer##" + getter.GetHashCode()))
                    {
                        getter.Healer = !getter.Healer;
                    }
                }

                ImGui.SameLine();

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_dps.png", out overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Melee, "Melee##" + getter.GetHashCode()))
                    {
                        getter.Melee = !getter.Melee;
                    }
                }

                ImGui.SameLine();

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_ranged.png", out overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Range, "Range##" + getter.GetHashCode()))
                    {
                        getter.Range = !getter.Range;
                    }
                }

                ImGui.SameLine();

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_dps_magic.png", out overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Caster, "Caster##" + getter.GetHashCode()))
                    {
                        getter.Caster = !getter.Caster;
                    }
                }
                break;

            case ObjectType.GameObject:
            case ObjectType.BattleCharactor:
                ImGui.SetNextItemWidth(150 * Scale);
                var name = getter.DataID;
                if (ImGui.InputText("Data ID## " + getter.GetHashCode(), ref name, 256))
                {
                    getter.DataID = name;
                }

                break;
        }

        RotationConfigWindow.StatusPopUp(key, RotationConfigWindow.BadStatus, ref _statusSearching, s => getter.Status = s.RowId);

        if (getter.Status != 0)
        {
            var time = getter.StatusTime;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Seconds, UiString.ConfigWindow_Timeline_Time.Local() + ": ##" + getter.GetHashCode(), ref time))
            {
                getter.StatusTime = time;
            }
        }

        var duration = getter.TimeDuration;
        if (ConditionDrawer.DrawDragFloat2(ConfigUnitType.Seconds, UiString.TimelineEffectDuration.Local(), ref duration, "Effect duration" + getter.GetHashCode(), "Start", "End"))
        {
            getter.TimeDuration = duration;
        }

        if (!ImGui.CollapsingHeader("Effect##" + getter.GetHashCode())) return;

        var vfx = getter.VfxPath;
        ImGui.SetNextItemWidth(300 * Scale);
        if (ImGui.InputText("Vfx ## " + getter.GetHashCode(), ref vfx, 256))
        {
            getter.VfxPath = vfx;
        }

        ImGui.NewLine();
        var param = (int)getter.ObjectEffect1;
        if (ConditionDrawer.DrawDragInt("Param1##" + getter.GetHashCode(), ref param))
        {
            getter.ObjectEffect1 = (ushort)param;
        }

        param = getter.ObjectEffect2;
        if (ConditionDrawer.DrawDragInt("Param2##" + getter.GetHashCode(), ref param))
        {
            getter.ObjectEffect2 = (ushort)param;
        }
    }

    private static void DrawTextDrawing(TextDrawing textDrawing, string name)
    {
        var text = textDrawing.Text;
        ImGui.SetNextItemWidth(300 * Scale);
        if (ImGui.InputText(name + "##" + textDrawing.GetHashCode(), ref text, 256))
        {
            textDrawing.Text = text;
        }

        if (string.IsNullOrEmpty(text)) return;

        using var indent = ImRaii.PushIndent();

        var positionOffset = textDrawing.PositionOffset;
        if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePositionOffset.Local() + ": ", ref positionOffset, textDrawing.GetHashCode().ToString(), "X", "Y", "Z"))
        {
            textDrawing.PositionOffset = positionOffset;
        }

        ImGui.SetNextItemWidth(Scale * 150);
        var scale = textDrawing.Scale;
        if (ImGui.DragFloat($"{UiString.TimelineScale.Local()}##{textDrawing.GetHashCode()}", ref scale, 0.1f, 0.1f, 20, $"{scale:F2}{ConfigUnitType.Percent.ToSymbol()}"))
        {
            textDrawing.Scale = scale;
        }

        var corner = textDrawing.Corner;
        if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Pixels, UiString.TimelineCorner.Local() + "##" + textDrawing.GetHashCode(), ref corner))
        {
            textDrawing.Corner = corner;
        }

        var padding = textDrawing.Padding;
        if (ConditionDrawer.DrawDragFloat2(ConfigUnitType.Pixels, UiString.TimelinePadding.Local() + ": ", ref padding, textDrawing.GetHashCode().ToString(), "X", "Y"))
        {
            textDrawing.Padding = padding;
        }

        var value = textDrawing.Color;
        ImGui.SetNextItemWidth(150 * 1.5f * Scale);
        if (ImGui.ColorEdit4($"{UiString.TimelineColor.Local()}##{textDrawing.GetHashCode()}", ref value))
        {
            textDrawing.Color = value;
        }

        value = textDrawing.BackgroundColor;
        ImGui.SetNextItemWidth(150 * 1.5f * Scale);
        if (ImGui.ColorEdit4($"{UiString.TimelineBackgroundColor.Local()}##{textDrawing.GetHashCode()}", ref value))
        {
            textDrawing.BackgroundColor = value;
        }
    }

    static string _statusSearching = string.Empty;
}
