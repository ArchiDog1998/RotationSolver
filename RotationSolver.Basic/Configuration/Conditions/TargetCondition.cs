using ECommons.DalamudServices;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Configuration.Conditions;

[Description("Target Condition")]
internal class TargetCondition : DelayCondition
{
    internal IBaseAction? _action;
    public ActionID ID { get; set; } = ActionID.None;

    public bool FromSelf;
    internal Status? Status;
    public StatusID StatusId { get; set; }
    public TargetType TargetType;
    public TargetConditionType TargetConditionType;

    public float DistanceOrTime;
    public int GCD, Param2;

    public string CastingActionName = string.Empty;

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        BattleChara? tar;
        if (_action != null)
        {
            tar = _action.TargetInfo.FindTarget(true, false)?.Target;
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

            case TargetConditionType.IsBossFromTTK:
                result = tar.IsBossFromTTK();
                break;

            case TargetConditionType.IsBossFromIcon:
                result = tar.IsBossFromIcon();
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
                        result = castTime > DistanceOrTime + DataCenter.DefaultGCDRemain;
                        break;
                    case 1:
                        result = castTime < DistanceOrTime + DataCenter.DefaultGCDRemain;
                        break;
                    case 2:
                        result = castTime == DistanceOrTime + DataCenter.DefaultGCDRemain;
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
        }

        return result;
    }
}

internal enum TargetType : byte
{
    [Description("Hostile Target")]
    HostileTarget,

    [Description("Player")]
    Player,

    [Description("Target")]
    Target,
}

internal enum TargetConditionType : byte
{
    [Description("Is Null")]
    IsNull,

    [Description("Has status")]
    HasStatus,

    [Description("Is Dying")]
    IsDying,

    [Description("Is Boss From TTK")]
    IsBossFromTTK,

    [Description("Is Boss From Icon")]
    IsBossFromIcon,

    [Description("In Combat")]
    InCombat,

    [Description("Distance")]
    Distance,

    [Description("Status end")]
    StatusEnd,

    [Description("Status End GCD")]
    StatusEndGCD,

    [Description("Casting Action")]
    CastingAction,

    [Description("Casting Action Time Until")]
    CastingActionTime,

    [Description("Time To Kill")]
    TimeToKill,

    [Description("HP")]
    HP,

    [Description("HP%")]
    HPRatio,

    [Description("MP")]
    MP,

    [Description("Target Name")]
    TargetName,
}