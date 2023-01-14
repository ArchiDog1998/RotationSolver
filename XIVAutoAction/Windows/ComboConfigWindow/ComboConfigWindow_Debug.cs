using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using System.Linq;
using XIVAutoAction.Actions.BaseAction;
using XIVAutoAction.Data;
using XIVAutoAction.Helpers;
using XIVAutoAction.SigReplacers;
using XIVAutoAction.Updaters;

namespace XIVAutoAction.Windows.ComboConfigWindow;
#if DEBUG
internal partial class ComboConfigWindow
{
    private void DrawDebug()
    {
        var str = TargetUpdater.EncryptString(Service.ClientState.LocalPlayer);
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(str).X + 10);
        ImGui.InputText("你的HASH! 发给秋水萌新吧！", ref str, 100);

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
            if (ActionUpdater.NextAction != null)
            {
                DrawAction(ActionUpdater.NextAction);
            }

            ImGui.Text("Ability Remain: " + ActionUpdater.AbilityRemain.ToString());
            ImGui.Text("Ability Count: " + ActionUpdater.AbilityRemainCount.ToString());

        }

        if (ImGui.CollapsingHeader("Last Action"))
        {
            DrawAction(Watcher.LastAction, nameof(Watcher.LastAction));
            DrawAction(Watcher.LastAbility, nameof(Watcher.LastAbility));
            DrawAction(Watcher.LastGCD, nameof(Watcher.LastGCD));
            DrawAction(Service.Address.LastComboAction, nameof(Service.Address.LastComboAction));
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

        if (ImGui.CollapsingHeader("Crafting Debug"))
        {
            ImGui.Text($"Progress: {SynthesisUpdater.CurrentProgress} / {SynthesisUpdater.MaxProgress}");
            ImGui.Text($"Quality: {SynthesisUpdater.CurrentQuality} / {SynthesisUpdater.MaxQuality}");
            ImGui.Text($"Durability: {SynthesisUpdater.CurrentDurability} / {SynthesisUpdater.StartingDurability}");
            ImGui.Text("Step Number: " + SynthesisUpdater.StepNumber.ToString());
            ImGui.Text("Condition: " + SynthesisUpdater.CraftCondition.ToString());
            ImGui.Text("LastCraft: " + ActionUpdater.LastCraftAction.ToString());
        }
    }

    private static void DrawAction(ActionID id, string type)
    {
        var action = new BaseAction(id);

        ImGui.Text($"{type}: {action}");
    }
}
#endif
