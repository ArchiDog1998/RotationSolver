using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.Gui;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal class MCHCombo : JobGaugeCombo<MCHGauge>
{
    internal override uint JobID => 31;
    private static bool initFinished = false;
    //private static bool MCH_Asocial = false;
    private static bool MCH_Opener = false;
    private static bool MCH_Automaton = false;
    internal struct Actions
    {
        public static readonly BaseAction
            //分裂弹
            SplitShot = new(2866),

            //独头弹
            SlugShot = new(2868)
            {
                OtherIDsCombo = new[] { 7411u },
            },

            //狙击弹
            CleanShot = new(2873)
            {
                OtherIDsCombo = new[] { 7412u },
            },

            //热冲击
            HeatBlast = new(7410),

            //散射
            SpreadShot = new(2870),

            //自动弩
            AutoCrossbow = new(16497),

            //热弹
            HotShow = new(2872),

            //空气锚
            AirAnchor = new(16500)
            {
                //过热不释放技能
                OtherCheck = b => !JobGauge.IsOverheated,
            },

            //钻头
            Drill = new(16498)
            {
                //过热不释放技能
                OtherCheck = b => !JobGauge.IsOverheated,
            },

            //回转飞锯
            ChainSaw = new(25788)
            {
                //过热不释放技能,打进爆发
                OtherCheck = b => !MCH_Opener || (initFinished && !JobGauge.IsOverheated),
            },

            //毒菌冲击
            Bioblaster = new(16499, isDot: true)
            {
                //过热不释放技能
                OtherCheck = b => !JobGauge.IsOverheated,
            },

            //整备
            Reassemble = new(2876)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Reassemble },
                OtherCheck = b => HaveHostileInRange,
            },

            //超荷
            Hypercharge = new(17209)
            {
                OtherCheck = b => !JobGauge.IsOverheated && JobGauge.Heat >= 50,
            },

            //野火
            Wildfire = new(2878)
            {
                OtherCheck = b => JobGauge.Heat >= 50 || IsLastAbility(false, Hypercharge), 
            },

            //虹吸弹
            GaussRound = new(2874),

            //弹射
            Ricochet = new(2890),

            //枪管加热
            BarrelStabilizer = new(7414)
            {
                OtherCheck = b => JobGauge.Heat <= 50 && !IsLastWeaponSkill(false, ChainSaw),
            },

            //车式浮空炮塔
            RookAutoturret = new(2864)
            {
                OtherCheck = b => JobGauge.Battery >= 50 && !JobGauge.IsRobotActive,
            },

            a = new(7557),

            //策动
            Tactician = new(16889, true)
            {
                BuffsProvide = new[]
                {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
                },
            };
    }
    internal override SortedList<DescType, string> Description => new()
    {
        {DescType.循环说明, $"请优先使用标准循环,即关闭自嗨循环.\n                     标准循环会在野火前攒热量来打偶数分钟爆发.\n                     AOE和攻击小怪时不会释放野火"},
        {DescType.爆发技能, $"{Actions.Wildfire.Action.Name}"},
        {DescType.范围防御, $"{Actions.Tactician.Action.Name}"},
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("MCH_Opener", true, "标准起手")
            .SetBool("MCH_Automaton", true, "机器人吃团辅")
            .SetBool("MCH_Reassemble", true, "整备优先链锯");
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //策动
        if (Actions.Tactician.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //野火
        if (CanUseWildfire(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
       
        MCH_Opener = Config.GetBoolByName("MCH_Opener");
        MCH_Automaton = Config.GetBoolByName("MCH_Automaton");
        
        //当上一个连击是热阻击弹时完成起手
        if (InBattle && (IsLastWeaponSkill(true, Actions.CleanShot) || Actions.Wildfire.RecastTimeRemain > 10 || Actions.SpreadShot.ShouldUse(out _)))
        {
            initFinished = true;
        }

        //不在战斗中时重置起手
        if (!InBattle)
        {
            //开场前整备,空气锚和钻头必须冷却好
            if ((!Actions.AirAnchor.IsCoolDown || !Actions.Drill.IsCoolDown) && Actions.Reassemble.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            initFinished = false;
        }

        if (!Config.GetBoolByName("MCH_Opener")) initFinished = true;

        //AOE,毒菌冲击
        if (Actions.Bioblaster.ShouldUse(out act)) return true;
        //单体,四个牛逼的技能。先空气锚再钻头
        if (Actions.AirAnchor.ShouldUse(out act)) return true;
        else if (Level < Actions.AirAnchor.Level && Actions.HotShow.ShouldUse(out act)) return true;
        if (Actions.Drill.ShouldUse(out act)) return true;
        if (Actions.ChainSaw.ShouldUse(out act, mustUse: true)) return true;

        //群体常规GCD
        if (JobGauge.IsOverheated && Actions.AutoCrossbow.ShouldUse(out act)) return true;
        if (Actions.SpreadShot.ShouldUse(out act)) return true;

        //单体常规GCD
        if (JobGauge.IsOverheated && Actions.HeatBlast.ShouldUse(out act)) return true;
        if (Actions.CleanShot.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.SlugShot.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.SplitShot.ShouldUse(out act, lastComboActionID)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //等级小于钻头时,绑定狙击弹
        if (Level < Actions.Drill.Level && nextGCD.IsAnySameAction(true, Actions.CleanShot))
        {
            if (Actions.Reassemble.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        //等级小于90时或自嗨时,整备不再留层数
        if ((Level < Actions.ChainSaw.Level || !Config.GetBoolByName("MCH_Reassemble")) 
            && nextGCD.IsAnySameAction(true, Actions.AirAnchor , Actions.Drill, Actions.ChainSaw))
        {
            if (Actions.Reassemble.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        //整备优先链锯
        if (Config.GetBoolByName("MCH_Reassemble") && nextGCD.IsAnySameAction(true, Actions.ChainSaw))
        {
            if (Actions.Reassemble.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        //如果接下来要搞三大金刚了，整备吧！
        if (nextGCD.IsAnySameAction(true, Actions.AirAnchor, Actions.Drill))
        {
            if (Actions.Reassemble.ShouldUse(out act)) return true;
        }
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //起手虹吸弹、弹射
        if (Actions.Ricochet.RecastTimeRemain == 0 && Actions.Ricochet.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.GaussRound.RecastTimeRemain == 0 && Actions.GaussRound.ShouldUse(out act, mustUse: true)) return true;

        //枪管加热
        if (Actions.BarrelStabilizer.ShouldUse(out act)) return true;

        //车式浮空炮塔
        if (CanUseRookAutoturret(out act)) return true;

        //超荷
        if (CanUseHypercharge(out act)) return true;

        if (Actions.GaussRound.RecastTimeRemain > Actions.Ricochet.RecastTimeRemain)
        {
            //弹射
            if (Actions.Ricochet.ShouldUse(out act, mustUse: true)) return true;
        }
        //虹吸弹
        if (Actions.GaussRound.ShouldUse(out act, mustUse: true)) return true;

        act = null!;
        return false;
    }

    private bool CanUseWildfire(out IAction act)
    {
        if (Actions.Wildfire.ShouldUse(out act))
        {
            //小怪AOE期间不打野火
            if (Actions.SpreadShot.ShouldUse(out _) || !Target.IsBoss()) return false;
            if (!Target.IsBoss() && IsTargetDying) return false;

            //机工起手判断
            if (!initFinished && MCH_Opener) return false;

            //在过热时
            if (JobGauge.IsOverheated) return true;

            if (!IsLastWeaponSkill(Actions.ChainSaw.ID)
            && (IsLastWeaponSkill(Actions.Drill.ID) || IsLastWeaponSkill(true, Actions.AirAnchor) || IsLastWeaponSkill(Actions.HeatBlast.ID))) return false;

            return true;
        }
        return false;
    }
    
    private bool CanUseHypercharge(out IAction act)
    {
        if (Actions.Hypercharge.ShouldUse(out act))
        {
            //小怪快死了不释放
            if (!Target.IsBoss() && IsTargetDying) return false;

            //有野火buff必须释放超荷
            if (LocalPlayer.HaveStatus(ObjectStatus.Wildfire)) return true;

            //在三大金刚还剩8秒冷却好时不释放超荷
            if (Level >= Actions.Drill.Level && Actions.Drill.RecastTimeRemain < 8) return false;
            if (Level >= Actions.AirAnchor.Level && Actions.AirAnchor.RecastTimeRemain < 8) return false;
            if (Level >= Actions.ChainSaw.Level && Actions.ChainSaw.RecastTimeRemain < 8) return false;

            //小怪AOE或者自嗨期间超荷判断
            if ((Actions.SpreadShot.ShouldUse(out _) || !Target.IsBoss()) && IsMoving) return false;
            if (((Actions.SpreadShot.ShouldUse(out _) || !Target.IsBoss()) && !IsMoving) || Level < Actions.Wildfire.Level) return true;

            uint wfTimer = 6;
            var wildfireCDTime = Actions.Wildfire.RecastTimeRemain;
            if (Level < Actions.BarrelStabilizer.Level) wfTimer = 12;

            //标准循环起手判断
            if (!initFinished && MCH_Opener) return false;

            //野火前攒热量
            if (15 < wildfireCDTime && wildfireCDTime < 43)
            {
                //如果期间热量溢出超过5,就释放一次超荷
                if (IsLastWeaponSkill(Actions.Drill.ID) && JobGauge.Heat >= 85) return true;
                return false;
            }

            //超荷释放判断
            if (wildfireCDTime >= wfTimer
            || IsLastWeaponSkill(Actions.ChainSaw.ID)
            || (!IsLastWeaponSkill(Actions.ChainSaw.ID) && (!Actions.Wildfire.IsCoolDown || wildfireCDTime <= 1))) return true;
         
        }
        return false;
    }

    private bool CanUseRookAutoturret(out IAction act)
    {
        if (Actions.RookAutoturret.ShouldUse(out act, mustUse: true))
        {
            //电量等于100,强制释放
            if (JobGauge.Battery == 100) return true;

            //小怪快死了不释放
            if (!Target.IsBoss() && IsTargetDying) return false;

            //自嗨与小怪AOE判断
            if (!MCH_Automaton || (!Target.IsBoss() && !IsMoving) || Level < Actions.Wildfire.ID) return true;
            if ((Actions.SpreadShot.ShouldUse(out _) || !Target.IsBoss()) && IsMoving) return false;

            //起手判断
            if (!initFinished && MCH_Opener) return false;

            //机器人吃团辅判断
            if (Actions.AirAnchor.RecastTimeRemain < 5 && JobGauge.Battery > 80) return true;
            if (Actions.ChainSaw.RecastTimeRemain < 5 || (Actions.ChainSaw.RecastTimeRemain > 55 && JobGauge.Battery <= 60)) return true;

        }
        return false;
    }
}
