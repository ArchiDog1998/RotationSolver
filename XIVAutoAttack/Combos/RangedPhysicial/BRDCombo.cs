using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal class BRDCombo : CustomComboJob<BRDGauge>
{
    //看看现在有没有开猛者强击
    protected static bool IsBreaking => BaseAction.HaveStatusSelfFromSelf(125);

    internal override uint JobID => 23;

    protected override bool CanHealSingleAbility => false;
    internal struct Actions
    {
        private static bool AddOnDot(BattleChara b, ushort status1, ushort status2)
        {
            var results = BaseAction.FindStatusFromSelf(b, status1, status2);
            if (results.Length != 2) return false;
            return results.Min() < 6f;
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
                OtherCheck = b =>
                {
                    bool needLow = AddOnDot(b, ObjectStatus.VenomousBite, ObjectStatus.Windbite);
                    bool needHigh = AddOnDot(b, ObjectStatus.CausticBite, ObjectStatus.Stormbite);
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
                OtherCheck = b =>
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
                OtherCheck = b => TargetHelper.WeakenPeople.Length > 0,
            },

            //战斗之声
            BattleVoice = new BaseAction(118, true),

            //大地神的抒情恋歌
            NaturesMinne = new BaseAction(7408),

            //侧风诱导箭
            Sidewinder = new BaseAction(3562),

            //绝峰箭
            ApexArrow = new BaseAction(16496),



            //光明神的最终乐章
            RadiantFinale = new BaseAction(25785, true),

            //行吟
            Troubadour = new BaseAction(7405, true)
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
        //行吟
        if (Actions.Troubadour.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility( byte abilityRemain, out BaseAction act)
    {
        //大地神的抒情恋歌
        if (Actions.NaturesMinne.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //放大招！
        if (JobGauge.SoulVoice == 100 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.BlastArrowReady))
        {
            if (Actions.ApexArrow.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //群体GCD
        if (Actions.Shadowbite.ShouldUseAction(out act)) return true;
        if (Actions.QuickNock.ShouldUseAction(out act)) return true;

        //直线射击
        if (Actions.StraitShoot.ShouldUseAction(out act)) return true;

        //上毒
        if (Actions.IronJaws.ShouldUseAction(out act)) return true;
        if (Actions.VenomousBite.ShouldUseAction(out act)) return true;
        if (Actions.Windbite.ShouldUseAction(out act)) return true;

        //强力射击
        if (Actions.HeavyShoot.ShouldUseAction(out act)) return true;


        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //如果接下来要上毒或者要直线射击，那算了。
        if (nextGCD.ActionID == Actions.StraitShoot.ActionID || nextGCD.ActionID == Actions.VenomousBite.ActionID ||
            nextGCD.ActionID == Actions.Windbite.ActionID || nextGCD.ActionID == Actions.IronJaws.ActionID)
        {
            act = null;
            return false;
        }
        else if(abilityRemain != 0 && 
            (Service.ClientState.LocalPlayer.Level < Actions.RagingStrikes.Level ||  BaseAction.HaveStatusSelfFromSelf(ObjectStatus.RagingStrikes)) &&
            (Service.ClientState.LocalPlayer.Level < Actions.BattleVoice.Level || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.BattleVoice)))
        {
            //纷乱箭
            if (Actions.Barrage.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        if(JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.NONE || Service.ClientState.LocalPlayer.Level < Actions.MagesBallad.Level)
        {
            act = null;
            return false;
        }

        //猛者强击
        if (Actions.RagingStrikes.ShouldUseAction(out act)) return true;

        //光明神只有在战斗之声释放之前0.6s内，才会释放。
        if (Actions.BattleVoice.RecastTimeRemain < 0.6)
        {
            //光明神的最终乐章
            if (Actions.RadiantFinale.ShouldUseAction(out act, mustUse: true)) return true;

            //战斗之声
            if (Actions.BattleVoice.ShouldUseAction(out act, mustUse: true)) return true;
        }
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        //放浪神的小步舞曲
        if (JobGauge.SongTimer < 3000 && Actions.WanderersMinuet.ShouldUseAction(out act)) return true;

        //完美音调
        if (Actions.PitchPerfect.ShouldUseAction(out act)) return true;

        //贤者的叙事谣
        if (JobGauge.SongTimer < 3000 && Actions.MagesBallad.ShouldUseAction(out act)) return true;

        //军神的赞美歌
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.MAGE
            || JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.NONE) && Actions.ArmysPaeon.ShouldUseAction(out act)) return true;


        //九天连箭
        if (Actions.EmpyrealArrow.ShouldUseAction(out act)) return true;

        //测风诱导箭
        if (Actions.Sidewinder.ShouldUseAction(out act)) return true;

        byte level = Service.ClientState.LocalPlayer.Level;

        //死亡剑雨
        if (Actions.RainofDeath.ShouldUseAction(out act, emptyOrSkipCombo: level == 90 ? IsBreaking : true)) return true;

        //失血箭
        if (Actions.Bloodletter.ShouldUseAction(out act, emptyOrSkipCombo: level == 90 ? IsBreaking : true)) return true;

        //光阴神的礼赞凯歌 减少Debuff
        if (Actions.WardensPaean.ShouldUseAction(out act)) return true;

        return false;
    }

}
