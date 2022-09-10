using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack;
using XIVAutoAttack.Combos;

namespace XIVAutoAttack.Combos.Tank;

internal class WARCombo : JobGaugeCombo<WARGauge>
{
    internal override uint JobID => 21;
    internal override bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Defiance);
    private protected override BaseAction Shield => Actions.Defiance;
    internal static float BuffTime
    {
        get
        {
            var time = BaseAction.FindStatusSelfFromSelf(ObjectStatus.SurgingTempest);
            if (time.Length == 0) return 0;
            return time[0];
        }
    }

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
                OtherCheck = b => BuffTime < 10,
            },

            //飞斧
            Tomahawk = new (46)
            {
                FilterForHostile = b => BaseAction.ProvokeTarget(b),
            },

            //猛攻
            Onslaught = new (7386, shouldEndSpecial: true),

            //动乱    
            Upheaval = new (7387),

            //超压斧
            Overpower = new (41),

            //秘银暴风
            MythrilTempest = new (16462),

            //群山隆起
            Orogeny = new (25752),

            //原初之魂
            InnerBeast = new (49),

            //钢铁旋风
            SteelCyclone = new (51),

            //战嚎
            Infuriate = new (52)
            {
                BuffsProvide = new [] { ObjectStatus.InnerRelease },
                OtherCheck = b => BaseAction.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge <= 50,
            },

            //狂暴
            Berserk = new (38)
            {
                OtherCheck = b => BaseAction.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0,
            },

            //战栗
            ThrillofBattle = new (40),

            //泰然自若
            Equilibrium = new (3552),

            //原初的勇猛
            NascentFlash = new (16464)
            {
                ChoiceFriend = BaseAction.FindAttackedTarget,
            },

            ////原初的血气
            //Bloodwhetting = new BaseAction(25751),

            //复仇
            Vengeance = new (44)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //原初的直觉
            RawIntuition = new (3551)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //摆脱
            ShakeItOff = new (7388),

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
        if (Actions.ShakeItOff.ShouldUseAction(out act)) return true;

        if (GeneralActions.Reprisal.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool MoveGCD(uint lastComboActionID, out IAction act)
    {
        //放个大 蛮荒崩裂 会往前飞
        if (Actions.PrimalRend.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //突进
        if (Actions.Onslaught.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;

    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //搞搞攻击
        if (Actions.PrimalRend.ShouldUseAction(out act, mustUse: true) && !IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.PrimalRend.Target) < 1)
            {
                return true;
            }
        }

        //兽魂输出
        if (JobGauge.BeastGauge >= 50 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.InnerRelease))
        {
            //钢铁旋风
            if (Actions.SteelCyclone.ShouldUseAction(out act)) return true;
            //原初之魂
            if (Actions.InnerBeast.ShouldUseAction(out act)) return true;
        }

        //群体
        if (Actions.MythrilTempest.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Overpower.ShouldUseAction(out act, lastComboActionID)) return true;

        //单体
        if (Actions.StormsEye.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.StormsPath.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Maim.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.HeavySwing.ShouldUseAction(out act, lastComboActionID)) return true;

        //够不着，随便打一个吧。
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.Tomahawk.ShouldUseAction(out act)) return true;

        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //死斗 如果血不够了。
        if (Actions.Holmgang.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {
            //原初的直觉（减伤10%）
            if (Actions.RawIntuition.ShouldUseAction(out act)) return true;

            //复仇（减伤30%）
            if (Actions.Vengeance.ShouldUseAction(out act)) return true;

            //铁壁（减伤20%）
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;

        }
        //降低攻击
        //雪仇
        if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //爆发
        if (BuffTime > 3 || Service.ClientState.LocalPlayer.Level < Actions.MythrilTempest.Level)
        {
            //战嚎
            if (Actions.Infuriate.ShouldUseAction(out act)) return true;
            //狂暴
            if (!new BaseAction(7389).IsCoolDown && Actions.Berserk.ShouldUseAction(out act)) return true;
            //战嚎
            if (Actions.Infuriate.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }

        if ((float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6)
        {
            //战栗
            if (Actions.ThrillofBattle.ShouldUseAction(out act)) return true;
            //泰然自若 自奶啊！
            if (Actions.Equilibrium.ShouldUseAction(out act)) return true;
        }

        //奶个队友啊。
        if (!HaveShield && Actions.NascentFlash.ShouldUseAction(out act)) return true;

        //普通攻击
        //群山隆起
        if (Actions.Orogeny.ShouldUseAction(out act)) return true;
        //动乱 
        if (Actions.Upheaval.ShouldUseAction(out act)) return true;

        //搞搞攻击
        if (Actions.Onslaught.ShouldUseAction(out act) && !IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.Onslaught.Target) < 2)
            {
                return true;
            }
        }

        return false;
    }
}
