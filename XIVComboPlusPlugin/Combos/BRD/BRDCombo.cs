using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal abstract class BRDCombo : CustomComboJob<BRDGauge>
{
    //看看现在有没有开猛者强击
    protected static bool IsBreaking => BaseAction.HaveStatusSelfFromSelf(125);

    internal struct Actions
    {
        private static bool AddOnDot(ushort status1, ushort status2)
        {
            var results = BaseAction.FindStatusTargetFromSelf(status1, status2);
            if(results.Length != 2) return false;
            return results.Min() < 5.5f;
        }

        public static readonly BaseAction
            //强力射击
            HeavyShoot = new BaseAction(97u) { BuffsProvide = new ushort[] { ObjectStatus.StraightShotReady } },

            //直线射击
            StraitShoot = new BaseAction(98u) { BuffsNeed = new ushort[] { ObjectStatus.StraightShotReady } },

            //猛者强击
            RagingStrikes = new BaseAction(101),

            //毒药箭
            VenomousBite = new BaseAction(100) { TargetStatus = new ushort[] { ObjectStatus.VenomousBite, ObjectStatus.CausticBite } },

            //风蚀箭
            Windbite = new BaseAction(113) { TargetStatus = new ushort[] { ObjectStatus.Windbite, ObjectStatus.Stormbite } },

            //伶牙俐齿
            IronJaws = new BaseAction(3560)
            {
                OtherCheck = () =>
                {
                    bool needLow = AddOnDot(ObjectStatus.VenomousBite, ObjectStatus.Windbite);
                    bool needHigh = AddOnDot(ObjectStatus.CausticBite, ObjectStatus.Stormbite);
                    return needLow || needHigh;
                },
            },

            //纷乱箭
            Barrage = new BaseAction(107)
            {
                BuffsProvide = new ushort[] { ObjectStatus.StraightShotReady },
            },

            //九天连箭
            EmpyrealArrow = new BaseAction(3558),

            //失血箭
            Bloodletter = new BaseAction(110),

            //死亡箭雨
            RainofDeath = new BaseAction(117),

            //连珠箭
            QuickNock = new BaseAction(106) { BuffsProvide = new ushort[] { ObjectStatus.ShadowbiteReady } },

            //影噬箭
            Shadowbite = new BaseAction(16494) { BuffsNeed = new ushort[] { ObjectStatus.ShadowbiteReady } },

            //贤者的叙事谣
            MagesBallad = new BaseAction(114),

            //军神的赞美歌
            ArmysPaeon = new BaseAction(116),

            //放浪神的小步舞曲
            WanderersMinuet = new BaseAction(3559),

            //完美音调
            PitchPerfect = new BaseAction(7404)
            {
                OtherCheck = () =>
            {
                if (JobGauge.Song != Dalamud.Game.ClientState.JobGauge.Enums.Song.WANDERER) return false;
                if (JobGauge.SongTimer < 3000 && JobGauge.Repertoire > 0) return true;
                if (JobGauge.Repertoire == 3) return true;
                return false;
            }
            },

            //光阴神的礼赞凯歌
            WardensPaean = new BaseAction(3561)
            {
                OtherCheck = () => TargetHelper.WeakenPeople.Length > 0,
            },

            //战斗之声
            BattleVoice = new BaseAction(118),

            //大地神的抒情恋歌
            NaturesMinne = new BaseAction(7408),

            //侧风诱导箭
            Sidewinder = new BaseAction(3562),

            //绝峰箭
            ApexArrow = new BaseAction(16496),



            //光明神的最终乐章
            RadiantFinale = new BaseAction(25785)
            {
                OtherCheck = () =>
                {
                    return JobGauge.Coda.Length > 2 || MagesBallad.CoolDown.CooldownRemaining < 0.1;
                },
            },

            //行吟
            Troubadour = new BaseAction(7405)
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

    private protected override bool HealSingleAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //大地神的抒情恋歌
        if (Actions.NaturesMinne.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //放大招！
        if (JobGauge.SoulVoice >= 80 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.BlastArrowReady))
        {
            if (Actions.ApexArrow.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //群体GCD
        if (Actions.Shadowbite.TryUseAction(level, out act)) return true;
        if (Actions.QuickNock.TryUseAction(level, out act)) return true;

        //直线射击
        if (Actions.StraitShoot.TryUseAction(level, out act)) return true;

        //上毒
        if (Actions.IronJaws.TryUseAction(level, out act)) return true;
        if (Actions.VenomousBite.TryUseAction(level, out act)) return true;
        if (Actions.Windbite.TryUseAction(level, out act)) return true;

        //强力射击
        if (Actions.HeavyShoot.TryUseAction(level, out act)) return true;

        
        return false;
    }

    private protected override bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        //如果接下来要上毒或者要直线射击，那算了。
        if(nextGCD.ActionID == Actions.StraitShoot.ActionID || nextGCD.ActionID == Actions.VenomousBite.ActionID ||
            nextGCD.ActionID == Actions.Windbite.ActionID || nextGCD.ActionID == Actions.IronJaws.ActionID)
        {
            act= null;
            return false;
        }
        else
        {
            //纷乱箭
            if (Actions.Barrage.TryUseAction(level, out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //放浪神的小步舞曲
        if (Actions.WanderersMinuet.TryUseAction(level, out act)) return true;

        //完美音调
        if (Actions.PitchPerfect.TryUseAction(level, out act)) return true;

        //贤者的叙事谣
        if (JobGauge.SongTimer < 3000 && Actions.MagesBallad.TryUseAction(level, out act)) return true;

        //军神的赞美歌
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.MAGE
            || JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.NONE) && Actions.ArmysPaeon.TryUseAction(level, out act)) return true;

        //猛者强击
        if (Actions.RagingStrikes.TryUseAction(level, out act)) return true;

        //战斗之声
        if (Actions.BattleVoice.TryUseAction(level, out act, mustUse: true)) return true;

        //光明神的最终乐章
        if (Actions.RadiantFinale.TryUseAction(level, out act)) return true;

        //九天连箭
        if (Actions.EmpyrealArrow.TryUseAction(level, out act)) return true;

        //测风诱导箭
        if (Actions.Sidewinder.TryUseAction(level, out act)) return true;

        //死亡剑雨
        if (Actions.RainofDeath.TryUseAction(level, out act, Empty: level == 90 ? IsBreaking : true)) return true;

        //失血箭
        if (Actions.Bloodletter.TryUseAction(level, out act, Empty: level == 90 ? IsBreaking : true)) return true;

        //光阴神的礼赞凯歌 减少Debuff
        if (Actions.WardensPaean.TryUseAction(level, out act)) return true;

        //伤腿
        if (GeneralActions.FootGraze.TryUseAction(level, out act)) return true;

        //伤足
        if (GeneralActions.LegGraze.TryUseAction(level, out act)) return true;

        //内丹
        if (GeneralActions.SecondWind.TryUseAction(level, out act)) return true;

        return false;
    }

}
