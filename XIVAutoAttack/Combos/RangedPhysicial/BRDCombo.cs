using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal sealed class BRDCombo : JobGaugeCombo<BRDGauge>
{

    internal override uint JobID => 23;

    internal struct Actions
    {
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
                    if (IsLastWeaponSkill(false, IronJaws)) return false;

                    if (Player.HaveStatus(ObjectStatus.RagingStrikes) && 
                        Player.WillStatusEndGCD(1, 1, true, ObjectStatus.RagingStrikes)) return true;

                    return b.HaveStatus(ObjectStatus.VenomousBite, ObjectStatus.CausticBite) & b.HaveStatus(ObjectStatus.Windbite, ObjectStatus.Stormbite)
                    & (b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, ObjectStatus.VenomousBite, ObjectStatus.CausticBite) 
                    | b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, ObjectStatus.Windbite, ObjectStatus.Stormbite));
                },
            },

            //贤者的叙事谣
            MagesBallad = new(114),

            //军神的赞美歌
            ArmysPaeon = new(116),

            //放浪神的小步舞曲
            WanderersMinuet = new(3559),

            //战斗之声
            BattleVoice = new(118, true),

            //猛者强击
            RagingStrikes = new(101)
            {
                OtherCheck = b =>
                {
                    if (JobGauge.Song == Song.WANDERER || !WanderersMinuet.EnoughLevel && BattleVoice.WillHaveOneChargeGCD(1, 1) 
                        || !BattleVoice.EnoughLevel) return true;

                    return false;
                },
            },

            //光明神的最终乐章
            RadiantFinale = new(25785, true)
            {
                OtherCheck = b =>
                {
                    static bool SongIsNotNone(Song value) => value != Song.NONE;
                    static bool SongIsWandererMinuet(Song value) => value == Song.WANDERER;
                    if ((Array.TrueForAll(JobGauge.Coda, SongIsNotNone) || Array.Exists(JobGauge.Coda, SongIsWandererMinuet))
                        && BattleVoice.WillHaveOneChargeGCD()
                        && RagingStrikes.IsCoolDown 
                        && Player.HaveStatus(ObjectStatus.RagingStrikes)
                        && RagingStrikes.ElapsedAfterGCD(1)) return true;
                    return false;
                },
            },

            //纷乱箭
            Barrage = new(107)
            {
                BuffsProvide = new[] { ObjectStatus.StraightShotReady },
                OtherCheck = b =>
                {
                    if (!EmpyrealArrow.IsCoolDown || EmpyrealArrow.WillHaveOneChargeGCD() || JobGauge.Repertoire == 3) return false;
                    return true;
                }
            },

            //九天连箭
            EmpyrealArrow = new(3558),

            //完美音调
            PitchPerfect = new(7404)
            {
                OtherCheck = b => JobGauge.Song == Song.WANDERER,
            },

            //失血箭
            Bloodletter = new(110)
            {
                OtherCheck = b =>
                {
                    if (EmpyrealArrow.EnoughLevel && (!EmpyrealArrow.IsCoolDown || EmpyrealArrow.WillHaveOneChargeGCD())) return false;
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
            Sidewinder = new(3562),

            //绝峰箭
            ApexArrow = new(16496)
            {
                OtherCheck = b =>
                {
                    //快爆发了,攒着等爆发
                    if (JobGauge.SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

                    //爆发快过了,如果手里还有绝峰箭,就把绝峰箭打出去
                    if (JobGauge.SoulVoice >= 80 && Player.HaveStatus(ObjectStatus.RagingStrikes) && !Player.WillStatusEnd(10, false, ObjectStatus.RagingStrikes)) return true;

                    if (JobGauge.SoulVoice == 100
                        && Player.HaveStatus(ObjectStatus.RagingStrikes)
                        && Player.HaveStatus(ObjectStatus.BattleVoice)
                        && (Player.HaveStatus(ObjectStatus.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

                    if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

                    //能量之声等于100或者在爆发箭预备状态
                    if (JobGauge.SoulVoice == 100 || Player.HaveStatus(ObjectStatus.BlastArrowReady)) return true;

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
        {DescType.范围防御, $"{Actions.Troubadour}"},
        {DescType.单体治疗, $"{Actions.NaturesMinne}"},
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
        //伶牙俐齿
        if (Actions.IronJaws.ShouldUse(out act)) return true;

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
        if (nextGCD.IsAnySameAction(true, Actions.StraitShoot, Actions.VenomousBite,
            Actions.Windbite, Actions.IronJaws))
        {
            return base.EmergercyAbility(abilityRemain, nextGCD, out act);
        }
        else if (abilityRemain != 0 &&
            (!Actions.RagingStrikes.EnoughLevel || Player.HaveStatus(ObjectStatus.RagingStrikes)) &&
            (!Actions.BattleVoice.EnoughLevel || Player.HaveStatus(ObjectStatus.BattleVoice)))
        {
            //纷乱箭
            if (Actions.Barrage.ShouldUse(out act)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if(SettingBreak && JobGauge.Song != Song.NONE && Actions.MagesBallad.EnoughLevel)
        {
            //猛者强击
            if (Actions.RagingStrikes.ShouldUse(out act)) return true;

            //光明神的最终乐章
            if (abilityRemain == 2 && Actions.RadiantFinale.ShouldUse(out act, mustUse: true)) return true;

            //战斗之声
            if (Actions.RadiantFinale.IsCoolDown && Actions.BattleVoice.ShouldUse(out act, mustUse: true)) return true;
        }

        if (Actions.RadiantFinale.IsCoolDown && !Actions.RadiantFinale.ElapsedAfterGCD())
        {
            act = null;
            return false;
        }
        //放浪神的小步舞曲
        if ((JobGauge.Song == Song.NONE || ((JobGauge.Song != Song.NONE || Player.HaveStatus(ObjectStatus.ArmyEthos)) && abilityRemain == 1))
            && JobGauge.SongTimer < 3000)
        {
            if (Actions.WanderersMinuet.ShouldUse(out act)) return true;
        }

        //九天连箭
        if (JobGauge.Song != Song.NONE && Actions.EmpyrealArrow.ShouldUse(out act)) return true;

        //完美音调
        if (Actions.PitchPerfect.ShouldUse(out act))
        {
            if (JobGauge.SongTimer < 3000 && JobGauge.Repertoire > 0) return true;

            if (JobGauge.Repertoire == 3 || JobGauge.Repertoire == 2 && Actions.EmpyrealArrow.WillHaveOneChargeGCD(1)) return true;
        }

        //贤者的叙事谣
        if (JobGauge.SongTimer < 3000 && Actions.MagesBallad.ShouldUse(out act)) return true;

        //军神的赞美歌
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Song.MAGE
            || JobGauge.Song == Song.NONE) && Actions.ArmysPaeon.ShouldUse(out act)) return true;

        //测风诱导箭
        if (Actions.Sidewinder.ShouldUse(out act))
        {
            if (Player.HaveStatus(ObjectStatus.BattleVoice) && Player.HaveStatus(ObjectStatus.RadiantFinale)) return true;

            if (!Actions.BattleVoice.WillHaveOneCharge(10, false) && !Actions.RadiantFinale.WillHaveOneCharge(10, false)) return true;

            if (!Actions.RadiantFinale.EnoughLevel) return true;
        }

        //看看现在有没有开猛者强击和战斗之声
        bool empty = (Player.HaveStatus(ObjectStatus.RagingStrikes) 
            && (Player.HaveStatus(ObjectStatus.BattleVoice) 
            || !Actions.BattleVoice.EnoughLevel)) || JobGauge.Song == Song.MAGE;
        //死亡剑雨
        if (Actions.RainofDeath.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        //失血箭
        if (Actions.Bloodletter.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        return false;
    }
}
