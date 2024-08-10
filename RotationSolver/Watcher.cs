using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Plugin.Ipc;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.Hooks;
using ECommons.Hooks.ActionEffectTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using NRender.Vfx;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Record;
using System.Text.RegularExpressions;
using GameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace RotationSolver;

public static class Watcher
{
#if DEBUG
    private unsafe delegate bool OnUseAction(ActionManager* manager, ActionType actionType, uint actionID, ulong targetID, uint a4, uint a5, uint a6, void* a7);
    private static Hook<OnUseAction>? _useActionHook;
#endif

    private unsafe delegate long ProcessObjectEffect(GameObject* a1, ushort a2, ushort a3, long a4);
    private static Hook<ProcessObjectEffect>? _processObjectEffectHook;


    private static ICallGateSubscriber<object, object>? IpcSubscriber;

    public static void Enable()
    {
        unsafe
        {
#if DEBUG
            _useActionHook = Svc.Hook.HookFromSignature<OnUseAction>("E8 ?? ?? ?? ?? B0 01 EB B6", UseActionDetour);
            //_useActionHook.Enable();
#endif
            VfxManager.OnActorVfxCreateEvent += VfxManager_OnActorVfxCreateEvent;

            //From https://github.com/PunishXIV/Splatoon/blob/main/Splatoon/Memory/ObjectEffectProcessor.cs#L14
            _processObjectEffectHook = Svc.Hook.HookFromSignature<ProcessObjectEffect>("40 53 55 57 41 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 44 0F B7 F2", ProcessObjectEffectDetour);
            _processObjectEffectHook.Enable();
        }
        IpcSubscriber = Svc.PluginInterface.GetIpcSubscriber<object, object>("PingPlugin.Ipc");
        IpcSubscriber.Subscribe(UpdateRTTDetour);

        ActionEffect.ActionEffectEvent += ActionFromEnemy;
        ActionEffect.ActionEffectEvent += ActionFromSelf;
        MapEffect.Init((a1, position, param1, param2) =>
        {
            var effect = new MapEffectData(position, param1, param2);
            Recorder.Enqueue(effect);
        });
    }

