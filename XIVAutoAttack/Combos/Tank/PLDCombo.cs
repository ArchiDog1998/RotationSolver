using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack;
using XIVAutoAttack.Combos;

namespace XIVAutoAttack.Combos.Tank;

internal class PLDCombo : CustomComboJob<PLDGauge>
{
    internal override uint JobID => 19;

    internal override bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.IronWill);

    private protected override BaseAction Shield => Actions.IronWill;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    internal struct Actions
    {
        public static readonly BaseAction
            //钢铁信念
            IronWill = new (28, shouldEndSpecial: true),

            //先锋剑
            FastBlade = new (9),

            //暴乱剑
            RiotBlade = new (15),

            //沥血剑
            GoringBlade = new (3538)
            {
                TargetStatus = new []
                {
                    ObjectStatus.GoringBlade,
                    ObjectStatus.BladeofValor,
                }
            },

            //战女神之怒
            RageofHalone = new (21),

            //投盾
            ShieldLob = new (24)
            {
                FilterForHostile = b => BaseAction.ProvokeTarget(b, out _),
            },

            //战逃反应
            FightorFlight = new (20),

            //全蚀斩
            TotalEclipse = new (7381),

            //日珥斩
            Prominence = new (16457),

            //预警
            Sentinel = new (17)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //厄运流转
            CircleofScorn = new (23),

            //深奥之灵
            SpiritsWithin = new (29),

            //神圣领域
            HallowedGround = new (30)
            {
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < Service.Configuration.HealthForDyingTank,
            },

            //圣光幕帘
            DivineVeil = new (3540),

            //深仁厚泽
            Clemency = new (3541, true, true),

            //干预
            Intervention = new (7382, true)
            {
                ChoiceFriend = BaseAction.FindAttackedTarget,
            },

            //调停
            Intervene = new (16461, shouldEndSpecial:true),

            //赎罪剑
            Atonement = new (16460)
            {
                BuffsNeed = new [] { ObjectStatus.SwordOath },
            },

            //偿赎剑
            Expiacion = new (25747),

            //英勇之剑
            BladeofValor = new (25750),

            //真理之剑
            BladeofTruth = new (25749),

            //信念之剑
            BladeofFaith = new (25748)
            {
                BuffsNeed = new [] { ObjectStatus.ReadyForBladeofFaith },
            },

            //安魂祈祷
            Requiescat = new (7383),

            //悔罪
            Confiteor = new (16459),

            //圣环
            HolyCircle = new (16458),

            //圣灵
            HolySpirit = new (7384),

            //武装戍卫
            PassageofArms = new (7385),

            //保护
            //Cover = new BaseAction(27, true),

            //盾阵
            Sheltron = new (3542);
        //盾牌猛击
        //ShieldBash = new BaseAction(16),
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体治疗, $"{Actions.Clemency.Action.Name}"},
        {DescType.范围防御, $"{Actions.DivineVeil.Action.Name}, {Actions.PassageofArms.Action.Name}"},
        {DescType.单体防御, $"{Actions.Sentinel.Action.Name}, {Actions.Sheltron.Action.Name}"},
        {DescType.移动, $"{Actions.Intervene.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //三个大招
        if (Actions.BladeofValor.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;
        if (Actions.BladeofFaith.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.BladeofTruth.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;

        //魔法三种姿势
        var status = BaseAction.FindStatusFromSelf(Service.ClientState.LocalPlayer).Where(status => status.StatusId == ObjectStatus.Requiescat);
        if (status != null && status.Count() > 0)
        {
            var s = status.First();
            if ((s.StackCount == 1 || s.RemainingTime < 2.5) &&
                Actions.Confiteor.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.HolyCircle.ShouldUseAction(out act)) return true;
            if (Actions.HolySpirit.ShouldUseAction(out act)) return true;
        }

        //AOE 二连
        if (Actions.Prominence.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.TotalEclipse.ShouldUseAction(out act, lastComboActionID)) return true;

        //赎罪剑
        if (Actions.Atonement.ShouldUseAction(out act)) return true;

        //单体三连
        if (Actions.GoringBlade.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.RageofHalone.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.RiotBlade.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.FastBlade.ShouldUseAction(out act, lastComboActionID)) return true;

        //投盾
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.ShieldLob.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //调停
        if (Actions.Intervene.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //深仁厚泽
        if (Actions.Clemency.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //圣光幕帘
        if (Actions.DivineVeil.ShouldUseAction(out act)) return true;

        //武装戍卫
        if (Actions.PassageofArms.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //战逃反应 加Buff
        if (Actions.FightorFlight.ShouldUseAction(out act)) return true;

        //厄运流转
        if (Actions.CircleofScorn.ShouldUseAction(out act, mustUse: true)) return true;

        //偿赎剑
        if (Actions.Expiacion.ShouldUseAction(out act, mustUse: true)) return true;

        //安魂祈祷
        if (Service.TargetManager.Target is BattleChara b && BaseAction.FindStatusFromSelf(b, ObjectStatus.GoringBlade, ObjectStatus.BladeofValor) is float[] times &&
            times != null && times.Length > 0 && times.Max() > 10 &&
            Actions.Requiescat.ShouldUseAction(out act, mustUse: true)) return true;

        //深奥之灵
        if (Actions.SpiritsWithin.ShouldUseAction(out act)) return true;

        //搞搞攻击
        if (Actions.Intervene.ShouldUseAction(out act) && !IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.Intervene.Target) < 1)
            {
                return true;
            }
        }

        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //神圣领域 如果谢不够了。
        if (Actions.HallowedGround.ShouldUseAction(out act)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 1)
        {

            //预警（减伤30%）
            if (Actions.Sentinel.ShouldUseAction(out act)) return true;

            //铁壁（减伤20%）
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;

            if (JobGauge.OathGauge >= 50)
            {
                //盾阵
                if (Actions.Sheltron.ShouldUseAction(out act)) return true;
            }
        }
        //降低攻击
        //雪仇
        if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;

        //干预（减伤10%）
        if (!HaveShield && Actions.Intervention.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }
}
