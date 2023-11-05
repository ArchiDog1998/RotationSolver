using ECommons.DalamudServices;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Configuration.Conditions;

internal class TargetCondition : DelayCondition
{
    internal IBaseAction _action;
    public ActionID ID { get; set; } = ActionID.None;

    public bool FromSelf;
    internal Status Status;
    public StatusID StatusId { get; set; }
    public TargetType TargetType;
    public TargetConditionType TargetConditionType;

    public float DistanceOrTime, TimeEnd;
    public int GCD, Param2;

    public string CastingActionName = string.Empty;

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        BattleChara tar;
        if (_action != null)
        {
            if(!_action.FindTarget(true, 0, out tar, out _))
            {
                tar = null;
            }
        }
        else
        {
            tar = TargetType switch
            {
                TargetType.Target => Svc.Targets.Target as BattleChara,
                TargetType.HostileTarget => DataCenter.HostileTarget,
                TargetType.Player => Player.Object,
                _ => null,
            };
        }

        if (TargetConditionType == TargetConditionType.IsNull)
        {
            return tar == null;
        }

        if (tar == null) return false;

        var result = false;

        switch (TargetConditionType)
        {
            case TargetConditionType.HasStatus:
                result = tar.HasStatus(FromSelf, StatusId);
                break;

            case TargetConditionType.IsBoss:
                result = tar.IsBoss();
                break;

            case TargetConditionType.IsDying:
                result = tar.IsDying();
                break;

            case TargetConditionType.InCombat:
                result = tar.InCombat();
                break;

            case TargetConditionType.Distance:
                switch (Param2)
                {
                    case 0:
                        result = tar.DistanceToPlayer() > DistanceOrTime;
                        break;
                    case 1:
                        result = tar.DistanceToPlayer() < DistanceOrTime;
                        break;
                    case 2:
                        result = tar.DistanceToPlayer() == DistanceOrTime;
                        break;
                }
                break;

            case TargetConditionType.StatusEnd:
                result = !tar.WillStatusEnd(DistanceOrTime, FromSelf, StatusId);
                break;

            case TargetConditionType.StatusEndGCD:
                result = !tar.WillStatusEndGCD((uint)GCD, DistanceOrTime, FromSelf, StatusId);
                break;

            case TargetConditionType.TimeToKill:
                switch (Param2)
                {
                    case 0:
                        result = tar.GetTimeToKill() > DistanceOrTime;
                        break;
                    case 1:
                        result = tar.GetTimeToKill() < DistanceOrTime;
                        break;
                    case 2:
                        result = tar.GetTimeToKill() == DistanceOrTime;
                        break;
                }
                break;

            case TargetConditionType.CastingAction:
                if (string.IsNullOrEmpty(CastingActionName) || tar.CastActionId == 0)
                {
                    result = false;
                    break;
                }

                var castName = Service.GetSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(tar.CastActionId)?.Name.ToString();

                result = CastingActionName == castName;
                break;

            case TargetConditionType.CastingActionTime:

                if (!tar.IsCasting || tar.CastActionId == 0)
                {
                    result = false;
                    break;
                }

                float castTime = tar.TotalCastTime - tar.CurrentCastTime;

                switch (Param2)
                {
                    case 0:
                        result = castTime > DistanceOrTime + DataCenter.WeaponRemain;
                        break;
                    case 1:
                        result = castTime < DistanceOrTime + DataCenter.WeaponRemain;
                        break;
                    case 2:
                        result = castTime == DistanceOrTime + DataCenter.WeaponRemain;
                        break;
                }
                break;

            case TargetConditionType.HP:
                switch (Param2)
                {
                    case 0:
                        result = tar.CurrentHp > GCD;
                        break;
                    case 1:
                        result = tar.CurrentHp < GCD;
                        break;
                    case 2:
                        result = tar.CurrentHp == GCD;
                        break;
                }
                break;

            case TargetConditionType.HPRatio:
                switch (Param2)
                {
                    case 0:
                        result = tar.GetHealthRatio() > DistanceOrTime;
                        break;
                    case 1:
                        result = tar.GetHealthRatio() < DistanceOrTime;
                        break;
                    case 2:
                        result = tar.GetHealthRatio() == DistanceOrTime;
                        break;
                }
                break;

            case TargetConditionType.MP:
                switch (Param2)
                {
                    case 0:
                        result = tar.CurrentMp > GCD;
                        break;
                    case 1:
                        result = tar.CurrentMp < GCD;
                        break;
                    case 2:
                        result = tar.CurrentMp == GCD;
                        break;
                }
                break;

            case TargetConditionType.TargetName:
                if (string.IsNullOrEmpty(CastingActionName))
                {
                    result = false;
                    break;
                }
                result = tar.Name.TextValue == CastingActionName;
                break;

            case TargetConditionType.ObjectEffect:
                foreach (var effect in DataCenter.ObjectEffects.Reverse())
                {
                    var time = effect.TimeDuration.TotalSeconds;
                    if (time > DistanceOrTime && time < TimeEnd
                        && effect.Param1 == GCD
                        && effect.Param2 == Param2)
                    {
                        if (!FromSelf || effect.ObjectId == tar.ObjectId)
                        {
                            result = true;
                            break;
                        }
                    }
                }
                break;

            case TargetConditionType.Vfx:
                foreach (var effect in DataCenter.VfxNewData.Reverse())
                {
                    var time = effect.TimeDuration.TotalSeconds;
                    if (time > DistanceOrTime && time < TimeEnd
                        && effect.Path == CastingActionName)
                    {
                        if (!FromSelf || effect.ObjectId == tar.ObjectId)
                        {
                            result = true;
                            break;
                        }
                    }
                }
                break;
        }

        return result;
    }
}

internal enum TargetType : byte
{
    HostileTarget,
    Player,
    Target,
}

internal enum TargetConditionType : byte
{
    IsNull,
    HasStatus,
    IsDying,
    IsBoss,
    InCombat,
    Distance,
    StatusEnd,
    StatusEndGCD,
    CastingAction,
    CastingActionTime,
    TimeToKill,
    HP,
    HPRatio,
    MP,
    TargetName,
    ObjectEffect,
    Vfx,
}