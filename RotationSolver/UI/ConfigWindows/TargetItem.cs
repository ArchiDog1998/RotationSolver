﻿using Dalamud.Game.ClientState.Keys;
using ECommons.ImGuiMethods;
using System.ComponentModel;
using XIVConfigUI;

namespace RotationSolver.UI.ConfigWindows;
[Description("Target")]
public class TargetItem : ConfigWindowItemRS
{
    private CollapsingHeaderGroup? _targetHeader;
    public override uint Icon => 16;
    public override string Description => UiString.Item_Target.Local();

    public override void Draw(ConfigWindow window)
    {
        _targetHeader ??= new(new()
            {
                {  () =>UiString.ConfigWindow_Target_Config.Local(), () => DrawTargetConfig(window) },
                {  () =>UiString.ConfigWindow_List_Hostile.Local(), DrawTargetHostile },
            });
        _targetHeader?.Draw();
    }

    private static void DrawTargetConfig(ConfigWindow window)
    {
        window.Collection.DrawItems((int)UiString.ConfigWindow_Target_Config);
    }

    private static void DrawTargetHostile()
    {
        if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "Add Hostile"))
        {
            Service.Config.TargetingWays.Add(TargetingType.Big);
        }
        ImGui.SameLine();
        ImGui.TextWrapped(UiString.ConfigWindow_Param_HostileDesc.Local());

        for (int i = 0; i < Service.Config.TargetingWays.Count; i++)
        {
            var targetType = Service.Config.TargetingWays[i];

            void Delete()
            {
                Service.Config.TargetingWays.RemoveAt(i);
            };

            void Up()
            {
                Service.Config.TargetingWays.RemoveAt(i);
                Service.Config.TargetingWays.Insert(Math.Max(0, i - 1), targetType);
            };
            void Down()
            {
                Service.Config.TargetingWays.RemoveAt(i);
                Service.Config.TargetingWays.Insert(Math.Min(Service.Config.TargetingWays.Count - 1, i + 1), targetType);
            }

            var key = $"Targeting Type Pop Up: {i}";

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                (LocalString.Remove.Local(), Delete, ["Delete"]),
                (LocalString.MoveUp.Local(), Up, ["↑"]),
                (LocalString.MoveDown.Local(), Down, ["↓"]));

            var targetingWay = Service.Config.TargetingWays[i];
            var targingType = targetingWay.TargetingType;

            if(ConditionDrawer.DrawByteEnum("##HostileCondition" + i.ToString(), ref targingType))
            {
                targetingWay.TargetingType = targingType;
            }

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                (Delete, [VirtualKey.DELETE]),
                (Up, [VirtualKey.UP]),
                (Down, [VirtualKey.DOWN]));
        }
    }
}
