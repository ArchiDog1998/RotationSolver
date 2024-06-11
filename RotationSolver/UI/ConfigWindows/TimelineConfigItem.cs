using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Timeline;
using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using RotationSolver.Updaters;
using System.ComponentModel;
using XIVConfigUI;
using XIVConfigUI.Attributes;
using XIVDrawer;

namespace RotationSolver.UI.ConfigWindows;

[Description("Timeline")]
public class TimelineConfigItem : ConfigWindowItemRS
{
    internal static uint _territoryId = 0;
    private static string _territorySearch = string.Empty;
    public override uint Icon => 73;
    public override string Description => UiString.Item_Timeline.Local();

    public override void Draw(ConfigWindow window)
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
            width = ImGui.CalcTextSize(name).X + (ImGui.GetStyle().ItemSpacing.X * 2);
        }

        ImGuiHelper.DrawItemMiddle(() =>
        {
            ImGuiHelperRS.SearchCombo("##Choice the specific dungeon", name, ref _territorySearch, territories, GetName, t =>
            {
                _territoryId = t?.RowId ?? 0;
            }, UiString.ConfigWindow_Condition_DutyName.Local(), imFont, ImGuiColors.DalamudYellow);
        }, ImGui.GetWindowWidth(), width);

        ImGuiHelperRS.DrawContentFinder(rightTerritory?.ContentFinderCondition.Value);

        var territoryConfig = OtherConfiguration.GetTerritoryConfigById(_territoryId);

        ImGui.Separator();

        if (ImGui.Button(LocalString.CopyToClipboard.Local()))
        {
            var str = JsonConvert.SerializeObject(territoryConfig, Formatting.Indented);
            ImGui.SetClipboardText(str);
        }

        ImGui.SameLine();

        if (ImGui.Button(LocalString.FromClipboard.Local()))
        {
            var str = ImGui.GetClipboardText();
            try
            {
                OtherConfiguration.SetTerritoryConfigById(_territoryId, str, true);
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

        var boolean = territoryConfig.AutoHeal;
        if(ImGui.Checkbox(UiString.AutoHealOnTimeline.Local(), ref boolean))
        {
            territoryConfig.AutoHeal = boolean;
        }

        ImGui.SameLine();
        boolean = territoryConfig.AutoDefense;
        if (ImGui.Checkbox(UiString.AutoDefenseOnTimeline.Local(), ref boolean))
        {
            territoryConfig.AutoDefense = boolean;
        }

        ImGuiHelperRS.DrawSupporterWarning();

        using var table = ImRaii.Table("Rotation Solver List Timeline", 4, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.ScrollY);
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

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_Timeline_JobActions.Local());

            ImGui.TableNextRow();

            foreach (var item in RaidTimeUpdater.GetRaidTime((ushort)_territoryId))
            {
                if (!item.IsShown) continue;

                ImGui.TableNextColumn();

                var color = item.IsInWindow ? ImRaii.PushColor(ImGuiCol.Text,
                    ImGuiColors.HealerGreen) : null;

                ImGui.Text(TimeSpan.FromSeconds(item.Time).ToString("hh\\:mm\\:ss\\.f"));

                ImGui.TableNextColumn();

                var itemName = item.Name;
#if DEBUG
                itemName += $" ({item.WindowMin}, {item.WindowMax})";
#endif
                ImGui.TextWrapped(itemName);
                color?.Dispose();

                ImGui.TableNextColumn();

                if (!territoryConfig.Config.Timeline.TryGetValue(item.Time, out var timeLineItems))
                {
                    territoryConfig.Config.Timeline[item.Time] = timeLineItems = [];
                }

                DrawItems(timeLineItems, item, false);

                ImGui.TableNextColumn();

                if (!territoryConfig.JobConfig.Timeline.TryGetValue(item.Time, out timeLineItems))
                {
                    territoryConfig.JobConfig.Timeline[item.Time] = timeLineItems = [];
                }

                DrawItems(timeLineItems, item, true);

                ImGui.TableNextRow();

                static void DrawItems(List<BaseTimelineItem> timeLineItems, TimelineItem item, bool isJob)
                {
                    AddButton();
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
                        }

                        void Up()
                        {
                            timeLineItems.RemoveAt(i);
                            timeLineItems.Insert(Math.Max(0, i - 1), timeLineItem);
                        }

                        void Down()
                        {
                            timeLineItems.RemoveAt(i);
                            timeLineItems.Insert(Math.Min(timeLineItems.Count, i + 1), timeLineItem);
                        }

                        void Execute()
                        {
                            Task.Run(async () =>
                            {
                                timeLineItem.TerritoryAction.Enable();
                                await Task.Delay(3000);
                                timeLineItem.TerritoryAction.Disable();
                            });
                        }

                        var key = $"TimelineItem Pop Up: {timeLineItem.GetHashCode()}";

                        ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                            (LocalString.Remove.Local(), Delete, ["Delete"]),
                            (LocalString.MoveUp.Local(), Up, ["↑"]),
                            (LocalString.MoveDown.Local(), Down, ["↓"]),
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

                    void AddButton()
                    {
                        if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddTimelineButton" + item.Time + isJob))
                        {
                            ImGui.OpenPopup("PopupTimelineButton" + item.Time + isJob);
                        }
                        ImguiTooltips.HoveredTooltip(UiString.AddTerritoryActionButton.Local());

                        using var popUp = ImRaii.Popup("PopupTimelineButton" + item.Time + isJob);
                        if (popUp)
                        {
                            if (isJob)
                            {
                                AddOneCondition<ActionTimelineItem>();
                            }
                            AddOneCondition<StateTimelineItem>();
                            AddOneCondition<DrawingTimelineItem>();
                            AddOneCondition<MacroTimelineItem>();
                            AddOneCondition<MoveTimelineItem>();
                            AddOneCondition<PathfindTimelineItem>();
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
    }

    private static void DrawTimelineItem(BaseTimelineItem timeLineItem, TimelineItem item)
    {
        var isOpen = TerritoryActionDrawer._openedTab == timeLineItem.GetHashCode();
        if (ImGuiEx.IconButton(FontAwesomeIcon.Cog, "Condition icon." + timeLineItem.GetHashCode()))
        {
            TerritoryActionDrawer._openedTab = isOpen ? 0 : timeLineItem.GetHashCode();
        }
        ImguiTooltips.HoveredTooltip(UiString.TimelineItemCondition.Local());

        ImGui.SameLine();

        var time = timeLineItem.Time;
        if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Seconds, $" ##Time{timeLineItem.GetHashCode()}", ref time, UiString.TimelineItemTime.Local()))
        {
            timeLineItem.Time = time;
        }

        var duration = timeLineItem.Duration;
        if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Seconds, $"##Duration{timeLineItem.GetHashCode()}", ref duration, UiString.TimelineItemDuration.Local()))
        {
            timeLineItem.Duration = duration;
        }

        TerritoryActionDrawer.DrawTerritoryAction(timeLineItem.TerritoryAction, item.ActionIDs);

        if (!isOpen) return;

        ImGui.Spacing();
        ImGui.Spacing();

        TimelineConditionDraw(timeLineItem.Condition, item);
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
                }

                void Up()
                {
                    set.Conditions.RemoveAt(i);
                    set.Conditions.Insert(Math.Max(0, i - 1), condition);
                }

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
                    (LocalString.Remove.Local(), Delete, ["Delete"]),
                    (LocalString.MoveUp.Local(), Up, ["↑"]),
                    (LocalString.MoveDown.Local(), Down, ["↓"]),
                    (LocalString.CopyToClipboard.Local(), Copy, ["Ctrl"]));

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
                if (!popUp) return;

                AddOneCondition<TimelineConditionSet>();
                AddOneCondition<TimelineConditionAction>();
                AddOneCondition<TimelineConditionTargetCount>();

                if (ImGui.Selectable(LocalString.FromClipboard.Local()))
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

            TerritoryActionDrawer.DrawObjectGetter(target.Getter, UiString.TimelineTargetGetter.Local());
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
}

