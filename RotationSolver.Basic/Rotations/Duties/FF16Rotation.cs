
namespace RotationSolver.Basic.Rotations.Duties;

/// <summary>
/// The Memory of Embers.
/// </summary>

[DutyTerritory(1166)]
public abstract class FF16Rotation : DutyRotation
{
    public override bool AttackAbility(out IAction? act)
    {
        if (RisingFlamesPvE.CanUse(out act, skipAoeCheck: true)) return true;
        return base.AttackAbility(out act);
    }
}

partial class DutyRotation
{
    static partial void ModifyDodgePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Precision];
    }

    static partial void ModifyPrecisionStrikePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Precision];
    }
}
