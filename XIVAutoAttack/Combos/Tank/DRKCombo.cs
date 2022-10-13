using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.Tank;

internal class DRKCombo : JobGaugeCombo<DRKGauge>
{
    public class DRKAction : BaseAction
    {
        internal override uint MPNeed => JobGauge.HasDarkArts ? 0 : base.MPNeed;
        internal DRKAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) 
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }

    internal override uint JobID => 32;
    internal override bool HaveShield => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Grit);
    private protected override BaseAction Shield => Actions.Grit;
    protected override bool CanHealSingleAbility => false;

    internal struct Actions
    {
        public static readonly BaseAction
            //重斩
            HardSlash = new (3617),

            //吸收斩
            SyphonStrike = new (3623),

            //释放
            Unleash = new (3621),

            //深恶痛绝
            Grit = new (3629, shouldEndSpecial: true),

            //伤残
            Unmend = new (3624)
            {
                FilterForTarget = b => TargetFilter.ProvokeTarget(b),
            },

            //噬魂斩
            Souleater = new (3632),

            //暗黑波动
            FloodofDarkness = new DRKAction(16466),

            //暗黑锋
            EdgeofDarkness = new DRKAction(16467),

            //嗜血
            BloodWeapon = new (3625)
            {
                OtherCheck = b => JobGauge.Blood <= 70 && Service.ClientState.LocalPlayer.CurrentMp <= 7000,
            },

            //暗影墙
            ShadowWall = new (3636)
            {
                BuffsProvide = new [] { ObjectStatus.ShadowWall },
            },

            //弃明投暗
            DarkMind = new (3634),

            //行尸走肉
            LivingDead = new (3638)
            {
                OtherCheck = BaseAction.TankBreakOtherCheck,
            },

            //腐秽大地
            SaltedEarth = new (3639),

            //跳斩
            Plunge = new (3640, shouldEndSpecial:true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget
            },

            //吸血深渊
            AbyssalDrain = new (3641),

            //精雕怒斩
            CarveandSpit = new (3643),

            //血溅
            Bloodspiller = new (7392),

            //寂灭
            Quietus = new (7391),

            //血乱
            Delirium = new (7390),

            //至黑之夜
            TheBlackestNight = new (7393)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //刚魂
            StalwartSoul = new (16468),

            //暗黑布道
            DarkMissionary = new (16471, true),

            //掠影示现
            LivingShadow = new (16472),

            //献奉
            Oblation = new (25754, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //暗影使者
            Shadowbringer = new (25757),

            //腐秽黑暗
            SaltandDarkness = new (25755)
            {
                BuffsNeed = new [] { ObjectStatus.SaltedEarth },
            };
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体治疗, $"{Actions.TheBlackestNight.Action.Name}，目标为被打的小可怜"},
        {DescType.范围防御, $"{Actions.DarkMissionary.Action.Name}"},
        {DescType.单体防御, $"{Actions.Oblation.Action.Name}, {Actions.ShadowWall.Action.Name}, {Actions.DarkMind.Action.Name}"},
        {DescType.移动, $"{Actions.Plunge.Action.Name}"},
    };
    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.TheBlackestNight.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.DarkMissionary.ShouldUseAction(out act)) return true;
        if (GeneralActions.Reprisal.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (JobGauge.Blood >= 80 || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Delirium))
        {
            if (Actions.Quietus.ShouldUseAction(out act)) return true;
            if (Actions.Bloodspiller.ShouldUseAction(out act)) return true;
        }

        //AOE
        if (Actions.StalwartSoul.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Unleash.ShouldUseAction(out act)) return true;

        //单体
        if (Actions.Souleater.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SyphonStrike.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.HardSlash.ShouldUseAction(out act, lastComboActionID)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.Unmend.ShouldUseAction(out act)) return true;

        return false;
    }
    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Blood >= 50 && Actions.LivingShadow.ShouldUseAction(out act)) return true;

        if (Actions.LivingDead.ShouldUseAction(out act)) return true;

        //续Buff
        if (Service.ClientState.LocalPlayer.CurrentMp >= 6000 || JobGauge.HasDarkArts)
        {
            if (Actions.FloodofDarkness.ShouldUseAction(out act)) return true;
            if (Actions.EdgeofDarkness.ShouldUseAction(out act)) return true;
            if (Actions.FloodofDarkness.ShouldUseAction(out act, mustUse: true)) return true;
        }
        if (JobGauge.DarksideTimeRemaining > 0 && Actions.Shadowbringer.ShouldUseAction(out act, mustUse:true, emptyOrSkipCombo:true)) return true;

        if (Actions.Delirium.ShouldUseAction(out act)) return true;

        if (Actions.SaltandDarkness.ShouldUseAction(out act)) return true;
        if (Actions.BloodWeapon.ShouldUseAction(out act)) return true;
        if (!IsMoving && Actions.SaltedEarth.ShouldUseAction(out act, mustUse: true)) return true;

        if (Actions.AbyssalDrain.ShouldUseAction(out act)) return true;
        if (Actions.CarveandSpit.ShouldUseAction(out act)) return true;
        if (Actions.AbyssalDrain.ShouldUseAction(out act, mustUse: true)) return true;


        //搞搞攻击
        if (Actions.Plunge.ShouldUseAction(out act) && !IsMoving)
        {
            if (TargetFilter.DistanceToPlayer(Actions.Plunge.Target) < 1)
            {
                return true;
            }
        }

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //上黑盾
        if (Actions.TheBlackestNight.ShouldUseAction(out act)) return true;

        if (abilityRemain == 2)
        {
            //减伤10%
            if (Actions.Oblation.ShouldUseAction(out act)) return true;

            //减伤30%
            if (Actions.ShadowWall.ShouldUseAction(out act)) return true;

            //减伤20%
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;
            if (Actions.DarkMind.ShouldUseAction(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Plunge.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }
}
