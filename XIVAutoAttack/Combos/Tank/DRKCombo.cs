using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Tank;

internal class DRKCombo : JobGaugeCombo<DRKGauge>
{
    public class DRKAction : PVEAction
    {
        internal override uint MPNeed => JobGauge.HasDarkArts ? 0 : base.MPNeed;
        internal DRKAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) 
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }

    internal override uint JobID => 32;
    internal override bool HaveShield => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Grit);
    private protected override PVEAction Shield => Actions.Grit;
    protected override bool CanHealSingleAbility => false;

    private static bool OpenerFinished = false;

    internal struct Actions
    {
        public static readonly PVEAction
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
            EdgeofDarkness = new DRKAction(16467)
            {
                OtherCheck = b => LastAbility != Service.IconReplacer.OriginalHook(EdgeofDarkness.ID) && LocalPlayer.CurrentMp >= 3000,
            },

            //嗜血
            BloodWeapon = new (3625)
            {
                OtherCheck = b => JobGauge.DarksideTimeRemaining > 0,
            },

            //暗影墙
            ShadowWall = new (3636)
            {
                BuffsProvide = new [] { ObjectStatus.ShadowWall },
                OtherCheck = PVEAction.TankDefenseSelf,
            },

            //弃明投暗
            DarkMind = new(3634)
            {
                OtherCheck = PVEAction.TankDefenseSelf,
            },

            //行尸走肉
            LivingDead = new (3638)
            {
                OtherCheck = PVEAction.TankBreakOtherCheck,
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
            Bloodspiller = new (7392)
            {
                OtherCheck = b => JobGauge.Blood >= 50 || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Delirium),
            },

            //寂灭
            Quietus = new (7391),

            //血乱
            Delirium = new (7390)
            {
                OtherCheck = b => JobGauge.DarksideTimeRemaining > 0,
            },

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
            LivingShadow = new (16472)
            {
                OtherCheck = b => JobGauge.Blood >= 50 && JobGauge.DarksideTimeRemaining > 1,
            },

            //献奉
            Oblation = new (25754, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },
            
            //暗影使者
            Shadowbringer = new (25757)
            {
                OtherCheck = b => JobGauge.DarksideTimeRemaining > 1 && LastAbility != Shadowbringer.ID && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Delirium),
            },

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

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("DRK_TheBlackestNight", true, "留3000蓝");
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.TheBlackestNight.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.DarkMissionary.ShouldUse(out act)) return true;
        if (GeneralActions.Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //嗜血
        if (Actions.BloodWeapon.ShouldUse(out act)) return true;

        //血乱
        if (Actions.Delirium.ShouldUse(out act)) return true;

        return base.BreakAbility(abilityRemain, out act);
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //起手判断
        if (!InBattle) OpenerFinished = false;
        if (LastWeaponskill == Actions.Souleater.ID) OpenerFinished = true;

        //寂灭
        if (JobGauge.Blood >= 80 || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Delirium))
        {
            if (Actions.Quietus.ShouldUse(out act)) return true;
        }

        //血溅
        if (Actions.Bloodspiller.ShouldUse(out act)) 
        {
            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Delirium) && Actions.Delirium.IsCoolDown && Actions.Delirium.RecastTimeElapsed > TargetHelper.WeaponTotal * 1) return true;

            if ((JobGauge.Blood >= 70 && Actions.BloodWeapon.RecastTimeRemain is > 0 and < 3) || (JobGauge.Blood >= 50 && Actions.Delirium.RecastTimeRemain > 37 && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Delirium))) return true;

            if (JobGauge.Blood >= 90) return true;
        }

        //AOE
        if (Actions.StalwartSoul.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Unleash.ShouldUse(out act)) return true;

        //单体
        if (Actions.Souleater.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.SyphonStrike.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.HardSlash.ShouldUse(out act, lastComboActionID)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.Unmend.ShouldUse(out act)) return true;

        return false;
    }
    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //掠影示现
        if (Actions.LivingShadow.ShouldUse(out act)) return true;

        //暗黑波动
        if (Actions.FloodofDarkness.ShouldUse(out act))
        {
            if ((LocalPlayer.CurrentMp >= 6000 || JobGauge.HasDarkArts) && Actions.Unleash.ShouldUse(out _)) return true;
        }

        //暗黑锋
        if (Actions.EdgeofDarkness.ShouldUse(out act))
        {do{
            //是否留3000蓝开黑盾
            if (Config.GetBoolByName("DRK_TheBlackestNight") && LocalPlayer.CurrentMp < 6000) break;
            
            //爆发期打完
            if (OpenerFinished && Actions.Delirium.RecastTimeElapsed > TargetHelper.WeaponTotal * 1 && Actions.Delirium.RecastTimeElapsed < TargetHelper.WeaponTotal * 8) return true;

            //非爆发期防止溢出+续buff
            if (JobGauge.HasDarkArts || (LocalPlayer.CurrentMp > 8500 && OpenerFinished) || JobGauge.DarksideTimeRemaining < 10) return true;
            } while (false);
        }
       

        if (Actions.Delirium.IsCoolDown && Actions.Delirium.RecastTimeElapsed > TargetHelper.WeaponTotal * 1)
        {
            //暗影使者
            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Delirium) && Actions.Shadowbringer.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;

            //吸血深渊+精雕怒斩
            if (Actions.AbyssalDrain.ShouldUse(out act)) return true;
            if (Actions.CarveandSpit.ShouldUse(out act)) return true;
        }

        //腐秽大地+腐秽黑暗
        if (Actions.SaltandDarkness.ShouldUse(out act)) return true;
        if (OpenerFinished && !IsMoving && Actions.SaltedEarth.ShouldUse(out act, mustUse: true)) return true;

        //搞搞攻击
        if (Actions.Plunge.ShouldUse(out act) && !IsMoving)
        {
            if (TargetFilter.DistanceToPlayer(Actions.Plunge.Target) < 1) return true;
        }

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //行尸走肉
        if (Actions.LivingDead.ShouldUse(out act)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //上黑盾
        if (Actions.TheBlackestNight.ShouldUse(out act)) return true;

        if (abilityRemain == 2)
        {
            //减伤10%
            if (Actions.Oblation.ShouldUse(out act)) return true;

            //减伤30%
            if (Actions.ShadowWall.ShouldUse(out act)) return true;

            //减伤20%
            if (GeneralActions.Rampart.ShouldUse(out act)) return true;
            if (Actions.DarkMind.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (GeneralActions.Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Plunge.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }
}
