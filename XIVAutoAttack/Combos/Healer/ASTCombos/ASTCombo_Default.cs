using Dalamud.Game.ClientState.JobGauge.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Healer.ASTCombos.ASTCombo_Default;

namespace XIVAutoAttack.Combos.Healer.ASTCombos;

internal sealed class ASTCombo_Default : ASTCombo_Base<CommandType>
{
    public override string GameVersion => "6.18";

    public override string Author => "汐ベMoon";

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
        {DescType.HealArea, $"GCD: {AspectedHelios}, {Helios}\n                     能力: {EarthlyStar}, {CrownPlay}, {CelestialOpposition}"},
        {DescType.HealSingle, $"GCD: {AspectedBenefic}, {Benefic2}, {Benefic}\n                     能力: {CelestialIntersection}, {EssentialDignity}"},
        {DescType.DefenseArea, $"{CollectiveUnconscious}"},
        {DescType.DefenseSingle, $"{Exaltation}，给被挨打的T"},
        {DescType.BreakingAction, $"{Divination}"}
    };

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //天星交错
        if (CelestialIntersection.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //给T减伤，这个很重要。
        if (Exaltation.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //来个命运之轮
        if (CollectiveUnconscious.ShouldUse(out act)) return true;

        return base.DefenceAreaAbility(abilityRemain, out act);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //群体输出
        if (Gravity.ShouldUse(out act)) return true;

        //单体输出
        if (Combust.ShouldUse(out act)) return true;
        if (Malefic.ShouldUse(out act)) return true;
        if (Combust.ShouldUse(out act, mustUse: IsMoving)) return true;
        /*        var times = StatusHelper.FindStatusFromSelf(Actions.Combust.Target,
                    new ushort[] { ObjectStatus.Combust, ObjectStatus.Combust2, ObjectStatus.Combust3 });
                if (times.Length == 0 || times.Max() < 25)
                {
                    if (Actions.Combust.ShouldUseAction(out act, mustUse: IsMoving && HaveTargetAngle)) return true;
                }
        */
        act = null!;
        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        //阳星相位
        if (AspectedHelios.ShouldUse(out act)) return true;

        //阳星
        if (Helios.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergencyAbility(abilityRemain, nextGCD, out act)) return true;

        //如果要群奶了，先上个天宫图！
        if (nextGCD.IsAnySameAction(true, AspectedHelios, Helios))
        {
            if (Horoscope.ShouldUse(out act)) return true;

            //中间学派
            if (NeutralSect.ShouldUse(out act)) return true;
        }

        //如果要单奶了，先上星位合图！
        if (nextGCD.IsAnySameAction(true, Benefic, Benefic2, AspectedBenefic))
        {
            if (Synastry.ShouldUse(out act)) return true;
        }
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //如果当前还没有卡牌，那就抽一张
        if (Draw.ShouldUse(out act)) return true;

        bool canUse = Astrodyne.ActionCheck(Service.ClientState.LocalPlayer);

        //如果当前卡牌已经拥有了，就重抽
        if (!canUse && Redraw.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //吉星相位
        if (AspectedBenefic.Target.GetHealthRatio() > 0.4
            && AspectedBenefic.ShouldUse(out act)) return true;

        //福星
        if (Benefic2.ShouldUse(out act)) return true;

        //吉星
        if (Benefic.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && Divination.ShouldUse(out act)) return true;

        //如果当前还没有皇冠卡牌，那就抽一张
        if (MinorArcana.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //如果当前还没有卡牌，那就抽一张
        if (Draw.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //光速，创造更多的内插能力技的机会。
        if (IsMoving && Lightspeed.ShouldUse(out act)) return true;


        if (!IsMoving)
        {
            //如果没有地星也没有巨星，那就试试看能不能放个。
            if (!Player.HasStatus(true, StatusID.EarthlyDominance, StatusID.GiantDominance))
            {
                if (EarthlyStar.ShouldUse(out act, mustUse: true)) return true;
            }
            //加星星的进攻Buff
            if (Astrodyne.ShouldUse(out act)) return true;
        }

        if (DrawnCrownCard == CardType.LORD || MinorArcana.WillHaveOneChargeGCD(1))
        {
            //进攻牌，随便发。或者CD要转好了，赶紧发掉。
            if (CrownPlay.ShouldUse(out act)) return true;
        }

        //发牌
        if (abilityRemain == 1 && DrawnCard != CardType.NONE && Seals.Contains(SealType.NONE))
        {
            if (PlayCard(out act)) return true;
        }

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (EssentialDignity.Target.GetHealthRatio() < 0.4
            && EssentialDignity.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        //常规奶
        if (EssentialDignity.ShouldUse(out act)) return true;
        //带盾奶
        if (CelestialIntersection.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //奶量牌，要看情况。
        if (DrawnCrownCard == CardType.LADY && CrownPlay.ShouldUse(out act)) return true;

        var tank = TargetUpdater.PartyTanks;
        var isBoss = Malefic.IsTargetBoss;
        if (EssentialDignity.IsCoolDown && tank.Length == 1 && tank.Any(t => t.GetHealthRatio() < 0.5) && !isBoss)
        {
            //群Hot
            if (CelestialOpposition.ShouldUse(out act)) return true;

            //如果有巨星主宰
            if (Player.HasStatus(true, StatusID.GiantDominance))
            {
                //需要回血的时候炸了。
                act = EarthlyStar;
                return true;
            }

            //天宫图
            if (!Player.HasStatus(true, StatusID.HoroscopeHelios, StatusID.Horoscope) && Horoscope.ShouldUse(out act)) return true;
            //阳星天宫图
            if (Player.HasStatus(true, StatusID.HoroscopeHelios) && Horoscope.ShouldUse(out act)) return true;
            //超紧急情况天宫图
            if (tank.Any(t => t.GetHealthRatio() < 0.3) && Horoscope.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //群Hot
        if (CelestialOpposition.ShouldUse(out act)) return true;

        //如果有巨星主宰
        if (Player.HasStatus(true, StatusID.GiantDominance))
        {
            //需要回血的时候炸了。
            act = EarthlyStar;
            return true;
        }

        //天宫图
        if (Player.HasStatus(true, StatusID.HoroscopeHelios) && Horoscope.ShouldUse(out act)) return true;

        //奶量牌，要看情况。
        if (DrawnCrownCard == CardType.LADY && CrownPlay.ShouldUse(out act)) return true;

        return false;
    }


}
