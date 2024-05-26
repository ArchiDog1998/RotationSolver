using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using RotationSolver.Updaters;
using System.ComponentModel;
using XIVConfigUI;
using ECommons.GameFunctions;

namespace RotationSolver.UI.ConfigWindows;

[Description("Debug")]
public unsafe class DebugItem : ConfigWindowItemRS
{
    public override uint Icon => 5;
    public override void Draw(ConfigWindow window)
    {
        if (Player.Available)
        {
            ImGui.Text("Hash: ");
            ImGui.SameLine();
            var hash = Player.Object.EncryptString();
            if (ImGui.Button(hash))
            {
                ImGui.SetClipboardText(hash);
                Notify.Success(UiString.CopiedYourHash.Local());
            }
        }

        window.Collection.DrawItems(-1);

        if (!Player.Available || !Service.Config.InDebug) return;

        _debugHeader?.Draw();
    }

    private static readonly CollapsingHeaderGroup _debugHeader = new(new()
    {
        {() => DataCenter.RightNowRotation != null ? "Rotation" : string.Empty, DrawDebugRotationStatus},
        {() =>"Status", DrawStatus },
        {() =>"Party", DrawParty },
        {() =>"Target Data", DrawTargetData },
        {() =>"Next Action", DrawNextAction },
        {() =>"Last Action", DrawLastAction },
        {() =>"Others", DrawOthers },
        {() =>"Effect",  () =>
            {
                ImGui.Text(Watcher.ShowStrSelf);
                ImGui.Separator();
                ImGui.Text(DataCenter.Role.ToString());
            } },
        });

    private static void DrawDebugRotationStatus()
    {
        DataCenter.RightNowRotation?.DisplayStatus();
    }

    private static unsafe void DrawStatus()
    {
        if ((nint)FateManager.Instance() != nint.Zero)
        {
            ImGui.Text("Fate: " + DataCenter.FateId.ToString());
        }
        ImGui.Text("Height: " + Player.Character->CalculateHeight().ToString());
        ImGui.Text("Moving: " + DataCenter.IsMoving.ToString());
        ImGui.Text("Stop Moving: " + DataCenter.StopMovingRaw.ToString());

        ImGui.Text("TerritoryType: " + DataCenter.TerritoryContentType.ToString());
        ImGui.Text("DPSTaken: " + DataCenter.DPSTaken.ToString());
        ImGui.Text("TimeToNext: " + DataCenter.NextAbilityToNextGCD.ToString());
        ImGui.Text("WeaponElapsed: " + DataCenter.WeaponElapsed.ToString());
        ImGui.Text("AnimationLock: " + DataCenter.AnimationLocktime.ToString());

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
    private static unsafe void DrawParty()
    {
        ImGui.Text("Party: " + DataCenter.PartyMembers.Length.ToString());
        ImGui.Text("Alliance: " + DataCenter.AllianceMembers.Length.ToString());

        ImGui.Text("PartyMembersAverHP: " + DataCenter.PartyMembersAverHP.ToString());

        ImGui.Text($"Your combat state: {DataCenter.InCombat}");
        ImGui.Text($"Your character combat: {Player.Object.InCombat()}");
        foreach (var p in Svc.Party)
        {
            if (p.GameObject is not BattleChara b) continue;
            ImGui.Text($"In Combat: {b.InCombat()}");
        }
    }

    private static unsafe void DrawTargetData()
    {
        if (Svc.Targets.Target != null)
        {
            ImGui.Text("Height: " + Svc.Targets.Target.Struct()->Height.ToString());
            ImGui.Text("Kind: " + Svc.Targets.Target.GetObjectKind().ToString());
            ImGui.Text("SubKind: " + Svc.Targets.Target.GetBattleNPCSubKind().ToString());
            var owner = Svc.Objects.SearchById(Svc.Targets.Target.OwnerId);
            if (owner != null)
            {
                ImGui.Text("Owner: " + owner.Name.ToString());
            }
        }
        if (Svc.Targets.Target is BattleChara b)
        {
            ImGui.Text("HP: " + b.CurrentHp + " / " + b.MaxHp);
            ImGui.Text("Is Boss TTK: " + b.IsBossFromTTK().ToString());
            ImGui.Text("Is Boss Icon: " + b.IsBossFromIcon().ToString());
            ImGui.Text("Rank: " + b.GetObjectNPC()?.Rank.ToString() ?? string.Empty);
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
            if (npc != null)
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
        ImGui.Text("Hostile: " + DataCenter.AllHostileTargets.Length.ToString());
        foreach (var item in DataCenter.AllHostileTargets)
        {
            ImGui.Text(item.Name.ToString());
        }
    }

    private static void DrawNextAction()
    {
        ImGui.Text(DataCenter.RightNowRotation?.GetType().GetCustomAttribute<RotationAttribute>()!.Name);
        ImGui.Text(DataCenter.SpecialType.ToString());

        ImGui.Text(ActionUpdater.NextAction?.Name ?? "null");
        ImGui.Text("Ability Remain: " + DataCenter.AbilityRemain.ToString());
        ImGui.Text("Action Remain: " + DataCenter.AnimationLocktime.ToString());
        ImGui.Text("Weapon Remain: " + DataCenter.WeaponRemain.ToString());
    }

    private static void DrawLastAction()
    {
        DrawAction(DataCenter.LastAction, nameof(DataCenter.LastAction));
        DrawAction(DataCenter.LastAbility, nameof(DataCenter.LastAbility));
        DrawAction(DataCenter.LastGCD, nameof(DataCenter.LastGCD));
        DrawAction(DataCenter.LastComboAction, nameof(DataCenter.LastComboAction));
    }

    private static unsafe void DrawOthers()
    {
        ImGui.Text("Combat Time: " + DataCenter.CombatTimeRaw.ToString());
        ImGui.Text("Limit Break: " + CustomRotation.LimitBreakLevel.ToString());
    }

    private static void DrawAction(ActionID id, string type)
    {
        ImGui.Text($"{type}: {id}");
    }
}
