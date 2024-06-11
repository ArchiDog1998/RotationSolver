using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Trigger;
using RotationSolver.Basic.Record;
using System.ComponentModel;
using XIVConfigUI;
using XIVConfigUI.Attributes;

namespace RotationSolver.UI.ConfigWindows;
[Description("Trigger")]
public class TriggerItem : ConfigWindowItemRS
{
    internal static TerritoryConfig? _territoryConfig;

    public override uint Icon => 24;
    public override string Description => UiString.Item_Trigger.Local();

    public static TriggerData TriggerData { get; set; } = default;
    public static bool IsJob { get; set; }

    public override void Draw(ConfigWindow window)
    {
        ImGuiHelperRS.DrawTerritoryHeader();
        ImGuiHelperRS.DrawContentFinder(DataCenter.ContentFinder);

        _territoryConfig = OtherConfiguration.GetTerritoryConfigById(Svc.ClientState.TerritoryType);

        ImGui.Separator();

        if (ImGui.Button(LocalString.CopyToClipboard.Local()))
        {
            var str = JsonConvert.SerializeObject(_territoryConfig, Formatting.Indented);
            ImGui.SetClipboardText(str);
        }

        ImGui.SameLine();

        if (ImGui.Button(LocalString.FromClipboard.Local()))
        {
            var str = ImGui.GetClipboardText();
            try
            {
                OtherConfiguration.SetTerritoryConfigById(Svc.ClientState.TerritoryType, str, true);
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "Failed to load the condition.");
            }
        }

        ImGui.SameLine();

        if (ImGui.Button(UiString.ConfigWindow_Trigger_Clear.Local()))
        {
            Recorder.Clear();
        }

        window.Collection.DrawItems((int)UiString.TimelineRaidTime);
        window.Collection.DrawItems((int)UiString.Item_Trigger);

        ImGuiHelperRS.DrawSupporterWarning();

        using var table = ImRaii.Table("Trigger Table", 3,
            ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.ScrollY);

        if (!table) return;

        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

        ImGui.TableNextColumn();
        ImGui.TableHeader(UiString.ConfigWindow_Trigger_Log.Local());

        ImGui.TableNextColumn();
        ImGui.TableHeader(UiString.ConfigWindow_Trigger_TriggerData.Local());

        ImGui.TableNextColumn();
        ImGui.TableHeader(IsJob ? UiString.ConfigWindow_Timeline_JobActions.Local()
            : UiString.ConfigWindow_Timeline_Actions.Local());

        ImGui.TableNextRow();

        ImGui.TableNextColumn();

        using (var recordChild = ImRaii.Child("Record Child", new Vector2(0, -2)))
        {
            DrawRecord();
        }

        ImGui.TableNextColumn();

        using (var recordChild = ImRaii.Child("Trigger Data Child", new Vector2(0, -2)))
        {
            DrawTriggerData();
        }

        ImGui.TableNextColumn();

