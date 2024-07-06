namespace RotationSolver.Basic.Rotations.Basic;

public partial class PictomancerRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Intelligence;

    /// <summary>
    /// Tracks use of subjective pallete
    /// </summary>
    public static byte PaletteGauge => JobGauge.PalleteGauge;

    /// <summary>
    /// Number of paint the player has.
    /// </summary>
    public static byte Paint => JobGauge.Paint;

    /// <summary>
    /// Creature Motif Stack
    /// </summary>
    public static bool CreatureMotifDrawn => JobGauge.CreatureMotifDrawn;

    /// <summary>
    /// Weapon Motif Stack
    /// </summary>
    public static bool WeaponMotifDrawn => JobGauge.WeaponMotifDrawn;

    /// <summary>
    /// Landscape Motif Stack
    /// </summary>
    public static bool LandscapeMotifDrawn => JobGauge.LandscapeMotifDrawn;

    /// <summary>
    /// Moogle Portrait Stack
    /// </summary>
    public static bool MooglePortraitReady => JobGauge.MooglePortraitReady;

    /// <summary>
    /// Madeen Portrait Stack
    /// </summary>
    public static bool MadeenPortraitReady => JobGauge.MadeenPortraitReady;

    /// <summary>
    /// Which creature flags are present.
    /// </summary>
    public static CreatureFlags CreatureFlags => JobGauge.CreatureFlags;

    /// <summary>
    /// Which canvas flags are present.
    /// </summary>
    public static CanvasFlags CanvasFlags => JobGauge.CanvasFlags;


    static partial void ModifyFireInRedPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Aetherhues];
    }

    static partial void ModifyAeroInGreenPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.AetherhuesIi];
        setting.StatusNeed = [StatusID.Aetherhues];
    }

    static partial void ModifyTemperaCoatPvE(ref ActionSetting setting)
    {
        
    }

    static partial void ModifyWaterInBluePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.AetherhuesIi];
    }

    static partial void ModifySmudgePvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyFireIiInRedPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Aetherhues];
    }

    static partial void ModifyCreatureMotifPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !CreatureMotifDrawn;
    }

    static partial void ModifyLivingMusePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => CreatureMotifDrawn;
    }

    static partial void ModifyMogOfTheAgesPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => MooglePortraitReady;
    }

    static partial void ModifyPomMotifPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !CreatureMotifDrawn;
    }

    static partial void ModifyWingMotifPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !CreatureMotifDrawn;
    }

    static partial void ModifyPomMusePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => CreatureMotifDrawn;
    }

    static partial void ModifyWingedMusePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => CreatureMotifDrawn;
    }

    static partial void ModifyAeroIiInGreenPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Aetherhues];
        setting.StatusProvide = [StatusID.AetherhuesIi];
    }

    static partial void ModifyWaterIiInBluePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.AetherhuesIi];
    }

    static partial void ModifyWeaponMotifPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !WeaponMotifDrawn;
    }

    static partial void ModifySteelMusePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => WeaponMotifDrawn;
    }

    static partial void ModifyHammerStampPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.HammerTime];
    }

    static partial void ModifyHammerMotifPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !WeaponMotifDrawn;
    }

    static partial void ModifyStrikingMusePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => WeaponMotifDrawn;
    }

    static partial void ModifyBlizzardInCyanPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Aetherhues];
    }

    static partial void ModifyBlizzardIiInCyanPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.AetherhuesIi];
    }

    static partial void ModifySubtractivePalettePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => PaletteGauge >= 50;
    }

    static partial void ModifyStoneInYellowPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => (Player.HasStatus(true, StatusID.Hyperphantasia) || Player.HasStatus(true, StatusID.SubtractivePalette));
        setting.StatusNeed = [StatusID.Aetherhues];
        setting.StatusProvide = [StatusID.AetherhuesIi];
    }

    static partial void ModifyThunderInMagentaPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => (Player.HasStatus(true, StatusID.Hyperphantasia) || Player.HasStatus(true, StatusID.SubtractivePalette));
        setting.StatusNeed = [StatusID.AetherhuesIi]; ;
    }

    static partial void ModifyStoneIiInYellowPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => (Player.HasStatus(true, StatusID.Hyperphantasia) || Player.HasStatus(true, StatusID.SubtractivePalette));
        setting.StatusNeed = [StatusID.Aetherhues];
        setting.StatusProvide = [StatusID.AetherhuesIi];
    }

    static partial void ModifyThunderIiInMagentaPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => (Player.HasStatus(true, StatusID.Hyperphantasia) || Player.HasStatus(true, StatusID.SubtractivePalette));
        setting.StatusNeed = [StatusID.AetherhuesIi]; ;
    }

    static partial void ModifyLandscapeMotifPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !LandscapeMotifDrawn;
    }

    static partial void ModifyScenicMusePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => LandscapeMotifDrawn;
    }

    static partial void ModifyStarrySkyMotifPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !LandscapeMotifDrawn;
    }

    static partial void ModifyStarryMusePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => LandscapeMotifDrawn;
    }

    static partial void ModifyHolyInWhitePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Paint > 0;
    }

    static partial void ModifyHammerBrushPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.HammerTime];
    }

    static partial void ModifyPolishingHammerPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.HammerTime];
    }

    static partial void ModifyTemperaGrassaPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.TemperaCoat];
    }

    static partial void ModifyCometInBlackPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Paint > 0;
        setting.StatusNeed = [StatusID.MonochromeTones];
    }

    static partial void ModifyRainbowDripPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.RainbowBright];
    }

    static partial void ModifyClawMotifPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !CreatureMotifDrawn;
    }

    static partial void ModifyMawMotifPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !CreatureMotifDrawn;
    }

    static partial void ModifyClawedMusePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => CreatureMotifDrawn;
    }

    static partial void ModifyFangedMusePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => CreatureMotifDrawn;
    }

    static partial void ModifyRetributionOfTheMadeenPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => MadeenPortraitReady;
    }

    static partial void ModifyStarPrismPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Starstruck];
    }
}