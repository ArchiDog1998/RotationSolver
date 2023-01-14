using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.PLDCombos.PLDCombo_Default;

namespace XIVAutoAttack.Combos.Tank.PLDCombos;

internal sealed class PLDCombo_Default : PLDCombo_Base
{
    public override string GameVersion => "6.18";

    public override string Author => "汐ベMoon";

    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealSingle, $"{Clemency}"},
        {DescType.DefenseArea, $"{DivineVeil}, {PassageofArms}"},
        {DescType.DefenseSingle, $"{Sentinel}, {Sheltron}"},
        {DescType.MoveAction, $"{Intervene}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //三个大招
        if (BladeofValor.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofFaith.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofTruth.ShouldUse(out act, mustUse: true)) return true;

        //魔法三种姿势,有安魂祈祷buff,且没在战逃中
        if (Player.HasStatus(true, StatusID.Requiescat) && !Player.HasStatus(true, StatusID.FightOrFlight, StatusID.SwordOath))
        {
            if (Player.StatusStack(true, StatusID.Requiescat) == 1 && Player.WillStatusEnd(3, false, StatusID.Requiescat) || Player.CurrentMp <= 2000)
            {
                if (Confiteor.ShouldUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (HolyCircle.ShouldUse(out act)) return true;
                if (HolySpirit.ShouldUse(out act)) return true;
            }
        }

        //AOE 二连
        if (Prominence.ShouldUse(out act)) return true;
        if (TotalEclipse.ShouldUse(out act)) return true;

        //赎罪剑
        if (Atonement.ShouldUse(out act))
        {
            if (Player.HasStatus(true, StatusID.FightOrFlight) && IsLastGCD(true, Atonement, RageofHalone) && !Player.WillStatusEndGCD(2, 0, true, StatusID.FightOrFlight)) return true;

            if (Player.StatusStack(true, StatusID.SwordOath) > 1) return true;
        }

        //单体三连
        if (GoringBlade.ShouldUse(out act)) return true;
        if (RageofHalone.ShouldUse(out act)) return true;
        if (RiotBlade.ShouldUse(out act)) return true;
        if (FastBlade.ShouldUse(out act)) return true;

        //投盾
        if (ShieldLob.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //圣光幕帘
        if (DivineVeil.ShouldUse(out act)) return true;

        //武装戍卫
        if (PassageofArms.ShouldUse(out act)) return true;

        //血仇
        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 1 && SettingBreak)
        {
            //战逃反应 加Buff
            if (FightorFlight.ShouldUse(out act)) return true;

            //安魂祈祷
            if (Requiescat.ShouldUse(out act, mustUse: true) && Player.HasStatus(true, StatusID.FightOrFlight) && Player.WillStatusEnd(17, true, StatusID.FightOrFlight) && Target.HasStatus(true, StatusID.GoringBlade)) return true;
        }

        //厄运流转
        if ((TotalEclipse.ShouldUse(out _) || FightorFlight.ElapsedAfterGCD(2)) && CircleofScorn.ShouldUse(out act, mustUse: true)) return true;

        //深奥之灵
        if ((TotalEclipse.ShouldUse(out _) || FightorFlight.ElapsedAfterGCD(3)) && SpiritsWithin.ShouldUse(out act, mustUse: true)) return true;

        //调停
        if (Target.HasStatus(true, StatusID.GoringBlade))
        {
            if (FightorFlight.ElapsedAfterGCD(2) && Intervene.ShouldUse(out act, mustUse: true)) return true;

            if (Intervene.ShouldUse(out act, mustUse: true) && !IsMoving) return true;
        }

        //盾阵,保护
        if (OathGauge == 100 && Player.CurrentHp < Player.MaxHp)
        {
            //盾阵
            if (HaveShield && Sheltron.ShouldUse(out act)) return true;
            //保护
            if (!HaveShield && Cover.ShouldUse(out act)) return true;
        }
        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //盾阵
        if (HaveShield && Sheltron.ShouldUse(out act)) return true;
        //保护
        if (!HaveShield && Cover.ShouldUse(out act)) return true;

        if (abilityRemain == 1)
        {
            //预警（减伤30%）
            if (Sentinel.ShouldUse(out act)) return true;

            //铁壁（减伤20%）
            if (Rampart.ShouldUse(out act)) return true;
        }

        //雪仇
        if (Reprisal.ShouldUse(out act)) return true;

        //干预（减伤10%）
        if (!HaveShield && Intervention.ShouldUse(out act)) return true;
        act = null;
        return false;
    }
}
