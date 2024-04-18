namespace RotationSolver.Basic.Rotations.Basic;

partial class BardRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Dexterity;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static byte Repertoire => JobGauge.Repertoire;

    /// <summary>
    /// 
    /// </summary>
    protected static Song Song => JobGauge.Song;

    /// <summary>
    /// 
    /// </summary>
    protected static Song LastSong => JobGauge.LastSong;

    /// <summary>
    /// 
    /// </summary>
    public static byte SoulVoice => JobGauge.SoulVoice;
    static float SongTimeRaw => JobGauge.SongTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float SongTime => SongTimeRaw - DataCenter.DefaultGCDRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool SongEndAfter(float time) => SongTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool SongEndAfterGCD(uint gctCount = 0, float offset = 0)
        => SongEndAfter(GCDTime(gctCount, offset));
    #endregion

    static partial void ModifyHeavyShotPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.StraightShotReady];
    }

    static partial void ModifyStraightShotPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.StraightShotReady];
    }

    static partial void ModifyVenomousBitePvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.VenomousBite, StatusID.CausticBite];
    }

    static partial void ModifyWindbitePvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.Windbite, StatusID.Stormbite];
        setting.UnlockedByQuestID = 65612;
    }

    static partial void ModifyIronJawsPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide = [StatusID.VenomousBite, StatusID.CausticBite, StatusID.Windbite, StatusID.Stormbite];
        setting.CanTarget = t =>
        {
            if (t.WillStatusEndGCD(0, 0, true, StatusID.VenomousBite, StatusID.CausticBite)) return false;
            if (t.WillStatusEndGCD(0, 0, true, StatusID.Windbite, StatusID.Stormbite)) return false;
            return true;
        };
        setting.UnlockedByQuestID = 67252;
    }

    static partial void ModifyPitchPerfectPvP(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Repertoire];
    }

    static partial void ModifySilentNocturnePvP(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Repertoire];
    }

    static partial void ModifyTheWardensPaeanPvP(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Repertoire];
    }

    static partial void ModifyBlastArrowPvP(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.BlastArrowReady_3142];
    }

    static partial void ModifyPitchPerfectPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Song == Song.WANDERER && Repertoire > 0;
    }

    static partial void ModifyQuickNockPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.ShadowbiteReady];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyShadowbitePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.ShadowbiteReady];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyApexArrowPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.BlastArrowReady];
        setting.ActionCheck = () => SoulVoice >= 20;
    }

    static partial void ModifyBlastArrowPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.BlastArrowReady];
    }

    static partial void ModifyRainOfDeathPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
        setting.UnlockedByQuestID = 66624;
    }

    static partial void ModifyRadiantFinalePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => JobGauge.Coda.Any(s => s != Song.NONE);
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyRagingStrikesPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyTroubadourPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.StatusProvide = StatusHelper.RangePhysicalDefense;
    }

    static partial void ModifyBattleVoicePvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
        setting.UnlockedByQuestID = 66626;
    }

    static partial void ModifyRepellingShotPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 65604;
    }

    static partial void ModifyMagesBalladPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66621;
    }

    static partial void ModifyTheWardensPaeanPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66622;
    }

    static partial void ModifyArmysPaeonPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 66623;
    }

    static partial void ModifyTheWanderersMinuetPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67250;
    }

    static partial void ModifyEmpyrealArrowPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67251;
    }

    static partial void ModifySidewinderPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67254;
    }

    static partial void ModifyRefulgentArrowPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 68430;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.TheWardensPaeanPvE)]
    protected override bool DispelGCD(out IAction? act)
    {
        if (TheWardensPaeanPvE.CanUse(out act)) return true;
        return base.DispelGCD(out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.NaturesMinnePvE)]
    protected sealed override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (NaturesMinnePvE.CanUse(out act)) return true;
        return base.HealSingleAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.TroubadourPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction act)
    {
        if (TroubadourPvE.CanUse(out act)) return true;
        return false;
    }
}
