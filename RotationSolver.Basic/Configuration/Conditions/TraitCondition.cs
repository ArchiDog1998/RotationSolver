using ECommons.GameHelpers;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Configuration.Conditions;

[Description("Trait Condition")]
internal class TraitCondition : DelayCondition
{
    public uint TraitID { get; set; } = 0;
    internal IBaseTrait? _trait;

    public override bool CheckBefore(ICustomRotation rotation)
    {
        if (TraitID != 0 && (_trait == null || _trait.ID != TraitID))
        {
            _trait = rotation.AllTraits.FirstOrDefault(a => a.ID == TraitID);
        }
        return base.CheckBefore(rotation);
    }

    protected override bool IsTrueInside(ICustomRotation rotation)
    {
        if (_trait == null || !Player.Available) return false;

        var result = _trait.EnoughLevel;
        return result;
    }
}
