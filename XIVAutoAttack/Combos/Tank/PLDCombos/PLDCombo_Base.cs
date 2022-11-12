using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Tank.PLDCombos;

internal abstract class PLDCombo_Base<TCmd> : JobGaugeCombo<PLDGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Paladin, ClassJobID.Gladiator };

    internal sealed override bool HaveShield => Player.HaveStatus(StatusID.IronWill);

    private sealed protected override BaseAction Shield => IronWill;

    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;


    public static readonly BaseAction
        //钢铁信念
        IronWill = new(28, shouldEndSpecial: true),

        //先锋剑
        FastBlade = new(9),

        //暴乱剑
        RiotBlade = new(15),

        //沥血剑
        GoringBlade = new(3538, isEot: true)
        {
            TargetStatus = new[]
            {
                    StatusID.GoringBlade,
                    StatusID.BladeofValor,
            }
        },

        //战女神之怒
        RageofHalone = new(21),

        //王权剑
        RoyalAuthority = new(3539),

        //投盾
        ShieldLob = new(24)
        {
            FilterForTarget = b => TargetFilter.ProvokeTarget(b),
        },

        //战逃反应
        FightorFlight = new(20)
        {
            OtherCheck = b =>
            {
                return true;
            },
        },

        //全蚀斩
        TotalEclipse = new(7381),

        //日珥斩
        Prominence = new(16457),

        //预警
        Sentinel = new(17)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //厄运流转
        CircleofScorn = new(23)
        {
            //OtherCheck = b =>
            //{
            //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

            //    if (FightorFlight.IsCoolDown) return true;

            //    return false;
            //}
        },

        //深奥之灵
        SpiritsWithin = new(29)
        {
            //OtherCheck = b =>
            //{
            //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

            //    if (FightorFlight.IsCoolDown) return true;

            //    return false;
            //}
        },

        //神圣领域
        HallowedGround = new(30)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        //圣光幕帘
        DivineVeil = new(3540),

        //深仁厚泽
        Clemency = new(3541, true, true),

        //干预
        Intervention = new(7382, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //调停
        Intervene = new(16461, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //赎罪剑
        Atonement = new(16460)
        {
            BuffsNeed = new[] { StatusID.SwordOath },
        },

        //偿赎剑
        Expiacion = new(25747),

        //英勇之剑
        BladeofValor = new(25750),

        //真理之剑
        BladeofTruth = new(25749),

        //信念之剑
        BladeofFaith = new(25748)
        {
            BuffsNeed = new[] { StatusID.ReadyForBladeofFaith },
        },

        //安魂祈祷
        Requiescat = new(7383),

        //悔罪
        Confiteor = new(16459)
        {
            OtherCheck = b => Player.CurrentMp >= 1000,
        },

        //圣环
        HolyCircle = new(16458)
        {
            OtherCheck = b => Player.CurrentMp >= 2000,
        },

        //圣灵
        HolySpirit = new(7384)
        {
            OtherCheck = b => Player.CurrentMp >= 2000,
        },

        //武装戍卫
        PassageofArms = new(7385),

        //保护
        Cover = new BaseAction(27, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //盾阵
        Sheltron = new(3542);
    //盾牌猛击
    //ShieldBash = new BaseAction(16),
}
