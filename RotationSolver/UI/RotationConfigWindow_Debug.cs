using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

internal partial class RotationConfigWindow
{
    private void DrawDebugTab()
    {
        if (!Player.Available) return;

        var str = SocialUpdater.EncryptString(Player.Object);
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(str).X + 10);
        ImGui.InputText("That is your HASH", ref str, 100);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("Debug Items"))
        {
            if(RotationUpdater.RightNowRotation != null)
            {
                DrawParamTabItem("Rotation", RotationUpdater.RightNowRotation.DisplayStatus);
            }
            DrawParamTabItem("Status", DrawStatus);
            DrawParamTabItem("Party", DrawParty);
            DrawParamTabItem("Target Data", DrawTargetData);
            DrawParamTabItem("Next Action", DrawNextAction);
            DrawParamTabItem("Last Action", DrawLastAction);
            DrawParamTabItem("Icon", DrawIcon);
            DrawParamTabItem("Effect", () =>
            {
                ImGui.Text(Watcher.ShowStrSelf);
                ImGui.Separator();
                ImGui.Text(Watcher.ShowStrEnemy);
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
        ImGui.Text("Moving: " + DataCenter.IsMoving.ToString());
        ImGui.Text("Stop Moving: " + DataCenter.StopMovingRaw.ToString());

        ImGui.Text("TerritoryType: " + DataCenter.TerritoryContentType.ToString());
        ImGui.Text("DPSTaken: " + DataCenter.DPSTaken.ToString());
        ImGui.Text("TimeToNext: " + DataCenter.NextAbilityToNextGCD.ToString());
        ImGui.Text("WeaponElapsed: " + DataCenter.WeaponElapsed.ToString());
        ImGui.Text("AnimationLock: " + DataCenter.ActionRemain.ToString());

        ImGui.Text("Have pet: " + DataCenter.HasPet.ToString());
        ImGui.Text("Hostile Near Count: " + DataCenter.NumberOfHostilesInRange.ToString());
        ImGui.Text("Hostile Near Count Max Range: " + DataCenter.NumberOfHostilesInMaxRange.ToString());
        ImGui.Text("Have Companion: " + DataCenter.HasCompanion.ToString());
        ImGui.Text("Ping: " + DataCenter.Ping.ToString());
        ImGui.Text("MP: " + DataCenter.CurrentMp.ToString());
        ImGui.Text("Count Down: " + Service.CountDownTime.ToString());
        ImGui.Text("Fetch Time: " + DataCenter.FetchTime.ToString());

        foreach (var status in Player.Object.StatusList)
        {
            var source = status.SourceId == Player.Object.ObjectId ? "You" : Svc.Objects.SearchById(status.SourceId) == null ? "None" : "Others";
            ImGui.Text($"{status.GameData.Name}: {status.StatusId} From: {source}");
        }
    }
    private unsafe void DrawParty()
    {
        ImGui.Text("Party Burst Ratio: " + DataCenter.RatioOfMembersIn2minsBurst.ToString());
        ImGui.Text("Party: " + DataCenter.PartyMembers.Count().ToString());
        ImGui.Text("CanHealSingleAbility: " + DataCenter.CanHealSingleAbility.ToString());
        ImGui.Text("CanHealSingleSpell: " + DataCenter.CanHealSingleSpell.ToString());
        ImGui.Text("CanHealAreaAbility: " + DataCenter.CanHealAreaAbility.ToString());
        ImGui.Text("CanHealAreaSpell: " + DataCenter.CanHealAreaSpell.ToString());
        ImGui.Text("CanHealAreaSpell: " + DataCenter.CanHealAreaSpell.ToString());
        ImGui.Text("PartyMembersAverHP: " + DataCenter.PartyMembersAverHP.ToString());
    }

    private unsafe void DrawTargetData()
    {
        if(Svc.Targets.Target != null)
        {
            ImGui.Text("Height: " + Svc.Targets.Target.Struct()->Height.ToString());
            ImGui.Text("Kind: " + Svc.Targets.Target.GetObjectKind().ToString());
            ImGui.Text("SubKind: " + Svc.Targets.Target.GetBattleNPCSubKind().ToString());
            var owner = Svc.Objects.SearchById(Svc.Targets.Target.OwnerId);
            if(owner != null)
            {
                ImGui.Text("Owner: " + owner.Name.ToString());
            }
        }
        if (Svc.Targets.Target is BattleChara b)
        {
            ImGui.Text("HP: " + b.CurrentHp + " / " + b.MaxHp);
            ImGui.Text("Is Boss: " + b.IsBoss().ToString());
            ImGui.Text("Has Positional: " + b.HasPositional().ToString());
            ImGui.Text("Is Dying: " + b.IsDying().ToString());
            ImGui.Text("EventType: " + b.GetEventType().ToString());
            ImGui.Text("NamePlate: " + b.GetNamePlateIcon().ToString());
            ImGui.Text("StatusFlags: " + b.StatusFlags.ToString());
            ImGui.Text("InView: " + Svc.GameGui.WorldToScreen(b.Position, out _).ToString());
            ImGui.Text("Name Id: " + b.NameId.ToString());
            ImGui.Text("Data Id: " + b.DataId.ToString());
            ImGui.Text("Targetable: " + b.Struct()->Character.GameObject.TargetableStatus.ToString());

            var npc = b.GetObjectNPC();
            if(npc != null)
            {
                ImGui.Text("Unknown12: " + npc.Unknown12.ToString());

                //ImGui.Text("Unknown15: " + npc.Unknown15.ToString());
                //ImGui.Text("Unknown18: " + npc.Unknown18.ToString());
                //ImGui.Text("Unknown19: " + npc.Unknown19.ToString());
                //ImGui.Text("Unknown20: " + npc.Unknown20.ToString());
                //ImGui.Text("Unknown21: " + npc.Unknown21.ToString());
            }

            foreach (var status in b.StatusList)
            {
                var source = status.SourceId == Player.Object.ObjectId ? "You" : Svc.Objects.SearchById(status.SourceId) == null ? "None" : "Others";
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

        ImGui.Text("Ability Remain: " + DataCenter.AbilityRemain.ToString());
        ImGui.Text("Action Remain: " + DataCenter.ActionRemain.ToString());
        ImGui.Text("Weapon Remain: " + DataCenter.WeaponRemain.ToString());
        ImGui.Text("Time: " + (DataCenter.CombatTimeRaw + DataCenter.WeaponRemain).ToString());

        ActionUpdater.NextAction?.Display(false);
    }

    private void DrawLastAction()
    {
        DrawAction(DataCenter.LastAction, nameof(DataCenter.LastAction));
        DrawAction(DataCenter.LastAbility, nameof(DataCenter.LastAbility));
        DrawAction(DataCenter.LastGCD, nameof(DataCenter.LastGCD));
        DrawAction(DataCenter.LastComboAction, nameof(DataCenter.LastComboAction));
    }

    private unsafe void DrawIcon()
    {
    }

    private static void DrawAction(ActionID id, string type)
    {
        ImGui.Text($"{type}: {id}");
    }
}
