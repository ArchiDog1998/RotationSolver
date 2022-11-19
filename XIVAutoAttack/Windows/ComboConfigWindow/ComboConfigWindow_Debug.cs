using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Windows.ComboConfigWindow;
#if DEBUG
internal partial class ComboConfigWindow
{
    private void DrawDebug()
    {
        if (ImGui.CollapsingHeader("自身附加给自己的状态"))
        {
            foreach (var item in Service.ClientState.LocalPlayer.StatusList)
            {

                if (item.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                {
                    ImGui.Text(item.GameData.Name + item.StatusId);
                }
            }
        }

        if (ImGui.CollapsingHeader("目标信息"))
        {
            if (Service.TargetManager.Target is BattleChara b)
            {
                ImGui.Text("Is Boss: " + b.IsBoss().ToString());
                ImGui.Text("Has Side: " + b.HasLocationSide().ToString());
                ImGui.Text("Is Dying: " + b.IsDying().ToString());

                foreach (var status in b.StatusList)
                {
                    if (status.SourceID == Service.ClientState.LocalPlayer.ObjectId)
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

        if (ImGui.CollapsingHeader("下一个技能"))
        {
            BaseAction baseAction = null;
            baseAction ??= ActionUpdater.NextAction as BaseAction;
            DrawAction(baseAction);

            ImGui.Text("Ability Remain: " + ActionUpdater.AbilityRemain.ToString());
            ImGui.Text("Ability Count: " + ActionUpdater.AbilityRemainCount.ToString());

        }

        if (ImGui.CollapsingHeader("上一个技能"))
        {
            DrawAction(Watcher.LastAction, nameof(Watcher.LastAction));
            DrawAction(Watcher.LastAbility, nameof(Watcher.LastAbility));
            DrawAction(Watcher.LastSpell, nameof(Watcher.LastSpell));
            DrawAction(Watcher.LastWeaponskill, nameof(Watcher.LastWeaponskill));
            DrawAction(Service.Address.LastComboAction, nameof(Service.Address.LastComboAction));
        }

        if (ImGui.CollapsingHeader("倒计时、按键"))
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
