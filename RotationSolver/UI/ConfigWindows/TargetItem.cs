using System.ComponentModel;
using XIVConfigUI;
using XIVConfigUI.ConditionConfigs;

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
                {  () => UiString.ConfigWindow_Target_Priority.Local(), () => DrawTargetPriority(window) },
                {  () => UiString.ConfigWindow_Target_Cant.Local(), () => DrawTargetCant(window) },
                {  () => UiString.ConfigWindow_Target_Config.Local(), () => DrawTargetConfig(window) },
                {  () => UiString.ConfigWindow_Basic_NamedConditions.Local(), () => DrawNamedConditions(window) },
                {  () => UiString.ConfigWindow_List_Hostile.Local(), () => DrawTargetHostile(window) },
            });
        _targetHeader?.Draw();
    }

    private static void DrawTargetPriority(ConfigWindow window)
    {
        window.Collection.DrawItems((int)UiString.ConfigWindow_Target_Priority);

        ImGui.Separator();

        ImGui.TextWrapped(UiString.ConfigWindow_Actions_PriorityTargeting_Description.Local());
        ConditionDrawer.Draw(Service.Config.PriorityTargeting);
    }

    private static void DrawTargetCant(ConfigWindow window)
    {
        window.Collection.DrawItems((int)UiString.ConfigWindow_Target_Cant);

        ImGui.Separator();

        ImGui.TextWrapped(UiString.ConfigWindow_Actions_CantTargeting_Description.Local());

        ConditionDrawer.Draw(Service.Config.CantTargeting);
    }

    private static void DrawTargetConfig(ConfigWindow window)
    {
        window.Collection.DrawItems((int)UiString.ConfigWindow_Target_Config);
    }

    private static void DrawTargetHostile(ConfigWindow window)
    {
        window.Collection.DrawItems((int)UiString.ConfigWindow_List_Hostile);
        ConditionDrawer.Draw(Service.Config.TargetingWays);
    }

    private static void DrawNamedConditions(ConfigWindow window)
    {
        ConditionDrawer.Draw(Service.Config.NamedTargetingConditions);
    }
}
