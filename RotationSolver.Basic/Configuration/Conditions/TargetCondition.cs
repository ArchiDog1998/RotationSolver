using ECommons.DalamudServices;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Configuration.Conditions;

internal class TargetCondition : DelayCondition
{
    internal IBaseAction _action;
    public ActionID ID { get; set; } = ActionID.None;

    public bool Condition;
    public bool FromSelf;
    internal Status Status;
    public StatusID StatusId { get; set; }
    public bool IsTarget;
    public TargetConditionType TargetConditionType;

    public float DistanceOrTime;
    public int GCD;

    public string CastingActionName = string.Empty;

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        BattleChara tar;
        if (_action != null)
        {
            _action.CanUse(out _, CanUseOption.EmptyOrSkipCombo | CanUseOption.MustUse
                | CanUseOption.IgnoreTarget);
            tar = _action.Target;
        }
        else
        {
            tar = IsTarget ? Svc.Targets.Target as BattleChara : Player.Object;
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
                result = tar.DistanceToPlayer() > DistanceOrTime;
                break;

            case TargetConditionType.StatusEnd:
                result = !tar.WillStatusEnd(DistanceOrTime, FromSelf, StatusId);
                break;

            case TargetConditionType.StatusEndGCD:
                result = !tar.WillStatusEndGCD((uint)GCD, DistanceOrTime, FromSelf, StatusId);
                break;

            case TargetConditionType.TimeToKill:
                result = tar.GetTimeToKill() > DistanceOrTime;
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

            case TargetConditionType.CastingActionTimeUntil:

                if (!tar.IsCasting || tar.CastActionId == 0)
                {
                    result = false;
                    break;
                }

                float castTime = tar.TotalCastTime - tar.CurrentCastTime;
                result = castTime > DistanceOrTime + DataCenter.WeaponRemain;
                break;

            case TargetConditionType.HP:
                result = tar.CurrentHp > GCD;
                break;

            case TargetConditionType.HPRatio:
                result = tar.GetHealthRatio() > DistanceOrTime;
                break;

            case TargetConditionType.MP:
                result = tar.CurrentMp > GCD;
                break;

            case TargetConditionType.TargetName:
                if (string.IsNullOrEmpty(CastingActionName))
                {
                    result = false;
                    break;
                }
                result = tar.Name.TextValue == CastingActionName;
                break;
        }

        return Condition ? !result : result;
    }
}

internal enum TargetConditionType : byte
{
    HasStatus,
    IsDying,
    IsBoss,
    InCombat,
    Distance,
    StatusEnd,
    StatusEndGCD,
    CastingAction,
    CastingActionTimeUntil,
    TimeToKill,
    HP,
    HPRatio,
    MP,
    TargetName,
}