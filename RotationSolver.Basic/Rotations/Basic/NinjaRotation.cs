﻿namespace RotationSolver.Basic.Rotations.Basic;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
partial class NinjaRotation
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Dexterity;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static byte Ninki => JobGauge.Ninki;

    static float HutonTimeRaw => JobGauge.HutonTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float HutonTime => HutonTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool HutonEndAfter(float time) => HutonTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool HutonEndAfterGCD(uint gctCount = 0, float offset = 0)
        => HutonEndAfter(GCDTime(gctCount, offset));
    #endregion

    static partial void ModifyArmorCrushPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HutonEndAfter(25) && !HutonEndAfterGCD();
    }

    static partial void ModifyHuraijinPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HutonEndAfterGCD();
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
        setting.ActionCheck = () => JobGauge.Ninki <= 60 && IsLongerThan(10);
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
        setting.ActionCheck = () => !HutonEndAfterGCD(2);
    }

    static partial void ModifyKassatsuPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Kassatsu, StatusID.TenChiJin];
    }

    static partial void ModifyHutonPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HutonEndAfterGCD();
    }

    static partial void ModifyDotonPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Doton];
    }

    static partial void ModifyShukuchiPvE(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
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

    /// <inheritdoc/>
    [RotationDesc(ActionID.ShukuchiPvE)]
    protected sealed override bool MoveForwardAbility(out IAction? act)
    {
        if (ShukuchiPvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.FeintPvE)]
    protected sealed override bool DefenseAreaAbility(out IAction? act)
    {
        if (FeintPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ShadeShiftPvE)]
    protected override bool DefenseSingleAbility(out IAction? act)
    {
        if (ShadeShiftPvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(out act);
    }

    //static partial void ModifySuitonPvE(ref ActionSetting setting)
    //{
    //    setting.StatusProvide = [StatusID.Suiton];
    //}

    static partial void ModifyFleetingRaijuPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.RaijuReady];
    }

    static partial void ModifyForkedRaijuPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.RaijuReady];
    }
}