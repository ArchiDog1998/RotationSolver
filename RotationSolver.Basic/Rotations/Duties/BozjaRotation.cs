namespace RotationSolver.Basic.Rotations.Duties;

partial class DutyRotation
{

    static partial void ModifyLostSpellforgePvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Physical;
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusNeed = [StatusID.MagicalAversion];
        setting.TargetStatusProvide = [StatusID.LostSpellforge, StatusID.LostSteelsting];
    }

    static partial void ModifyLostSteelstingPvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Magical;
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusNeed = [StatusID.PhysicalAversion];
        setting.TargetStatusProvide = [StatusID.LostSpellforge, StatusID.LostSteelsting];
    }

    static partial void ModifyLostRampagePvE(ref ActionSetting setting)
    {
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusNeed = [StatusID.PhysicalAversion];
        setting.StatusProvide = [StatusID.LostRampage];
    }

    static partial void ModifyLostBurstPvE(ref ActionSetting setting)
    {
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusNeed = [StatusID.MagicalAversion];
        setting.StatusProvide = [StatusID.LostBurst];
    }

    static partial void ModifyLostBloodRagePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.LostBravery];
    }

    static partial void ModifyLostProtectPvE(ref ActionSetting setting)
    {
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.LostProtect, StatusID.LostProtectIi];
    }

    static partial void ModifyLostProtectIiPvE(ref ActionSetting setting)
    {
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.LostProtectIi];
    }

    static partial void ModifyLostShellPvE(ref ActionSetting setting)
    {
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.LostShell, StatusID.LostShellIi];
    }

    static partial void ModifyLostShellIiPvE(ref ActionSetting setting)
    {
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.LostShellIi];
    }

    static partial void ModifyLostBubblePvE(ref ActionSetting setting)
    {
        setting.TargetStatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.LostBubble];
    }

    static partial void ModifyLostStoneskinPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Stoneskin];
    }

    static partial void ModifyLostStoneskinIiPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Stoneskin];
    }

    static partial void ModifyLostFlareStarPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.LostFlareStar];
    }

    static partial void ModifyLostSeraphStrikePvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.ClericStance_2484];
    }
}

[DutyTerritory] //TODO: the bozja territory ids!
public class BozjaRotation : DutyRotation
{
    public override bool ProvokeAbility(out IAction? act)
    {
        if (VariantUltimatumPvE.CanUse(out act)) return true;
        return base.ProvokeAbility(out act);
    }

    public override bool AttackAbility(out IAction? act)
    {
        if (VariantSpiritDartPvE.CanUse(out act, skipAoeCheck: true)) return true;
        if (VariantSpiritDartPvE_33863.CanUse(out act, skipAoeCheck: true)) return true;
        if (VariantRampartPvE.CanUse(out act)) return true;
        if (VariantRampartPvE_33864.CanUse(out act)) return true;
        return base.AttackAbility(out act);
    }

    public override bool HealSingleGCD(out IAction? act)
    {
        if (VariantCurePvE.CanUse(out act)) return true;
        if (VariantCurePvE_33862.CanUse(out act)) return true;
        return base.HealSingleGCD(out act);
    }

    public override bool DefenseSingleGCD(out IAction? act)
    {
        if (LostStoneskinPvE.CanUse(out act)) return true;
        return base.DefenseSingleGCD(out act);
    }

    public override bool DefenseAreaGCD(out IAction? act)
    {
        if (LostStoneskinIiPvE.CanUse(out act)) return true;
        return base.DefenseAreaGCD(out act);
    }

    public override bool EmergencyGCD(out IAction? act)
    {
        #region Bozja
        //if (LostSpellforge.CanUse(out act)) return true;
        //if (LostSteelsting.CanUse(out act)) return true;
        //if (LostRampage.CanUse(out act)) return true;
        //if (LostBurst.CanUse(out act)) return true;

        //if (LostBravery.CanUse(out act)) return true;
        //if (LostBubble.CanUse(out act)) return true;
        //if (LostShell2.CanUse(out act)) return true;
        //if (LostShell.CanUse(out act)) return true;
        //if (LostProtect2.CanUse(out act)) return true;
        //if (LostProtect.CanUse(out act)) return true;

        ////Add your own logic here.
        //if (LostFlarestar.CanUse(out act)) return true;
        //if (LostSeraphStrike.CanUse(out act)) return true;

        #endregion
        return base.EmergencyGCD(out act);
    }
}
