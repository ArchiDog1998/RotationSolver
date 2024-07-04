namespace RotationSolver.Basic.Rotations.Basic;

partial class NinjaRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Dexterity;

    #region Job Gauge
    /// <summary>
    /// Current Ninki
    /// </summary>
    public static byte Kazematoi => JobGauge.Ninki;

    /// <summary>
    /// Number of charges of Kazematoi
    /// </summary>
    public static byte Ninki => (byte)JobGauge.HutonTimer;
    #endregion

    static partial void ModifyArmorCrushPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67221;
    }

    static partial void ModifyAeolianEdgePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Kazematoi > 0;
    }

    static partial void ModifyPhantomKamaitachiPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.PhantomKamaitachiReady];
    }

    static partial void ModifyThrowingDaggerPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MeleeRange;
    }

    static partial void ModifyBhavacakraPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ninki >= 50;
    }

    static partial void ModifyHellfrogMediumPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ninki >= 50;
    }

    static partial void ModifyMeisuiPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Suiton];
        setting.ActionCheck = () => Ninki <= 50;
    }

    static partial void ModifyMugPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => IsLongerThan(10);
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyDokumoriPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ninki <= 60 && IsLongerThan(10);
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyTrickAttackPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Suiton, StatusID.Hidden];
        setting.CreateConfig = () => new ActionConfig()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyBunshinPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ninki >= 50;
    }

    static partial void ModifyTenChiJinPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Kassatsu, StatusID.TenChiJin];
        setting.UnlockedByQuestID = 68488;
    }

    static partial void ModifyKassatsuPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Kassatsu, StatusID.TenChiJin];
        setting.UnlockedByQuestID = 65770;
    }

    static partial void ModifyDotonPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Doton];
    }

    static partial void ModifyShukuchiPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
        setting.UnlockedByQuestID = 65752;
    }

    static partial void ModifyTenPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 65748;
    }

    static partial void ModifyChiPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 65750;
    }

    static partial void ModifyJinPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 65768;
    }

    static partial void ModifyHakkeMujinsatsuPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67220;
    }

    static partial void ModifyDreamWithinADreamPvE(ref ActionSetting setting)
    {
        setting.UnlockedByQuestID = 67222;
    }

    /// <summary>
    /// 
    /// </summary>
    public NinjaRotation()
    {
        FumaShurikenPvE.Setting.Ninjutsu = [TenPvE];
        KatonPvE.Setting.Ninjutsu = [ChiPvE, TenPvE];
        RaitonPvE.Setting.Ninjutsu = [TenPvE, ChiPvE];
        HyotonPvE.Setting.Ninjutsu = [TenPvE, JinPvE];
        HutonPvE.Setting.Ninjutsu = [JinPvE, ChiPvE, TenPvE];
        DotonPvE.Setting.Ninjutsu = [JinPvE, TenPvE, ChiPvE];
        SuitonPvE.Setting.Ninjutsu = [TenPvE, ChiPvE, JinPvE];
        GokaMekkyakuPvE.Setting.Ninjutsu = [ChiPvE, TenPvE];
        HyoshoRanryuPvE.Setting.Ninjutsu = [TenPvE, JinPvE];
    }

    static partial void ModifyKatonPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifySuitonPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.ShadowWalker];
    }

    static partial void ModifyHutonPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.ShadowWalker];
    }

    static partial void ModifyShukuchiPvP(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ShukuchiPvE)]
    protected sealed override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (ShukuchiPvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.FeintPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (FeintPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ShadeShiftPvE)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (ShadeShiftPvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(nextGCD, out act);
    }

    static partial void ModifyFleetingRaijuPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.RaijuReady];
    }

    static partial void ModifyForkedRaijuPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.RaijuReady];
    }

    static partial void ModifyDeathfrogMediumPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Higi];
    }

    static partial void ModifyZeshoMeppoPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Higi];
    }

    static partial void ModifyTenriJindoPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.TenriJindoReady];
    }
}