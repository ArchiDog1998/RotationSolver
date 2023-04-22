using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using RotationSolver.Updaters;

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
            if(RotationUpdater.RightNowRotation != null)
            {
                DrawParamTabItem("Rotation", RotationUpdater.RightNowRotation.DisplayStatus);
            }
            DrawParamTabItem("Status", DrawStatus);
            DrawParamTabItem("Party", DrawParty);
            DrawParamTabItem("Target Data", DrawTargetData);
            DrawParamTabItem("Next Action", DrawNextAction);
            DrawParamTabItem("Last Action", DrawLastAction);
            DrawParamTabItem("CD, EX", DrawCDEX);
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
        ImGui.Text("TerritoryType: " + DataCenter.TerritoryContentType.ToString());
        ImGui.Text("DPSTaken: " + DataCenter.DPSTaken.ToString());
        ImGui.Text("TimeToNext: " + DataCenter.NextAbilityToNextGCD.ToString());
        ImGui.Text("WeaponElapsed: " + DataCenter.WeaponElapsed.ToString());
        ImGui.Text("AnimationLock: " + DataCenter.ActionRemain.ToString());
        ImGui.Text("Ping: " + DataCenter.Ping.ToString());  

        ImGui.Text("Have pet: " + DataCenter.HasPet.ToString());
        ImGui.Text("Hostile Near Count: " + DataCenter.NumberOfHostilesInRange.ToString());
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

        ImGui.Text("Party Burst Ratio: " + DataCenter.RatioOfMembersIn2minsBurst.ToString());
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
            ImGui.Text("Name Id: " + b.NameId.ToString());
            ImGui.Text("Data Id: " + b.DataId.ToString());
            ImGui.Text("Targetable: " + b.GetAddress()->TargetableStatus.ToString());

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
        ImGui.Text("Action Remain: " + DataCenter.ActionRemain.ToString());
        ImGui.Text("Weapon Remain: " + DataCenter.WeaponRemain.ToString());
        ImGui.Text("Elapsed: " + CustomRotation.CombatElapsedLess(10).ToString());
        ImGui.Text("Time: " + (DataCenter.CombatTime + DataCenter.WeaponRemain).ToString());

    }
    private void DrawLastAction()
    {
        DrawAction(DataCenter.LastAction, nameof(DataCenter.LastAction));
        DrawAction(DataCenter.LastAbility, nameof(DataCenter.LastAbility));
        DrawAction(DataCenter.LastGCD, nameof(DataCenter.LastGCD));
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

    private unsafe void DrawIcon()
    {
        //var pointer = (AddonActionCross*) Service.GetAddon<AddonActionCross>();
        //if (pointer != null) 
        //{
        //    ImGui.Text($"LTRT: {pointer->ExpandedHoldControlsLTRT}");
        //    ImGui.Text($"RTLT: {pointer->ExpandedHoldControlsRTLT}");
        //}
        //var pointer2 = (AddonActionDoubleCrossBase*)Service.GetAddon<AddonActionDoubleCrossBase>();
        //if (pointer2 != null)
        //{
        //    ImGui.Text($"ShowDPadSlots: {pointer2->ShowDPadSlots}");
        //    ImGui.Text($"BarTarget: {pointer2->BarTarget}");
        //    ImGui.Text($"UseLeftSide: {pointer2->UseLeftSide}");
        //    ImGui.Text($"MergedPositioning: {pointer2->MergedPositioning}");
        //}
    }

    private static void DrawAction(ActionID id, string type)
    {
        ImGui.Text($"{type}: {id}");
    }
}
