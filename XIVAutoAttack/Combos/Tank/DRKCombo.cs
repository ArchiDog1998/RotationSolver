using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal class DRKCombo : CustomComboJob<DRKGauge>
{
    internal override uint JobID => 32;
    internal override bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Grit);
    private protected override BaseAction Shield => Actions.Grit;
    protected override bool CanHealSingleAbility => false;
    internal override string Description => "黑盾在单体治疗中，选择的是被打的那个小可怜。";
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
            Plunge = new BaseAction(3640),

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

    private protected override bool HealSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.TheBlackestNight.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.DarkMissionary.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        if(JobGauge.Blood >= 50 && Actions.LivingShadow.ShouldUseAction(out act)) return true;
        if (JobGauge.Blood >= 50 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Delirium))
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
    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        //续Buff
        if(JobGauge.DarksideTimeRemaining < 30000 && Service.ClientState.LocalPlayer.CurrentMp >= 6000)
        {
            if (Actions.FloodofDarkness.ShouldUseAction(out act)) return true;
            if (Actions.EdgeofDarkness.ShouldUseAction(out act)) return true;
            if (Actions.FloodofDarkness.ShouldUseAction(out act, mustUse: true)) return true;
        }
        if (JobGauge.DarksideTimeRemaining > 0 && Actions.Shadowbringer.ShouldUseAction(out act)) return true;

        if (Actions.Delirium.ShouldUseAction(out act)) return true;

        if (Actions.SaltandDarkness.ShouldUseAction(out act)) return true;
        if (Actions.BloodWeapon.ShouldUseAction(out act)) return true;
        if (!IsMoving && Actions.SaltedEarth.ShouldUseAction(out act, mustUse:true)) return true;

        if (Actions.AbyssalDrain.ShouldUseAction(out act)) return true;
        if (Actions.CarveandSpit.ShouldUseAction(out act)) return true;
        if (Actions.AbyssalDrain.ShouldUseAction(out act, mustUse:true)) return true;


        //搞搞攻击
        if (Actions.Plunge.ShouldUseAction(out act) && ! IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.Plunge.Target, true) < 1)
            {
                return true;
            }
        }

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (Actions.LivingDead.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
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

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Plunge.ShouldUseAction(out act, Empty:true)) return true;

        return false;
    }

    public static class Buffs
    {
        public const ushort BloodWeapon = 742;

        public const ushort Darkside = 751;

        public const ushort Delirium = 1972;
    }

    public static class Debuffs
    {
        public const ushort Placeholder = 0;
    }

    public static class Levels
    {
        public const byte SyphonStrike = 2;

        public const byte Souleater = 26;

        public const byte FloodOfDarkness = 30;

        public const byte EdgeOfDarkness = 40;

        public const byte SaltedEarth = 52;

        public const byte AbyssalDrain = 56;

        public const byte CarveAndSpit = 60;

        public const byte Bloodpiller = 62;

        public const byte Quietus = 64;

        public const byte Delirium = 68;

        public const byte StalwartSoul = 72;

        public const byte Shadow = 74;

        public const byte SaltAndDarkness = 86;

        public const byte Shadowbringer = 90;
    }

    public const uint HardSlash = 3617u;

    public const uint Unleash = 3621u;

    public const uint SyphonStrike = 3623u;

    public const uint Souleater = 3632u;

    public const uint SaltedEarth = 3639u;

    public const uint AbyssalDrain = 3641u;

    public const uint CarveAndSpit = 3643u;

    public const uint Quietus = 7391u;

    public const uint Bloodspiller = 7392u;

    public const uint FloodOfDarkness = 16466u;

    public const uint EdgeOfDarkness = 16467u;

    public const uint StalwartSoul = 16468u;

    public const uint FloodOfShadow = 16469u;

    public const uint 弗雷 = 16472u;

    public const uint EdgeOfShadow = 16470u;

    public const uint SaltAndDarkness = 25755u;

    public const uint Shadowbringer = 25757u;
}
