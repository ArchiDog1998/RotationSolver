using Dalamud.Game.ClientState.JobGauge.Enums;
using System.Collections.Generic;

namespace RotationSolver.Basic.Rotations.Basic;

public partial class ViperRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Dexterity;

    /// <summary>
    /// 
    /// </summary>
    public static byte RattlingCoilStacks => JobGauge.RattlingCoilStacks;

    /// <summary>
    /// 
    /// </summary>
    public static byte SerpentOffering => JobGauge.SerpentOffering;

    /// <summary>
    /// 
    /// </summary>
    public static byte AnguineTribute => JobGauge.AnguineTribute;

    /// <summary>
    /// 
    /// </summary>
    public static DreadCombo DreadCombo => JobGauge.DreadCombo;

    static partial void ModifySteelFangsPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyHuntersStingPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.HuntersInstinct];
    }

    static partial void ModifyDreadFangsPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyWrithingSnapPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MeleeRange;
    }

    static partial void ModifySwiftskinsStingPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Swiftscaled];
    }

    static partial void ModifySteelMawPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyFlankstingStrikePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.HindstungVenom];
    }

    static partial void ModifyFlanksbaneFangPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.HindsbaneVenom];
    }

    static partial void ModifyHindstingStrikePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.FlanksbaneVenom];
    }

    static partial void ModifyHindsbaneFangPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.FlankstungVenom];
    }

    static partial void ModifyDreadMawPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifySlitherPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyHuntersBitePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.HuntersInstinct];
    }

    static partial void ModifySwiftskinsBitePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Swiftscaled];
    }

    static partial void ModifyJaggedMawPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.GrimskinsVenom];
    }

    static partial void ModifyBloodiedMawPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.GrimhuntersVenom];
    }

    static partial void ModifySerpentsTailPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyDeathRattlePvE(ref ActionSetting setting)
    {
        
    }

    static partial void ModifyLastLashPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyDreadwinderPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyHuntersCoilPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.HuntersVenom];
    }

    static partial void ModifySwiftskinsCoilPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SwiftskinsVenom];
    }

    static partial void ModifyPitOfDreadPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyHuntersDenPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.FellhuntersVenom];
    }

    static partial void ModifySwiftskinsDenPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.FellskinsVenom];
    }

    static partial void ModifyTwinfangPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyTwinbloodPvE(ref ActionSetting setting)
    {

    }

    static partial void ModifyTwinfangBitePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SwiftskinsVenom];
    }

    static partial void ModifyTwinbloodBitePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.HuntersVenom];
    }

    static partial void ModifyTwinfangThreshPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.FellskinsVenom];
    }

    static partial void ModifyTwinbloodThreshPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.FellhuntersVenom];
    }

    static partial void ModifyUncoiledFuryPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.PoisedForTwinfang];
    }

    static partial void ModifySerpentsIrePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.ReadyToReawaken];
    }

    static partial void ModifyReawakenPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => SerpentOffering >= 50 || Player.HasStatus(true, StatusID.ReadyToReawaken);
    }

    static partial void ModifyFirstGenerationPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }

    static partial void ModifySecondGenerationPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }

    static partial void ModifyThirdGenerationPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }

    static partial void ModifyFourthGenerationPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }

    static partial void ModifyUncoiledTwinfangPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.PoisedForTwinfang];
        setting.StatusProvide = [StatusID.PoisedForTwinblood];
    }

    static partial void ModifyUncoiledTwinbloodPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }

    static partial void ModifyOuroborosPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }

    static partial void ModifyFirstLegacyPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }

    static partial void ModifySecondLegacyPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }

    static partial void ModifyThirdLegacyPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }

    static partial void ModifyFourthLegacyPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Reawakened];
    }
}





























