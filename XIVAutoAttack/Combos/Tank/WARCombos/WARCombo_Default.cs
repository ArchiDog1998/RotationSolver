using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.WARCombos.WARCombo_Default;

namespace XIVAutoAttack.Combos.Tank.WARCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Tank/WARCombos/WARCombo_Default.cs")]
internal sealed class WARCombo_Default : WARCombo<CommandType>
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


    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.范围防御, $"{ShakeItOff}"},
        {DescType.单体防御, $"{RawIntuition}, {Vengeance}"},
        {DescType.移动技能, $"GCD: {PrimalRend}，目标为面向夹角小于30°内最远目标。\n                     能力: {Onslaught}, "},
    };
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //摆脱 队友套盾
        if (ShakeItOff.ShouldUse(out act, mustUse: true)) return true;

        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool MoveGCD(out IAction act)
    {
        //放个大 蛮荒崩裂 会往前飞
        if (PrimalRend.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //突进
        if (Onslaught.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //搞搞攻击
        if (PrimalRend.ShouldUse(out act, mustUse: true) && !IsMoving)
        {
            if (PrimalRend.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        //兽魂输出
        //钢铁旋风
        if (SteelCyclone.ShouldUse(out act)) return true;
        //原初之魂
        if (InnerBeast.ShouldUse(out act)) return true;

        //群体
        if (MythrilTempest.ShouldUse(out act)) return true;
        if (Overpower.ShouldUse(out act)) return true;

        //单体
        if (StormsEye.ShouldUse(out act)) return true;
        if (StormsPath.ShouldUse(out act)) return true;
        if (Maim.ShouldUse(out act)) return true;
        if (HeavySwing.ShouldUse(out act)) return true;

        //够不着，随便打一个吧。
        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Tomahawk.ShouldUse(out act)) return true;

        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //死斗 如果血不够了。
        if (Holmgang.ShouldUse(out act)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {
            if (TargetUpdater.HostileTargets.Length == 1)
            {
                //复仇（减伤30%）
                if (Vengeance.ShouldUse(out act)) return true;
            }

            //原初的直觉（减伤10%）
            if (RawIntuition.ShouldUse(out act)) return true;

            //复仇（减伤30%）
            if (Vengeance.ShouldUse(out act)) return true;

            //铁壁（减伤20%）
            if (Rampart.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {

        //爆发
        if (!Player.WillStatusEndGCD(3, 0, true, StatusIDs.SurgingTempest) || !MythrilTempest.EnoughLevel)
        {
            //狂暴
            if (!new BaseAction(7389).IsCoolDown && Berserk.ShouldUse(out act)) return true;
        }

        if (Player.GetHealthRatio() < 0.6f)
        {
            //战栗
            if (ThrillofBattle.ShouldUse(out act)) return true;
            //泰然自若 自奶啊！
            if (Equilibrium.ShouldUse(out act)) return true;
        }

        //奶个队友啊。
        if (!HaveShield && NascentFlash.ShouldUse(out act)) return true;

        //战嚎
        if (Infuriate.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //普通攻击
        //群山隆起
        if (Orogeny.ShouldUse(out act)) return true;
        //动乱 
        if (Upheaval.ShouldUse(out act)) return true;

        //搞搞攻击
        if (Onslaught.ShouldUse(out act) && !IsMoving)
        {
            if (Onslaught.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        return false;
    }
}
