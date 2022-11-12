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
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Healer.ASTCombos;

internal abstract class ASTCombo<TCmd> : JobGaugeCombo<ASTGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Astrologian };

    private sealed protected override BaseAction Raise => Ascend;


    public static readonly BaseAction
        //生辰
        Ascend = new(3603, true),

        //凶星
        Malefic = new(3596),

        //烧灼
        Combust = new(3599, isEot: true)
        {
            TargetStatus = new StatusID[]
            {
                StatusID.Combust,
                StatusID.Combust2,
                StatusID.Combust3,
                StatusID.Combust4,
            }
        },

        //重力    
        Gravity = new(3615),

        //吉星
        Benefic = new(3594, true),

        //福星
        Benefic2 = new(3610, true),

        //吉星相位
        AspectedBenefic = new(3595, true, isEot: true)
        {
            TargetStatus = new StatusID[] { StatusID.AspectedBenefic },
        },

        //先天禀赋
        EssentialDignity = new(3614, true),

        //星位合图
        Synastry = new(3612, true),

        //天星交错
        CelestialIntersection = new(16556, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,

            TargetStatus = new StatusID[] { StatusID.Intersection },
        },

        //擢升
        Exaltation = new(25873, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //阳星
        Helios = new(3600, true),

        //阳星相位
        AspectedHelios = new(3601, true, isEot: true)
        {
            BuffsProvide = new StatusID[] { StatusID.AspectedHelios },
        },

        //天星冲日
        CelestialOpposition = new(16553, true),

        //地星
        EarthlyStar = new(7439, true),

        //命运之轮 减伤，手动放。
        CollectiveUnconscious = new(3613, true),

        //天宫图
        Horoscope = new(16557, true),

        //光速
        Lightspeed = new(3606),

        //中间学派
        NeutralSect = new(16559),

        //大宇宙
        Macrocosmos = new(25874),

        //星力
        Astrodyne = new(25870)
        {
            OtherCheck = b =>
            {
                if (JobGauge.Seals.Length != 3) return false;
                if (JobGauge.Seals.Contains(SealType.NONE)) return false;
                return true;
            },
        },

        //占卜
        Divination = new(16552, true),

        //抽卡
        Draw = new(3590),

        //重抽
        Redraw = new(3593)
        {
            BuffsNeed = new[] { StatusID.ClarifyingDraw },
        },

        //小奥秘卡
        MinorArcana = new(7443),

        //出王冠卡
        CrownPlay = new(25869),

        //太阳神之衡
        Balance = new(4401)
        {
            ChoiceTarget = TargetFilter.ASTMeleeTarget,
        },

        //放浪神之箭
        Arrow = new(4402)
        {
            ChoiceTarget = TargetFilter.ASTMeleeTarget,
        },

        //战争神之枪
        Spear = new(4403)
        {
            ChoiceTarget = TargetFilter.ASTMeleeTarget,
        },

        //世界树之干
        Bole = new(4404)
        {
            ChoiceTarget = TargetFilter.ASTRangeTarget,
        },

        //河流神之瓶
        Ewer = new(4405)
        {
            ChoiceTarget = TargetFilter.ASTRangeTarget,
        },

        //建筑神之塔
        Spire = new(4406)
        {
            ChoiceTarget = TargetFilter.ASTRangeTarget,
        };

}
