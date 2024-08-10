using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using NRender;
using RotationSolver.Commands;
using RotationSolver.Vfx;

namespace RotationSolver.Updaters;

internal static class ActionUpdater
{
    internal static DateTime AutoCancelTime { get; set; } = DateTime.MinValue;

    static RandomDelay _GCDDelay = new(() => Service.Config.WeaponDelay);

    private static IAction? _nextAction;
    internal static IAction? NextAction
    {
        get => _nextAction;
        set
        {
            if (_nextAction == value) return;
            _nextAction = value;
            Service.NextActionID = value?.ID ?? 0;
        }
    }

    private static StaticVfx? circle, sector, rectangle, positionalSect, line;
    private static IBaseAction? _nextGCDAction;
    const float gcdHeight = 5, positionalHeight = 1;
    internal static IBaseAction? NextGCDAction
    {
        get => _nextGCDAction;
        set
        {
            UpdateOmen(value);
            UpdatePositional(value);
            if (_nextGCDAction == value) return;
            _nextGCDAction = value;
        }
    }

    private static void UpdateOmen(IBaseAction? value)
    {
        var player = Player.Object;
        if (player == null) return;

        circle ??= new StaticVfx(StaticOmen.Circle, default, player, Service.Config.TargetOmenColor);
        sector ??= new StaticVfx(StaticOmen.Fan, default, player, Service.Config.TargetOmenColor)
        {
            Radian = MathF.PI * 2 / 3,
            FixRotation = true,
        };
        rectangle ??= new StaticVfx(StaticOmen.Rect, default, player, Service.Config.TargetOmenColor)
        {
            FixRotation = true,
        };

        circle.Scale = sector.Scale = rectangle.Scale = default;
        circle.Owner = sector.Owner = rectangle.Owner = player;

        if (!Service.Config.ShowTarget) return;
        if (value == null) return;

        var target = value.Target.Target ?? player;

        var range = value.Action.EffectRange;
        var size = new Vector3(range, gcdHeight, range);
        var dir = target.Position - player.Position;
        var rotation = MathF.Atan2(dir.X, dir.Z);
        switch (value.Action.CastType)
        {
            //case 1:
            case 2 when Service.Config.ShowCircleTarget:
                circle.Owner = target;
                circle.Scale = size;
                circle.Color = Service.Config.TargetOmenColor;
                break;

            case 3 when Service.Config.ShowSectorTarget:
                sector.Scale = size;
                sector.Rotation = rotation;
                sector.Color = Service.Config.TargetOmenColor;

                break;

            case 4 when Service.Config.ShowRectangleTarget:
                size.X = value.Action.XAxisModifier / 2;
                rectangle.Scale = size;
                rectangle.Rotation = rotation;
                rectangle.Color = Service.Config.TargetOmenColor;
                break;
        }
    }

    private static void UpdatePositional(IBaseAction? value)
    {
        var player = Player.Object;
        if (player == null) return;

        line ??= new StaticVfx(StaticOmen.Rect, default, player, Service.Config.PositionalColor);
        positionalSect ??= new StaticVfx(StaticOmen.Fan, default, player, Service.Config.PositionalColor)
        {
            Radian = MathF.PI / 2,
        };

        positionalSect.Scale = line.Scale = default;
        positionalSect.Owner = line.Owner = player;

        if (!Service.Config.ShowPositional) return;
        if (value == null) return;
        if (!player.IsJobCategory(JobRole.Melee)) return;
        if (value.Target.Target is not IBattleChara enemy) return;

        Vector3 pPosition = enemy.Position;
        Vector2 faceVec = enemy.GetFaceVector();

        Vector3 dir = player.Position - pPosition;
        Vector2 dirVec = new(dir.Z, dir.X);

        bool isLeft = faceVec.X * dirVec.Y > faceVec.Y * dirVec.X;

        var scale = new Vector3(0, positionalHeight, enemy.HitboxRadius + 3 + player.HitboxRadius);

        var correct = enemy.FindEnemyPositional() == value.Setting.EnemyPositional;
        line.Color = Service.Config.PositionalColor;
        positionalSect.Color = correct ? Service.Config.PositionalCorrectColor : Service.Config.PositionalColor;

        switch (value.Setting.EnemyPositional)
        {
            case EnemyPositional.Rear:
                positionalSect.Owner = enemy;
                scale.X = scale.Z;
                positionalSect.Scale = scale;
                positionalSect.Rotation = MathF.PI;
                break;

            case EnemyPositional.Flank:
                positionalSect.Owner = enemy;
                scale.X = scale.Z;

                positionalSect.Scale = scale;

                if (isLeft)
                {
                    positionalSect.Rotation = MathF.PI / 2;
                }
                else
                {
                    positionalSect.Rotation = -MathF.PI / 2;
                }

                break;

            default:
                if (enemy == player) break;
                if (!Service.Config.ShowPositionalLine) break;

                line.Owner = enemy;
                scale.X = Service.Config.PositionalLineWidth;
                scale.Z = Service.Config.PositionalLineLength + enemy.HitboxRadius + player.HitboxRadius;

                line.Scale = scale;
                if (isLeft)
                {
                    line.Rotation = MathF.PI * 3 / 4;
                }
                else
                {
                    line.Rotation = -MathF.PI * 3 / 4;
                }
                break;
        }
    }

