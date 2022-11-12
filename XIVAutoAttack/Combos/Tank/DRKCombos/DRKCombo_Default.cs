using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.DRKCombos.DRKCombo_Default;

namespace XIVAutoAttack.Combos.Tank.DRKCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Tank/DRKCombos/DRKCombo_Default.cs")]
internal sealed class DRKCombo_Default : DRKCombo_Base<CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    protected override bool CanHealSingleAbility => false;

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
        {DescType.单体治疗, $"{TheBlackestNight}，目标为被打的小可怜"},
        {DescType.范围防御, $"{DarkMissionary}"},
        {DescType.单体防御, $"{Oblation}, {ShadowWall}, {DarkMind}"},
        {DescType.移动技能, $"{Plunge}"},
    };

    public override string Author => "汐ベMoon";

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("TheBlackestNight", true, "留3000蓝");
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (TheBlackestNight.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (DarkMissionary.ShouldUse(out act)) return true;
        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //寂灭
        if (JobGauge.Blood >= 80 || Player.HaveStatus(StatusID.Delirium))
        {
            if (Quietus.ShouldUse(out act)) return true;
        }

        //血溅
        if (Bloodspiller.ShouldUse(out act))
        {
            if (Player.HaveStatus(StatusID.Delirium) && Player.FindStatusStack(StatusID.BloodWeapon) <= 3) return true;

            if (JobGauge.Blood >= 50 && BloodWeapon.WillHaveOneChargeGCD(1) || JobGauge.Blood >= 90 && !Player.HaveStatus(StatusID.Delirium)) return true;

            if (!Delirium.EnoughLevel) return true;

        }

        //AOE
        if (StalwartSoul.ShouldUse(out act)) return true;
        if (Unleash.ShouldUse(out act)) return true;

        //单体
        if (Souleater.ShouldUse(out act)) return true;
        if (SyphonStrike.ShouldUse(out act)) return true;
        if (HardSlash.ShouldUse(out act)) return true;

        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Unmend.ShouldUse(out act))
        {
            if (InDungeonsMiddle && Target.DistanceToPlayer() < 5) return false;
            return true;
        }

        return false;
    }
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && (InDungeonsMiddle && CanUseSpellInDungeonsMiddle || !InDungeonsMiddle))
        {
            //嗜血
            if (BloodWeapon.ShouldUse(out act)) return true;

            //血乱
            if (Delirium.ShouldUse(out act)) return true;
        }

        //掠影示现
        if (LivingShadow.ShouldUse(out act)) return true;

        //暗黑波动
        if (FloodofDarkness.ShouldUse(out act))
        {
            if ((Player.CurrentMp >= 6000 || JobGauge.HasDarkArts) && Unleash.ShouldUse(out _)) return true;
        }

        //暗黑锋
        if (CanUseEdgeofDarkness(out act)) return true;

        //腐秽大地
        if (!IsMoving && SaltedEarth.ShouldUse(out act, mustUse: true)) return true;

        if (Delirium.ElapsedAfterGCD(1) && !Delirium.ElapsedAfterGCD(8))
        {
            //暗影使者
            if (Shadowbringer.ShouldUse(out act)) return true;

            //吸血深渊+精雕怒斩
            if (AbyssalDrain.ShouldUse(out act)) return true;
            if (CarveandSpit.ShouldUse(out act)) return true;

            if (Shadowbringer.ShouldUse(out act, mustUse: true)) return true;

        }
        //吸血深渊+精雕怒斩
        if (!Delirium.EnoughLevel && AbyssalDrain.ShouldUse(out act)) return true;
        if (!Delirium.EnoughLevel && CarveandSpit.ShouldUse(out act)) return true;

        //腐秽大地+腐秽黑暗
        if (SaltandDarkness.ShouldUse(out act)) return true;

        //搞搞攻击
        if (Plunge.ShouldUse(out act) && !IsMoving)
        {
            if (Plunge.Target.DistanceToPlayer() < 1) return true;
        }

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //行尸走肉
        if (LivingDead.ShouldUse(out act)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //上黑盾
        if (TheBlackestNight.ShouldUse(out act)) return true;

        if (abilityRemain == 2)
        {
            //减伤10%
            if (Oblation.ShouldUse(out act)) return true;

            //减伤30%
            if (ShadowWall.ShouldUse(out act)) return true;

            //减伤20%
            if (Rampart.ShouldUse(out act)) return true;
            if (DarkMind.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Plunge.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    private bool CanUseEdgeofDarkness(out IAction act)
    {
        if (!EdgeofDarkness.ShouldUse(out act)) return false;

        if (InDungeonsMiddle && TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 25).Length >= 3) return false;

        //是否留3000蓝开黑盾
        if (Config.GetBoolByName("TheBlackestNight") && Player.CurrentMp < 6000) return false;

        //爆发期打完
        if (Delirium.IsCoolDown && Delirium.ElapsedAfterGCD(1) && !Delirium.ElapsedAfterGCD(7)) return true;

        //非爆发期防止溢出+续buff
        if (JobGauge.HasDarkArts || Player.CurrentMp > 8500 || JobGauge.DarksideTimeRemaining < 10) return true;

        return false;
    }
    private protected override IAction CountDownAction(float remainTime)
    {
        //战斗前嗜血和血乱
        if (remainTime <= 7 && Delirium.ShouldUse(out var act)) return act;
        if (remainTime <= 5 && BloodWeapon.ShouldUse(out var act1)) return act1;
        return base.CountDownAction(remainTime);
    }
}
