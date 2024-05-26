using Dalamud.Game.ClientState.Keys;
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
            Service.Config.TargetingTypes.Add(TargetingType.Big);
        }
        ImGui.SameLine();
        ImGui.TextWrapped(UiString.ConfigWindow_Param_HostileDesc.Local());

        for (int i = 0; i < Service.Config.TargetingTypes.Count; i++)
        {
            var targetType = Service.Config.TargetingTypes[i];

            void Delete()
            {
                Service.Config.TargetingTypes.RemoveAt(i);
            };

            void Up()
            {
                Service.Config.TargetingTypes.RemoveAt(i);
                Service.Config.TargetingTypes.Insert(Math.Max(0, i - 1), targetType);
            };
            void Down()
            {
                Service.Config.TargetingTypes.RemoveAt(i);
                Service.Config.TargetingTypes.Insert(Math.Min(Service.Config.TargetingTypes.Count - 1, i + 1), targetType);
            }

            var key = $"Targeting Type Pop Up: {i}";

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]),
                (UiString.ConfigWindow_Actions_MoveUp.Local(), Up, ["↑"]),
                (UiString.ConfigWindow_Actions_MoveDown.Local(), Down, ["↓"]));

            var names = Enum.GetNames(typeof(TargetingType));
            var targingType = (int)Service.Config.TargetingTypes[i];
            var text = UiString.ConfigWindow_Param_HostileCondition.Local();
            ImGui.SetNextItemWidth(ImGui.CalcTextSize(text).X + 30 * Scale);
            if (ImGui.Combo(text + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
            {
                Service.Config.TargetingTypes[i] = (TargetingType)targingType;
            }

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                (Delete, [VirtualKey.DELETE]),
                (Up, [VirtualKey.UP]),
                (Down, [VirtualKey.DOWN]));
        }
    }
}
