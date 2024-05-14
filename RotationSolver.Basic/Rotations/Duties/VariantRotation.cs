namespace RotationSolver.Basic.Rotations.Duties;

[DutyTerritory(1069, 1075, 1076, 1137, 1176)] //TODO: the variant territory ids!
public abstract class VariantRotation : DutyRotation
{
}

partial class DutyRotation
{
    static partial void ModifyVariantCurePvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.VariantCureSet];
    }

    static partial void ModifyVariantCurePvE_33862(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.VariantCureSet];
    }
}
