using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.RangedMagicial.BLM;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System;
using System.Linq;

namespace RotationSolver.Windows.RotationConfigWindow;
#if DEBUG
internal partial class RotationConfigWindow
{
    private unsafe void DrawDebugTab()
    {
        var str = TargetUpdater.EncryptString(Service.ClientState.LocalPlayer);
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(str).X + 10);
        ImGui.InputText("That is your HASH", ref str, 100);

        if (ImGui.CollapsingHeader("Status"))
        {
            ImGui.Text(ActionUpdater.WeaponRemain.ToString());

            if ((IntPtr)FateManager.Instance() != IntPtr.Zero)
            {
                ImGui.Text("Fate: " + TargetUpdater.FateId.ToString());
            }

            foreach (var status in Service.ClientState.LocalPlayer.StatusList)
            {
                var source = Service.ObjectTable.SearchById(status.SourceId)?.Name ?? "None";
                ImGui.Text($"{status.GameData.Name}: {status.StatusId} From: {source}");
            }
        }

        if (ImGui.CollapsingHeader("Party"))
        {
            ImGui.Text("Friends: " + TargetUpdater.PartyMembers.Count().ToString());
            ImGui.Text("CanHealSingleAbility: " + TargetUpdater.CanHealSingleAbility.ToString());
            ImGui.Text("CanHealSingleSpell: " + TargetUpdater.CanHealSingleSpell.ToString());
            ImGui.Text("CanHealAreaAbility: " + TargetUpdater.CanHealAreaAbility.ToString());
            ImGui.Text("CanHealAreaSpell: " + TargetUpdater.CanHealAreaSpell.ToString());
        }

        if (ImGui.CollapsingHeader("Target Data"))
        {
            if (Service.TargetManager.Target is BattleChara b)
            {
                ImGui.Text("Is Boss: " + b.IsBoss().ToString());
                ImGui.Text("Has Positional: " + b.HasPositional().ToString());
                ImGui.Text("Is Dying: " + b.IsDying().ToString());

                foreach (var status in b.StatusList)
                {
                    var source = Service.ObjectTable.SearchById(status.SourceId)?.Name ?? "None";
                    ImGui.Text($"{status.GameData.Name}: {status.StatusId} From: {source}");
                }
            }

            ImGui.Text("All: " + TargetUpdater.AllTargets.Count().ToString());
            ImGui.Text("Hostile: " + TargetUpdater.HostileTargets.Count().ToString());
            foreach (var item in TargetUpdater.HostileTargets)
            {
                ImGui.Text(item.Name.ToString());
            }
        }

        if (ImGui.CollapsingHeader("Next Action"))
        {
            ImGui.Text(RotationUpdater.RightNowRotation.RotationName);
            ImGui.Text(RSCommands.SpecialType.ToString());

            ActionUpdater.NextAction?.Display(false);
            ImGui.Text("Ability Remain: " + ActionUpdater.AbilityRemain.ToString());
            ImGui.Text("Ability Count: " + ActionUpdater.AbilityRemainCount.ToString());
        }

        if (ImGui.CollapsingHeader("Last Action"))
        {
            DrawAction(Watcher.LastAction, nameof(Watcher.LastAction));
            DrawAction(Watcher.LastAbility, nameof(Watcher.LastAbility));
            DrawAction(Watcher.LastGCD, nameof(Watcher.LastGCD));
            DrawAction(ActionUpdater.LastComboAction, nameof(ActionUpdater.LastComboAction));
        }

        if (ImGui.CollapsingHeader("Countdown, Exception"))
        {
            ImGui.Text("Count Down: " + CountDown.CountDownTime.ToString());

            if (ActionUpdater.exception != null)
            {
                ImGui.Text(ActionUpdater.exception.Message);
                ImGui.Text(ActionUpdater.exception.StackTrace);
            }
        }
    }

    private static void DrawAction(ActionID id, string type)
    {
        var action = new BaseAction(id);

        ImGui.Text($"{type}: {action}");
    }
}
#endif
