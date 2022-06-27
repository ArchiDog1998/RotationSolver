using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVAutoAttack.Combos.Healer;

internal class ASTCombo : CustomComboJob<ASTGauge>
{
    internal override uint JobID => 33;

    private protected override BaseAction Raise => Actions.Ascend;

    internal struct Actions
    {
        public static readonly BaseAction
            //生辰
            Ascend = new BaseAction(3603, true),

            //凶星
            Malefic = new BaseAction(3596),

            //烧灼
            Combust = new BaseAction(3599)
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
            Gravity = new BaseAction(3615),

            //吉星
            Benefic = new BaseAction(3594, true),

            //福星
            Benefic2 = new BaseAction(3610, true),

            //吉星相位
            AspectedBenefic = new BaseAction(3595, true)
            {
                TargetStatus = new ushort[] { ObjectStatus.AspectedBenefic },
            },

            //先天禀赋
            EssentialDignity = new BaseAction(3614, true),


            //星位合图
            Synastry = new BaseAction(3612, true),

            //天星交错
            CelestialIntersection = new BaseAction(16556, true),

            //擢升
            Exaltation = new BaseAction(25873, true),

            //阳星
            Helios = new BaseAction(3600, true),

            //阳星相位
            AspectedHelios = new BaseAction(3601, true)
            {
                BuffsProvide = new ushort[] { ObjectStatus.AspectedHelios },
            },

            //天星冲日
            CelestialOpposition = new BaseAction(16553, true),

            //地星
            EarthlyStar = new BaseAction(7439, true),

            //命运之轮 减伤，手动放。
            CollectiveUnconscious = new BaseAction(3613, true),

            //天宫图
            Horoscope = new BaseAction(16557, true),

            //光速
            Lightspeed = new BaseAction(3606),

            //中间学派
            NeutralSect = new BaseAction(16559),

            //大宇宙
            Macrocosmos = new BaseAction(25874),

            //星力
            Astrodyne = new BaseAction(25870)
            {
                OtherCheck = b =>
                {
                    if (JobGauge.Seals.Length != 3) return false;
                    if (JobGauge.Seals.Contains(SealType.NONE)) return false;
                    return true;
                },
            },

            //占卜
            Divination = new BaseAction(16552, true),

            //抽卡
            Draw = new BaseAction(3590),

            //重抽
            Redraw = new BaseAction(3593)
            {
                BuffsNeed = new ushort[] { ObjectStatus.ClarifyingDraw },
            },

            //小奥秘卡
            MinorArcana = new BaseAction(7443),

            //出王冠卡
            CrownPlay = new BaseAction(25869),

            //太阳神之衡
            Balance = new BaseAction(4401)
            {
                ChoiceFriend = ASTMeleeTarget,
            },

            //放浪神之箭
            Arrow = new BaseAction(4402)
            {
                ChoiceFriend = ASTMeleeTarget,
            },

            //战争神之枪
            Spear = new BaseAction(4403)
            {
                ChoiceFriend = ASTMeleeTarget,
            },

            //世界树之干
            Bole = new BaseAction(4404)
            {
                ChoiceFriend = ASTRangeTarget,
            },

            //河流神之瓶
            Ewer = new BaseAction(4405)
            {
                ChoiceFriend = ASTRangeTarget,
            },

