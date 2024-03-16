namespace RotationSolver.Basic.Rotations.Duties;

/// <summary>
/// The bozja action.
/// </summary>
[DutyTerritory(920, 975)] //TODO: the bozja territory ids!
public abstract class BozjaRotation : DutyRotation
{
}

partial class DutyRotation
{
    static partial void ModifyLostSpellforgePvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Physical;
        setting.StatusFromSelf = false;
        setting.TargetStatusNeed = [StatusID.MagicalAversion];
        setting.TargetStatusProvide = [StatusID.LostSpellforge, StatusID.LostSteelsting];
    }

    static partial void ModifyLostSteelstingPvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Magical;
        setting.StatusFromSelf = false;
        setting.TargetStatusNeed = [StatusID.PhysicalAversion];
        setting.TargetStatusProvide = [StatusID.LostSpellforge, StatusID.LostSteelsting];
    }

    static partial void ModifyLostRampagePvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.TargetStatusNeed = [StatusID.PhysicalAversion];
        setting.StatusProvide = [StatusID.LostRampage];
    }

    static partial void ModifyLostBurstPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.TargetStatusNeed = [StatusID.MagicalAversion];
        setting.StatusProvide = [StatusID.LostBurst];
    }

    static partial void ModifyLostBloodRagePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.LostBravery];
    }

    static partial void ModifyLostProtectPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.LostProtect, StatusID.LostProtectIi];
    }

    static partial void ModifyLostProtectIiPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.LostProtectIi];
    }

    static partial void ModifyLostShellPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.LostShell, StatusID.LostShellIi];
    }

    static partial void ModifyLostShellIiPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.TargetStatusProvide = [StatusID.LostShellIi];
    }

    static partial void ModifyLostBubblePvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
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