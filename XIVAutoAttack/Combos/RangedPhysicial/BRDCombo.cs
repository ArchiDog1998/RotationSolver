using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal class BRDCombo : JobGaugeCombo<BRDGauge>
{

    internal override uint JobID => 23;
    private static bool initFinished = false;
    private static float RagingStrikes1GCDDelayTime = 0;
    private static float RadiantFinaleCurrent0GCDRemain = 0;
    private static DateTime RagingStrikesNowTime;

    internal struct Actions
    {
        private static bool AddOnDot(BattleChara b, ushort status1, ushort status2, float duration)
        {
            var results = StatusHelper.FindStatusTimes(b, status1, status2);
            if (results.Length != 2) return false;
            return results.Min() < duration;
        }

        public static readonly BaseAction
            //强力射击
            HeavyShoot = new(97) { BuffsProvide = new[] { ObjectStatus.StraightShotReady } },

            //直线射击
            StraitShoot = new(98) { BuffsNeed = new[] { ObjectStatus.StraightShotReady } },

            //毒药箭
            VenomousBite = new(100, isDot: true) { TargetStatus = new[] { ObjectStatus.VenomousBite, ObjectStatus.CausticBite } },

            //风蚀箭
            Windbite = new(113, isDot: true) { TargetStatus = new[] { ObjectStatus.Windbite, ObjectStatus.Stormbite } },

            //伶牙俐齿
            IronJaws = new(3560, isDot: true)
            {
                OtherCheck = b =>
                {
                    bool needLow = AddOnDot(b, ObjectStatus.VenomousBite, ObjectStatus.Windbite, 4);
                    bool needHigh = AddOnDot(b, ObjectStatus.CausticBite, ObjectStatus.Stormbite, 4);

                    bool needLow1 = AddOnDot(b, ObjectStatus.VenomousBite, ObjectStatus.Windbite, 40);
                    bool needHigh1 = AddOnDot(b, ObjectStatus.CausticBite, ObjectStatus.Stormbite, 40);

                    if (LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes) && 
                        LocalPlayer.FindStatusTime(ObjectStatus.RagingStrikes) < 4 
                        && (needLow1 || needHigh1)) return true;

                    return needLow || needHigh;
                },
            },

            //贤者的叙事谣
            MagesBallad = new(114),

            //军神的赞美歌
            ArmysPaeon = new(116),

            //放浪神的小步舞曲
            WanderersMinuet = new(3559),

            //战斗之声
            BattleVoice = new(118, true)
            {
                OtherCheck = b =>
                {
                    if (IsLastAbility(true, RadiantFinale)) return true;

                    return false;
                },
            },

            //猛者强击
            RagingStrikes = new(101)
            {
                OtherCheck = b =>
                {
                    if (JobGauge.Song == Song.WANDERER || Level < WanderersMinuet.Level && BattleVoice.RecastTimeRemain <= 5.38f 
                        || Level < BattleVoice.Level) return true;

                    return false;
                },
                AfterUse = () =>
                {
                    RagingStrikes1GCDDelayTime = WeaponRemain(1);
                    RagingStrikesNowTime = DateTime.Now;
                }
            },

            //光明神的最终乐章
            RadiantFinale = new(25785, true)
            {
                OtherCheck = b =>
                {
                    var canUse = DateTime.Now - RagingStrikesNowTime >= new TimeSpan(0, 0, 0, 0, (int)(RagingStrikes1GCDDelayTime * 1000));
                    static bool SongIsNotNone(Song value) => value != Song.NONE;
                    static bool SongIsWandererMinuet(Song value) => value == Song.WANDERER;
                    if ((Array.TrueForAll(JobGauge.Coda, SongIsNotNone) || Array.Exists(JobGauge.Coda, SongIsWandererMinuet))
                        && BattleVoice.RecastTimeRemain < 1f
                        && RagingStrikes.IsCoolDown 
                        && LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes)
                        && canUse) return true;

                    return false;
                },
                AfterUse = () =>
                {
                    RadiantFinaleCurrent0GCDRemain = WeaponRemain();
                }
            },

            //纷乱箭
            Barrage = new(107)
            {
                BuffsProvide = new[] { ObjectStatus.StraightShotReady },
                OtherCheck = b =>
                {
                    if (!EmpyrealArrow.IsCoolDown || EmpyrealArrow.RecastTimeRemain < 2) return false;
                    return true;
                }
            },

            //九天连箭
            EmpyrealArrow = new(3558)
            {
                OtherCheck = b =>
                {
                    if (!initFinished || (initFinished && BattleVoice.RecastTimeRemain >= WeaponRemain(1))) return true;

                    return false;
                },
            },

            //完美音调
            PitchPerfect = new(7404)
            {
                OtherCheck = b =>
                {
                    if (JobGauge.Song != Song.WANDERER) return false;

                    if (initFinished && (!initFinished || BattleVoice.RecastTimeRemain < 3.5f)) return false;

                    if (JobGauge.SongTimer < 3000 && JobGauge.Repertoire > 0) return true;
                    if (JobGauge.Repertoire == 3 || JobGauge.Repertoire == 2 && EmpyrealArrow.RecastTimeRemain < 2) return true;
                    return false;
                },
            },

            //失血箭
            Bloodletter = new(110)
            {
                OtherCheck = b =>
                {
                    if (!EmpyrealArrow.IsCoolDown || EmpyrealArrow.RecastTimeRemain < 1) return false;
                    return true;
                }
            },

            //死亡箭雨
            RainofDeath = new(117),

            //连珠箭
            QuickNock = new(106) { BuffsProvide = new[] { ObjectStatus.ShadowbiteReady } },

            //影噬箭
            Shadowbite = new(16494) { BuffsNeed = new[] { ObjectStatus.ShadowbiteReady } },

            //光阴神的礼赞凯歌
            WardensPaean = new(3561),

            //大地神的抒情恋歌
            NaturesMinne = new(7408),

            //侧风诱导箭
            Sidewinder = new(3562)
            {
                OtherCheck = b =>
                {
                    if (!initFinished || (initFinished && BattleVoice.RecastTimeRemain >= 3.5f))
                    {
                        if (LocalPlayer.HaveStatus(ObjectStatus.BattleVoice) || BattleVoice.RecastTimeRemain > 10
                        && LocalPlayer.HaveStatus(ObjectStatus.RadiantFinale) || RadiantFinale.RecastTimeRemain > 10
                        || Level < RadiantFinale.Level) return true;
                    }

                    return false;
                },
            },

            //绝峰箭
            ApexArrow = new(16496)
            {
                OtherCheck = b =>
                {
                    //快爆发了,攒着等爆发
                    if (JobGauge.SoulVoice == 100 && BattleVoice.RecastTimeRemain <= 25) return false;

                    //爆发快过了,如果手里还有绝峰箭,就把绝峰箭打出去
                    if (JobGauge.SoulVoice >= 80 && LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes) && LocalPlayer.FindStatusTime(ObjectStatus.RagingStrikes) < 10) return true;

                    if (JobGauge.SoulVoice == 100
                        && LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes)
                        && LocalPlayer.HaveStatus(ObjectStatus.BattleVoice)
                        && (LocalPlayer.HaveStatus(ObjectStatus.RadiantFinale) || Level < RadiantFinale.Level)) return true;

                    if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

                    //能量之声等于100或者在爆发箭预备状态
                    if (JobGauge.SoulVoice == 100 || LocalPlayer.HaveStatus(ObjectStatus.BlastArrowReady)) return true;

                    return false;
                },
            },

            //行吟
            Troubadour = new(7405, true)
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
        {DescType.范围防御, $"{Actions.Troubadour.Action.Name}"},
        {DescType.单体治疗, $"{Actions.NaturesMinne.Action.Name}"},
    };
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //行吟
        if (Actions.Troubadour.ShouldUse(out act)) return true;


        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //大地神的抒情恋歌
        if (Actions.NaturesMinne.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (!InBattle)
        {
            //if (UseBreakItem(out act)) return true;
            initFinished = false;
        }

        //伶牙俐齿
        if (Actions.IronJaws.ShouldUse(out act))
        {
            initFinished = true;
            return true;
        }

        //放大招！
        if (Actions.ApexArrow.ShouldUse(out act, mustUse: true)) return true;

        //群体GCD
        if (Actions.Shadowbite.ShouldUse(out act)) return true;
        if (Actions.QuickNock.ShouldUse(out act)) return true;

        //直线射击
        if (Actions.StraitShoot.ShouldUse(out act)) return true;

        //上毒
        if (Actions.VenomousBite.ShouldUse(out act)) return true;
        if (Actions.Windbite.ShouldUse(out act)) return true;

        //强力射击
        if (Actions.HeavyShoot.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //如果接下来要上毒或者要直线射击，那算了。
        if (nextGCD == Actions.StraitShoot || nextGCD == Actions.VenomousBite ||
            nextGCD == Actions.Windbite || nextGCD == Actions.IronJaws)
        {
            return base.EmergercyAbility(abilityRemain, nextGCD, out act);
        }
        else if (abilityRemain != 0 &&
            (Level < Actions.RagingStrikes.Level || LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes)) &&
            (Level < Actions.BattleVoice.Level || LocalPlayer.HaveStatus(ObjectStatus.BattleVoice)))
        {
            //纷乱箭
            if (Actions.Barrage.ShouldUse(out act)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);

    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Song == Song.NONE || Level < Actions.MagesBallad.Level)
        {
            act = null!;
            return false;
        }

        //猛者强击
        if (Actions.RagingStrikes.ShouldUse(out act)) return true;

        //光明神的最终乐章
        if (abilityRemain == 2 && Actions.RadiantFinale.ShouldUse(out act, mustUse: true)) return true;

        //战斗之声
        if (abilityRemain == 1 && IsLastAbility(true, Actions.RadiantFinale) && Actions.BattleVoice.ShouldUse(out act, mustUse: true)) return true;


        act = null!;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.RadiantFinale.IsCoolDown && LocalPlayer.FindStatusTime(ObjectStatus.RadiantFinale) > RadiantFinaleCurrent0GCDRemain)
        {
            act = null;
            return false;
        }
        //放浪神的小步舞曲
        if ((JobGauge.Song == Song.NONE || ((JobGauge.Song != Song.NONE || LocalPlayer.HaveStatus(ObjectStatus.ArmyEthos)) && abilityRemain == 1))
            && JobGauge.SongTimer < 3000)
        {
            if (Actions.WanderersMinuet.ShouldUse(out act)) return true;
        }

        //九天连箭
        if (JobGauge.Song != Song.NONE && Actions.EmpyrealArrow.ShouldUse(out act)) return true;

        //完美音调
        if (Actions.PitchPerfect.ShouldUse(out act)) return true;

        //贤者的叙事谣
        if (JobGauge.SongTimer < 3000 && Actions.MagesBallad.ShouldUse(out act)) return true;

        //军神的赞美歌
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Song.MAGE
            || JobGauge.Song == Song.NONE) && Actions.ArmysPaeon.ShouldUse(out act)) return true;

        //测风诱导箭
        if (Actions.Sidewinder.ShouldUse(out act)) return true;

        //看看现在有没有开猛者强击和战斗之声
        bool empty = (LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes) 
            && (LocalPlayer.HaveStatus(ObjectStatus.BattleVoice) 
            || Level < Actions.BattleVoice.Level)) || JobGauge.Song == Song.MAGE;
        //死亡剑雨
        if (Actions.RainofDeath.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        //失血箭
        if (Actions.Bloodletter.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        return false;
    }
}
