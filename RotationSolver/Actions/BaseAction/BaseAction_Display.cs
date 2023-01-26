using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using RotationSolver.Commands;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Windows.RotationConfigWindow;

namespace RotationSolver.Actions.BaseAction
{
    internal partial class BaseAction
    {
        public unsafe void Display(bool IsActive) => this.DrawEnableTexture(IsActive, () =>
        {
            if (IsTimeline) RotationConfigWindow.ActiveAction = this;
        }, otherThing: () =>
        {
            if (IsTimeline) OtherCommandType.Actions.DisplayCommandHelp($"{this}-{5}",
               type => string.Format(LocalizationManager.RightLang.Configwindow_Helper_InsertCommand, this), false);

#if DEBUG
            try
            {
                ImGui.Text("Have One:" + HaveOneChargeDEBUG.ToString());
                ImGui.Text("Is Real GCD: " + IsRealGCD.ToString());
                ImGui.Text("Recast One: " + RecastTimeOneChargeDEBUG.ToString());
                ImGui.Text("Recast Elapsed: " + RecastTimeElapsedDEBUG.ToString());
                ImGui.Text("Recast Remain: " + RecastTimeRemainDEBUG.ToString());
                ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Spell, AdjustedID).ToString());

                ImGui.Text("Cast Time: " + CastTime.ToString());
                ImGui.Text("MP: " + MPNeed.ToString());
                ImGui.Text($"Can Use: {CanUse(out _)} ");
                ImGui.Text("Must Use:" + CanUse(out _, mustUse: true).ToString());
                ImGui.Text("Empty Use:" + CanUse(out _, emptyOrSkipCombo: true).ToString());
                ImGui.Text("IsUnlocked: " + UIState.Instance()->IsUnlockLinkUnlocked(AdjustedID).ToString());
                //if (Target != null)
                //{
                //    ImGui.Text("Target Name: " + Target.Name);
                //}
            }
            catch
            {

            }
#endif
        });
    }
}
