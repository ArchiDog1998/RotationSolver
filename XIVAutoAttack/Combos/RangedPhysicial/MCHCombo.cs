using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.RangedPhysicial;

//目前90级循环差不多完善了,但低等级可能会出问题,出问题了了再修
internal class MCHCombo : JobGaugeCombo<MCHGauge>
{
    internal override uint JobID => 31;
    internal static byte level = Service.ClientState.LocalPlayer!.Level;
    private static bool initFinished = false;
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
                OtherCheck = b => initFinished && !JobGauge.IsOverheated,
            },

            //毒菌冲击
            Bioblaster = new(16499)
            {
                //过热不释放技能
                OtherCheck = b => !JobGauge.IsOverheated,
            },

            //整备
            Reassemble = new(2876)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Reassemble },
            },

            //超荷
            Hypercharge = new(17209)
            {
                OtherCheck = b =>
                {
                    if (SpreadShot.ShouldUseAction(out _))
                    {
                        //AOE期间超荷判断
                        if (JobGauge.IsOverheated || JobGauge.Heat < 50) return false;
                    }
                    else
                    {
                        //单体期间超荷判断
                        if (!initFinished || JobGauge.IsOverheated || JobGauge.Heat < 50) return false;

                        //在野火期间释放
                        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Wildfire) || level < 45) return true;
                    }

                    //这个嵌套怪我编程习惯不好...,回头优化
                    if (level >= Drill.Level && Drill.RecastTimeRemain >= 8)
                    {
                        if (level >= AirAnchor.Level && AirAnchor.RecastTimeRemain >= 8)
                        {
                            if (level >= ChainSaw.Level && ChainSaw.RecastTimeRemain >= 8 || level < ChainSaw.Level) return true;
                        }
                        else if (level < AirAnchor.Level) return true;
                    }
                    else if (level < Drill.Level) return true;

                    return false;
                },
            },

            //野火
            Wildfire = new(2878)
            {
                OtherCheck = b =>
                {
                    // 起手是否完成,AOE期间不打野火
                    if (!initFinished || SpreadShot.ShouldUseAction(out _)) return false;

                    if (JobGauge.Heat < 50 && LastAbility != Hypercharge.ID) return false;

                    if (LastWeaponskill != ChainSaw.ID
                        && (LastWeaponskill == Drill.ID || LastWeaponskill == AirAnchor.ID || LastWeaponskill == HeatBlast.ID)) return false;

                    return true;
                },
            },

            //虹吸弹
            GaussRound = new(2874),

            //弹射
            Ricochet = new(2890),

            //枪管加热
            BarrelStabilizer = new(7414)
            {
                OtherCheck = b =>
                {
                    var wildfireCDTime = Wildfire.RecastTimeRemain;
                    //枪管加热可以允许有5热量的溢出
                    if (JobGauge.Heat <= 55 && LastWeaponskill != ChainSaw.ID &&
                        (wildfireCDTime <= 9 || (wildfireCDTime >= 110 && JobGauge.IsOverheated))) return true;

                    return false;
                },
            },

            //车式浮空炮塔
            RookAutoturret = new(2864)
            {
                OtherCheck = b =>
                {
                    if (JobGauge.Battery < 50 || JobGauge.IsRobotActive || !initFinished) return false;

                    //在战斗的不同时间段使用时机不同
                    if (JobGauge.Battery == 100) return true;
                    else if (JobGauge.Battery >= 50 && (TargetHelper.CombatEngageDuration.Seconds is >= 55 or <= 05 || (TargetHelper.CombatEngageDuration.Minutes == 0 && LastWeaponskill != CleanShot.ID))) return true;
                    else if (JobGauge.Battery >= 80 && TargetHelper.CombatEngageDuration.Seconds is >= 50 or <= 05) return true;

                    return false;
                },
            },

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
        {DescType.范围防御, $"{Actions.Tactician.Action.Name}"},
    };
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //策动
        if (Actions.Tactician.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //当上一个连击是热阻击弹时完成起手
        if (lastComboActionID == Actions.CleanShot.ID)
        {
            initFinished = true;
        }

        //不在战斗中时重置起手
        if (!TargetHelper.InBattle)
        {
            //开场前整备,空气锚和钻头必须冷却好
            if ((!Actions.AirAnchor.IsCoolDown || !Actions.Drill.IsCoolDown) && Actions.Reassemble.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
            initFinished = false;
        }

        //AOE,毒菌冲击
        if (Actions.Bioblaster.ShouldUseAction(out act)) return true;
        //单体,四个牛逼的技能。先空气锚再钻头
        if (Actions.AirAnchor.ShouldUseAction(out act)) return true;
        else if (level < Actions.AirAnchor.Level && Actions.HotShow.ShouldUseAction(out act)) return true;
        if (Actions.Drill.ShouldUseAction(out act)) return true;
        if (Actions.ChainSaw.ShouldUseAction(out act, mustUse: true)) return true;

        //群体常规GCD
        if (JobGauge.IsOverheated && Actions.AutoCrossbow.ShouldUseAction(out act)) return true;
        if (Actions.SpreadShot.ShouldUseAction(out act)) return true;

        //单体常规GCD
        if (JobGauge.IsOverheated && Actions.HeatBlast.ShouldUseAction(out act)) return true;
        if (Actions.CleanShot.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SlugShot.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SplitShot.ShouldUseAction(out act, lastComboActionID)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //等级小于钻头时,绑定狙击弹
        if (level < Actions.Drill.Level && nextGCD.ID == Actions.CleanShot.ID)
        {
            if (Actions.Reassemble.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }
        //等级小于90时,整备不再留层数,直接全部给空气锚或者钻头
        if (level < Actions.ChainSaw.Level && (nextGCD.ID == Actions.AirAnchor.ID || nextGCD.ID == Actions.Drill.ID))
        {
            if (Actions.Reassemble.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }
        //整备优先链锯
        if (nextGCD.ID == Actions.ChainSaw.ID)
        {
            if (Actions.Reassemble.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }
        //如果接下来要搞三大金刚了，整备吧！
        if (nextGCD.ID == Actions.AirAnchor.ID || nextGCD.ID == Actions.Drill.ID)
        {
            if (Actions.Reassemble.ShouldUseAction(out act)) return true;
        }
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {

        //起手虹吸弹、弹射
        if (Actions.Ricochet.RecastTimeRemain == 0 && Actions.Ricochet.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.GaussRound.RecastTimeRemain == 0 && Actions.GaussRound.ShouldUseAction(out act, mustUse: true)) return true;

        //枪管加热
        if (Actions.BarrelStabilizer.ShouldUseAction(out act)) return true;

        //野火,GCD后半段使用
        if (abilityRemain == 1 && Actions.Wildfire.ShouldUseAction(out act)) return true;

        //车式浮空炮塔
        if (Actions.RookAutoturret.ShouldUseAction(out act, mustUse: true)) return true;

        var wildfireCDTime = Actions.Wildfire.RecastTimeRemain;
        //超荷
        if (UseHypercharge(JobGauge, wildfireCDTime) && Actions.Hypercharge.ShouldUseAction(out act)) return true;


        if (Actions.GaussRound.RecastTimeRemain > Actions.Ricochet.RecastTimeRemain)
        {
            //弹射
            if (Actions.Ricochet.ShouldUseAction(out act, mustUse: true)) return true;
        }
        //虹吸弹
        if (Actions.GaussRound.ShouldUseAction(out act, mustUse: true)) return true;

        act = null!;
        return false;
    }

    // 先抄一下sc,回头优化
    private static bool UseHypercharge(MCHGauge gauge, float wildfireCDTime)
    {
        uint wfTimer = 6; //默认计时器
        if (level < Actions.BarrelStabilizer.Level) wfTimer = 12;

        // 我真的不记得为什么我把 > 70 放在这里，我担心如果我把它拿掉它会打破它哈哈
        if (TargetHelper.CombatEngageDuration.Minutes == 0 && (gauge.Heat > 70 || TargetHelper.CombatEngageDuration.Seconds <= 30) && LastWeaponskill != Service.IconReplacer.OriginalHook(Actions.CleanShot.ID)) return true;

        if (TargetHelper.CombatEngageDuration.Minutes > 0 && (wildfireCDTime >= wfTimer || LastAbility == Actions.Wildfire.ID) || LastWeaponskill == Actions.ChainSaw.ID && (!Actions.Wildfire.IsCoolDown || wildfireCDTime < 1))
        {
            if (TargetHelper.CombatEngageDuration.Minutes % 2 == 1 && gauge.Heat >= 90) return true;

            if (TargetHelper.CombatEngageDuration.Minutes % 2 == 0) return true;
        }

        return false;
    }
}
