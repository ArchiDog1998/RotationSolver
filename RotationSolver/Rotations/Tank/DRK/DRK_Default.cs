using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using System.Collections.Generic;
using System.Linq;
using RotationSolver.Updaters;
using RotationSolver.Commands;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Tank.DRK;

internal sealed class DRK_Default : DRK_Base
{
    public override string GameVersion => "6.18";
    public override string RotationName => "Default";

    protected override bool CanHealSingleAbility => false;

    /// <summary>
    /// 在4人本的道中已经聚好怪可以使用相关技能(不移动且身边有大于3只小怪)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetUpdater.PartyMembers.Count() is > 1 and <= 4 && !Target.IsBoss() && !IsMoving
                                                    && TargetUpdater.HostileTargets.GetObjectInRadius(5).Count() >= 3;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealSingle, $"{TheBlackestNight}"},
        {DescType.DefenseArea, $"{DarkMissionary}"},
        {DescType.DefenseSingle, $"{Oblation}, {ShadowWall}, {DarkMind}"},
        {DescType.MoveAction, $"{Plunge}"},
    };


    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("TheBlackestNight", true, "Keep 3000 MP");
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (TheBlackestNight.CanUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (DarkMissionary.CanUse(out act)) return true;
        if (Reprisal.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //寂灭
        if (Blood >= 80 || Player.HasStatus(true, StatusID.Delirium))
        {
            if (Quietus.CanUse(out act)) return true;
        }

        //血溅
        if (Bloodspiller.CanUse(out act))
        {
            if (Player.HasStatus(true, StatusID.Delirium) && Player.StatusStack(true, StatusID.BloodWeapon) <= 3) return true;

            if (Blood >= 50 && BloodWeapon.WillHaveOneChargeGCD(1) || Blood >= 90 && !Player.HasStatus(true, StatusID.Delirium)) return true;

            if (!Delirium.EnoughLevel) return true;

        }

        //AOE
        if (StalwartSoul.CanUse(out act)) return true;
        if (Unleash.CanUse(out act)) return true;

        //单体
        if (Souleater.CanUse(out act)) return true;
        if (SyphonStrike.CanUse(out act)) return true;
        if (HardSlash.CanUse(out act)) return true;

        if (RSCommands.SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;

        if (Unmend.CanUse(out act)) return true;

        return false;
    }
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && (!IsFullParty && CanUseSpellInDungeonsMiddle || IsFullParty))
        {
            //嗜血
            if (BloodWeapon.CanUse(out act)) return true;

            //血乱
            if (Delirium.CanUse(out act)) return true;
        }

        //掠影示现
        if (LivingShadow.CanUse(out act)) return true;

        //暗黑波动
        if (FloodofDarkness.CanUse(out act))
        {
            if ((Player.CurrentMp >= 6000 || HasDarkArts) && Unleash.CanUse(out _)) return true;
        }

        //暗黑锋
        if (CanUseEdgeofDarkness(out act)) return true;

        //腐秽大地
        if (!IsMoving && SaltedEarth.CanUse(out act, mustUse: true)) return true;

        if (Delirium.ElapsedAfterGCD(1) && !Delirium.ElapsedAfterGCD(8))
        {
            //暗影使者
            if (Shadowbringer.CanUse(out act, mustUse: true)) return true;

            //吸血深渊+精雕怒斩
            if (AbyssalDrain.CanUse(out act)) return true;
            if (CarveandSpit.CanUse(out act)) return true;

            if (Shadowbringer.CanUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;

        }
        //吸血深渊+精雕怒斩
        if (!Delirium.EnoughLevel && AbyssalDrain.CanUse(out act)) return true;
        if (!Delirium.EnoughLevel && CarveandSpit.CanUse(out act)) return true;

        //腐秽大地+腐秽黑暗
        if (SaltandDarkness.CanUse(out act)) return true;

        //搞搞攻击
        if (Plunge.CanUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }


    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //上黑盾
        if (TheBlackestNight.CanUse(out act)) return true;

        if (abilityRemain == 2)
        {
            //减伤10%
            if (Oblation.CanUse(out act)) return true;

            //减伤30%
            if (ShadowWall.CanUse(out act)) return true;

            //减伤20%
            if (Rampart.CanUse(out act)) return true;
            if (DarkMind.CanUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (Reprisal.CanUse(out act)) return true;

        act = null;
        return false;
    }

    private bool CanUseEdgeofDarkness(out IAction act)
    {
        if (!EdgeofDarkness.CanUse(out act)) return false;

        //if (!IsFullParty && TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 25).Length >= 3) return false;

        if (HasDarkArts) return true;

        //是否留3000蓝开黑盾
        if (Configs.GetBool("TheBlackestNight") && Player.CurrentMp < 6000) return false;

        //爆发期打完
        if (Delirium.IsCoolDown && Delirium.ElapsedAfterGCD(1) && !Delirium.ElapsedAfterGCD(7)) return true;

        //非爆发期防止溢出+续buff
        if (Player.CurrentMp > 8500 || DarkSideEndAfterGCD(3)) return true;

        return false;
    }
    private protected override IAction CountDownAction(float remainTime)
    {
        //战斗前嗜血和血乱
        if (remainTime <= 7 && Delirium.CanUse(out var act)) return act;
        if (remainTime <= 5 && BloodWeapon.CanUse(out var act1)) return act1;
        return base.CountDownAction(remainTime);
    }
}
