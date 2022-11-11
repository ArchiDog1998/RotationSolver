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

namespace XIVAutoAttack.Combos.RangedPhysicial;

public abstract class BRDCombo<TCmd> : JobGaugeCombo<BRDGauge, TCmd> where TCmd : Enum
{
    public  sealed override uint[] JobIDs => new uint[] { 23, 5 };

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
                if (Player.HaveStatus(ObjectStatus.BlastArrowReady) || (QuickNock.ShouldUse(out _) && JobGauge.SoulVoice == 100)) return true;

                //快爆发了,攒着等爆发
                if (JobGauge.SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

                //爆发快过了,如果手里还有绝峰箭,就把绝峰箭打出去
                if (JobGauge.SoulVoice >= 80 && Player.HaveStatus(ObjectStatus.RagingStrikes) && Player.WillStatusEnd(10, false, ObjectStatus.RagingStrikes)) return true;

                if (JobGauge.SoulVoice == 100
                    && Player.HaveStatus(ObjectStatus.RagingStrikes)
                    && Player.HaveStatus(ObjectStatus.BattleVoice)
                    && (Player.HaveStatus(ObjectStatus.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

                if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

                //能量之声等于100或者在爆发箭预备状态
                if (!Player.HaveStatus(ObjectStatus.RagingStrikes) && JobGauge.SoulVoice == 100) return true;

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
    protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (CommandController.EsunaOrShield && TargetUpdater.WeakenPeople.Length > 0 || TargetUpdater.DyingPeople.Length > 0)
        {
            if (WardensPaean.ShouldUse(out act, mustUse: true)) return true;
        }
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }
}
