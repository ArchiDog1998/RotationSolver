using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using RotationSolver;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System.Linq;
using RotationSolver.Actions.BaseAction;

namespace RotationSolver.Windows.RotationConfigWindow;
#if DEBUG
internal partial class RotationConfigWindow
{
    private void DrawDebugTab()
    {
        var str = TargetUpdater.EncryptString(Service.ClientState.LocalPlayer);
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(str).X + 10);
        ImGui.InputText("That is your HASH, send to ArchiTed", ref str, 100);

        ImGui.Text("Hostile: " + TargetUpdater.HostileTargets.Count().ToString());
        ImGui.Text("Friends: " + TargetUpdater.PartyMembers.Count().ToString());

        if (ImGui.CollapsingHeader("Status from self."))
        {
            foreach (var item in Service.ClientState.LocalPlayer.StatusList)
            {
                if (item.SourceId == Service.ClientState.LocalPlayer.ObjectId)
                {
                    ImGui.Text(item.GameData.Name + item.StatusId);
                }
            }
        }

        if (ImGui.CollapsingHeader("Target Data"))
        {
            var count = TargetUpdater.AllTargets?.Count() ??　0;
            ImGui.Text(count.ToString());

            if (Service.TargetManager.Target is BattleChara b)
            {
                ImGui.Text("Is Boss: " + b.IsBoss().ToString());
                ImGui.Text("Has Side: " + b.HasLocationSide().ToString());
                ImGui.Text("Is Dying: " + b.IsDying().ToString());

                foreach (var status in b.StatusList)
                {
                    if (status.SourceId == Service.ClientState.LocalPlayer.ObjectId)
                    {
                        ImGui.Text(status.GameData.Name + status.StatusId);
                    }
                }
            }
            ImGui.Text("");
            foreach (var item in TargetUpdater.HostileTargets)
            {
                ImGui.Text(item.Name.ToString());
            }
        }

        if (ImGui.CollapsingHeader("Next Action"))
        {
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