    internal static IAction? WrongAction { get; set; }
    static readonly Random _wrongRandom = new();

    internal static void ClearNextAction()
    {
        SetAction(0);
        WrongAction = NextAction = NextGCDAction = null;
    }

    internal static void UpdateNextAction()
    {
        IPlayerCharacter localPlayer = Player.Object;
        var customRotation = DataCenter.RightNowRotation;

        try
        {
            if (localPlayer != null && customRotation != null
                && customRotation.TryInvoke(out var newAction, out var gcdAction))
            {
                if (Service.Config.MistakeRatio > 0)
                {
                    var actions = customRotation.AllActions.Where(a =>
                    {
                        if (a.ID == newAction?.ID) return false;
                        if (a is IBaseAction action)
                        {
                            return !action.Setting.IsFriendly && action.Config.IsInMistake
                            && action.Setting.TargetType != TargetType.Move
                            && action.CanUse(out _, usedUp: true, skipStatusProvideCheck: true, skipClippingCheck: true, skipAoeCheck: true);
                        }
                        return false;
                    });

                    var count = actions.Count();
                    WrongAction = count > 0 ? actions.ElementAt(_wrongRandom.Next(count)) : null;
                }

                NextAction = newAction;

                if (gcdAction == null)
                {
                    NextGCDAction = null;
                }
                if (gcdAction is IBaseAction GcdAction)
                {
                    NextGCDAction = GcdAction;
                }
                return;
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Failed to update next action.");
        }

        WrongAction = NextAction = NextGCDAction = null;
    }

    private static List<uint>? actionOverrideList;

    private static void SetAction(uint id)
    {
        actionOverrideList ??= Svc.PluginInterface.GetOrCreateData("Avarice.ActionOverride", () => new List<uint>());

        if (actionOverrideList.Count == 0)
        {
            actionOverrideList.Add(id);
        }
        else
        {
            actionOverrideList[0] = id;
        }
    }

    internal unsafe static void UpdateActionInfo()
    {
        SetAction(NextGCDAction?.ID ?? 0);
        UpdateWeaponTime();
        UpdateCombatTime();
        UpdateSlots();
        UpdateMoving();
    }

    private unsafe static void UpdateSlots()
    {
        for (int i = 0; i < DataCenter.BluSlots.Length; i++)
        {
            DataCenter.BluSlots[i] = ActionManager.Instance()->GetActiveBlueMageActionInSlot(i);
        }
        for (ushort i = 0; i < DataCenter.DutyActions.Length; i++)
        {
            DataCenter.DutyActions[i] = ActionManager.GetDutyActionId(i);
        }
    }

    static DateTime _stopMovingTime = DateTime.MinValue;
    private unsafe static void UpdateMoving()
    {
        var last = DataCenter.IsMoving;
        DataCenter.IsMoving = AgentMap.Instance()->IsPlayerMoving > 0;
        if (last && !DataCenter.IsMoving)
        {
            _stopMovingTime = DateTime.Now;
        }
        else if (DataCenter.IsMoving)
        {
            _stopMovingTime = DateTime.MinValue;
        }

        if (_stopMovingTime == DateTime.MinValue)
        {
            DataCenter.StopMovingRaw = 0;
        }
        else
        {
            DataCenter.StopMovingRaw = (float)(DateTime.Now - _stopMovingTime).TotalSeconds;
        }
    }

    static DateTime _startCombatTime = DateTime.MinValue;
    private static void UpdateCombatTime()
    {
        var last = DataCenter.InCombat;
        DataCenter.InCombat = Svc.Condition[ConditionFlag.InCombat];
        if (!last && DataCenter.InCombat)
        {
            _startCombatTime = DateTime.Now;
        }
        else if (last && !DataCenter.InCombat)
        {
            _startCombatTime = DateTime.MinValue;

            if (Service.Config.AutoOffAfterCombat)
            {
                AutoCancelTime = DateTime.Now.AddSeconds(Service.Config.AutoOffAfterCombatTime);
            }
        }

        if (_startCombatTime == DateTime.MinValue)
        {
            DataCenter.CombatTimeRaw = 0;
        }
        else
        {
            if (DataCenter.CombatTimeRaw == 0)
            {
                foreach (var item in DataCenter.TimelineItems)
                {
                    if (!item.IsInWindow) continue;
                    if (item.Type is not TimelineType.InCombat) continue;

                    item.UpdateRaidTimeOffset();
                    break;
                }
            }
            DataCenter.CombatTimeRaw = (float)(DateTime.Now - _startCombatTime).TotalSeconds;
        }
    }

    private static unsafe void UpdateWeaponTime()
    {
        var player = Player.Object;
        if (player == null) return;

        var instance = ActionManager.Instance();

        var castTotal = player.TotalCastTime;

        var weaponTotal = instance->GetRecastTime(ActionType.Action, 11);
        if (castTotal > 0) castTotal += 0.1f;
        if (player.IsCasting) weaponTotal = Math.Max(castTotal, weaponTotal);

        DataCenter.WeaponElapsed = instance->GetRecastTimeElapsed(ActionType.Action, 11);
        DataCenter.WeaponRemain = DataCenter.WeaponElapsed == 0 ? player.TotalCastTime - player.CurrentCastTime
            : Math.Max(weaponTotal - DataCenter.WeaponElapsed, player.TotalCastTime - player.CurrentCastTime);

        //Casting time.
        if (DataCenter.WeaponElapsed < 0.3) DataCenter.CastingTotal = castTotal;
        if (weaponTotal > 0 && DataCenter.WeaponElapsed > 0.2) DataCenter.WeaponTotal = weaponTotal;
    }

    internal unsafe static bool CanDoAction()
    {
        if (Svc.Condition[ConditionFlag.OccupiedInQuestEvent]
            || Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent]
            || Svc.Condition[ConditionFlag.Occupied33]
            || Svc.Condition[ConditionFlag.Occupied38]
            || Svc.Condition[ConditionFlag.Jumping61]
            || Svc.Condition[ConditionFlag.BetweenAreas]
            || Svc.Condition[ConditionFlag.BetweenAreas51]
            || Svc.Condition[ConditionFlag.Mounted]
            //|| Svc.Condition[ConditionFlag.SufferingStatusAffliction] //Because of BLU30!
            || Svc.Condition[ConditionFlag.SufferingStatusAffliction2]
            || Svc.Condition[ConditionFlag.RolePlaying]
            || Svc.Condition[ConditionFlag.InFlight]
            || (ActionManager.Instance()->ActionQueued && NextAction != null
                && ActionManager.Instance()->QueuedActionId != NextAction.ID)
            || Player.Object.CurrentHp == 0) return false;

        var maxAhead = Math.Max(DataCenter.MinAnimationLock - DataCenter.Ping, 0.08f);
        var ahead = Math.Min(maxAhead, Service.Config.ActionAhead);

        //GCD
        var canUseGCD = DataCenter.WeaponRemain <= ahead;
        if (_GCDDelay.Delay(canUseGCD))
        {
            return RSCommands.CanDoAnAction(true);
        }
        if (canUseGCD) return false;

        var nextAction = NextAction;
        if (nextAction == null) return false;

        var timeToNext = DataCenter.AnimationLocktime;

        //Skip when casting
        if (DataCenter.WeaponElapsed <= DataCenter.CastingTotal) return false;

        //The last one.
        if (timeToNext + nextAction.AnimationLockTime + DataCenter.Ping + DataCenter.MinAnimationLock > DataCenter.WeaponRemain)
        {
            if (DataCenter.WeaponRemain > nextAction.AnimationLockTime + DataCenter.Ping +
                Math.Max(ahead, Service.Config.MinLastAbilityAdvanced)) return false;

            return RSCommands.CanDoAnAction(false);
        }
        else if (timeToNext < ahead)
        {
            return RSCommands.CanDoAnAction(false);
        }

        return false;
    }
}
