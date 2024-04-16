namespace RotationSolver.Basic.Rotations.Basic;

partial class WhiteMageRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Mind;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static byte Lily => JobGauge.Lily;

    /// <summary>
    /// 
    /// </summary>
    public static byte BloodLily => JobGauge.BloodLily;

    static float LilyTimeRaw => JobGauge.LilyTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float LilyTime => LilyTimeRaw + DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool LilyAfter(float time) => LilyTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool LilyAfterGCD(uint gcdCount = 0, float offset = 0)
        => LilyAfter(GCDTime(gcdCount, offset));
    #endregion

    private protected sealed override IBaseAction Raise => RaisePvE;

    static partial void ModifyMedicaIiPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.MedicaIi, StatusID.TrueMedicaIi];
    }

    static partial void ModifyRegenPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [
            StatusID.Regen,
            StatusID.Regen_897,
            StatusID.Regen_1330,
        ];
        setting.UnlockedByQuestID = 66616;
    }

    static partial void ModifyHolyPvE(ref ActionSetting setting)
    {
        setting.IsFriendly = false;
        setting.UnlockedByQuestID = 66619;
    }

    static partial void ModifyHolyIiiPvE(ref ActionSetting setting)
    {
        setting.IsFriendly = false;
    }

    static partial void ModifyAfflatusSolacePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Lily > 0;
    }

    static partial void ModifyDivineBenisonPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.DivineBenison];
    }

    static partial void ModifyAfflatusRapturePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Lily > 0;
    }

    static partial void ModifyAeroPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide =
        [
            StatusID.Aero,
            StatusID.AeroIi,
            StatusID.Dia,
        ];
    }

    static partial void ModifyAfflatusMiseryPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => BloodLily == 3;
    }

    static partial void ModifyPresenceOfMindPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !IsMoving;
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
        setting.UnlockedByQuestID = 66615;
    }

    static partial void ModifyCureIiiPvP(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.CureIiiReady];
        setting.UnlockedByQuestID = 66617;
    }

    static partial void ModifyCureIiPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 65977;
    }

    static partial void ModifyBenedictionPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66620;
    }

    static partial void ModifyAsylumPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67256;
    }

    static partial void ModifyStoneIiiPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67257;
    }

    static partial void ModifyAssizePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67258;
    }

    static partial void ModifyThinAirPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67259;
    }

    static partial void ModifyTetragrammatonPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67261;
    }

    static partial void ModifyPlenaryIndulgencePvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67954;
    }
}