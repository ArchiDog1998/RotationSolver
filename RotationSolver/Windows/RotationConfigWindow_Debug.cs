using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Rotations.RangedMagicial.BLM;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System;
using System.Linq;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow;
#if DEBUG
internal partial class RotationConfigWindow
{
    private void DrawDebugTab()
    {
        var str = TargetUpdater.EncryptString(Service.ClientState.LocalPlayer);
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(str).X + 10);
        ImGui.InputText("That is your HASH", ref str, 100);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("Debug Items"))
        {
            DrawParamTabItem("Status", DrawStatus);
            DrawParamTabItem("Party", DrawParty);
            DrawParamTabItem("Target Data", DrawTargetData);
            DrawParamTabItem("Next Action", DrawNextAction);
            DrawParamTabItem("Last Action", DrawLastAction);
            DrawParamTabItem("CD, EX", DrawCDEX);
            DrawParamTabItem("Icon", DrawIcon);

            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private unsafe void DrawStatus()
    {
        if ((IntPtr)FateManager.Instance() != IntPtr.Zero)
        {
            ImGui.Text("Fate: " + TargetUpdater.FateId.ToString());
        }

        ImGui.Text("Have pet: " + TargetUpdater.HavePet.ToString());
        ImGui.Text("Have Companion: " + TargetUpdater.HaveCompanion.ToString());


        foreach (var status in Service.ClientState.LocalPlayer.StatusList)
        {
            var source = Service.ObjectTable.SearchById(status.SourceId)?.Name ?? "None";
            ImGui.Text($"{status.GameData.Name}: {status.StatusId} From: {source}");
        }
    }
    private unsafe void DrawParty()
    {
        ImGui.Text("Party: " + TargetUpdater.PartyMembers.Count().ToString());
        ImGui.Text("PartyP: " + TargetUpdater.PartyMembers.Count(m => GroupManager.Instance()->IsObjectIDInParty(m.ObjectId)).ToString());
        ImGui.Text("PartyA: " + TargetUpdater.PartyMembers.Count(m => GroupManager.Instance()->IsObjectIDInAlliance(m.ObjectId)).ToString());
        ImGui.Text("CanHealSingleAbility: " + TargetUpdater.CanHealSingleAbility.ToString());
        ImGui.Text("CanHealSingleSpell: " + TargetUpdater.CanHealSingleSpell.ToString());
        ImGui.Text("CanHealAreaAbility: " + TargetUpdater.CanHealAreaAbility.ToString());
        ImGui.Text("CanHealAreaSpell: " + TargetUpdater.CanHealAreaSpell.ToString());
    }

    private unsafe void DrawTargetData()
    {
        if (Service.TargetManager.Target is BattleChara b)
        {
            ImGui.Text("HP: " + b.CurrentHp + " / " + b.MaxHp);
            ImGui.Text("Is Boss: " + b.IsBoss().ToString());
            ImGui.Text("Has Positional: " + b.HasPositional().ToString());
            ImGui.Text("Is Dying: " + b.IsDying().ToString());
            ImGui.Text("Kind: " + b.GetObjectKind().ToString());
            ImGui.Text("Subkind: " + b.GetBattleNPCSubkind().ToString());
            ImGui.Text("EventType: " + b.GetEventType().ToString());
            ImGui.Text("NamePlate: " + b.GetNamePlateIcon().ToString());
            ImGui.Text("StatusFlags: " + b.StatusFlags.ToString());

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
    private void DrawNextAction()
    {
        ImGui.Text(RotationUpdater.RightNowRotation.RotationName);
        ImGui.Text(RSCommands.SpecialType.ToString());

        ActionUpdater.NextAction?.Display(false);
        ImGui.Text("Ability Remain: " + ActionUpdater.AbilityRemain.ToString());
        ImGui.Text("Ability Count: " + ActionUpdater.AbilityRemainCount.ToString());

    }
    private void DrawLastAction()
    {
        DrawAction(Watcher.LastAction, nameof(Watcher.LastAction));
        DrawAction(Watcher.LastAbility, nameof(Watcher.LastAbility));
        DrawAction(Watcher.LastGCD, nameof(Watcher.LastGCD));
        DrawAction(ActionUpdater.LastComboAction, nameof(ActionUpdater.LastComboAction));
    }

    private void DrawCDEX()
    {
        ImGui.Text("Count Down: " + CountDown.CountDownTime.ToString());

        if (ActionUpdater.exception != null)
        {
            ImGui.Text(ActionUpdater.exception.Message);
            ImGui.Text(ActionUpdater.exception.StackTrace);
        }
    }

    private void DrawIcon()
    {
        ImGui.Image(IconSet.GetTexture(60094).ImGuiHandle, new Vector2(24, 24));
        ImGui.Image(IconSet.GetTexture(71224).ImGuiHandle, new Vector2(24, 24));
    }

    private static void DrawAction(ActionID id, string type)
    {
        var action = new BaseAction(id);

        ImGui.Text($"{type}: {action}");
    }
}
#endif
