using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal abstract class ASTCombo : CustomComboJob<ASTGauge>
{
    internal struct Actions
    {
        public static readonly BaseAction
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
                BuffsProvide = new ushort[] {ObjectStatus.AspectedHelios},
            },

            //天星冲日
            CelestialOpposition = new BaseAction(16553, true),

            //地星
            EarthlyStar = new BaseAction(7439, true),

            //命运之轮 减伤，手动放。
            CollectiveUnconscious = new BaseAction(3613),

            //天宫图
            Horoscope = new BaseAction(16557, true),

            //生辰
            Ascend = new BaseAction(3603, true)
            {
                OtherCheck = () => TargetHelper.DeathPeopleAll.Length > 0,
            },


            //光速
            Lightspeed = new BaseAction(3606),

            //中间学派
            NeutralSect = new BaseAction(16559),

            //大宇宙
            Macrocosmos = new BaseAction(25874),

            //星力
            Astrodyne = new BaseAction(25870)
            {
                OtherCheck = () =>
                {
                    if (JobGauge.Seals.Length != 3) return false;
                    foreach (var item in JobGauge.Seals)
                    {
                        if(item == SealType.NONE) return false;
                    }
                    return true;
                },
            },

            //占卜
            Divination = new BaseAction(16552),

            //抽卡
            Draw = new BaseAction(3590),

            //出卡
            Play = new BaseAction(17055),

            //重抽
            Redraw = new BaseAction(3593)
            {
                BuffsNeed = new ushort[] { ObjectStatus.ClarifyingDraw},
            },

            //小奥秘卡
            MinorArcana = new BaseAction(7443),

            //出王冠卡
            CrownPlay = new BaseAction(25869),

            //太阳神之衡
            Balance = new BaseAction(4401),

            //放浪神之箭
            Arrow = new BaseAction(4402),

            //战争神之枪
            Spear = new BaseAction(4403),

            //世界树之干
            Bole = new BaseAction(4404),

            //河流神之瓶
            Ewer = new BaseAction(4405),

            //建筑神之塔
            Spire = new BaseAction(4406);
    }

    private protected override bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //有某些非常危险的状态。
        if (TargetHelper.DyingPeople.Length > 0)
        {
            if (GeneralActions.Esuna.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //有人死了，看看能不能救。
        if (TargetHelper.DeathPeopleParty.Length > 0)
        {
            //如果有人倒了，赶紧即刻拉人！
            if (!GeneralActions.Swiftcast.CoolDown.IsCooldown || HaveSwift)
            {
                if (Actions.Ascend.TryUseAction(level, out act, mustUse: true)) return true;
            }
        }

        act = null;
        return false;
    }

    private protected override bool AttackGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //大宇宙
        if (Actions.Macrocosmos.TryUseAction(level, out act, mustUse: true)) return true;
        //群体输出
        if (Actions.Gravity.TryUseAction(level, out act)) return true;

        //单体输出
        if (Actions.Combust.TryUseAction(level, out act)) return true;
        if (Actions.Malefic.TryUseAction(level, out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealAreaGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //阳星相位
        if (Actions.AspectedHelios.TryUseAction(level, out act)) return true;

        //阳星
        if (Actions.Helios.TryUseAction(level, out act)) return true;

        act = null;
        return false;
    }

    private protected override bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (base.FirstActionAbility(level, abilityRemain, nextGCD, out act)) return true;

        //如果要群奶了，先上个天宫图！
        if (nextGCD == Actions.AspectedHelios || nextGCD == Actions.Helios)
        {
            if (Actions.Horoscope.TryUseAction(level, out act)) return true;
        }

        //如果要单奶了，先上星位合图！
        if (nextGCD == Actions.Benefic || nextGCD == Actions.Benefic2 || nextGCD == Actions.AspectedBenefic)
        {
            if (Actions.Synastry.TryUseAction(level, out act)) return true;
        }
        return false;
    }

    private protected override bool GeneralAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //如果当前还没有皇冠卡牌，那就抽一张
        if (Actions.MinorArcana.TryUseAction(level, out act, Empty: true)) return true;

        //如果当前还没有卡牌，那就抽一张
        if (JobGauge.DrawnCard == CardType.NONE
            && Actions.Draw.TryUseAction(level, out act, Empty: true)) return true;

        //如果当前卡牌已经拥有了，就重抽
        if (JobGauge.DrawnCard != CardType.NONE && JobGauge.Seals.Contains(GetCardSeal(JobGauge.DrawnCard))
            && (JobGauge.Seals.Length < 3 || !JobGauge.Seals.Contains(SealType.NONE)) && Actions.Redraw.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //吉星相位
        if (Actions.AspectedBenefic.TryUseAction(level, out act)) return true;

        //福星
        if (Actions.Benefic2.TryUseAction(level, out act)) return true;

        //吉星
        if (Actions.Benefic.TryUseAction(level, out act)) return true;

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //光速，创造更多的内插能力技的机会。
        if (Actions.Lightspeed.TryUseAction(level, out act)) return true;
        //给T减伤，这个很重要。
        if (Actions.Exaltation.TryUseAction(level, out act)) return true;

        //团队增伤害
        if (Actions.Divination.TryUseAction(level, out act)) return true;

        //如果没有地星也没有巨星，那就试试看能不能放个。
        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.EarthlyDominance)
            && !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.GiantDominance))
        {
            if (!IsMoving && Actions.EarthlyStar.TryUseAction(level, out act)) return true;
        }

        if (JobGauge.DrawnCrownCard == CardType.LORD)
        {
            //进攻牌，随便发。
            if (Actions.CrownPlay.TryUseAction(level, out act)) return true;
        }

        //加星星的进攻Buff
        if (Actions.Astrodyne.TryUseAction(level, out act)) return true;

        //发牌
        if (JobGauge.DrawnCard != CardType.NONE && Actions.Play.TryUseAction(level, out act, mustUse: true)) return true;

        //加个醒梦
        if (GeneralActions.LucidDreaming.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //带盾奶
        if (Actions.CelestialIntersection.TryUseAction(level, out act)) return true;
        //单体Hot
        if (Actions.AspectedBenefic.TryUseAction(level, out act)) return true;
        //常规奶
        if (Actions.EssentialDignity.TryUseAction(level, out act)) return true;
        //带盾奶
        if (Actions.CelestialIntersection.TryUseAction(level, out act, Empty: true)) return true;

        return false;
    }

    private protected override bool HealAreaAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //如果有巨星主宰
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.GiantDominance))
        {
            //需要回血的时候炸了。
            if (Actions.EarthlyStar.TryUseAction(level, out act)) return true;
        }
        //奶量牌，要看情况。
        if (JobGauge.DrawnCrownCard == CardType.LADY && Actions.CrownPlay.TryUseAction(level, out act)) return true;
        //群Hot
        if (Actions.CelestialOpposition.TryUseAction(level, out act)) return true;

        return false;
    }

    private static SealType GetCardSeal(CardType card)
    {
        switch (card)
        {
            default:return SealType.NONE;

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
