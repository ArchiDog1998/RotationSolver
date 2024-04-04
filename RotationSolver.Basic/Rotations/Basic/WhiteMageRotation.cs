namespace RotationSolver.Basic.Rotations.Basic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
partial class WhiteMageRotation
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
    }

    static partial void ModifyHolyPvE(ref ActionSetting setting)
    {
        setting.IsFriendly = false;
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
    }

    static partial void ModifyCureIiiPvP(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.CureIiiReady];
    }
}