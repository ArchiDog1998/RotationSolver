using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Tank;

internal class GNBCombo : JobGaugeCombo<GNBGauge>
{
    internal override uint JobID => 37;
    internal override bool HaveShield => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RoyalGuard);
    private protected override PVEAction Shield => Actions.RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    private static string ammoCound = "";

    internal struct Actions
    {
        public static readonly PVEAction
            //王室亲卫
            RoyalGuard = new(16142, shouldEndSpecial: true),

            //利刃斩
            KeenEdge = new(16137),

            //无情
            NoMercy = new(16138),

            //残暴弹
            BrutalShell = new(16139),

            //伪装
            Camouflage = new(16140)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = PVEAction.TankDefenseSelf,
            },

            //恶魔切
            DemonSlice = new(16141),

            //闪雷弹
            LightningShot = new(16143),

            //危险领域
            DangerZone = new(16144),

            //迅连斩
            SolidBarrel = new(16145),

            //爆发击
            BurstStrike = new(16162)
            {
                OtherCheck = b => JobGauge.Ammo > 0,
            },

            //星云
            Nebula = new (16148)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = PVEAction.TankDefenseSelf,
            },

            //恶魔杀
            DemonSlaughter = new (16149),

            //极光
            Aurora = new PVEAction(16151, true)
            {
                BuffsProvide = new [] { ObjectStatus.Aurora },
            },

            //超火流星
            Superbolide = new (16152)
            {
                OtherCheck = PVEAction.TankBreakOtherCheck,
            },

            //音速破
            SonicBreak = new (16153),

            //粗分斩
            RoughDivide = new (16154, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget
            },

            //烈牙
            GnashingFang = new (16146)
            {
                OtherCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
            },

            //弓形冲波
            BowShock = new (16159),

            //光之心
            HeartofLight = new (16160, true),

            //石之心
            HeartofStone = new (16161, true)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //命运之环
            FatedCircle = new (16163)
            {
                OtherCheck = b => JobGauge.Ammo > (Level >= 88 ? 2 : 1),
            },

            //血壤
            Bloodfest = new (16164)
            {
                OtherCheck = b => JobGauge.Ammo == 0,
            },

            //倍攻
            DoubleDown = new (25760)
            {
                OtherCheck = b => JobGauge.Ammo >= 2,
            },

            //猛兽爪
            SavageClaw = new (16147),

            //凶禽爪
            WickedTalon = new (16150),

            //撕喉
            JugularRip = new (16156)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == JugularRip.ID,
            },

            //裂膛
            AbdomenTear = new (16157)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == AbdomenTear.ID,
            },

            //穿目
            EyeGouge = new (16158)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == EyeGouge.ID,
            },

            //超高速
            Hypervelocity = new (25759)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == Hypervelocity.ID,
            };
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体治疗, $"{Actions.Aurora.Action.Name}"},
        {DescType.范围防御, $"{Actions.HeartofLight.Action.Name}"},
        {DescType.单体防御, $"{Actions.HeartofStone.Action.Name}, {Actions.Nebula.Action.Name}, {Actions.Camouflage.Action.Name}"},
        {DescType.移动, $"{Actions.RoughDivide.Action.Name}"},
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetCombo("GNB_Opener", 4, new string[]
        {
            "4GCD起手",

        }, "起手选择");
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //无情
        if (Level >= Actions.BurstStrike.Level && abilityRemain == 1 && Actions.NoMercy.ShouldUse(out act))
        {
            if (LastWeaponskill == Actions.KeenEdge.ID && JobGauge.Ammo == 1 && Actions.GnashingFang.RecastTimeRemain == 0 && !Actions.Bloodfest.IsCoolDown)
            {
                ammoCound = "4B";
                return true;
            }
            //3弹进无情
            else if (JobGauge.Ammo == (Level >= 88 ? 3 : 2))
            {
                return true;
            }
            //2弹进无情
            else if (JobGauge.Ammo == 2 && Actions.GnashingFang.RecastTimeRemain > 0)
            {
                return true;
            }
        }
        if (Level < Actions.BurstStrike.Level && abilityRemain == 1 && Actions.NoMercy.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //AOE
        //if (breakUseAmmo && Actions.DoubleDown.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.FatedCircle.ShouldUse(out act)) return true;
        if (Actions.DemonSlaughter.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.DemonSlice.ShouldUse(out act, lastComboActionID)) return true;

        //烈牙
        if ((JobGauge.Ammo == (Level >= 88 ? 3 : 2) && (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.NoMercy) || Actions.NoMercy.RecastTimeRemain > 55)) ||    //3弹烈牙
            (JobGauge.Ammo > 0 && Actions.NoMercy.RecastTimeRemain > 17 && Actions.NoMercy.RecastTimeRemain < 35) ||    //无无情烈牙
            (JobGauge.Ammo == 3 && LastWeaponskill == Actions.BrutalShell.ID && Actions.NoMercy.RecastTimeRemain < 3) || 
            (JobGauge.Ammo == 1 && Actions.NoMercy.RecastTimeRemain > 55 && Actions.Bloodfest.RecastTimeRemain < 5) ||
            (JobGauge.Ammo == 1 && Actions.NoMercy.RecastTimeRemain > 55 && ((!Actions.Bloodfest.IsCoolDown && Level >= Actions.Bloodfest.Level) || Level < Actions.Bloodfest.Level)))
        {
            if (Actions.GnashingFang.ShouldUse(out act)) return true;
        }

        //音速破
        if (Actions.SonicBreak.ShouldUse(out act))
        {
            if (Actions.GnashingFang.RecastTimeRemain > 0 && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.NoMercy)) return true;

            if (Level < Actions.DoubleDown.Level && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.ReadyToRip) 
                && Actions.GnashingFang.RecastTimeRemain > 0) return true;
        }

        //倍攻
        if (Actions.DoubleDown.ShouldUse(out act, mustUse: true))
        {
            if (Actions.SonicBreak.RecastTimeRemain > 0 && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.NoMercy)) return true;

            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.NoMercy) && Actions.NoMercy.RecastTimeRemain > 55 && Actions.Bloodfest.RecastTimeRemain < 5) return true;
        }

        //烈牙后二连
        uint remap = Service.IconReplacer.OriginalHook(Actions.GnashingFang.ID);
        if (remap == Actions.WickedTalon.ID && Actions.WickedTalon.ShouldUse(out act)) return true;
        if (remap == Actions.SavageClaw.ID && Actions.SavageClaw.ShouldUse(out act)) return true;

        //爆发击   
        if (Actions.BurstStrike.ShouldUse(out act))
        {
            if (Actions.SonicBreak.RecastTimeRemain > 0 && Actions.SonicBreak.RecastTimeRemain < 0.5) return false;

            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.NoMercy) && 
                JobGauge.AmmoComboStep == 0 && 
                Actions.GnashingFang.RecastTimeRemain > 1) return true;

            if (LastWeaponskill == Actions.BrutalShell.ID && 
                (JobGauge.Ammo == (Level >= 88 ? 3 : 2) || 
                (Actions.Bloodfest.RecastTimeRemain < 6 && JobGauge.Ammo <= 2 && Actions.NoMercy.RecastTimeRemain > 10 && Level >= Actions.Bloodfest.Level))) return true;
        }

        //单体三连
        if (Actions.GnashingFang.RecastTimeRemain > 0 && Actions.GnashingFang.RecastTimeRemain < 0.5) return false;
        if (Actions.SolidBarrel.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.BrutalShell.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.KeenEdge.ShouldUse(out act, lastComboActionID)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.LightningShot.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //神圣领域 如果谢不够了。
        if (Actions.Superbolide.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //危险领域
        if (Actions.DangerZone.ShouldUse(out act))
        {
            //非爆发期
            if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.NoMercy)
            && ((Actions.GnashingFang.RecastTimeRemain > 20)
            || (Level < Actions.GnashingFang.Level) && Actions.NoMercy.IsCoolDown)) return true;

            //爆发期
            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.NoMercy))
            {
                //烈牙冷却中
                if (Actions.GnashingFang.RecastTimeRemain > 0) return true;
            }
        }

        //弓形冲波
        if (Actions.BowShock.ShouldUse(out act, mustUse: true))
        {
            //爆发期
            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.NoMercy))
            {
                //音速破在冷却中
                if (Actions.SonicBreak.RecastTimeRemain > 0) return true;
            }

            //音速破在冷却中
            if (Actions.SonicBreak.IsCoolDown && Level < Actions.DoubleDown.Level)
            {
                //弓形冲波
                if (Actions.BowShock.ShouldUse(out act, mustUse: true)) return true;
                //危险领域
                if (Actions.DangerZone.ShouldUse(out act)) return true;
            }
        }

        //续剑
        if (Actions.JugularRip.ShouldUse(out act)) return true;
        if (Actions.AbdomenTear.ShouldUse(out act)) return true;
        if (Actions.EyeGouge.ShouldUse(out act)) return true;
        if (Actions.Hypervelocity.ShouldUse(out act)) return true;

        //血壤
        if (Actions.GnashingFang.RecastTimeRemain > 0 && Actions.Bloodfest.ShouldUse(out act)) return true;

        //搞搞攻击
        if (Actions.RoughDivide.Target.DistanceToPlayer() < 1 && !IsMoving)
        {  
            if (Actions.RoughDivide.ShouldUse(out act)) return true;
            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.NoMercy) && Actions.RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.HeartofLight.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (GeneralActions.Reprisal.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //突进
        if (Actions.RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {

            //减伤10%）
            if (Actions.HeartofStone.ShouldUse(out act)) return true;

            //星云（减伤30%）
            if (Actions.Nebula.ShouldUse(out act)) return true;

            //铁壁（减伤20%）
            if (GeneralActions.Rampart.ShouldUse(out act)) return true;

            //伪装（减伤10%）
            if (Actions.Camouflage.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (GeneralActions.Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Aurora.ShouldUse(out act, emptyOrSkipCombo: true) && abilityRemain == 1) return true;

        return false;
    }
}
