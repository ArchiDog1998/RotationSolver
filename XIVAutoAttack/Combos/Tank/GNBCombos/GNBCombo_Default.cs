using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.GNBCombos.GNBCombo_Default;

namespace XIVAutoAttack.Combos.Tank.GNBCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Tank/GNBCombos/GNBCombo_Default.cs")]
internal sealed class GNBCombo_Default : GNBCombo_Base<CommandType>
{
    public override string Author => "汐ベMoon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    /// <summary>
    /// 在4人本的道中已经聚好怪可以使用相关技能(不移动且身边有大于3只小怪)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss() && !IsMoving
                                                    && TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length >= 3;

    /// <summary>
    /// 在4人本的道中
    /// </summary>
    private static bool InDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss();

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.单体治疗, $"{Aurora}"},
        {DescType.范围防御, $"{HeartofLight}"},
        {DescType.单体防御, $"{HeartofStone}, {Nebula}, {Camouflage}"},
        {DescType.移动技能, $"{RoughDivide}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //烈牙
        if (CanUseGnashingFang(out act)) return true;

        //音速破
        if (CanUseSonicBreak(out act)) return true;

        //倍攻
        if (CanUseDoubleDown(out act)) return true;

        //烈牙后二连
        if (WickedTalon.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (SavageClaw.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //命运之环 AOE
        if (FatedCircle.ShouldUse(out act)) return true;

        //爆发击   
        if (CanUseBurstStrike(out act)) return true;

        //AOE
        if (DemonSlaughter.ShouldUse(out act)) return true;
        if (DemonSlice.ShouldUse(out act)) return true;

        //单体三连
        //如果烈牙剩0.5秒冷却好,不释放基础连击,主要因为技速不同可能会使烈牙延后太多所以判定一下
        if (GnashingFang.IsCoolDown && GnashingFang.WillHaveOneCharge((float)0.5, false) && GnashingFang.EnoughLevel) return false;
        if (SolidBarrel.ShouldUse(out act)) return true;
        if (BrutalShell.ShouldUse(out act)) return true;
        if (KeenEdge.ShouldUse(out act)) return true;

        if (CommandController.Move && MoveAbility(1, out act)) return true;

        if (LightningShot.ShouldUse(out act))
        {
            if (InDungeonsMiddle && LightningShot.Target.DistanceToPlayer() > 3) return true;
        }
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //超火流星 如果谢不够了。
        if (Superbolide.ShouldUse(out act)) return true;
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //无情,目前只有4GCD起手的判断
        if (SettingBreak && abilityRemain == 1 && CanUseNoMercy(out act)) return true;

        //危险领域
        if (DangerZone.ShouldUse(out act))
        {
            if (InDungeonsMiddle) return true;

            //等级小于烈牙,
            if (!GnashingFang.EnoughLevel && (Player.HaveStatusFromSelf(StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(15, false))) return true;

            //爆发期,烈牙之后
            if (Player.HaveStatusFromSelf(StatusID.NoMercy) && GnashingFang.IsCoolDown) return true;

            //非爆发期
            if (!Player.HaveStatusFromSelf(StatusID.NoMercy) && !GnashingFang.WillHaveOneCharge(20, false)) return true;
        }

        //弓形冲波
        if (CanUseBowShock(out act)) return true;

        //续剑
        if (JugularRip.ShouldUse(out act)) return true;
        if (AbdomenTear.ShouldUse(out act)) return true;
        if (EyeGouge.ShouldUse(out act)) return true;
        if (Hypervelocity.ShouldUse(out act)) return true;

        //血壤
        if (GnashingFang.IsCoolDown && Bloodfest.ShouldUse(out act)) return true;

        //搞搞攻击,粗分斩
        if (RoughDivide.Target.DistanceToPlayer() < 1 && !IsMoving)
        {
            if (RoughDivide.ShouldUse(out act)) return true;
            if (Player.HaveStatusFromSelf(StatusID.NoMercy) && RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (HeartofLight.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //突进
        if (RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {

            //减伤10%）
            if (HeartofStone.ShouldUse(out act)) return true;

            //星云（减伤30%）
            if (Nebula.ShouldUse(out act)) return true;

            //铁壁（减伤20%）
            if (Rampart.ShouldUse(out act)) return true;

            //伪装（减伤10%）
            if (Camouflage.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Aurora.ShouldUse(out act, emptyOrSkipCombo: true) && abilityRemain == 1) return true;

        return false;
    }

    private bool CanUseNoMercy(out IAction act)
    {
        if (InDungeonsMiddle && NoMercy.ShouldUse(out act))
        {
            if (CanUseSpellInDungeonsMiddle) return true;
            return false;
        }
        //等级低于爆发击是判断
        if (!BurstStrike.EnoughLevel && NoMercy.ShouldUse(out act)) return true;

        if (BurstStrike.EnoughLevel && NoMercy.ShouldUse(out act))
        {
            //4GCD起手判断
            if (IsLastWeaponSkill(KeenEdge.ID) && JobGauge.Ammo == 1 && !GnashingFang.IsCoolDown && !Bloodfest.IsCoolDown) return true;

            //3弹进无情
            else if (JobGauge.Ammo == (Level >= 88 ? 3 : 2)) return true;

            //2弹进无情
            else if (JobGauge.Ammo == 2 && GnashingFang.IsCoolDown) return true;
        }


        act = null;
        return false;
    }

    private bool CanUseGnashingFang(out IAction act)
    {
        //基础判断
        if (GnashingFang.ShouldUse(out act))
        {
            //在4人本道中使用
            if (InDungeonsMiddle) return true;

            //无情中3弹烈牙
            if (JobGauge.Ammo == (Level >= 88 ? 3 : 2) && (Player.HaveStatusFromSelf(StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(55, false))) return true;

            //无情外烈牙
            if (JobGauge.Ammo > 0 && !NoMercy.WillHaveOneCharge(17, false) && NoMercy.WillHaveOneCharge(35, false)) return true;

            //3弹且将会溢出子弹的情况,提前在无情前进烈牙
            if (JobGauge.Ammo == 3 && IsLastWeaponSkill(BrutalShell.ID) && NoMercy.WillHaveOneCharge(3, false)) return true;

            //1弹且血壤快冷却好了
            if (JobGauge.Ammo == 1 && !NoMercy.WillHaveOneCharge(55, false) && Bloodfest.WillHaveOneCharge(5, false)) return true;

            //4GCD起手烈牙判断
            if (JobGauge.Ammo == 1 && !NoMercy.WillHaveOneCharge(55, false) && (!Bloodfest.IsCoolDown && Bloodfest.EnoughLevel || !Bloodfest.EnoughLevel)) return true;
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
        if (SonicBreak.ShouldUse(out act))
        {
            //在4人本道中不使用
            if (InDungeonsMiddle) return false;

            if (!GnashingFang.EnoughLevel && Player.HaveStatusFromSelf(StatusID.NoMercy)) return true;

            //在烈牙后面使用音速破
            if (GnashingFang.IsCoolDown && Player.HaveStatusFromSelf(StatusID.NoMercy)) return true;

            //其他判断
            if (!DoubleDown.EnoughLevel && Player.HaveStatusFromSelf(StatusID.ReadyToRip)
                && GnashingFang.IsCoolDown) return true;

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
        if (DoubleDown.ShouldUse(out act, mustUse: true))
        {
            //在4人本道中
            if (InDungeonsMiddle)
            {
                //在4人本的道中已经聚好怪可以使用相关技能(不移动且身边有大于3只小怪),有无情buff
                if (Player.HaveStatusFromSelf(StatusID.NoMercy)) return true;

                return false;
            }

            //在音速破后使用倍攻
            if (SonicBreak.IsCoolDown && Player.HaveStatusFromSelf(StatusID.NoMercy)) return true;

            //2弹无情的特殊判断,提前使用倍攻
            if (Player.HaveStatusFromSelf(StatusID.NoMercy) && !NoMercy.WillHaveOneCharge(55, false) && Bloodfest.WillHaveOneCharge(5, false)) return true;

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
        if (BurstStrike.ShouldUse(out act))
        {
            //在4人本道中且AOE时不使用
            if (InDungeonsMiddle && DemonSlice.ShouldUse(out _)) return false;

            //如果烈牙剩0.5秒冷却好,不释放爆发击,主要因为技速不同可能会使烈牙延后太多所以判定一下
            if (SonicBreak.IsCoolDown && SonicBreak.WillHaveOneCharge((float)0.5, false) && GnashingFang.EnoughLevel) return false;

            //无情中爆发击判定
            if (Player.HaveStatusFromSelf(StatusID.NoMercy) &&
                JobGauge.AmmoComboStep == 0 &&
                !GnashingFang.WillHaveOneCharge(1, false)) return true;
            if (Level < 88 && JobGauge.Ammo == 2) return true;
            //无情外防止溢出
            if (IsLastWeaponSkill(BrutalShell.ID) &&
                (JobGauge.Ammo == (Level >= 88 ? 3 : 2) ||
                Bloodfest.WillHaveOneCharge(6, false) && JobGauge.Ammo <= 2 && !NoMercy.WillHaveOneCharge(10, false) && Bloodfest.EnoughLevel)) return true;

        }
        return false;
    }

    private bool CanUseBowShock(out IAction act)
    {
        if (BowShock.ShouldUse(out act, mustUse: true))
        {
            if (InDungeonsMiddle) return true;

            if (!SonicBreak.EnoughLevel && Player.HaveStatusFromSelf(StatusID.NoMercy)) return true;

            //爆发期,无情中且音速破在冷却中
            if (Player.HaveStatusFromSelf(StatusID.NoMercy) && SonicBreak.IsCoolDown) return true;

        }
        return false;
    }
}

