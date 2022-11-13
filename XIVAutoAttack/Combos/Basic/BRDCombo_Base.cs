using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class BRDCombo_Base<TCmd> : JobGaugeCombo<BRDGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new [] { ClassJobID.Bard, ClassJobID.Archer };

    public static readonly BaseAction
        //强力射击
        HeavyShoot = new(ActionID.HeavyShoot) { BuffsProvide = new[] { StatusID.StraightShotReady } },

        //直线射击
        StraitShoot = new(ActionID.StraitShoot) { BuffsNeed = new[] { StatusID.StraightShotReady } },

        //毒咬箭
        VenomousBite = new(ActionID.VenomousBite, isEot: true) 
        { 
            TargetStatus = new[] { StatusID.VenomousBite, StatusID.CausticBite } 
        },

        //风蚀箭
        Windbite = new(ActionID.Windbite, isEot: true) 
        { 
            TargetStatus = new[] { StatusID.Windbite, StatusID.Stormbite } 
        },

        //伶牙俐齿
        IronJaws = new(ActionID.IronJaws, isEot: true)
        {
            OtherCheck = b =>
            {
                return b.HaveStatus(true, VenomousBite.TargetStatus) 
                    & b.HaveStatus(true, Windbite.TargetStatus)

                & (b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, VenomousBite.TargetStatus)
                | b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, Windbite.TargetStatus));
            },
        },

        //放浪神的小步舞曲
        WanderersMinuet = new(ActionID.WanderersMinuet),

        //贤者的叙事谣
        MagesBallad = new(ActionID.MagesBallad),

        //军神的赞美歌
        ArmysPaeon = new(ActionID.ArmysPaeon),

        //战斗之声
        BattleVoice = new(ActionID.BattleVoice, true),

        //猛者强击
        RagingStrikes = new(ActionID.RagingStrikes, true),

        //光明神的最终乐章
        RadiantFinale = new(ActionID.RadiantFinale, true)
        {
            OtherCheck = b => JobGauge.Coda.Any(s => s != Song.NONE),
        },

        //纷乱箭
        Barrage = new(ActionID.Barrage),

        //九天连箭
        EmpyrealArrow = new(ActionID.EmpyrealArrow),

        //完美音调
        PitchPerfect = new(ActionID.PitchPerfect)
        {
            OtherCheck = b => JobGauge.Song == Song.WANDERER,
        },

        //失血箭
        Bloodletter = new(ActionID.Bloodletter),

        //死亡箭雨
        RainofDeath = new(ActionID.RainofDeath),

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
