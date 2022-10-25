using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Helpers.TargetHelper;

namespace XIVAutoAttack.Combos.Healer;

internal class ASTCombo : JobGaugeCombo<ASTGauge>
{
    internal override uint JobID => 33;

    private protected override BaseAction Raise => Actions.Ascend;

    internal struct Actions
    {
        public static readonly BaseAction
            //生辰
            Ascend = new (3603, true),

            //凶星
            Malefic = new (3596),

            //烧灼
            Combust = new (3599, isDot:true)
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.Combust,
                    ObjectStatus.Combust2,
                    ObjectStatus.Combust3,
                    ObjectStatus.Combust4,
                }
            },

            //重力    
            Gravity = new (3615),

            //吉星
            Benefic = new (3594, true),

            //福星
            Benefic2 = new (3610, true),

            //吉星相位
            AspectedBenefic = new (3595, true)
            {
                TargetStatus = new ushort[] { ObjectStatus.AspectedBenefic },
            },

            //先天禀赋
            EssentialDignity = new (3614, true),

            //星位合图
            Synastry = new (3612, true),

            //天星交错
            CelestialIntersection = new (16556, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,

                TargetStatus = new ushort[] { ObjectStatus.Intersection },
            },

            //擢升
            Exaltation = new (25873, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //阳星
            Helios = new (3600, true),

            //阳星相位
            AspectedHelios = new (3601, true)
            {
                BuffsProvide = new ushort[] { ObjectStatus.AspectedHelios },
            },

            //天星冲日
            CelestialOpposition = new (16553, true),

            //地星
            EarthlyStar = new (7439, true),

            //命运之轮 减伤，手动放。
            CollectiveUnconscious = new (3613, true),

            //天宫图
            Horoscope = new (16557, true),

            //光速
            Lightspeed = new (3606),

            //中间学派
            NeutralSect = new (16559),

            //大宇宙
            Macrocosmos = new (25874),

            //星力
            Astrodyne = new (25870)
            {
                OtherCheck = b =>
                {
                    if (JobGauge.Seals.Length != 3) return false;
                    if (JobGauge.Seals.Contains(SealType.NONE)) return false;
                    return true;
                },
            },

            //占卜
            Divination = new (16552, true),

            //抽卡
            Draw = new (3590),

            //重抽
            Redraw = new (3593)
            {
                BuffsNeed = new [] { ObjectStatus.ClarifyingDraw },
            },

            //小奥秘卡
            MinorArcana = new (7443),

            //出王冠卡
            CrownPlay = new (25869),

            //太阳神之衡
            Balance = new (4401)
            {
                ChoiceTarget = TargetFilter.ASTMeleeTarget,
            },

            //放浪神之箭
            Arrow = new (4402)
            {
                ChoiceTarget = TargetFilter.ASTMeleeTarget,
            },

            //战争神之枪
            Spear = new (4403)
            {
                ChoiceTarget = TargetFilter.ASTMeleeTarget,
            },

            //世界树之干
            Bole = new (4404)
            {
                ChoiceTarget = TargetFilter.ASTRangeTarget,
            },

            //河流神之瓶
            Ewer = new (4405)
            {
                ChoiceTarget = TargetFilter.ASTRangeTarget,
            },

            //建筑神之塔
            Spire = new (4406)
            {
                ChoiceTarget = TargetFilter.ASTRangeTarget,
            };
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.范围治疗, $"GCD: {Actions.AspectedHelios.Action.Name}, {Actions.Helios.Action.Name}\n                     能力: {Actions.EarthlyStar.Action.Name}, {Actions.CrownPlay.Action.Name}, {Actions.CelestialOpposition.Action.Name}"},
        {DescType.单体治疗, $"GCD: {Actions.AspectedBenefic.Action.Name}, {Actions.Benefic2.Action.Name}, {Actions.Benefic.Action.Name}\n                     能力: {Actions.CelestialIntersection.Action.Name}, {Actions.EssentialDignity.Action.Name}"},
        {DescType.范围防御, $"{Actions.CollectiveUnconscious.Action.Name}"},
        {DescType.单体防御, $"{Actions.Exaltation.Action.Name}，给被挨打的T"},
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("AutoDivination", true, "自动使用占卜");
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (Config.GetBoolByName("AutoDivination"))
        {
            //团队增伤害,占卜
            if (Actions.Divination.ShouldUse(out act)) return true;
        }

        act = null!;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //天星交错
        if (Actions.CelestialIntersection.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //给T减伤，这个很重要。
        if (Actions.Exaltation.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //来个命运之轮
        if (Actions.CollectiveUnconscious.ShouldUse(out act)) return true;

        return base.DefenceAreaAbility(abilityRemain, out act);
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //群体输出
        if (Actions.Gravity.ShouldUse(out act)) return true;

        //单体输出
        if (Actions.Combust.ShouldUse(out act)) return true;
        if (Actions.Malefic.ShouldUse(out act)) return true;
        if (Actions.Combust.ShouldUse(out act, mustUse: IsMoving && HaveHostileInRange)) return true;
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

    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        //阳星相位
        if (Actions.AspectedHelios.ShouldUse(out act)) return true;

        //阳星
        if (Actions.Helios.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //如果要群奶了，先上个天宫图！
        if (nextGCD.IsAnySameAction(true, Actions.AspectedHelios, Actions.Helios))
        {
            if (Actions.Horoscope.ShouldUse(out act)) return true;

            //中间学派
            if (Actions.NeutralSect.ShouldUse(out act)) return true;
        }

        //如果要单奶了，先上星位合图！
        if (nextGCD.IsAnySameAction(true, Actions.Benefic, Actions.Benefic2 , Actions.AspectedBenefic))
        {
            if (Actions.Synastry.ShouldUse(out act)) return true;
        }
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //如果当前还没有卡牌，那就抽一张
        if (JobGauge.DrawnCard == CardType.NONE
            && Actions.Draw.ShouldUse(out act)) return true;

        bool canUse = Actions.Astrodyne.OtherCheck(Service.ClientState.LocalPlayer);

        //如果当前卡牌已经拥有了，就重抽
        if (!canUse && JobGauge.DrawnCard != CardType.NONE && JobGauge.Seals.Contains(GetCardSeal(JobGauge.DrawnCard))
            && Actions.Redraw.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //吉星相位
        if (Actions.AspectedBenefic.Target.GetHealthRatio() > 0.4
            && Actions.AspectedBenefic.ShouldUse(out act)) return true;

        //福星
        if (Actions.Benefic2.ShouldUse(out act)) return true;

        //吉星
        if (Actions.Benefic.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //如果当前还没有皇冠卡牌，那就抽一张
        if (Actions.MinorArcana.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //如果当前还没有卡牌，那就抽一张
        if (JobGauge.DrawnCard == CardType.NONE
            && Actions.Draw.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //光速，创造更多的内插能力技的机会。
        if (IsMoving && Actions.Lightspeed.ShouldUse(out act)) return true;


        if (!IsMoving)
        {
            //如果没有地星也没有巨星，那就试试看能不能放个。
            if (!LocalPlayer.HaveStatus(ObjectStatus.EarthlyDominance, ObjectStatus.GiantDominance))
            {
                if (Actions.EarthlyStar.ShouldUse(out act, mustUse: true)) return true;
            }
            //加星星的进攻Buff
            if (Actions.Astrodyne.ShouldUse(out act)) return true;
        }

        if (JobGauge.DrawnCrownCard == CardType.LORD || Actions.MinorArcana.RecastTimeRemain < 3)
        {
            //进攻牌，随便发。或者CD要转好了，赶紧发掉。
            if (Actions.CrownPlay.ShouldUse(out act)) return true;
        }

        //发牌
        if (abilityRemain == 1 && JobGauge.DrawnCard != CardType.NONE && JobGauge.Seals.Contains(SealType.NONE))
        {
            switch (JobGauge.DrawnCard)
            {
                case CardType.BALANCE:
                    if (Actions.Balance.ShouldUse(out act)) return true;
                    break;
                case CardType.ARROW:
                    if (Actions.Arrow.ShouldUse(out act)) return true;
                    break;
                case CardType.SPEAR:
                    if (Actions.Spear.ShouldUse(out act)) return true;
                    break;
                case CardType.BOLE:
                    if (Actions.Bole.ShouldUse(out act)) return true;
                    break;
                case CardType.EWER:
                    if (Actions.Ewer.ShouldUse(out act)) return true;
                    break;
                case CardType.SPIRE:
                    if (Actions.Spire.ShouldUse(out act)) return true;
                    break;
            }
        }

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.EssentialDignity.Target.GetHealthRatio() < 0.4
            && Actions.EssentialDignity.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        //常规奶
        if (Actions.EssentialDignity.ShouldUse(out act)) return true;
        //带盾奶
        if (Actions.CelestialIntersection.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //奶量牌，要看情况。
        if (JobGauge.DrawnCrownCard == CardType.LADY && Actions.CrownPlay.ShouldUse(out act)) return true;

        var tank = TargetUpdater.PartyTanks;
        var isBoss = Actions.Malefic.IsTargetBoss;
        if (Actions.EssentialDignity.IsCoolDown && tank.Length == 1 && tank.Any(t => t.GetHealthRatio() < 0.5) && !isBoss)
        {
            //群Hot
            if (Actions.CelestialOpposition.ShouldUse(out act)) return true;

            //如果有巨星主宰
            if (LocalPlayer.HaveStatus(ObjectStatus.GiantDominance))
            {
                //需要回血的时候炸了。
                act = Actions.EarthlyStar;
                return true;
            }

            //天宫图
            if (!LocalPlayer.HaveStatus(ObjectStatus.HoroscopeHelios, ObjectStatus.Horoscope) && Actions.Horoscope.ShouldUse(out act)) return true;
            //阳星天宫图
            if (LocalPlayer.HaveStatus(ObjectStatus.HoroscopeHelios) && Actions.Horoscope.ShouldUse(out act)) return true;
            //超紧急情况天宫图
            if (tank.Any(t => t.GetHealthRatio() < 0.3) && Actions.Horoscope.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //群Hot
        if (Actions.CelestialOpposition.ShouldUse(out act)) return true;

        //如果有巨星主宰
        if (LocalPlayer.HaveStatus(ObjectStatus.GiantDominance))
        {
            //需要回血的时候炸了。
            act = Actions.EarthlyStar;
            return true;
        }

        //天宫图
        if (LocalPlayer.HaveStatus(ObjectStatus.HoroscopeHelios) && Actions.Horoscope.ShouldUse(out act)) return true;

        //奶量牌，要看情况。
        if (JobGauge.DrawnCrownCard == CardType.LADY && Actions.CrownPlay.ShouldUse(out act)) return true;

        return false;
    }

    private static SealType GetCardSeal(CardType card)
    {
        switch (card)
        {
            default: return SealType.NONE;

            case CardType.BALANCE:
            case CardType.BOLE:
                return SealType.SUN;

            case CardType.ARROW:
            case CardType.EWER:
                return SealType.MOON;

            case CardType.SPEAR:
            case CardType.SPIRE:
                return SealType.CELESTIAL;
        }
    }
}
