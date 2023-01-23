using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Updaters;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Rotations.Tank.WAR;

internal sealed class WAR_Default : WAR_Base
{
    public override string GameVersion => "6.0";

    public override string RotationName => "Default";

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.DefenseArea, $"{ShakeItOff}"},
        {DescType.DefenseSingle, $"{RawIntuition}, {Vengeance}"},
        {DescType.MoveAction, $"GCD: {PrimalRend}\n{Onslaught}"},
    };

    static WAR_Default()
    {
        InnerBeast.RotationCheck = b => !Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest);
    }

    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //摆脱 队友套盾
        if (ShakeItOff.CanUse(out act, mustUse: true)) return true;

        if (Reprisal.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool MoveGCD(out IAction act)
    {
        //放个大 蛮荒崩裂 会往前飞
        if (PrimalRend.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //搞搞攻击
        if (PrimalRend.CanUse(out act, mustUse: true) && !IsMoving)
        {
            if (PrimalRend.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        //兽魂输出
        //钢铁旋风
        if (SteelCyclone.CanUse(out act)) return true;
        //原初之魂
        if (InnerBeast.CanUse(out act)) return true;

        //群体
        if (MythrilTempest.CanUse(out act)) return true;
        if (Overpower.CanUse(out act)) return true;

        //单体
        if (StormsEye.CanUse(out act)) return true;
        if (StormsPath.CanUse(out act)) return true;
        if (Maim.CanUse(out act)) return true;
        if (HeavySwing.CanUse(out act)) return true;

        //够不着，随便打一个吧。
        if (RSCommands.SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
        if (Tomahawk.CanUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (abilitiesRemaining == 2)
        {
            if (TargetUpdater.HostileTargets.Count() > 1)
            {
                //原初的直觉（减伤10%）
                if (RawIntuition.CanUse(out act)) return true;
            }

            //复仇（减伤30%）
            if (Vengeance.CanUse(out act)) return true;

            //铁壁（减伤20%）
            if (Rampart.CanUse(out act)) return true;

            //原初的直觉（减伤10%）
            if (RawIntuition.CanUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (Reprisal.CanUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        //爆发
        if (!Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest) || !MythrilTempest.EnoughLevel)
        {
            //狂暴
            if (!InnerRelease.IsCoolingDown && Berserk.CanUse(out act)) return true;
        }

        if (Player.GetHealthRatio() < 0.6f)
        {
            //战栗
            if (ThrillofBattle.CanUse(out act)) return true;
            //泰然自若 自奶啊！
            if (Equilibrium.CanUse(out act)) return true;
        }

        //奶个队友啊。
        if (!HasShield && NascentFlash.CanUse(out act)) return true;

        //战嚎
        if (Infuriate.CanUse(out act, emptyOrSkipCombo: true)) return true;

        //普通攻击
        //群山隆起
        if (Orogeny.CanUse(out act)) return true;
        //动乱 
        if (Upheaval.CanUse(out act)) return true;

        //搞搞攻击
        if (Onslaught.CanUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }
}
