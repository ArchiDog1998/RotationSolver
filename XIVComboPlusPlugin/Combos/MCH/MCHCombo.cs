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
            Reassemble = new BaseAction(2876)
            {
                OtherCheck = () => HaveTargetAngle,
            },

            //超荷
            Hypercharge = new BaseAction(17209)
            {
                OtherCheck = () =>
                {
                    //热量不足，怎么超荷？
                    if (JobGauge.Heat < 50) return false;

                    byte level = Service.ClientState.LocalPlayer.Level;
                    if (level >= Drill.Level && Drill.CoolDown.CooldownRemaining < 8) return false;
                    if(level >= AirAnchor.Level)
                    {
                        if(AirAnchor.CoolDown.CooldownRemaining < 8) return false;
                    }
                    else
                    {
                        if (level >= HotShow.Level && HotShow.CoolDown.CooldownRemaining < 8) return false;
                    }
                    if (level >= ChainSaw.Level && ChainSaw.CoolDown.CooldownRemaining < 8) return false;

                    //两个能力技满了，暂时不超荷
                    if (level >= GaussRound.Level && !GaussRound.CoolDown.IsCooldown) return false;
                    if (level >= Ricochet.Level && !Ricochet.CoolDown.IsCooldown) return false;
                    return true;
                },
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
            };
    }

    protected bool CanAddAbility(byte level, out uint action)
    {
        action = 0;

        if (CanInsertAbility)
        {
            //超荷
            if (Actions.Hypercharge.TryUseAction(level, out _))
            {
                //野火
                if (Actions.Wildfire.TryUseAction(level, out action)) return true;
                action = Actions.Hypercharge.ActionID;
                if (WeaponRemain <= 1) return true;
            }

            //两个能力技都还在冷却
            if (Actions.GaussRound.CoolDown.IsCooldown && Actions.Ricochet.CoolDown.IsCooldown)
            {
                //车式浮空炮塔
                if (Actions.RookAutoturret.TryUseAction(level, out action)) return true;

                //枪管加热
                if (Actions.BarrelStabilizer.TryUseAction(level, out action)) return true;
            }


            if (!Actions.Ricochet.CoolDown.IsCooldown || Actions.GaussRound.CoolDown.CooldownRemaining > Actions.Ricochet.CoolDown.CooldownRemaining)
            {
                //弹射
                if (Actions.Ricochet.TryUseAction(level, out action, mustUse:true)) return true;
            }
                //虹吸弹
                if (Actions.GaussRound.TryUseAction(level, out action, mustUse: true)) return true;

            //伤腿
            if (GeneralActions.FootGraze.TryUseAction(level, out action)) return true;

            //伤足
            if (GeneralActions.LegGraze.TryUseAction(level, out action)) return true;

            //内丹
            if (GeneralActions.SecondWind.TryUseAction(level, out action)) return true;
        }
        return false;
    }

}
