using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.RangedPhysicial.BRDCombos.BRDCombo_Default;

namespace XIVAutoAttack.Combos.RangedPhysicial.BRDCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/RangedPhysicial/BRDCombos/BRDCombo_Default.cs")]
internal sealed class BRDCombo_Default : BRDCombo_Base<CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.范围防御, $"{Troubadour}"},
        {DescType.单体治疗, $"{NaturesMinne}"},
    };

    public override string Author => "汐ベMoon";

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
        if (CanUseApexArrow(out act)) return true;

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
            (!RagingStrikes.EnoughLevel || Player.HaveStatus(StatusID.RagingStrikes)) &&
            (!BattleVoice.EnoughLevel || Player.HaveStatus(StatusID.BattleVoice)))
        {
            if (EmpyrealArrow.IsCoolDown || !EmpyrealArrow.WillHaveOneChargeGCD() || JobGauge.Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
            {
                //纷乱箭
                if (Barrage.ShouldUse(out act)) return true;
            }      
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && JobGauge.Song != Song.NONE && MagesBallad.EnoughLevel)
        {

            //猛者强击
            if (RagingStrikes.ShouldUse(out act))
            {
                if (JobGauge.Song != Song.NONE) return true;
            }

            //光明神的最终乐章
            if (abilityRemain == 2 && RadiantFinale.ShouldUse(out act, mustUse: true))
            {
                if (RagingStrikes.IsCoolDown && Player.HaveStatus(StatusID.RagingStrikes) && RagingStrikes.ElapsedAfterGCD(1)) return true;
            }

            //战斗之声
            if (abilityRemain == 1 && BattleVoice.ShouldUse(out act, mustUse: true))
            {
                if (RagingStrikes.IsCoolDown && Player.HaveStatus(StatusID.RagingStrikes) && RagingStrikes.ElapsedAfterGCD(1)) return true;
            }
        }

        if (RadiantFinale.IsCoolDown && !RadiantFinale.ElapsedAfterGCD())
        {
            act = null;
            return false;
        }
        //放浪神的小步舞曲
        if ((JobGauge.Song == Song.NONE || (JobGauge.Song != Song.NONE || Player.HaveStatus(StatusID.ArmyEthos)) && abilityRemain == 1)
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
            if (Player.HaveStatus(StatusID.BattleVoice) && (Player.HaveStatus(StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

            if (!BattleVoice.WillHaveOneCharge(10, false) && !RadiantFinale.WillHaveOneCharge(10, false)) return true;

            if (RagingStrikes.IsCoolDown && !Player.HaveStatus(StatusID.RagingStrikes)) return true;
        }

        //看看现在有没有开猛者强击和战斗之声
        bool empty = Player.HaveStatus(StatusID.RagingStrikes)
            && (Player.HaveStatus(StatusID.BattleVoice)
            || !BattleVoice.EnoughLevel) || JobGauge.Song == Song.MAGE;

        if (EmpyrealArrow.IsCoolDown || !EmpyrealArrow.WillHaveOneChargeGCD() || JobGauge.Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
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

        if (Player.HaveStatus(StatusID.BlastArrowReady) || (QuickNock.ShouldUse(out _) && JobGauge.SoulVoice == 100)) return true;

        //快爆发了,攒着等爆发
        if (JobGauge.SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

        //爆发快过了,如果手里还有绝峰箭,就把绝峰箭打出去
        if (JobGauge.SoulVoice >= 80 && Player.HaveStatus(StatusID.RagingStrikes) && Player.WillStatusEnd(10, false, StatusID.RagingStrikes)) return true;

        if (JobGauge.SoulVoice == 100
            && Player.HaveStatus(StatusID.RagingStrikes)
            && Player.HaveStatus(StatusID.BattleVoice)
            && (Player.HaveStatus(StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

        if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

        //能量之声等于100或者在爆发箭预备状态
        if (!Player.HaveStatus(StatusID.RagingStrikes) && JobGauge.SoulVoice == 100) return true;

        return false;
    }
}
