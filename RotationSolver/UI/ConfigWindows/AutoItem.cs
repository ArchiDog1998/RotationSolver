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
                () => DataCenter.RightSet.HealAreaConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_HealSingleConditionSet.Local(),
                () => DataCenter.RightSet.HealSingleConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_DefenseAreaConditionSet.Local(),
                () => DataCenter.RightSet.DefenseAreaConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_DefenseSingleConditionSet.Local(),
                () => DataCenter.RightSet.DefenseSingleConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () =>  UiString.ConfigWindow_Auto_DispelStancePositionalConditionSet.Local(),
                () => DataCenter.RightSet.DispelStancePositionalConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () =>  UiString.ConfigWindow_Auto_RaiseShirkConditionSet.Local(),
                () => DataCenter.RightSet.RaiseShirkConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_MoveForwardConditionSet.Local(),
                () => DataCenter.RightSet.MoveForwardConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_MoveBackConditionSet.Local(),
                () => DataCenter.RightSet.MoveBackConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_AntiKnockbackConditionSet.Local(),
                () => DataCenter.RightSet.AntiKnockbackConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_SpeedConditionSet.Local(),
                () => DataCenter.RightSet.SpeedConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_LimitBreakConditionSet.Local(),
                () => DataCenter.RightSet.LimitBreakConditionSet?.DrawMain(DataCenter.RightNowRotation)
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
