namespace RotationSolver.Basic.Rotations.Basic;

partial class SageRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Mind;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static bool HasEukrasia => JobGauge.Eukrasia;

    /// <summary>
    /// 
    /// </summary>
    public static byte Addersgall => JobGauge.Addersgall;

    /// <summary>
    /// 
    /// </summary>
    public static byte Addersting => JobGauge.Addersting;

    static float AddersgallTimerRaw => JobGauge.AddersgallTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float AddersgallTime => AddersgallTimerRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfter(float time) => AddersgallTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfterGCD(uint gctCount = 0, float offset = 0)
        => AddersgallEndAfter(GCDTime(gctCount, offset));
    #endregion
    private protected sealed override IBaseAction Raise => EgeiroPvE;

    static partial void ModifyEukrasianDiagnosisPvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.BeAttacked;
        setting.ActionCheck = () => !DataCenter.AllianceMembers.Any(m => m.HasStatus(true, StatusID.EukrasianDiagnosis));
    }

    static partial void ModifyEukrasianDosisPvE(ref ActionSetting setting)
    {
        setting.TargetStatusProvide =
        [
            StatusID.EukrasianDosis,
            StatusID.EukrasianDosisIi,
            StatusID.EukrasianDosisIii
        ];
    }

    static partial void ModifyDyskrasiaPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyToxikonPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Addersting > 0;
    }

    static partial void ModifyRhizomataPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Addersting < 3;
    }

    static partial void ModifyKardiaPvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Tank;
        setting.ActionCheck = () => !DataCenter.AllianceMembers.Any(m => m.HasStatus(true, StatusID.Kardion));
    }

    static partial void ModifyEukrasiaPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => !HasEukrasia;
    }

    static partial void ModifyKeracholePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Addersgall > 0;
    }

    static partial void ModifyIxocholePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Addersgall > 0;
    }

    static partial void ModifyTaurocholePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Addersgall > 0;
    }

    static partial void ModifyDruocholePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Addersgall > 0;
    }

    static partial void ModifyPepsisPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () =>
        {
            foreach (var chara in DataCenter.PartyMembers)
            {
                if (chara.HasStatus(true, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis)
                && chara.GetHealthRatio() < 0.9) return true;
            }

            return false;
        };
    }

    static partial void ModifyIcarusPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyIcarusPvP(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyPanhaimaPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 69608;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.IcarusPvE)]
    protected sealed override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (IcarusPvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(nextGCD, out act);
    }
}
