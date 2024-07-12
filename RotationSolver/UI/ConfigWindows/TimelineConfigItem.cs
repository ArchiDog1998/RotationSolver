using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Updaters;
using System.ComponentModel;
using XIVConfigUI;
using XIVDrawer;

namespace RotationSolver.UI.ConfigWindows;

[Description("Timeline")]
public class TimelineConfigItem : ConfigWindowItemRS
{
    internal static uint _territoryId = 0;
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
            var index = Array.IndexOf(territories, rightTerritory);
            if(ImGuiHelper.SelectableCombo("##Choice the specific dungeon", [..territories.Select(GetName)], ref index, imFont, ImGuiColors.DalamudYellow))
            {
                _territoryId = territories[index]?.RowId ?? 0;
            }
        }, ImGui.GetWindowWidth(), width);

        ImGuiHelperRS.DrawContentFinder(rightTerritory?.ContentFinderCondition.Value);

        var territoryConfig = OtherConfiguration.GetTerritoryConfigById(_territoryId);

        ImGui.Separator();

        if (ImGui.Button(LocalString.CopyToClipboard.Local()))
        {
            var str = JsonHelper.SerializeObject(territoryConfig);
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

                foreach (var i in timeLineItems)
                {
                    i.TimelineItem = item;
                }

                XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(timeLineItems);

                ImGui.TableNextColumn();

                if (!territoryConfig.JobConfig.Timeline.TryGetValue(item.Time, out timeLineItems))
                {
                    territoryConfig.JobConfig.Timeline[item.Time] = timeLineItems = [];
                }

                foreach (var i in timeLineItems)
                {
                    i.TimelineItem = item;
                }

                XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(timeLineItems);

                ImGui.TableNextRow();
            }
        }
    }
}

