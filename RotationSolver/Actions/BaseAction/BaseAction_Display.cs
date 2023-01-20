using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using RotationSolver.Helpers;
using RotationSolver.Windows.RotationConfigWindow;
using RotationSolver.Localization;
using RotationSolver.Commands;

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
               type => string.Format(LocalizationManager.RightLang.Configwindow_Helper_InsertCommand, this));

#if DEBUG
            ImGui.Text("Have One:" + HaveOneChargeDEBUG.ToString());
            ImGui.Text("Is Real GCD: " + IsRealGCD.ToString());
            ImGui.Text("Recast One: " + RecastTimeOneChargeDEBUG.ToString());
            ImGui.Text("Recast Elapsed: " + RecastTimeElapsedDEBUG.ToString());
            ImGui.Text("Recast Remain: " + RecastTimeRemainDEBUG.ToString());
            ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Spell, AdjustedID).ToString());

            ImGui.Text("Cast Time: " + CastTime.ToString());
            ImGui.Text("MP: " + MPNeed.ToString());
            ImGui.Text($"Can Use: {ShouldUse(out _)} ");
            ImGui.Text("Must Use:" + ShouldUse(out _, mustUse: true).ToString());
            ImGui.Text("Empty Use:" + ShouldUse(out _, emptyOrSkipCombo: true).ToString());
            ImGui.Text("IsUnlocked: " + UIState.Instance()->IsUnlockLinkUnlocked(AdjustedID).ToString());
            if (Target != null)
            {
                ImGui.Text("Target Name: " + Target.Name);
            }
#endif
        });
    }
}
