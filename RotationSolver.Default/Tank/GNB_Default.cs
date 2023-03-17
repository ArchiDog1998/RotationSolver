namespace RotationSolver.Default.Tank;

[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Tank/GNB_Default.cs")]
public sealed class GNB_Default : GNB_Base
{
    public override string GameVersion => "6.18";

    public override string RotationName => "Default";

    protected override bool CanHealSingleSpell => false;

    protected override bool CanHealAreaSpell => false;

    protected override bool GeneralGCD(out IAction act)
    {
        //倍攻
        if (CanUseDoubleDown(out act)) return true;

        //命运之环 AOE
        if (FatedCircle.CanUse(out act)) return true;

        //AOE
        if (DemonSlaughter.CanUse(out act)) return true;
        if (DemonSlice.CanUse(out act)) return true;

        //烈牙
        if (CanUseGnashingFang(out act)) return true;

        //音速破
        if (CanUseSonicBreak(out act)) return true;

        //烈牙后二连
        if (WickedTalon.CanUse(out act, emptyOrSkipCombo: true)) return true;
        if (SavageClaw.CanUse(out act, emptyOrSkipCombo: true)) return true;

        //爆发击   
        if (CanUseBurstStrike(out act)) return true;

        if (SolidBarrel.CanUse(out act)) return true;
        if (BrutalShell.CanUse(out act)) return true;
        if (KeenEdge.CanUse(out act)) return true;

        if (SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;

        if (LightningShot.CanUse(out act)) return true;

        return false;
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        //无情,目前只有4GCD起手的判断
        if (InBurst && abilitiesRemaining == 1 && CanUseNoMercy(out act)) return true;

        //危险领域
        if (DangerZone.CanUse(out act))
        {
            if (!IsFullParty && !DangerZone.IsTargetBoss) return true;

            //等级小于烈牙,
            if (!GnashingFang.EnoughLevel && (Player.HasStatus(true, StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(15))) return true;

            //爆发期,烈牙之后
            if (Player.HasStatus(true, StatusID.NoMercy) && GnashingFang.IsCoolingDown) return true;

            //非爆发期
            if (!Player.HasStatus(true, StatusID.NoMercy) && !GnashingFang.WillHaveOneCharge(20)) return true;
        }

        //弓形冲波
        if (CanUseBowShock(out act)) return true;

        //续剑
        if (JugularRip.CanUse(out act)) return true;
        if (AbdomenTear.CanUse(out act)) return true;
        if (EyeGouge.CanUse(out act)) return true;
        if (Hypervelocity.CanUse(out act)) return true;

        //血壤
        if (GnashingFang.IsCoolingDown && Bloodfest.CanUse(out act)) return true;

        //搞搞攻击,粗分斩
        if (RoughDivide.CanUse(out act, mustUse: true) && !IsMoving) return true;
        if (Player.HasStatus(true, StatusID.NoMercy) && RoughDivide.CanUse(out act, mustUse: true)) return true;

        act = null;
        return false;
    }

    [RotationDesc(ActionID.HeartofLight, ActionID.Reprisal)]
    protected override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (HeartofLight.CanUse(out act, emptyOrSkipCombo: true)) return true;
        if (Reprisal.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    [RotationDesc(ActionID.HeartofStone, ActionID.Nebula, ActionID.Rampart, ActionID.Camouflage, ActionID.Reprisal)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (abilitiesRemaining == 2)
        {
            //10
            if (HeartofStone.CanUse(out act)) return true;

            //30
            if (Nebula.CanUse(out act)) return true;

            //20
            if (Rampart.CanUse(out act)) return true;

            //10
            if (Camouflage.CanUse(out act)) return true;
        }

        if (Reprisal.CanUse(out act)) return true;

        act = null;
        return false;
    }

    [RotationDesc(ActionID.Aurora)]
    protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Aurora.CanUse(out act, emptyOrSkipCombo: true) && abilitiesRemaining == 1) return true;
        return false;
    }

    private bool CanUseNoMercy(out IAction act)
    {
        if (!NoMercy.CanUse(out act)) return false;

        if (!IsFullParty && !IsTargetBoss && !IsMoving && DemonSlice.CanUse(out _)) return true;

        //等级低于爆发击是判断
        if (!BurstStrike.EnoughLevel) return true;

        if (BurstStrike.EnoughLevel)
        {
            //4GCD起手判断
            if (IsLastGCD((ActionID)KeenEdge.ID) && Ammo == 1 && !GnashingFang.IsCoolingDown && !Bloodfest.IsCoolingDown) return true;

            //3弹进无情
            else if (Ammo == (Level >= 88 ? 3 : 2)) return true;

            //2弹进无情
            else if (Ammo == 2 && GnashingFang.IsCoolingDown) return true;
        }

        act = null;
        return false;
    }

    private bool CanUseGnashingFang(out IAction act)
    {
        //基础判断
        if (GnashingFang.CanUse(out act))
        {
            //在4人本道中使用
            if (DemonSlice.CanUse(out _)) return false;

            //无情中3弹烈牙
            if (Ammo == (Level >= 88 ? 3 : 2) && (Player.HasStatus(true, StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(55))) return true;

            //无情外烈牙
            if (Ammo > 0 && !NoMercy.WillHaveOneCharge(17) && NoMercy.WillHaveOneCharge(35)) return true;

            //3弹且将会溢出子弹的情况,提前在无情前进烈牙
            if (Ammo == 3 && IsLastGCD((ActionID)BrutalShell.ID) && NoMercy.WillHaveOneCharge(3)) return true;

            //1弹且血壤快冷却好了
            if (Ammo == 1 && !NoMercy.WillHaveOneCharge(55) && Bloodfest.WillHaveOneCharge(5)) return true;

            //4GCD起手烈牙判断
            if (Ammo == 1 && !NoMercy.WillHaveOneCharge(55) && (!Bloodfest.IsCoolingDown && Bloodfest.EnoughLevel || !Bloodfest.EnoughLevel)) return true;
        }
        return false;
    }

    /// <summary>
    /// 音速破
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseSonicBreak(out IAction act)
    {
        //基础判断
        if (SonicBreak.CanUse(out act))
        {
            //在4人本道中不使用
            if (DemonSlice.CanUse(out _)) return false;

            //if (!IsFullParty && !SonicBreak.IsTargetBoss) return false;

            if (!GnashingFang.EnoughLevel && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //在烈牙后面使用音速破
            if (GnashingFang.IsCoolingDown && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //其他判断
            if (!DoubleDown.EnoughLevel && Player.HasStatus(true, StatusID.ReadyToRip)
                && GnashingFang.IsCoolingDown) return true;

        }
        return false;
    }

    /// <summary>
    /// 倍攻
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseDoubleDown(out IAction act)
    {
        //基本判断
        if (DoubleDown.CanUse(out act, mustUse: true))
        {
            //在4人本道中
            if (DemonSlice.CanUse(out _) && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //在音速破后使用倍攻
            if (SonicBreak.IsCoolingDown && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //2弹无情的特殊判断,提前使用倍攻
            if (Player.HasStatus(true, StatusID.NoMercy) && !NoMercy.WillHaveOneCharge(55) && Bloodfest.WillHaveOneCharge(5)) return true;

        }
        return false;
    }

    /// <summary>
    /// 爆发击
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseBurstStrike(out IAction act)
    {
        if (BurstStrike.CanUse(out act))
        {
            //在4人本道中且AOE时不使用
            if (DemonSlice.CanUse(out _)) return false;

            //如果烈牙剩0.5秒冷却好,不释放爆发击,主要因为技速不同可能会使烈牙延后太多所以判定一下
            if (SonicBreak.IsCoolingDown && SonicBreak.WillHaveOneCharge(0.5f) && GnashingFang.EnoughLevel) return false;

            //无情中爆发击判定
            if (Player.HasStatus(true, StatusID.NoMercy) &&
                AmmoComboStep == 0 &&
                !GnashingFang.WillHaveOneCharge(1)) return true;
            if (Level < 88 && Ammo == 2) return true;
            //无情外防止溢出
            if (IsLastGCD((ActionID)BrutalShell.ID) &&
                (Ammo == (Level >= 88 ? 3 : 2) ||
                Bloodfest.WillHaveOneCharge(6) && Ammo <= 2 && !NoMercy.WillHaveOneCharge(10) && Bloodfest.EnoughLevel)) return true;

        }
        return false;
    }

    private bool CanUseBowShock(out IAction act)
    {
        if (BowShock.CanUse(out act, mustUse: true))
        {
            if (DemonSlice.CanUse(out _) && !IsFullParty) return true;

            if (!SonicBreak.EnoughLevel && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //爆发期,无情中且音速破在冷却中
            if (Player.HasStatus(true, StatusID.NoMercy) && SonicBreak.IsCoolingDown) return true;
        }
        return false;
    }
}