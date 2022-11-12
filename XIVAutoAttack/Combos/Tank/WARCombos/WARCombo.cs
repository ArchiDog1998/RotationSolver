using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Tank.WARCombos;

internal abstract class WARCombo<TCmd> : JobGaugeCombo<WARGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Warrior, ClassJobID.Marauder };
    internal sealed override bool HaveShield => Player.HaveStatusFromSelf(StatusID.Defiance);
    private sealed protected override BaseAction Shield => Defiance;

    public static readonly BaseAction
        //守护
        Defiance = new(48, shouldEndSpecial: true),

        //重劈
        HeavySwing = new(31),

        //凶残裂
        Maim = new(37),

        //暴风斩 绿斧
        StormsPath = new(42),

        //暴风碎 红斧
        StormsEye = new(45)
        {
            OtherCheck = b => Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest),
        },

        //飞斧
        Tomahawk = new(46)
        {
            FilterForTarget = b => TargetFilter.ProvokeTarget(b),
        },

        //猛攻
        Onslaught = new(7386, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //动乱    
        Upheaval = new(7387)
        {
            BuffsNeed = new StatusID[] { StatusID.SurgingTempest },
        },

        //超压斧
        Overpower = new(41),

        //秘银暴风
        MythrilTempest = new(16462),

        //群山隆起
        Orogeny = new(25752),

        //原初之魂
        InnerBeast = new(49)
        {
            OtherCheck = b => !Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest) && (JobGauge.BeastGauge >= 50 || Player.HaveStatusFromSelf(StatusID.InnerRelease)),
        },

        //钢铁旋风
        SteelCyclone = new(51)
        {
            OtherCheck = InnerBeast.OtherCheck,
        },

        //战嚎
        Infuriate = new(52)
        {
            BuffsProvide = new[] { StatusID.InnerRelease },
            OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge < 50,
        },

        //狂暴
        Berserk = new(38)
        {
            OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0,
        },

        //战栗
        ThrillofBattle = new(40),

        //泰然自若
        Equilibrium = new(3552),

        //原初的勇猛
        NascentFlash = new(16464)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        ////原初的血气
        //Bloodwhetting = new BaseAction(25751),

        //复仇
        Vengeance = new(44)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //原初的直觉
        RawIntuition = new(3551)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //摆脱
        ShakeItOff = new(7388, true),

        //死斗
        Holmgang = new(43)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        ////原初的解放
        //InnerRelease = new BaseAction(7389),

        //蛮荒崩裂
        PrimalRend = new(25753)
        {
            BuffsNeed = new[] { StatusID.PrimalRendReady },
        };
}