    private static void VfxManager_OnActorVfxCreateEvent(string path, ulong objectId)
    {
        var obj = Svc.Objects.SearchById(objectId);

        if (NotFrom(path) && obj != null)
        {
            var effect = new VfxNewData(obj, path);
            Recorder.Enqueue(effect);
        }

        static bool NotFrom(string path)
        {
            return NotName(path, "dk") && NotName(path, "cmpp") && Enum.GetNames(typeof(Job)).All(s => NotName(path, s.ToLower()));

            static bool NotName(string path, string trail)
            {
                return !path.StartsWith("vfx/common/eff/" + trail, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    public static void Disable()
    {
#if DEBUG
        _useActionHook?.Dispose();
#endif
        _processObjectEffectHook?.Dispose();

        IpcSubscriber?.Unsubscribe(UpdateRTTDetour);
        MapEffect.Dispose();
        ActionEffect.ActionEffectEvent -= ActionFromEnemy;
        ActionEffect.ActionEffectEvent -= ActionFromSelf;
    }

    private static unsafe long ProcessObjectEffectDetour(GameObject* a1, ushort a2, ushort a3, long a4)
    {
        try
        {
            var obj = Svc.Objects.CreateObjectReference((nint)a1);

            if (obj != null)
            {
                var effect = new ObjectEffectData(obj, a2, a3);
                Recorder.Enqueue(effect);
            }
        }
        catch (Exception e)
        {
            Svc.Log.Warning(e, "Failed to execute the object effect!");
        }
        return _processObjectEffectHook!.Original(a1, a2, a3, a4);
    }
#if DEBUG
    private static unsafe bool UseActionDetour(ActionManager* manager, ActionType actionType, uint actionID, ulong targetID, uint a4, uint a5, uint a6, void* a7)
    {
        try
        {
            Svc.Log.Debug($"Type: {actionType}, ID: {actionID}, Tar: {targetID}, 4: {a4}, 5: {a5}, 6: {a6}");
        }
        catch (Exception e)
        {
            Svc.Log.Warning(e, "Failed to detour actions");
        }
        return _useActionHook!.Original(manager, actionType, actionID, targetID, a4, a5, a6, a7);
    }
#endif

    private static void UpdateRTTDetour(dynamic obj)
    {
        Svc.Log.Verbose($"LastRTT:{obj.LastRTT}");
        DataCenter.RTT = (long)obj.LastRTT / 1000f;
    }

    public static string ShowStrSelf { get; private set; } = string.Empty;
    public static string ShowStrEnemy { get; private set; } = string.Empty;

    private static void ActionFromEnemy(ActionEffectSet set)
    {
        //Check Source.
        var source = set.Source;
        if (source == null) return;
        if (source is not IBattleChara battle) return;
        if (battle is IPlayerCharacter) return;
        if (battle.SubKind == 9) return; //Friend!
        if (Svc.Objects.SearchById(battle.EntityId) is IPlayerCharacter) return;

        Recorder.Enqueue(new ActionEffectSetData(set));

        var damageRatio = set.TargetEffects
            .Where(e => e.TargetID == Player.Object.EntityId)
            .SelectMany(e => new EffectEntry[]
            {
                e[0], e[1], e[2], e[3], e[4], e[5], e[6], e[7]
            })
            .Where(e => e.type == ActionEffectType.Damage)
            .Sum(e => (float)e.value / Player.Object.MaxHp);

        DataCenter.AddDamageRec(damageRatio);

        ShowStrEnemy = $"Damage Ratio: {damageRatio}\n{set}";

        foreach (var effect in set.TargetEffects)
        {
            if (effect.TargetID != Player.Object.EntityId) continue;
            if (effect.GetSpecificTypeEffect(ActionEffectType.Knockback, out var entry))
            {
                var knock = Svc.Data.GetExcelSheet<Knockback>()?.GetRow(entry.value);
                if (knock != null)
                {
                    DataCenter.KnockbackStart = DateTime.Now;
                    DataCenter.KnockbackFinished = DateTime.Now + TimeSpan.FromSeconds(knock.Distance / (float)knock.Speed);
                }

                if (!OtherConfiguration.HostileCastingKnockback.Contains(set.Action.RowId) && Service.Config.RecordKnockback)
                {
                    OtherConfiguration.HostileCastingKnockback.Add(set.Action.RowId);
                    OtherConfiguration.Save();
                }
                break;
            }
        }

        if (set.Header.ActionType == ActionType.Action && DataCenter.PartyMembers.Length >= 4 && set.Action?.Cast100ms > 0)
        {
            var type = set.Action.GetActionCate();

            if (type is ActionCate.Spell or ActionCate.Weaponskill or ActionCate.Ability)
            {
                if (set.TargetEffects.Count(e =>
                    DataCenter.PartyMembers.Any(p => p.EntityId == e.TargetID)
                    && e.GetSpecificTypeEffect(ActionEffectType.Damage, out var effect)
                    && (effect.value > 0 || (effect.param0 & 6) == 6))
                    == DataCenter.PartyMembers.Length)
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
        if (set.Source.EntityId != Player.Object.EntityId) return;
        if (set.Header.ActionType != ActionType.Action && set.Header.ActionType != ActionType.Item) return;
        if (set.Action == null) return;
        if ((ActionCate)set.Action.ActionCategory.Value!.RowId == ActionCate.Autoattack) return;

        var id = set.Action.RowId;
        if (!set.Action.IsRealGCD() && (set.Action.ClassJob.Row > 0 || Enum.IsDefined((ActionID)id)))
        {
            OtherConfiguration.AnimationLockTime[id] = set.Header.AnimationLockTime;
        }

        if (set.TargetEffects.Length == 0) return;

        var action = set.Action;
        var tar = set.Target;

        if (tar == null || action == null) return;

        //Record
        DataCenter.AddActionRec(action);
        ShowStrSelf = set.ToString();

        DataCenter.HealHP = set.GetSpecificTypeEffect(ActionEffectType.Heal);

        var targetEffects = set.GetSpecificTypeEffect(ActionEffectType.ApplyStatusEffectTarget);
        OtherConfiguration.TargetStatusProvide[action.RowId] = [..targetEffects.Values.Select(i => (StatusID)(ushort)i).ToHashSet()];
        DataCenter.ApplyStatus = targetEffects;

        var sourceEffects = set.GetSpecificTypeEffect(ActionEffectType.ApplyStatusEffectSource);
        OtherConfiguration.StatusProvide[action.RowId] = [.. sourceEffects.Values.Select(i => (StatusID)(ushort)i).ToHashSet()];
        foreach (var effect in sourceEffects)
        {
            DataCenter.ApplyStatus[effect.Key] = effect.Value;
        }
        DataCenter.MPGain = (uint)set.GetSpecificTypeEffect(ActionEffectType.MpGain).Where(i => i.Key == Player.Object.EntityId).Sum(i => i.Value);
        DataCenter.EffectTime = DateTime.Now;
        DataCenter.EffectEndTime = DateTime.Now.AddSeconds(set.Header.AnimationLockTime + 1);

        foreach (var effect in set.TargetEffects)
        {
            if (!effect.GetSpecificTypeEffect(ActionEffectType.Damage, out _)) continue;

            if (DataCenter.AttackedTargets.Any(i => i.id == effect.TargetID)) continue;

            if (DataCenter.AttackedTargets.Count >= DataCenter.ATTACKED_TARGETS_COUNT)
            {
                DataCenter.AttackedTargets.Dequeue();
            }
            DataCenter.AttackedTargets.Enqueue((effect.TargetID, DateTime.Now));
        }

        //Macro
        foreach (var item in Service.Config.Events)
        {
            if (!new Regex(item.Name).Match(action.Name).Success) continue;
            if (item.AddMacro(tar)) break;
        }
    }
}
