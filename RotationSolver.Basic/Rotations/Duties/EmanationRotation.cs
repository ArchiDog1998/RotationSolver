
namespace RotationSolver.Basic.Rotations.Duties;

/// <summary>
/// The variant action.
/// </summary>
[DutyTerritory(263, 264)] //TODO: the variant territory ids!
public abstract class EmanationRotation : DutyRotation
{
}

partial class DutyRotation
{
    static partial void ModifyVrilPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = true;
        setting.TargetStatusProvide = [StatusID.Vril];
    }

    static partial void ModifyVrilPvE_9345(ref ActionSetting setting)
    {
        setting.StatusFromSelf = true;
        setting.TargetStatusProvide = [StatusID.Vril];
    }
}