            //建筑神之塔
            Spire = new BaseAction(4406)
            {
                ChoiceFriend = ASTRangeTarget,
            };
    }
    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.范围治疗, $"GCD: {Actions.AspectedHelios.Action.Name}, {Actions.Helios.Action.Name}\n                     能力: {Actions.EarthlyStar.Action.Name}, {Actions.CrownPlay.Action.Name}, {Actions.CelestialOpposition.Action.Name}"},
        {DescType.单体治疗, $"GCD: {Actions.AspectedBenefic.Action.Name}, {Actions.Benefic2.Action.Name}, {Actions.Benefic.Action.Name}\n                     能力: {Actions.CelestialIntersection.Action.Name}, {Actions.EssentialDignity.Action.Name}"},
        {DescType.范围防御, $"{Actions.CollectiveUnconscious.Action.Name}"},
        {DescType.单体防御, $"{Actions.Exaltation.Action.Name}，给被挨打的T"},
    };

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //给T减伤，这个很重要。
        if (Actions.Exaltation.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //来个命运之轮
        if (IconReplacer.DefenseArea && Actions.CollectiveUnconscious.ShouldUseAction(out act)) return true;

        //大宇宙
        if (Actions.Macrocosmos.ShouldUseAction(out act, mustUse: true)) return true;
        //群体输出
        if (Actions.Gravity.ShouldUseAction(out act)) return true;

        //单体输出
        if (Actions.Combust.ShouldUseAction(out act)) return true;
        if (Actions.Malefic.ShouldUseAction(out act)) return true;
        if (Actions.Combust.ShouldUseAction(out act, mustUse: IsMoving && HaveTargetAngle)) return true;

        act = null;
        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        //阳星相位
        if (Actions.AspectedHelios.ShouldUseAction(out act)) return true;

        //阳星
        if (Actions.Helios.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //来个命运之轮
        if (IconReplacer.DefenseArea && Actions.CollectiveUnconscious.ShouldUseAction(out act)) return true;

        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //如果要群奶了，先上个天宫图！
        if (nextGCD == Actions.AspectedHelios || nextGCD == Actions.Helios)
        {
            if (Actions.Horoscope.ShouldUseAction(out act)) return true;
        }

        //如果要单奶了，先上星位合图！
        if (nextGCD == Actions.Benefic || nextGCD == Actions.Benefic2 || nextGCD == Actions.AspectedBenefic)
        {
            if (Actions.Synastry.ShouldUseAction(out act)) return true;
        }
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //如果当前还没有卡牌，那就抽一张
        if (JobGauge.DrawnCard == CardType.NONE
            && Actions.Draw.ShouldUseAction(out act)) return true;

        bool canUse = Actions.Astrodyne.OtherCheck(Service.ClientState.LocalPlayer);

        //如果当前卡牌已经拥有了，就重抽
        if (!canUse && JobGauge.DrawnCard != CardType.NONE && JobGauge.Seals.Contains(GetCardSeal(JobGauge.DrawnCard))
            && Actions.Redraw.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //吉星相位
        if (Actions.AspectedBenefic.ShouldUseAction(out act)) return true;

        //福星
        if (Actions.Benefic2.ShouldUseAction(out act)) return true;

        //吉星
        if (Actions.Benefic.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //如果当前还没有皇冠卡牌，那就抽一张
        if (Actions.MinorArcana.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        //如果当前还没有卡牌，那就抽一张
        if (JobGauge.DrawnCard == CardType.NONE
            && Actions.Draw.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        //光速，创造更多的内插能力技的机会。
        if (Actions.Lightspeed.ShouldUseAction(out act)) return true;


        if (!IsMoving)
        {
            //团队增伤害
            if (Actions.Divination.ShouldUseAction(out act)) return true;

            //如果没有地星也没有巨星，那就试试看能不能放个。
            if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.EarthlyDominance)
                && !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.GiantDominance))
            {
                if (Actions.EarthlyStar.ShouldUseAction(out act, mustUse: true)) return true;
            }
            //加星星的进攻Buff
            if (Actions.Astrodyne.ShouldUseAction(out act)) return true;
        }

        if (JobGauge.DrawnCrownCard == CardType.LORD || Actions.MinorArcana.RecastTimeRemain < 3)
        {
            //进攻牌，随便发。或者CD要转好了，赶紧发掉。
            if (Actions.CrownPlay.ShouldUseAction(out act)) return true;
        }

        //发牌
        if (abilityRemain == 1 && JobGauge.DrawnCard != CardType.NONE && JobGauge.Seals.Contains(SealType.NONE))
        {
            switch (JobGauge.DrawnCard)
            {
                case CardType.BALANCE:
                    if (Actions.Balance.ShouldUseAction(out act)) return true;
                    break;
                case CardType.ARROW:
                    if (Actions.Arrow.ShouldUseAction(out act)) return true;
                    break;
                case CardType.SPEAR:
                    if (Actions.Spear.ShouldUseAction(out act)) return true;
                    break;
                case CardType.BOLE:
                    if (Actions.Bole.ShouldUseAction(out act)) return true;
                    break;
                case CardType.EWER:
                    if (Actions.Ewer.ShouldUseAction(out act)) return true;
                    break;
                case CardType.SPIRE:
                    if (Actions.Spire.ShouldUseAction(out act)) return true;
                    break;
            }
        }

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //带盾奶
        if (Actions.CelestialIntersection.ShouldUseAction(out act)) return true;
        //常规奶
        if (Actions.EssentialDignity.ShouldUseAction(out act)) return true;
        //带盾奶
        if (Actions.CelestialIntersection.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //如果有巨星主宰
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.GiantDominance))
        {
            //需要回血的时候炸了。
            act = Actions.EarthlyStar;
            return true;
        }
        //奶量牌，要看情况。
        if (JobGauge.DrawnCrownCard == CardType.LADY && Actions.CrownPlay.ShouldUseAction(out act)) return true;
        //群Hot
        if (Actions.CelestialOpposition.ShouldUseAction(out act)) return true;

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

    private static BattleChara ASTRangeTarget(BattleChara[] ASTTargets)
    {
        ASTTargets = ASTTargets.Where(b => b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0).ToArray();


        var targets = GetASTTargets(TargetHelper.GetJobCategory(ASTTargets, Role.远程));
        if (targets.Length > 0) return RandomObject(targets);

        targets = GetASTTargets(TargetHelper.GetJobCategory(ASTTargets, Role.近战));
        if (targets.Length > 0) return RandomObject(targets);

        targets = GetASTTargets(ASTTargets);
        if (targets.Length > 0) return RandomObject(targets);

        return null;
    }

    private static BattleChara ASTMeleeTarget(BattleChara[] ASTTargets)
    {
        ASTTargets = ASTTargets.Where(b => b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0).ToArray();

        var targets = GetASTTargets(TargetHelper.GetJobCategory(ASTTargets, Role.近战));
        if (targets.Length > 0) return RandomObject(targets);

        targets = GetASTTargets(TargetHelper.GetJobCategory(ASTTargets, Role.远程));
        if (targets.Length > 0) return RandomObject(targets);

        targets = GetASTTargets(ASTTargets);
        if (targets.Length > 0) return RandomObject(targets);

        return null;
    }

    private static BattleChara[] GetASTTargets(BattleChara[] sources)
    {
        var allStatus = new uint[]
        {
            ObjectStatus.TheArrow,
            ObjectStatus.TheBalance,
            ObjectStatus.TheBole,
            ObjectStatus.TheEwer,
            ObjectStatus.TheSpear,
            ObjectStatus.TheSpire,
        };
        return sources.Where((t) =>
        {
            foreach (Status status in t.StatusList)
            {
                if (allStatus.Contains(status.StatusId) && status.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                {
                    return false;
                }
            }
            return true;
        }).ToArray();
    }

    internal static BattleChara RandomObject(BattleChara[] objs)
    {
        Random ran = new Random(DateTime.Now.Millisecond);
        return objs[ran.Next(objs.Length)];
    }
}
