using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal abstract class MCHCombo : CustomComboJob<MCHGauge>
{
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
                OtherCheck = () => JobGauge.Heat >= 50,
            },

            //野火
            Wildfire = new BaseAction(2878),

            //虹吸弹
            GaussRound = new BaseAction(2874),

            //弹射
            Ricochet = new BaseAction(2890),

            //枪管加热
            BarrelStabilizer = new BaseAction(7414)
            {
                OtherCheck = () => JobGauge.Heat <= 50,
            },

            //车式浮空炮塔
            RookAutoturret = new BaseAction(2864)
            {
                OtherCheck = () => JobGauge.Battery >= 50,
            },

            //策动
            Tactician = new BaseAction(16889)
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

    private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        // 四个牛逼的技能。
        if (Actions.Bioblaster.TryUseAction(level, out act)) return true;
        if (Actions.Drill.TryUseAction(level, out act)) return true;
        if (Actions.AirAnchor.TryUseAction(level, out _) && Actions.HotShow.TryUseAction(level, out act)) return true;
        if (Actions.ChainSaw.TryUseAction(level, out act)) return true;

        //群体常规GCD
        if (JobGauge.IsOverheated && Actions.AutoCrossbow.TryUseAction(level, out act)) return true;
        if (Actions.SpreadShot.TryUseAction(level, out act)) return true;

        //单体常规GCD
        if (JobGauge.IsOverheated && Actions.HeatBlast.TryUseAction(level, out act)) return true;
        if (Actions.CleanShot.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.SlugShot.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.SplitShot.TryUseAction(level, out act, lastComboActionID)) return true;

        return false;
    }

    private protected override bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        //如果接下来要搞三大金刚了，整备吧！
        if(nextGCD.ActionID == Actions.HotShow.ActionID || nextGCD.ActionID == Actions.Drill.ActionID || nextGCD.ActionID == Actions.ChainSaw.ActionID)
        {
            if (Actions.Reassemble.TryUseAction(level, out act, mustUse: true)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        float time = WeaponRemain + 8;
        
        if((level < Actions.HotShow.Level || Actions.HotShow.CoolDown.CooldownRemaining > time
            || Actions.AirAnchor.CoolDown.CooldownRemaining > time) &&
            (level < Actions.Drill.Level || Actions.Drill.CoolDown.CooldownRemaining > time) &&
            (level < Actions.ChainSaw.Level || Actions.ChainSaw.CoolDown.CooldownRemaining > time))
        {
            if(abilityRemain == 1 && Actions.Hypercharge.TryUseAction(level, out act)) return true;
            if(abilityRemain > 1 && Actions.Wildfire.TryUseAction(level, out act)) return true;
        }

        //两个能力技都还在冷却
        if (Actions.GaussRound.CoolDown.IsCooldown && Actions.Ricochet.CoolDown.IsCooldown)
        {
            //车式浮空炮塔
            if (Actions.RookAutoturret.TryUseAction(level, out act)) return true;

            //枪管加热
            if (Actions.BarrelStabilizer.TryUseAction(level, out act)) return true;
        }


        if (!Actions.Ricochet.CoolDown.IsCooldown || Actions.GaussRound.CoolDown.CooldownRemaining > Actions.Ricochet.CoolDown.CooldownRemaining)
        {
            //弹射
            if (Actions.Ricochet.TryUseAction(level, out act, mustUse: true)) return true;
        }
        //虹吸弹
        if (Actions.GaussRound.TryUseAction(level, out act, mustUse: true)) return true;

        //伤腿
        if (GeneralActions.FootGraze.TryUseAction(level, out act)) return true;

        //伤足
        if (GeneralActions.LegGraze.TryUseAction(level, out act)) return true;

        //内丹
        if (GeneralActions.SecondWind.TryUseAction(level, out act)) return true;

        return false;
    }

}
