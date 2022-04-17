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
                BuffsProvide = new ushort[] { ObjectStatus.AspectedBenefic },
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
            //CollectiveUnconscious = new BaseAction(3613),

            //天宫图
            Horoscope = new BaseAction(16557, true),

            //生辰
            Ascend = new BaseAction(3603, true),

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

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        uint act = 0;

        //有某些非常危险的状态。
        if (TargetHelper.DyingPeople.Length > 0)
        {
            if (GeneralActions.Esuna.TryUseAction(level, out act, mustUse: true)) return act;
        }

        //有人死了，看看能不能救。
        if (TargetHelper.DeathPeopleParty.Length != 0)
        {
            //如果有人倒了，赶紧即刻拉人！
            if (GeneralActions.Swiftcast.TryUseAction(level, out act, mustUse: true)) return act;

            bool haveSwift = false;
            foreach (var status in Service.ClientState.LocalPlayer.StatusList)
            {
                if (GeneralActions.Swiftcast.BuffsProvide.Contains((ushort)status.StatusId))
                {
                    haveSwift = true;
                    break;
                }
            }
            if (haveSwift && Actions.Ascend.TryUseAction(level, out act)) return act;
        }

        //在移动，还有光速，肯定用啊！
        if (IsMoving && Actions.Lightspeed.TryUseAction(level, out act)) return act;
        //在移动，没光速，需要瞬发。
        if (IsMoving && HaveTargetAngle && !BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.LightSpeed)))
        {
            if (Actions.Combust.TryUseAction(level, out act, mustUse: true)) return act;
        }

        //如果现在可以增加能力技
        if (CanAddAbility(level, false, false, out act)) return act;


        //攻击后奶。
        if (Actions.Macrocosmos.TryUseAction(level, out act, mustUse:true)) return act;
        //群体输出
        if (Actions.Gravity.TryUseAction(level, out act)) return act;

        //单体输出
        if (Actions.Combust.TryUseAction(level, out act)) return act;
        if (Actions.Malefic.TryUseAction(level, out act)) return act;

        return 0;
    }

    protected bool CanAddAbility(byte level, bool healSingle, bool healArea, out uint act)
    {
        act = 0;

        if (CanInsertAbility)
        {
            //光速，创造更多的内插能力技的机会。
            if (HaveTargetAngle && Actions.Lightspeed.TryUseAction(level, out act)) return true;
            //给T减伤，这个很重要。
            if (Actions.Exaltation.TryUseAction(level, out act)) return true;

            //团队曾伤害
            if(Actions.Divination.TryUseAction(level, out act)) return true;
            bool needAreaHeal  = healArea || TargetHelper.PartyMembersAverHP < 0.8 && TargetHelper.PartyMembersDifferHP < 0.3;

            //如果有巨星主宰
            if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.GiantDominance)))
            {
                //需要回血的时候炸了。
                if (needAreaHeal && Actions.EarthlyStar.TryUseAction(level, out act)) return true;
            }
            //如果没有地星也没有巨星，那就试试看能不能放个。
            else if (!BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.EarthlyDominance)))
            {
                if (!IsMoving && Actions.EarthlyStar.TryUseAction(level, out act)) return true;
            }

            #region 群奶
            if (needAreaHeal)
            {
                //群Hot
                if (Actions.CelestialOpposition.TryUseAction(level, out act)) return true;
            }
            #endregion

            #region 单奶
            bool haveOneNeedHeal = false;
            foreach (float rate in TargetHelper.PartyMembersHP)
            {
                if (rate < 0.7)
                {
                    haveOneNeedHeal = true;
                    break;
                }
            }

            if (healSingle || haveOneNeedHeal)
            {
                //带盾奶
                if (Actions.CelestialIntersection.TryUseAction(level, out act)) return true;
                //单体Hot
                if (Actions.AspectedBenefic.TryUseAction(level, out act)) return true;
                //常规奶
                if (Actions.EssentialDignity.TryUseAction(level, out act)) return true;

                if (Actions.CelestialIntersection.TryUseAction(level, out act, Empty:true)) return true;
            }

            #endregion

            //发牌
            if(DrawCard(level, false, out act)) return true;
            if(DrawCrownCard(level, out act)) return true;
            if(DrawCard(level, true, out act)) return true;

            //加个醒梦
            if (GeneralActions.LucidDreaming.TryUseAction(level, out act)) return true;

        }

        return false;
    }

    protected bool DrawCard(byte level, bool empty, out uint act)
    {
        //加Buff
        if (Actions.Astrodyne.TryUseAction(level, out act)) return true;

        //如果当前还没有卡牌，那就抽一张
        if (JobGauge.DrawnCard == CardType.NONE
            && Actions.Draw.TryUseAction(level, out act, Empty: empty)) return true;

        //如果当前卡牌已经拥有了，就重抽
        if (JobGauge.DrawnCard != CardType.NONE &&　JobGauge.Seals.Contains(GetCardSeal(JobGauge.DrawnCard))
            && Actions.Redraw.TryUseAction(level, out act)) return true;

        //有牌了，也不需要重抽，那就只能发出。
        if (JobGauge.DrawnCard != CardType.NONE && Actions.Play.TryUseAction(level, out act, mustUse:true)) return true;

        return false;
    }

    protected bool DrawCrownCard(byte level, out uint act)
    {
        act = 0;

        //如果当前还没有皇冠卡牌，那就抽一张
        if (JobGauge.DrawnCrownCard == CardType.NONE
            && Actions.MinorArcana.TryUseAction(level, out act, Empty: true)) return true;

        if(JobGauge.DrawnCrownCard == CardType.LADY)
        {
            //奶量牌，要看情况。
            if (TargetHelper.PartyMembersAverHP < 0.85 && TargetHelper.PartyMembersDifferHP < 0.3
                && Actions.CrownPlay.TryUseAction(level, out act)) return true;
        }
        else if (JobGauge.DrawnCrownCard == CardType.LORD)
        {
            //进攻牌，随便发。
            if (Actions.CrownPlay.TryUseAction(level, out act)) return true;
        }

        return false;
    }

    internal static SealType GetCardSeal(CardType card)
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
