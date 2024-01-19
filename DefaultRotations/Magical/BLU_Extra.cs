namespace ExtraRotations.Magical;

[SourceCode(Path = "main/DefaultRotations/Magical/BLU_Extra.cs")]
public sealed class BLU_Extra : BLU_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.3";

    public override string RotationName => "Extra";

    public override string Description => "This is a simplified version for me (ArchiTed) using, \nwhich doesn't contain all actions.";

    protected override bool AttackAbility(out IAction act)
    {
        act = null;
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if (ChocoMeteor.CanUse(out act)) return true;
        if (DrillCannons.CanUse(out act)) return true;

        if (TripleTrident.OnSlot && TripleTrident.RightType && TripleTrident.WillHaveOneChargeGCD(OnSlotCount(Whistle, Tingle), 0))
        {
            if ((TripleTrident.CanUse(out _, CanUseOption.MustUse) || !HasHostilesInRange) && Whistle.CanUse(out act)) return true;

            if (!Player.HasStatus(true, StatusID.Tingling)
                && Tingle.CanUse(out act, CanUseOption.MustUse)) return true;
            if (OffGuard.CanUse(out act)) return true;

            if (TripleTrident.CanUse(out act, CanUseOption.MustUse)) return true;
        }
        if (ChocoMeteor.CanUse(out act, HasCompanion ? CanUseOption.MustUse : CanUseOption.None)) return true;

        if (SonicBoom.CanUse(out act)) return true;
        if (DrillCannons.CanUse(out act, CanUseOption.MustUse)) return true;

        return false;
    }
}
