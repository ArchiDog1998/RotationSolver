using Dalamud.Interface.Windowing;
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
                {  () =>UiString.ConfigWindow_List_Hostile.Local(), () => DrawTargetHostile(window) },
            });
        _targetHeader?.Draw();
    }

    private static void DrawTargetConfig(ConfigWindow window)
    {
        window.Collection.DrawItems((int)UiString.ConfigWindow_Target_Config);
    }

    private static void DrawTargetHostile(ConfigWindow window)
    {
        window.Collection.DrawItems((int)UiString.ConfigWindow_List_Hostile);
        XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(Service.Config.TargetingWays);
    }
}
