using System.ComponentModel;
using XIVConfigUI;

namespace RotationSolver.UI.ConfigWindows;
[Description("Auto")]
public class AutoItem : ConfigWindowItemRS
{
    private CollapsingHeaderGroup? _autoHeader;
    public override uint Icon => 29;
    public override string Description => UiString.Item_Auto.Local();

    public override void Draw(ConfigWindow window)
    {
        ImGui.TextWrapped(UiString.ConfigWindow_Auto_Description.Local());
        _autoHeader ??= new(new()
            {
                {  () => UiString.ConfigWindow_Auto_ActionUsage.Local(), () =>
                    {
                        ImGui.TextWrapped(UiString.ConfigWindow_Auto_ActionUsage_Description
                            .Local());
                        ImGui.Separator();

                        window.Collection.DrawItems((int)UiString.ConfigWindow_Auto_ActionUsage);
                    }
                },
                {  () => UiString.ConfigWindow_Auto_ActionCondition.Local(), () => DrawAutoActionCondition(window) },
                {  () => UiString.ConfigWindow_Auto_StateCondition.Local(), () => _autoState?.Draw() },
            });

        _autoHeader?.Draw();
    }

    private static readonly CollapsingHeaderGroup _autoState = new(new()
        {
            {
                () => UiString.ConfigWindow_Auto_HealAreaConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.HealAreaConditionSet)
            },

            {
                () => UiString.ConfigWindow_Auto_HealSingleConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.HealSingleConditionSet)
            },

            {
                () => UiString.ConfigWindow_Auto_DefenseAreaConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.DefenseAreaConditionSet)
            },

            {
                () => UiString.ConfigWindow_Auto_DefenseSingleConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.DefenseSingleConditionSet)
            },

            {
                () =>  UiString.ConfigWindow_Auto_DispelStancePositionalConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.DispelStancePositionalConditionSet)
            },

            {
                () =>  UiString.ConfigWindow_Auto_RaiseShirkConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.RaiseShirkConditionSet)
            },

            {
                () => UiString.ConfigWindow_Auto_MoveForwardConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.MoveForwardConditionSet)
            },

            {
                () => UiString.ConfigWindow_Auto_MoveBackConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.MoveBackConditionSet)
            },

            {
                () => UiString.ConfigWindow_Auto_AntiKnockbackConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.AntiKnockbackConditionSet)
            },

            {
                () => UiString.ConfigWindow_Auto_SpeedConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.SpeedConditionSet)
            },

            {
                () => UiString.ConfigWindow_Auto_LimitBreakConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.LimitBreakConditionSet)
            },
        })
    {
        HeaderSize = FontSize.Fourth,
    };

    private static void DrawAutoActionCondition(ConfigWindow window)
    {
        ImGui.TextWrapped(UiString.ConfigWindow_Auto_ActionCondition_Description.Local());
        ImGui.Separator();

        window.Collection.DrawItems((int)UiString.ConfigWindow_Auto_ActionCondition);
    }
}
