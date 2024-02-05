namespace RotationSolver.Basic.Actions;
public interface ICooldown
{
    internal float RecastTimeOneChargeRaw { get; }
    internal float RecastTimeElapsedRaw { get; }
    bool IsCoolingDown { get; }
    ushort MaxCharges { get; }
    ushort CurrentCharges { get; }
}
