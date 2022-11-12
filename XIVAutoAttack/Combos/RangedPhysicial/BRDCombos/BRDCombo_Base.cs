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
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedPhysicial.BRDCombos;

internal abstract class BRDCombo_Base<TCmd> : JobGaugeCombo<BRDGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Bard, ClassJobID.Archer };

    public static readonly BaseAction
        //强力射击
        HeavyShoot = new(97) { BuffsProvide = new[] { StatusID.StraightShotReady } },

        //直线射击
        StraitShoot = new(98) { BuffsNeed = new[] { StatusID.StraightShotReady } },

        //毒药箭
        VenomousBite = new(100, isEot: true) { TargetStatus = new[] { StatusID.VenomousBite, StatusID.CausticBite } },

        //风蚀箭
        Windbite = new(113, isEot: true) { TargetStatus = new[] { StatusID.Windbite, StatusID.Stormbite } },

        //伶牙俐齿
        IronJaws = new(3560, isEot: true)
        {
            OtherCheck = b =>
            {
                if (IsLastWeaponSkill(false, IronJaws)) return false;

                if (Player.HaveStatusFromSelf(StatusID.RagingStrikes) &&
                    Player.WillStatusEndGCD(1, 1, true, StatusID.RagingStrikes)) return true;

                return b.HaveStatusFromSelf(StatusID.VenomousBite, StatusID.CausticBite) & b.HaveStatusFromSelf(StatusID.Windbite, StatusID.Stormbite)
                & (b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, StatusID.VenomousBite, StatusID.CausticBite)
                | b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, StatusID.Windbite, StatusID.Stormbite));
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
        RagingStrikes = new(101),

        //光明神的最终乐章
        RadiantFinale = new(25785, true),

        //纷乱箭
        Barrage = new(107),

        //九天连箭
        EmpyrealArrow = new(3558),

        //完美音调
        PitchPerfect = new(7404)
        {
            OtherCheck = b => JobGauge.Song == Song.WANDERER,
        },

        //失血箭
        Bloodletter = new(110),

        //死亡箭雨
        RainofDeath = new(117),

        //连珠箭
        QuickNock = new(106) { BuffsProvide = new[] { StatusID.ShadowbiteReady } },

        //影噬箭
        Shadowbite = new(16494) { BuffsNeed = new[] { StatusID.ShadowbiteReady } },

        //光阴神的礼赞凯歌
        WardensPaean = new(3561),

        //大地神的抒情恋歌
        NaturesMinne = new(7408),

        //侧风诱导箭
        Sidewinder = new(3562),

        //绝峰箭
        ApexArrow = new(16496)
        {
            //    OtherCheck = b =>
            //    {
            //        if (Player.HaveStatus(StatusIDs.BlastArrowReady) || (QuickNock.ShouldUse(out _) && JobGauge.SoulVoice == 100)) return true;

            //        //快爆发了,攒着等爆发
            //        if (JobGauge.SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

            //        //爆发快过了,如果手里还有绝峰箭,就把绝峰箭打出去
            //        if (JobGauge.SoulVoice >= 80 && Player.HaveStatus(StatusIDs.RagingStrikes) && Player.WillStatusEnd(10, false, StatusIDs.RagingStrikes)) return true;

            //        if (JobGauge.SoulVoice == 100
            //            && Player.HaveStatus(StatusIDs.RagingStrikes)
            //            && Player.HaveStatus(StatusIDs.BattleVoice)
            //            && (Player.HaveStatus(StatusIDs.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

            //        if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

            //        //能量之声等于100或者在爆发箭预备状态
            //        if (!Player.HaveStatus(StatusIDs.RagingStrikes) && JobGauge.SoulVoice == 100) return true;

            //        return false;
            //    },
        },

            //行吟
            Troubadour = new(7405, true)
            {
                BuffsProvide = new[]
            {
                    StatusID.Troubadour,
                    StatusID.Tactician1,
                    StatusID.Tactician2,
                    StatusID.ShieldSamba,
            },
            };
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //有某些非常危险的状态。
        if (CommandController.EsunaOrShield && TargetUpdater.WeakenPeople.Length > 0 || TargetUpdater.DyingPeople.Length > 0)
        {
            if (WardensPaean.ShouldUse(out act, mustUse: true)) return true;
        }
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }
}
