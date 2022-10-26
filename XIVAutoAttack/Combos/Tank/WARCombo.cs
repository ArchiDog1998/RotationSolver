using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Controllers;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Tank;

internal sealed class WARCombo : JobGaugeCombo<WARGauge>
{
    internal override uint JobID => 21;
    internal override bool HaveShield => LocalPlayer.HaveStatus(ObjectStatus.Defiance);
    private protected override BaseAction Shield => Actions.Defiance;
    internal struct Actions
    {
        public static readonly BaseAction
            //守护
            Defiance = new (48, shouldEndSpecial: true),

            //重劈
            HeavySwing = new (31),

            //凶残裂
            Maim = new (37),

            //暴风斩 绿斧
            StormsPath = new (42),

            //暴风碎 红斧
            StormsEye = new (45)
            {
                OtherCheck = b => LocalPlayer.WillStatusEndGCD(3, 0, true, ObjectStatus.SurgingTempest),
            },

            //飞斧
            Tomahawk = new (46)
            {
                FilterForTarget = b => TargetFilter.ProvokeTarget(b),
            },

            //猛攻
            Onslaught = new (7386, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget,
            },

            //动乱    
            Upheaval = new(7387)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SurgingTempest },
            },

            //超压斧
            Overpower = new (41),

            //秘银暴风
            MythrilTempest = new (16462),

            //群山隆起
            Orogeny = new (25752),

            //原初之魂
            InnerBeast = new (49)
            {
                OtherCheck = b => !LocalPlayer.WillStatusEndGCD(3, 0, true, ObjectStatus.SurgingTempest) && ( JobGauge.BeastGauge >= 50 || LocalPlayer.HaveStatus(ObjectStatus.InnerRelease)),
            },

            //钢铁旋风
            SteelCyclone = new(51)
            {
                OtherCheck = InnerBeast.OtherCheck,
            },

            //战嚎
            Infuriate = new (52)
            {
                BuffsProvide = new [] { ObjectStatus.InnerRelease },
                OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge < 50,
            },

            //狂暴
            Berserk = new (38)
            {
                OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0,
            },

            //战栗
            ThrillofBattle = new (40),

            //泰然自若
            Equilibrium = new (3552),

            //原初的勇猛
            NascentFlash = new (16464)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            ////原初的血气
            //Bloodwhetting = new BaseAction(25751),

            //复仇
            Vengeance = new (44)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //原初的直觉
            RawIntuition = new (3551)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //摆脱
            ShakeItOff = new (7388, true),

            //死斗
            Holmgang = new (43)
            {
                OtherCheck = BaseAction.TankBreakOtherCheck,
            },

            ////原初的解放
            //InnerRelease = new BaseAction(7389),

            //蛮荒崩裂
            PrimalRend = new (25753)
            {
                BuffsNeed = new [] { ObjectStatus.PrimalRendReady },
            };
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.范围防御, $"{Actions.ShakeItOff.Action.Name}"},
        {DescType.单体防御, $"{Actions.RawIntuition.Action.Name}, {Actions.Vengeance.Action.Name}"},
        {DescType.移动, $"GCD: {Actions.PrimalRend.Action.Name}，目标为面向夹角小于30°内最远目标。\n                     能力: {Actions.Onslaught.Action.Name}, "},
    };
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //摆脱 队友套盾
        if (Actions.ShakeItOff.ShouldUse(out act, mustUse:true)) return true;

        if (GeneralActions.Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool MoveGCD(uint lastComboActionID, out IAction act)
    {
        //放个大 蛮荒崩裂 会往前飞
        if (Actions.PrimalRend.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //突进
        if (Actions.Onslaught.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //搞搞攻击
        if (Actions.PrimalRend.ShouldUse(out act, mustUse: true) && !IsMoving)
        {
            if (Actions.PrimalRend.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        //兽魂输出
        //钢铁旋风
        if (Actions.SteelCyclone.ShouldUse(out act)) return true;
        //原初之魂
        if (Actions.InnerBeast.ShouldUse(out act)) return true;

        //群体
        if (Actions.MythrilTempest.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Overpower.ShouldUse(out act, lastComboActionID)) return true;

        //单体
        if (Actions.StormsEye.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.StormsPath.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Maim.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.HeavySwing.ShouldUse(out act, lastComboActionID)) return true;

        //够不着，随便打一个吧。
        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Actions.Tomahawk.ShouldUse(out act)) return true;

        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //死斗 如果血不够了。
        if (Actions.Holmgang.ShouldUse(out act)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {
            if(TargetUpdater.HostileTargets.Length == 1)
            {
                //复仇（减伤30%）
                if (Actions.Vengeance.ShouldUse(out act)) return true;
            }

            //原初的直觉（减伤10%）
            if (Actions.RawIntuition.ShouldUse(out act)) return true;

            //复仇（减伤30%）
            if (Actions.Vengeance.ShouldUse(out act)) return true;

            //铁壁（减伤20%）
            if (GeneralActions.Rampart.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (GeneralActions.Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        
        //爆发
        if (!LocalPlayer.WillStatusEndGCD(3, 0, true, ObjectStatus.SurgingTempest) || !Actions.MythrilTempest.EnoughLevel)
        {
            //狂暴
            if (!new BaseAction(7389).IsCoolDown && Actions.Berserk.ShouldUse(out act)) return true;
        }

        if (LocalPlayer.GetHealthRatio() < 0.6f)
        {
            //战栗
            if (Actions.ThrillofBattle.ShouldUse(out act)) return true;
            //泰然自若 自奶啊！
            if (Actions.Equilibrium.ShouldUse(out act)) return true;
        }

        //奶个队友啊。
        if (!HaveShield && Actions.NascentFlash.ShouldUse(out act)) return true;

        //战嚎
        if (Actions.Infuriate.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //普通攻击
        //群山隆起
        if (Actions.Orogeny.ShouldUse(out act)) return true;
        //动乱 
        if (Actions.Upheaval.ShouldUse(out act)) return true;

        //搞搞攻击
        if (Actions.Onslaught.ShouldUse(out act) && !IsMoving)
        {
            if (Actions.Onslaught.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        return false;
    }
}
