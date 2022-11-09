using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.RangedPhysicial.BRDCombo;

namespace XIVAutoAttack.Combos.RangedPhysicial;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/RangedPhysicial/BRDCombo.cs",
    ComboAuthor.Armolion)]
internal sealed class BRDCombo : JobGaugeCombo<BRDGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };

    public override uint[] JobIDs => new uint[] { 23, 5 };

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
                if (JobGauge.SoulVoice >= 80 && Player.HaveStatus(ObjectStatus.RagingStrikes) && !Player.WillStatusEnd(10, false, ObjectStatus.RagingStrikes)) return true;

                if (JobGauge.SoulVoice == 100
                    && Player.HaveStatus(ObjectStatus.RagingStrikes)
                    && Player.HaveStatus(ObjectStatus.BattleVoice)
                    && (Player.HaveStatus(ObjectStatus.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

                if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

                //能量之声等于100或者在爆发箭预备状态
                if (JobGauge.SoulVoice == 100) return true;

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
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.范围防御, $"{Troubadour}"},
        {DescType.单体治疗, $"{NaturesMinne}"},
    };
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //行吟
        if (Troubadour.ShouldUse(out act)) return true;


        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //大地神的抒情恋歌
        if (NaturesMinne.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //伶牙俐齿
        if (IronJaws.ShouldUse(out act)) return true;

        //放大招！
        if (ApexArrow.ShouldUse(out act, mustUse: true)) return true;

        //群体GCD
        if (Shadowbite.ShouldUse(out act)) return true;
        if (QuickNock.ShouldUse(out act)) return true;

        //直线射击
        if (StraitShoot.ShouldUse(out act)) return true;

        //上毒
        if (VenomousBite.ShouldUse(out act)) return true;
        if (Windbite.ShouldUse(out act)) return true;

        //强力射击
        if (HeavyShoot.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //如果接下来要上毒或者要直线射击，那算了。
        if (nextGCD.IsAnySameAction(true, StraitShoot, VenomousBite,
            Windbite, IronJaws))
        {
            return base.EmergercyAbility(abilityRemain, nextGCD, out act);
        }
        else if (abilityRemain != 0 &&
            (!RagingStrikes.EnoughLevel || Player.HaveStatus(ObjectStatus.RagingStrikes)) &&
            (!BattleVoice.EnoughLevel || Player.HaveStatus(ObjectStatus.BattleVoice)))
        {
            //纷乱箭
            if (Barrage.ShouldUse(out act)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && JobGauge.Song != Song.NONE && MagesBallad.EnoughLevel)
        {
            //猛者强击
            if (RagingStrikes.ShouldUse(out act)) return true;

            //光明神的最终乐章
            if (abilityRemain == 2 && RadiantFinale.ShouldUse(out act, mustUse: true)) return true;

            //战斗之声
            if (RadiantFinale.IsCoolDown && BattleVoice.ShouldUse(out act, mustUse: true)) return true;
        }

        if (RadiantFinale.IsCoolDown && !RadiantFinale.ElapsedAfterGCD())
        {
            act = null;
            return false;
        }
        //放浪神的小步舞曲
        if ((JobGauge.Song == Song.NONE || ((JobGauge.Song != Song.NONE || Player.HaveStatus(ObjectStatus.ArmyEthos)) && abilityRemain == 1))
            && JobGauge.SongTimer < 3000)
        {
            if (WanderersMinuet.ShouldUse(out act)) return true;
        }

        //九天连箭
        if (JobGauge.Song != Song.NONE && EmpyrealArrow.ShouldUse(out act)) return true;

        //完美音调
        if (PitchPerfect.ShouldUse(out act))
        {
            if (JobGauge.SongTimer < 3000 && JobGauge.Repertoire > 0) return true;

            if (JobGauge.Repertoire == 3 || JobGauge.Repertoire == 2 && EmpyrealArrow.WillHaveOneChargeGCD(1)) return true;
        }

        //贤者的叙事谣
        if (JobGauge.SongTimer < 3000 && MagesBallad.ShouldUse(out act)) return true;

        //军神的赞美歌
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Song.MAGE
            || JobGauge.Song == Song.NONE) && ArmysPaeon.ShouldUse(out act)) return true;

        //测风诱导箭
        if (Sidewinder.ShouldUse(out act))
        {
            if (Player.HaveStatus(ObjectStatus.BattleVoice) && Player.HaveStatus(ObjectStatus.RadiantFinale)) return true;

            if (!BattleVoice.WillHaveOneCharge(10, false) && !RadiantFinale.WillHaveOneCharge(10, false)) return true;

            if (!RadiantFinale.EnoughLevel) return true;
        }

        //看看现在有没有开猛者强击和战斗之声
        bool empty = (Player.HaveStatus(ObjectStatus.RagingStrikes)
            && (Player.HaveStatus(ObjectStatus.BattleVoice)
            || !BattleVoice.EnoughLevel)) || JobGauge.Song == Song.MAGE;
        //死亡剑雨
        if (RainofDeath.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        //失血箭
        if (Bloodletter.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        return false;
    }
}
