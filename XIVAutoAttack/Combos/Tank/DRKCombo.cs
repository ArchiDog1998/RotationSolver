using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace XIVAutoAttack.Combos.Tank;

internal class DRKCombo : CustomComboJob<DRKGauge>
{
    internal override uint JobID => 32;
    internal override bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Grit);
    private protected override BaseAction Shield => Actions.Grit;
    protected override bool CanHealSingleAbility => false;

    internal struct Actions
    {
        public static readonly BaseAction
            //重斩
            HardSlash = new BaseAction(3617),

            //吸收斩
            SyphonStrike = new BaseAction(3623),

            //释放
            Unleash = new BaseAction(3621),

            //深恶痛绝
            Grit = new BaseAction(3629, shouldEndSpecial: true),

            //伤残
            Unmend = new BaseAction(3624)
            {
                FilterForHostile = b => BaseAction.ProvokeTarget(b, out _),
            },

            //噬魂斩
            Souleater = new BaseAction(3632),

            //暗黑波动
            FloodofDarkness = new BaseAction(16466),

            //嗜血
            BloodWeapon = new BaseAction(3625)
            {
                OtherCheck = b => JobGauge.Blood <= 70 && Service.ClientState.LocalPlayer.CurrentMp <= 7000,
            },

            //暗影墙
            ShadowWall = new BaseAction(3636)
            {
                BuffsProvide = new ushort[] { ObjectStatus.ShadowWall },
            },

            //暗黑锋
            EdgeofDarkness = new BaseAction(16467, true),

            //弃明投暗
            DarkMind = new BaseAction(3634),

            //行尸走肉
            LivingDead = new BaseAction(3638)
            {
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < Service.Configuration.HealthForDyingTank,
            },

            //腐秽大地
            SaltedEarth = new BaseAction(3639),

            //跳斩
            Plunge = new BaseAction(3640, shouldEndSpecial:true),

            //吸血深渊
            AbyssalDrain = new BaseAction(3641),

            //精雕怒斩
            CarveandSpit = new BaseAction(3643),

            //血溅
            Bloodspiller = new BaseAction(7392),

            //寂灭
            Quietus = new BaseAction(7391),

            //血乱
            Delirium = new BaseAction(7390),

            //至黑之夜
            TheBlackestNight = new BaseAction(7393)
            {
                ChoiceFriend = BaseAction.FindAttackedTarget,
            },

            //刚魂
            StalwartSoul = new BaseAction(16468),

            //暗黑布道
            DarkMissionary = new BaseAction(16471, true),

            //掠影示现
            LivingShadow = new BaseAction(16472),

            //献奉
            Oblation = new BaseAction(25754, true),

            //暗影使者
            Shadowbringer = new BaseAction(25757),

            //腐秽黑暗
            SaltandDarkness = new BaseAction(25755)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SaltedEarth },
            };
    }
    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
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

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (JobGauge.Blood >= 50 && Actions.LivingShadow.ShouldUseAction(out act)) return true;
        if (JobGauge.Blood >= 80 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Delirium))
        {
            if (Actions.Quietus.ShouldUseAction(out act)) return true;
            if (Actions.Bloodspiller.ShouldUseAction(out act)) return true;
        }

        //AOE
        if (Actions.Unleash.ShouldUseAction(out act, lastComboActionID)) return true;

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
        if (Actions.LivingDead.ShouldUseAction(out act)) return true;

        //续Buff
        if (Service.ClientState.LocalPlayer.CurrentMp >= 6000)
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
            if (BaseAction.DistanceToPlayer(Actions.Plunge.Target) < 1)
            {
                return true;
            }
        }

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 1)
        {

            //减伤10%
            if (Actions.Oblation.ShouldUseAction(out act)) return true;

            //减伤30%
            if (Actions.ShadowWall.ShouldUseAction(out act)) return true;

            //减伤20%
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;
            if (Actions.DarkMind.ShouldUseAction(out act)) return true;

            //降低攻击
            //雪仇
            if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Plunge.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }
}
