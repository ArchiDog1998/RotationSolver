using RotationSolver.Basic.Configuration;
using System.ComponentModel;
using XIVConfigUI;
using XIVConfigUI.ConditionConfigs;

namespace RotationSolver.UI.ConfigWindows;

[Description("Extra")]
public class ExtraItem : ConfigWindowItemRS
{
    private CollapsingHeaderGroup? _extraHeader;
    public override uint Icon => 51;
    public override string Description => UiString.Item_Extra.Local();

    public override void Draw(ConfigWindow window)
    {
        _extraHeader ??= new(new()
            {
                {   () =>UiString.ConfigWindow_EventItem.Local(), DrawEventTab },
                {
                    () =>UiString.ConfigWindow_Extra_Others.Local(),
                    () => window.Collection.DrawItems((int)UiString.ConfigWindow_Extra_Others)
                },
            });
        ImGui.TextWrapped(UiString.ConfigWindow_Extra_Description.Local());
        _extraHeader?.Draw();
    }

    private static void DrawEventTab()
    {
        if (ImGui.Button(UiString.ConfigWindow_Events_AddEvent.Local()))
        {
            Service.Config.Events.Add(new ActionEventInfo());
        }
        ImGui.SameLine();

        ImGui.TextWrapped(UiString.ConfigWindow_Events_Description.Local());

        ImGui.Text(UiString.ConfigWindow_Events_DutyStart.Local());
        ImGui.SameLine();
        ConditionDrawer.Draw(Service.Config.DutyStart);

        ImGui.Text(UiString.ConfigWindow_Events_DutyEnd.Local());
        ImGui.SameLine();
        ConditionDrawer.Draw(Service.Config.DutyEnd);

        ImGui.Separator();
        ConditionDrawer.Draw(Service.Config.Events);
    }
}
