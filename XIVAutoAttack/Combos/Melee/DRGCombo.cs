using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Healer;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.Melee.DRGCombo;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class DRGCombo : JobGaugeCombo<DRGGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    public override uint[] JobIDs => new uint[] { 22, 4 };
    private static bool safeMove = false;

    public static readonly BaseAction
        //精准刺
        TrueThrust = new(75),

        //贯通刺
        VorpalThrust = new(78) { OtherIDsCombo = new[] { 16479u } },

        //直刺
        FullThrust = new(84),

        //苍穹刺
        HeavensThrust = new(25771),

        //开膛枪
        Disembowel = new(87) { OtherIDsCombo = new[] { 16479u } },

        //樱花怒放
        ChaosThrust = new(88),

        //樱花怒放
        ChaoticSpring = new(25772),

        //龙牙龙爪
        FangandClaw = new(3554)
        {
            BuffsNeed = new ushort[] { ObjectStatus.SharperFangandClaw },
        },

        //龙尾大回旋
        WheelingThrust = new(3556)
        {
            BuffsNeed = new ushort[] { ObjectStatus.EnhancedWheelingThrust },
        },

        //龙眼雷电
        RaidenThrust = new(16479),

        //贯穿尖
        PiercingTalon = new(90),

        //死天枪
        DoomSpike = new(86),

        //音速刺
        SonicThrust = new(7397) { OtherIDsCombo = new[] { 25770u } },

        //山境酷刑
        CoerthanTorment = new(16477),

        //破碎冲
        SpineshatterDive = new(95)
        {
            OtherCheck = b =>
            {
                if (safeMove && b.DistanceToPlayer() > 2) return false;
                if (IsLastAction(true, SpineshatterDive)) return false;

                return true;
            }
        },

        //龙炎冲
        DragonfireDive = new(96)
        {
            OtherCheck = b => !safeMove || b.DistanceToPlayer() < 2,
        },

        //跳跃
        Jump = new(92)
        {
            BuffsProvide = new ushort[] { ObjectStatus.DiveReady },
            OtherCheck = b => (!safeMove || b.DistanceToPlayer() < 2) && Player.HaveStatus(ObjectStatus.PowerSurge),
        },
        //高跳
        HighJump = new(16478)
        {
            OtherCheck = Jump.OtherCheck,
        },
        //幻象冲
        MirageDive = new(7399)
        {
            BuffsNeed = new[] { ObjectStatus.DiveReady },

            OtherCheck = b => !Geirskogul.WillHaveOneChargeGCD(4)
        },

        //武神枪
        Geirskogul = new(3555)
        {
            OtherCheck = b => Jump.IsCoolDown || HighJump.IsCoolDown,
        },

        //死者之岸
        Nastrond = new(7400)
        {
            OtherCheck = b => JobGauge.IsLOTDActive,
        },

        //坠星冲
        Stardiver = new(16480)
        {
            OtherCheck = b => JobGauge.IsLOTDActive && JobGauge.LOTDTimer < 25000,
        },

        //天龙点睛
        WyrmwindThrust = new(25773)
        {
            OtherCheck = b => JobGauge.FirstmindsFocusCount == 2 && !IsLastAction(true, Stardiver),
        },

        //龙剑
        LifeSurge = new(83)
        {
            BuffsProvide = new[] { ObjectStatus.LifeSurge },

            OtherCheck = b => !IsLastAbility(true, LifeSurge),
        },

        //猛枪
        LanceCharge = new(85),

        //巨龙视线
        DragonSight = new(7398)
        {
            ChoiceTarget = Targets =>
            {
                Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
                b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0).ToArray();

                var targets = TargetFilter.GetJobCategory(Targets, Role.近战);
                if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                targets = TargetFilter.GetJobCategory(Targets, Role.远程);
                if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                targets = Targets;
                if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                return Player;
            },

            BuffsNeed = new[] { ObjectStatus.PowerSurge },

        },

        //战斗连祷
        BattleLitany = new(3557)
        {
            BuffsNeed = new[] { ObjectStatus.PowerSurge },
        };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("DRG_ShouldDelay", true, "延后红龙血")
            .SetBool("DRG_Opener", false, "88级起手")
            .SetBool("DRG_SafeMove", true, "安全位移");
    }

    public override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.移动技能, $"{SpineshatterDive}, {DragonfireDive}"},
    };

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain > 1)
        {
            if (SpineshatterDive.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            if (DragonfireDive.ShouldUse(out act, mustUse: true)) return true;
        }

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD.IsAnySameAction(true, FullThrust, CoerthanTorment)
            || Player.HaveStatus(ObjectStatus.LanceCharge) && nextGCD.IsAnySameAction(false, FangandClaw))
        {
            //龙剑
            if (abilityRemain == 1 && LifeSurge.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //猛枪
            if (LanceCharge.ShouldUse(out act, mustUse: true))
            {
                if (abilityRemain == 1 && !Player.HaveStatus(ObjectStatus.PowerSurge)) return true;
                if (Player.HaveStatus(ObjectStatus.PowerSurge)) return true;
            }

            //巨龙视线
            if (DragonSight.ShouldUse(out act, mustUse: true)) return true;

            //战斗连祷
            if (BattleLitany.ShouldUse(out act, mustUse: true)) return true;
        }

        //死者之岸
        if (Nastrond.ShouldUse(out act, mustUse: true)) return true;

        //坠星冲
        if (Stardiver.ShouldUse(out act, mustUse: true)) return true;

        //高跳
        if (HighJump.EnoughLevel)
        {
            if (HighJump.ShouldUse(out act)) return true;
        }
        else
        {
            if (Jump.ShouldUse(out act)) return true;
        }

        //尝试进入红龙血
        if (Geirskogul.ShouldUse(out act, mustUse: true)) return true;

        //破碎冲
        if (SpineshatterDive.ShouldUse(out act, emptyOrSkipCombo: true))
        {
            if (Player.HaveStatus(ObjectStatus.LanceCharge) && LanceCharge.ElapsedAfterGCD(3)) return true;
        }
        if (Player.HaveStatus(ObjectStatus.PowerSurge) && SpineshatterDive.ChargesCount != 1 && SpineshatterDive.ShouldUse(out act)) return true;

        //幻象冲
        if (MirageDive.ShouldUse(out act)) return true;

        //龙炎冲
        if (DragonfireDive.ShouldUse(out act, mustUse: true))
        {
            if (Player.HaveStatus(ObjectStatus.LanceCharge) && LanceCharge.ElapsedAfterGCD(3)) return true;
        }

        //天龙点睛
        if (WyrmwindThrust.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        safeMove = Config.GetBoolByName("DRG_SafeMove");

        #region 群伤
        if (CoerthanTorment.ShouldUse(out act)) return true;
        if (SonicThrust.ShouldUse(out act)) return true;
        if (DoomSpike.ShouldUse(out act)) return true;

        #endregion

        #region 单体
        if (Config.GetBoolByName("ShouldDelay"))
        {
            if (WheelingThrust.ShouldUse(out act)) return true;
            if (FangandClaw.ShouldUse(out act)) return true;
        }
        else
        {
            if (FangandClaw.ShouldUse(out act)) return true;
            if (WheelingThrust.ShouldUse(out act)) return true;
        }

        //看看是否需要续Buff
        if (!Player.WillStatusEndGCD(5, 0, true, ObjectStatus.PowerSurge))
        {
            if (FullThrust.ShouldUse(out act)) return true;
            if (VorpalThrust.ShouldUse(out act)) return true;
            if (ChaosThrust.ShouldUse(out act)) return true;
        }
        else
        {
            if (Disembowel.ShouldUse(out act)) return true;
        }
        if (TrueThrust.ShouldUse(out act)) return true;

        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (PiercingTalon.ShouldUse(out act)) return true;

        return false;

        #endregion
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }
}
