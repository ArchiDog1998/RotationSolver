using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Default.Melee;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Updaters;
using System.Numerics;

namespace RotationSolver.UI;

internal partial class RotationConfigWindow
{
    private void DrawDebugTab()
    {
        if (Service.Player == null) return;
        var str = SocialUpdater.EncryptString(Service.Player);
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
            DrawParamTabItem("Effect", () =>
            {
                ImGui.Text(Watcher.ShowStr.ToString());
            });

            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private unsafe void DrawStatus()
    {
        if ((IntPtr)FateManager.Instance() != IntPtr.Zero)
        {
            ImGui.Text("Fate: " + DataCenter.FateId.ToString());
        }

        ImGui.Text("Have pet: " + DataCenter.HasPet.ToString());
        ImGui.Text("Hostile Near: " + DataCenter.HasHostilesInRange.ToString());
        ImGui.Text("Have Companion: " + DataCenter.HasCompanion.ToString());
        ImGui.Text("Targetable: " + Service.Player.IsTargetable().ToString());


        foreach (var status in Service.Player.StatusList)
        {
            var source = status.SourceId == Service.Player.ObjectId ? "You" : Service.ObjectTable.SearchById(status.SourceId) == null ? "None" : "Others";
            ImGui.Text($"{status.GameData.Name}: {status.StatusId} From: {source}");
        }
    }
    private unsafe void DrawParty()
    {
        //var status = AgentDeepDungeonStatus.Instance();
        //if ((IntPtr)status != IntPtr.Zero) 
        //{
        //    foreach (var item in status->Data->PomanderSpan)
        //    {
        //        ImGui.Text(item.Name.ToString() + " : " + item.ItemId.ToString());
        //    }

        //    foreach (var item in status->Data->MagiciteSpan)
        //    {
        //        ImGui.Text(item.Name.ToString() + " : " + item.ItemId.ToString());
        //    }
        //}

        ImGui.Text("Party: " + DataCenter.PartyMembers.Count().ToString());
        ImGui.Text("CanHealSingleAbility: " + DataCenter.CanHealSingleAbility.ToString());
        ImGui.Text("CanHealSingleSpell: " + DataCenter.CanHealSingleSpell.ToString());
        ImGui.Text("CanHealAreaAbility: " + DataCenter.CanHealAreaAbility.ToString());
        ImGui.Text("CanHealAreaSpell: " + DataCenter.CanHealAreaSpell.ToString());
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
            ImGui.Text("SubKind: " + b.GetBattleNPCSubKind().ToString());
            ImGui.Text("EventType: " + b.GetEventType().ToString());
            ImGui.Text("NamePlate: " + b.GetNamePlateIcon().ToString());
            ImGui.Text("StatusFlags: " + b.StatusFlags.ToString());
            ImGui.Text("InView: " + Service.WorldToScreen(b.Position, out _).ToString());
            ImGui.Text("NameId: " + b.NameId.ToString());

            foreach (var status in b.StatusList)
            {
                var source = status.SourceId == Service.Player.ObjectId ? "You" : Service.ObjectTable.SearchById(status.SourceId) == null ? "None" : "Others";
                ImGui.Text($"{status.GameData.Name}: {status.StatusId} From: {source}");
            }
        }

        ImGui.Text("All: " + DataCenter.AllTargets.Count().ToString());
        ImGui.Text("Hostile: " + DataCenter.HostileTargets.Count().ToString());
        foreach (var item in DataCenter.HostileTargets)
        {
            ImGui.Text(item.Name.ToString());
        }
    }
    private void DrawNextAction()
    {
        ImGui.Text(RotationUpdater.RightNowRotation.RotationName);
        ImGui.Text(DataCenter.SpecialType.ToString());

        ActionUpdater.NextAction?.Display(false);
        ImGui.Text("Ability Remain: " + DataCenter.AbilityRemain.ToString());
        ImGui.Text("Ability Count: " + DataCenter.AbilityRemainCount.ToString());

    }
    private void DrawLastAction()
    {
        DrawAction(Watcher.LastAction, nameof(Watcher.LastAction));
        DrawAction(Watcher.LastAbility, nameof(Watcher.LastAbility));
        DrawAction(Watcher.LastGCD, nameof(Watcher.LastGCD));
        DrawAction(DataCenter.LastComboAction, nameof(DataCenter.LastComboAction));
    }

    private void DrawCDEX()
    {
        ImGui.Text("Count Down: " + Service.CountDownTime.ToString());

        if (ActionUpdater.exception != null)
        {
            ImGui.Text(ActionUpdater.exception.Message);
            ImGui.Text(ActionUpdater.exception.StackTrace);
        }
    }

    private void DrawIcon()
    {
        //ControlWindow.DrawIAction(CustomRotation.Addle, 256, 1.6f);
        for (int i = 0; i <= 200; i++)
        {
            if (i % 10 != 0) ImGui.SameLine();
            ControlWindow.DrawIAction(CustomRotation.Addle, 48, i / 100f);
        }
    }

    private static void DrawAction(ActionID id, string type)
    {
        var action = new BaseAction(id);

        ImGui.Text($"{type}: {action}");
    }
}
