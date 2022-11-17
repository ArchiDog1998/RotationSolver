using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.RangedPhysicial.BRDCombos.BRDCombo_Default;

namespace XIVAutoAttack.Combos.RangedPhysicial.BRDCombos;

internal sealed class BRDCombo_Default : BRDCombo_Base<CommandType>
{
    public override string Author => "汐ベMoon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetCombo("FirstSong", 0, "第一首歌", "旅神歌", "贤者歌", "军神歌")
            .SetFloat("WANDTime", 43, "旅神歌时间", min: 0, max: 45, speed: 1)
            .SetFloat("MAGETime", 34, "贤者歌时间", min: 0, max: 45, speed: 1)
            .SetFloat("ARMYTime", 43, "军神歌时间", min: 0, max: 45, speed: 1);
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.循环说明, $"请确保三首歌时间加在一起等于120秒!"},
        {DescType.范围防御, $"{Troubadour}"},
        {DescType.单体治疗, $"{NaturesMinne}"},
    };

    private int FirstSong => Config.GetComboByName("FirstSong");
    private float WANDRemainTime => 45 - Config.GetFloatByName("WANDTime");
    private float MAGERemainTime => 45 - Config.GetFloatByName("MAGETime");
    private float ARMYRemainTime => 45 - Config.GetFloatByName("ARMYTime");

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
        if (IronJaws.ShouldUse(out act))
        {
            var b = IronJaws.Target;
            if (b.HasStatus(true, VenomousBite.TargetStatus) & b.HasStatus(true, Windbite.TargetStatus)
            & (b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, VenomousBite.TargetStatus)
            | b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, Windbite.TargetStatus))) return true;

            if (Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEndGCD(1, 0, true, StatusID.RagingStrikes)) return true;
        }

        //放大招！
        if (CanUseApexArrow(out act)) return true;

        //群体GCD
        if (Shadowbite.ShouldUse(out act)) return true;
        if (QuickNock.ShouldUse(out act)) return true;

        //上毒
        if (VenomousBite.ShouldUse(out act)) return true;
        if (Windbite.ShouldUse(out act)) return true;

        //直线射击
        if (StraitShoot.ShouldUse(out act)) return true;

        //强力射击
        if (HeavyShoot.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //脱战速行
        if (!InCombat && Player.WillStatusEndGCD(1) && Peloton.ShouldUse(out act)) return true;
        return base.GeneralAbility(abilityRemain, out act);
    }

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //如果接下来要上毒或者要直线射击，那算了。
        if (nextGCD.IsAnySameAction(true, StraitShoot, VenomousBite,
            Windbite, IronJaws))
        {
            return base.EmergencyAbility(abilityRemain, nextGCD, out act);
        }
        else if (abilityRemain != 0 &&
            (!RagingStrikes.EnoughLevel || Player.HasStatus(true, StatusID.RagingStrikes)) &&
            (!BattleVoice.EnoughLevel || Player.HasStatus(true, StatusID.BattleVoice)))
        {
            if (EmpyrealArrow.IsCoolDown || !EmpyrealArrow.WillHaveOneChargeGCD() || Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
            {
                //纷乱箭
                if (Barrage.ShouldUse(out act)) return true;
            }
        }

        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && Song != Song.NONE && MagesBallad.EnoughLevel)
        {

            //猛者强击
            if (RagingStrikes.ShouldUse(out act))
            {
                if (Song != Song.NONE) return true;
            }

            //光明神的最终乐章
            if (abilityRemain == 2 && RadiantFinale.ShouldUse(out act, mustUse: true))
            {
                if (RagingStrikes.IsCoolDown && Player.HasStatus(true, StatusID.RagingStrikes) && RagingStrikes.ElapsedAfterGCD(1)) return true;
            }

            //战斗之声
            if (abilityRemain == 1 && BattleVoice.ShouldUse(out act, mustUse: true))
            {
                if (RagingStrikes.IsCoolDown && Player.HasStatus(true, StatusID.RagingStrikes) && RagingStrikes.ElapsedAfterGCD(1)) return true;
            }
        }

        if (RadiantFinale.IsCoolDown && !RadiantFinale.ElapsedAfterGCD())
        {
            act = null;
            return false;
        }

        if (Song == Song.NONE)
        {
            if (FirstSong == 0 && WanderersMinuet.ShouldUse(out act)) return true;
            if (FirstSong == 1 && MagesBallad.ShouldUse(out act)) return true;
            if (FirstSong == 2 && ArmysPaeon.ShouldUse(out act)) return true;

        }

        //放浪神的小步舞曲
        if ((Song != Song.NONE || Player.HasStatus(true, StatusID.ArmyEthos)) && abilityRemain == 1 && SongEndAfter(ARMYRemainTime))
        {
            if (WanderersMinuet.ShouldUse(out act)) return true;
        }

        //贤者的叙事谣
        if (SongEndAfter(WANDRemainTime) && MagesBallad.ShouldUse(out act)) return true;

        //军神的赞美歌
        if (SongEndAfter(MAGERemainTime) && (Song == Song.MAGE || Song == Song.NONE) && ArmysPaeon.ShouldUse(out act)) return true;

        //九天连箭
        if (Song != Song.NONE && EmpyrealArrow.ShouldUse(out act)) return true;

        //完美音调
        if (PitchPerfect.ShouldUse(out act))
        {
            if (SongEndAfter(3) && Repertoire > 0) return true;

            if (Repertoire == 3 || Repertoire == 2 && EmpyrealArrow.WillHaveOneChargeGCD(1)) return true;
        }

        //测风诱导箭
        if (Sidewinder.ShouldUse(out act))
        {
            if (Player.HasStatus(true, StatusID.BattleVoice) && (Player.HasStatus(true, StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

            if (!BattleVoice.WillHaveOneCharge(10) && !RadiantFinale.WillHaveOneCharge(10)) return true;

            if (RagingStrikes.IsCoolDown && !Player.HasStatus(true, StatusID.RagingStrikes)) return true;
        }

        //看看现在有没有开猛者强击和战斗之声
        bool empty = Player.HasStatus(true, StatusID.RagingStrikes)
            && (Player.HasStatus(true, StatusID.BattleVoice)
            || !BattleVoice.EnoughLevel) || Song == Song.MAGE;

        if (EmpyrealArrow.IsCoolDown || !EmpyrealArrow.WillHaveOneChargeGCD() || Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
        {
            //死亡剑雨
            if (RainofDeath.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

            //失血箭
            if (Bloodletter.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;
        }

        return false;
    }

    private bool CanUseApexArrow(out IAction act)
    {
        //放大招！
        if (!ApexArrow.ShouldUse(out act, mustUse: true)) return false;

        if (Player.HasStatus(true, StatusID.BlastArrowReady) || (QuickNock.ShouldUse(out _) && SoulVoice == 100)) return true;

        //快爆发了,攒着等爆发
        if (SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

        //爆发快过了,如果手里还有绝峰箭,就把绝峰箭打出去
        if (SoulVoice >= 80 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEnd(10, false, StatusID.RagingStrikes)) return true;

        if (SoulVoice == 100
            && Player.HasStatus(true, StatusID.RagingStrikes)
            && Player.HasStatus(true, StatusID.BattleVoice)
            && (Player.HasStatus(true, StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

        if (Song == Song.MAGE && SoulVoice >= 80 && SongEndAfter(22) && SongEndAfter(18)) return true;

        //能量之声等于100或者在爆发箭预备状态
        if (!Player.HasStatus(true, StatusID.RagingStrikes) && SoulVoice == 100) return true;

        return false;
    }
}