        using (var recordChild = ImRaii.Child("Trigger Child", new Vector2(0, -2)))
        {
            DrawTrigger();
        }
    }

    private static void DrawRecord()
    {
        int index = 0;
        foreach ((var time, var data) in Recorder.Data)
        {
            ImGui.Text(time.ToString("HH:mm:ss.fff") + " |");
            ImGui.SameLine();

            if (ImGui.Button($"{data}##{index++}"))
            {
                AddTriggerData(data.ToTriggerData(), false);
            }
            if (ImGui.IsItemHovered())
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    AddTriggerData(data.ToTriggerData(), true);
                }
                ImGuiHelper.ShowTooltip(UiString.ConfigWindow_Trigger_AddTriggerDataDesc.Local());
            }
        }

        static void AddTriggerData(TriggerData data, bool isJob)
        {
            if (_territoryConfig == null) return;

            var dict = isJob ? _territoryConfig.JobConfig.Trigger : _territoryConfig.Config.Trigger;

            TriggerData = data;
            IsJob = isJob;

            if (dict.Any(d => d.Item1.Equals(TriggerData))) return;

            dict.Add((data, []));
        }
    }

    private static void DrawTriggerData()
    {
        if (_territoryConfig == null) return;

        var dict = IsJob ? _territoryConfig.JobConfig.Trigger : _territoryConfig.Config.Trigger;

        for (int i = 0; i < dict.Count; i++)
        {
            void Delete()
            {
                dict.RemoveAt(i);
            };

            var pair = dict[i];
            var key = pair.Item1;

            var popKey = $"TriggerDataPopup{i}";
            ImGuiHelper.DrawHotKeysPopup(popKey, string.Empty,
                (LocalString.Remove.Local(), Delete, ["Delete"]));

            if (ImGui.Selectable($"{key}##{i}", key.Equals(TriggerData)))
            {
                TriggerData = key;
            }

            ImGuiHelper.ExecuteHotKeysPopup(popKey, string.Empty, string.Empty, true,
                (Delete, [VirtualKey.DELETE]));
        }
    }

    private static void DrawTrigger()
    {
        if (_territoryConfig == null) return;
        var dict = IsJob ? _territoryConfig.JobConfig.Trigger : _territoryConfig.Config.Trigger;

        List<BaseTriggerItem>? data = null;
        foreach (var pair in dict)
        {
            if (pair.Item1.Equals(TriggerData))
            {
                data = pair.Item2;
                break;
            }
        }

        if (data == null) return;

        DrawItems(data, IsJob);

        static void DrawItems(List<BaseTriggerItem> triggerItems, bool isJob)
        {
            AddButton();
            for (int i = 0; i < triggerItems.Count; i++)
            {
                if (i != 0)
                {
                    ImGui.Separator();
                }
                var triggerItem = triggerItems[i];

                void Delete()
                {
                    triggerItems.RemoveAt(i);
                };

                void Up()
                {
                    triggerItems.RemoveAt(i);
                    triggerItems.Insert(Math.Max(0, i - 1), triggerItem);
                };

                void Down()
                {
                    triggerItems.RemoveAt(i);
                    triggerItems.Insert(Math.Min(triggerItems.Count, i + 1), triggerItem);
                }

                void Execute()
                {
                    Task.Run(async () =>
                    {
                        triggerItem.TerritoryAction.Enable();
                        await Task.Delay(3000);
                        triggerItem.TerritoryAction.Disable();
                    });
                }

                var key = $"TimelineItem Pop Up: {triggerItem.GetHashCode()}";

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                    (LocalString.Remove.Local(), Delete, ["Delete"]),
                    (LocalString.MoveUp.Local(), Up, ["↑"]),
                    (LocalString.MoveDown.Local(), Down, ["↓"]),
                    (UiString.TimelineExecute.Local(), Execute, ["→"]));

                ConditionDrawer.DrawCondition(true);

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                    (Delete, [VirtualKey.DELETE]),
                    (Up, [VirtualKey.UP]),
                    (Down, [VirtualKey.DOWN]),
                    (Execute, [VirtualKey.RIGHT]));

                ImGui.SameLine();
                using var grp = ImRaii.Group();

                var time = triggerItem.StartTime;
                if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Seconds, $" ##Time{triggerItem.GetHashCode()}", ref time, UiString.TriggerItemTime.Local()))
                {
                    triggerItem.StartTime = time;
                }

                ImGui.SameLine();

                var duration = triggerItem.Duration;
                if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Seconds, $"##Duration{triggerItem.GetHashCode()}", ref duration, UiString.TimelineItemDuration.Local()))
                {
                    triggerItem.Duration = duration;
                }

                TerritoryActionDrawer.DrawTerritoryAction(triggerItem.TerritoryAction, []);
            }

            void AddButton()
            {
                if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddTriggerButton" + isJob))
                {
                    ImGui.OpenPopup("PopupTriggerButton" + isJob);
                }
                ImguiTooltips.HoveredTooltip(UiString.AddTerritoryActionButton.Local());

                using var popUp = ImRaii.Popup("PopupTriggerButton" + isJob);
                if (!popUp) return;

                if (isJob)
                {
                    AddOneCondition<ActionTriggerItem>();
                }
                AddOneCondition<StateTriggerItem>();
                AddOneCondition<DrawingTriggerItem>();
                AddOneCondition<MacroTriggerItem>();
                AddOneCondition<MoveTriggerItem>();
                AddOneCondition<PathfindTriggerItem>();

                void AddOneCondition<T>() where T : BaseTriggerItem
                {
                    if (ImGui.Selectable(typeof(T).Local()))
                    {
                        triggerItems.Add(Activator.CreateInstance<T>());
                        ImGui.CloseCurrentPopup();
                    }
                }
            }
        }
    }

}
