using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Interface.Colors;
using Dalamud.Logging;
using Dalamud.Plugin.Ipc;
using Dalamud.Utility.Signatures;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Hooks;
using ECommons.Hooks.ActionEffectTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;
using System.Text.RegularExpressions;

namespace RotationSolver;

public static class Watcher
{
    public static ICallGateSubscriber<object, object> IpcSubscriber;

    public static void Enable()
    {
        IpcSubscriber = Svc.PluginInterface.GetIpcSubscriber<object, object>("PingPlugin.Ipc");
        IpcSubscriber.Subscribe(UpdateRTTDetour);

        ActionEffect.ActionEffectEvent += ActionFromEnemy;
        ActionEffect.ActionEffectEvent += ActionFromSelf;
    }

    public static void Disable()
    {
        IpcSubscriber.Unsubscribe(UpdateRTTDetour);
        ActionEffect.ActionEffectEvent -= ActionFromEnemy;
        ActionEffect.ActionEffectEvent -= ActionFromSelf;
    }

    private static void UpdateRTTDetour(dynamic obj)
    {
        PluginLog.LogDebug($"LastRTT:{obj.LastRTT}");
        DataCenter.RTT = (long)obj.LastRTT / 1000f;
    }

    public static string ShowStrSelf { get; private set; } = string.Empty;
    public static string ShowStrEnemy { get; private set; } = string.Empty;

    private static void ActionFromEnemy(ActionEffectSet set)
    {
        //Check Source.
        var source = set.Source;
        if (source == null) return;
        if (source is not BattleChara battle) return;
        if (battle is PlayerCharacter) return;
        if (battle.SubKind == 9) return; //Friend!
        if (Svc.Objects.SearchById(battle.ObjectId) is PlayerCharacter) return;

        var damageRatio = set.TargetEffects
            .Where(e => e.TargetID == Player.Object.ObjectId)
            .SelectMany(e => new EffectEntry[]
            {
                e[0], e[1], e[2], e[3], e[4], e[5], e[6], e[7]
            })
            .Where(e => e.type == ActionEffectType.Damage)
            .Sum(e => (float)e.value / Player.Object.MaxHp);

        DataCenter.AddDamageRec(damageRatio);
        ShowStrEnemy = $"Damage Ratio: {damageRatio}\n{set}";

        if(set.Header.ActionType == ActionType.Spell && DataCenter.PartyMembers.Count() >= 4 && set.Action.Cast100ms > 0)
        {
            var type = set.Action.GetActionType();

            if(type is ActionCate.Spell or ActionCate.WeaponSkill or ActionCate.Ability)
            {
                if (set.TargetEffects.Count(e =>
                    DataCenter.PartyMembers.Any(p => p.ObjectId == e.TargetID)
                    && e.GetSpecificTypeEffect(ActionEffectType.Damage, out var effect)
                    && (effect.value > 0 || (effect.param0 & 6) == 6))
                    == DataCenter.PartyMembers.Count())
                {
                    if (Service.Config.RecordCastingArea)
                    {
                        OtherConfiguration.HostileCastingArea.Add(set.Action.RowId);
                        OtherConfiguration.SaveHostileCastingArea();
                    }
                }
            }
        }
    }

    private static void ActionFromSelf(ActionEffectSet set)
    {
        if (set.Source.ObjectId != Player.Object.ObjectId) return;
        if (set.Header.ActionType != ActionType.Spell && set.Header.ActionType != ActionType.Item) return;
        if (set.Action == null) return;
        if ((ActionCate)set.Action.ActionCategory.Value.RowId == ActionCate.AutoAttack) return;

        var id = set.Action.RowId;
        if(set.Action.ClassJob.Row > 0 || Enum.IsDefined((ActionID)id))
        {
            OtherConfiguration.AnimationLockTime[id] = set.Header.AnimationLockTime;
        }

        if (!set.TargetEffects.Any()) return;
        var flag = set.TargetEffects.FirstOrDefault()[0].param2;

        var action = set.Action;
        var tar = set.Target;

        if (tar == null || action == null) return;

        //Record
        DataCenter.AddActionRec(action);
        ShowStrSelf = set.ToString();

        DataCenter.HealHP = set.GetSpecificTypeEffect(ActionEffectType.Heal);
        DataCenter.ApplyStatus = set.GetSpecificTypeEffect(ActionEffectType.ApplyStatusEffectTarget);
        DataCenter.MPGain = (uint)set.GetSpecificTypeEffect(ActionEffectType.MpGain).Where(i => i.Key == Player.Object.ObjectId).Sum(i => i.Value);
        DataCenter.EffectTime = DateTime.Now;
        DataCenter.EffectEndTime = DateTime.Now.AddSeconds(set.Header.AnimationLockTime + 1);

        //Macro
        foreach (var item in Service.Config.Events)
        {
            if (!new Regex(item.Name).Match(action.Name).Success) continue;
            if (item.AddMacro(tar)) break;
        }

        if (flag != 0 && Service.Config.ShowActionFlag)
        {
            Svc.FlyText.AddFlyText(Dalamud.Game.Gui.FlyText.FlyTextKind.NamedIcon, 0, 0, 0, "Flag:" + flag.ToString(), "",
            ImGui.GetColorU32(ImGuiColors.DPSRed), 0, action.Icon);
        }
    }
}
