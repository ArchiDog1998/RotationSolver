using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class MCHCombo : CustomComboJob<MCHGauge>
{
    internal override uint JobID => 31;

    internal struct Actions
    {
        public static readonly BaseAction
            //分裂弹
            SplitShot = new BaseAction(2866),

            //独头弹
            SlugShot = new BaseAction(2868)
            {
                OtherIDsCombo = new uint[] { 7411 },
            },

            //狙击弹
            CleanShot = new BaseAction(2873)
            {
                OtherIDsCombo = new uint[] { 7412 },
            },

            //热冲击
            HeatBlast = new BaseAction(7410),

            //散射
            SpreadShot = new BaseAction(2870),

            //自动弩
            AutoCrossbow = new BaseAction(16497),

            //热弹
            HotShow = new BaseAction(2872),

            //空气锚
            AirAnchor = new BaseAction(16500),

            //钻头
            Drill = new BaseAction(16498),

            //回转飞锯
            ChainSaw = new BaseAction(25788),

            //毒菌冲击
            Bioblaster = new BaseAction(16499),

            //整备
            Reassemble = new BaseAction(2876),

            //超荷
            Hypercharge = new BaseAction(17209)
            {
                OtherCheck = b => JobGauge.Heat >= 50,
            },

            //野火
            Wildfire = new BaseAction(2878),

            //虹吸弹
            GaussRound = new BaseAction(2874),

            //弹射
            Ricochet = new BaseAction(2890),

            //枪管加热
            BarrelStabilizer = new BaseAction(7414),

            //车式浮空炮塔
            RookAutoturret = new BaseAction(2864)
            {
                OtherCheck = b => JobGauge.Battery >= 50,
            },

            //策动
            Tactician = new BaseAction(16889, true)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
                },
            };
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //策动
        if (Actions.Tactician.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //四个牛逼的技能。
        if (Actions.Bioblaster.ShouldUseAction(out act)) return true;
        if (Actions.Drill.ShouldUseAction(out act)) return true;
        if (Actions.AirAnchor.ShouldUseAction(out act) ||
            (Service.ClientState.LocalPlayer.Level < Actions.AirAnchor.Level && Actions.HotShow.ShouldUseAction(out _))) return true;
        if (Actions.ChainSaw.ShouldUseAction(out act, mustUse:true)) return true;

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

    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if(base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //如果接下来要搞三大金刚了，整备吧！
        if(nextGCD.ActionID == Actions.HotShow.ActionID || nextGCD.ActionID == Actions.Drill.ActionID || nextGCD.ActionID == Actions.ChainSaw.ActionID)
        {
            if (Actions.Reassemble.ShouldUseAction(out act, mustUse: true)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        float time = 10;
        
        byte level = Service.ClientState.LocalPlayer.Level;

        if(JobGauge.Heat >= 50 && (level < Actions.HotShow.Level || Actions.HotShow.RecastTimeRemain > time
            || Actions.AirAnchor.RecastTimeRemain > time) &&
            (level < Actions.Drill.Level || Actions.Drill.RecastTimeRemain > time) &&
            (level < Actions.ChainSaw.Level || Actions.ChainSaw.RecastTimeRemain > time))
        {
            if(abilityRemain == 1 && Actions.Hypercharge.ShouldUseAction(out act)) return true;
            if(abilityRemain > 1 && Actions.Wildfire.ShouldUseAction(out act)) return true;
        }

        //两个能力技都还在冷却
        if (Actions.GaussRound.IsCoolDown && Actions.Ricochet.IsCoolDown)
        {
            //车式浮空炮塔
            if (Actions.RookAutoturret.ShouldUseAction(out act, mustUse:true)) return true;

            //枪管加热
            if (Actions.BarrelStabilizer.ShouldUseAction(out act)) return true;
        }


        if (!Actions.Ricochet.IsCoolDown || Actions.GaussRound.RecastTimeRemain > Actions.Ricochet.RecastTimeRemain)
        {
            //弹射
            if (Actions.Ricochet.ShouldUseAction(out act, mustUse: true)) return true;
        }
        //虹吸弹
        if (Actions.GaussRound.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

}
