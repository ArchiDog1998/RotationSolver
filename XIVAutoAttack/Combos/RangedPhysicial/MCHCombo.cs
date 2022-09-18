using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack;
using XIVAutoAttack.Combos;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal class MCHCombo : JobGaugeCombo<MCHGauge>
{
    internal override uint JobID => 31;

    internal struct Actions
    {
        public static readonly BaseAction
            //分裂弹
            SplitShot = new (2866),

            //独头弹
            SlugShot = new (2868)
            {
                OtherIDsCombo = new [] { 7411u },
            },

            //狙击弹
            CleanShot = new (2873)
            {
                OtherIDsCombo = new [] { 7412u },
            },

            //热冲击
            HeatBlast = new (7410),

            //散射
            SpreadShot = new (2870),

            //自动弩
            AutoCrossbow = new (16497),

            //热弹
            HotShow = new (2872),

            //空气锚
            AirAnchor = new (16500),

            //钻头
            Drill = new (16498),

            //回转飞锯
            ChainSaw = new (25788),

            //毒菌冲击
            Bioblaster = new (16499),

            //整备
            Reassemble = new (2876),

            //超荷
            Hypercharge = new (17209)
            {
                OtherCheck = b => JobGauge.Heat >= 50,
            },

            //野火
            Wildfire = new (2878),

            //虹吸弹
            GaussRound = new (2874),

            //弹射
            Ricochet = new (2890),

            //枪管加热
            BarrelStabilizer = new (7414),

            //车式浮空炮塔
            RookAutoturret = new (2864)
            {
                OtherCheck = b => JobGauge.Battery >= 50,
            },

            //策动
            Tactician = new (16889, true)
            {
                BuffsProvide = new []
                {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
                },
            };
    }
    internal override SortedList<DescType, string> Description => new ()
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
        //四个牛逼的技能。
        if (Actions.Bioblaster.ShouldUseAction(out act)) return true;
        if (Actions.Drill.ShouldUseAction(out act)) return true;
        if (Actions.AirAnchor.ShouldUseAction(out act)) return true;
        else if(Service.ClientState.LocalPlayer.Level < Actions.AirAnchor.Level && Actions.HotShow.ShouldUseAction(out act)) return true;
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
        //如果接下来要搞三大金刚了，整备吧！
        if (nextGCD.ID == Actions.AirAnchor.ID || nextGCD.ID == Actions.Drill.ID || nextGCD.ID == Actions.ChainSaw.ID)
        {
            if (abilityRemain == 1 && Actions.Reassemble.ShouldUseAction(out act, mustUse:true)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Wildfire))
        {
            if (Actions.Hypercharge.ShouldUseAction(out act)) return true;
        }

        float time = 10;
        byte level = Service.ClientState.LocalPlayer.Level;

        if (JobGauge.Heat >= 50 && (level < Actions.HotShow.Level || Actions.HotShow.RecastTimeRemain > time
            || Actions.AirAnchor.RecastTimeRemain > time) &&
            (level < Actions.Drill.Level || Actions.Drill.RecastTimeRemain > time) &&
            (level < Actions.ChainSaw.Level || Actions.ChainSaw.RecastTimeRemain > time)
            && abilityRemain == 1)
        {
            if (Actions.Wildfire.ShouldUseAction(out act)) return true;
            if (Actions.Hypercharge.ShouldUseAction(out act)) return true;
        }


        //两个能力技都还在冷却
        if (Actions.GaussRound.RecastTimeRemain > 0 && Actions.Ricochet.RecastTimeRemain > 0)
        {
            //车式浮空炮塔
            if (Actions.RookAutoturret.ShouldUseAction(out act, mustUse: true)) return true;

            //枪管加热
            if (Actions.BarrelStabilizer.ShouldUseAction(out act)) return true;
        }

        if (Actions.Ricochet.RecastTimeRemain == 0 && Actions.Ricochet.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.GaussRound.RecastTimeRemain == 0 && Actions.GaussRound.ShouldUseAction(out act, mustUse: true)) return true;

        if (Actions.GaussRound.RecastTimeRemain > Actions.Ricochet.RecastTimeRemain)
        {
            //弹射
            if (Actions.Ricochet.ShouldUseAction(out act, mustUse: true)) return true;
        }
        //虹吸弹
        if (Actions.GaussRound.ShouldUseAction(out act, mustUse: true)) return true;

        act = null;
        return false;
    }
}
