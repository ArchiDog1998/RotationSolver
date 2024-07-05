using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations.Basic;

namespace RotationSolver.DummyRotations;

[Rotation("Honktomancer", CombatType.PvE, GameVersion = "7.0")]
[Api(2)]
public class Honktomancer : PictomancerRotation
{
    public override MedicineType MedicineType { get; }
}