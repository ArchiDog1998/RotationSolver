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
using XIVAutoAttack.Controllers;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class DRGCombo : JobGaugeCombo<DRGGauge>
{
    internal override uint JobID => 22;
    private static bool inOpener = true;
    private static bool safeMove = false;

    internal struct Actions
    {
        public static readonly BaseAction
            //¾«×¼´Ì
            TrueThrust = new (75),

            //¹áÍ¨´Ì
            VorpalThrust = new (78) { OtherIDsCombo = new [] { 16479u } },

            //Ö±´Ì
            FullThrust = new (84),

            //²Ôñ·´Ì
            HeavensThrust = new (25771),

            //¿ªÌÅÇ¹
            Disembowel = new (87) { OtherIDsCombo = new [] { 16479u } },

            //Ó£»¨Å­·Å
            ChaosThrust = new (88),

            //Ó£»¨Å­·Å
            ChaoticSpring = new(25772),

            //ÁúÑÀÁú×¦
            FangandClaw = new (3554)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SharperFangandClaw },
            },

            //ÁúÎ²´ó»ØÐý
            WheelingThrust = new (3556)
            {
                BuffsNeed = new ushort[] { ObjectStatus.EnhancedWheelingThrust },
            },

            //ÁúÑÛÀ×µç
            RaidenThrust = new (16479),

            //¹á´©¼â
            PiercingTalon = new (90),

            //ËÀÌìÇ¹
            DoomSpike = new (86),

            //ÒôËÙ´Ì
            SonicThrust = new (7397) { OtherIDsCombo = new [] { 25770u } },

            //É½¾³¿áÐÌ
            CoerthanTorment = new (16477),

            //ÆÆËé³å
            SpineshatterDive = new (95)
            {
                OtherCheck = b =>
                {

                    if (safeMove && b.DistanceToPlayer() > 2) return false;
                    if (IsLastAction(true, SpineshatterDive)) return false;

                    if (inOpener && IsLastWeaponSkill(true,  FangandClaw, HeavensThrust)) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //ÁúÑ×³å
            DragonfireDive = new (96)
            {
                OtherCheck = b =>
                {
                    if (safeMove && b.DistanceToPlayer() > 2) return false;

                    if (inOpener && IsLastWeaponSkill(true, RaidenThrust)) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //ÌøÔ¾
            Jump = new (92)
            {
                BuffsProvide = new ushort[] { ObjectStatus.DiveReady },
                OtherCheck = b =>
                {
                    if (safeMove && b.DistanceToPlayer() > 2) return false;

                    if (inOpener && IsLastWeaponSkill(true, ChaoticSpring)) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },
            //¸ßÌø
            HighJump = new (16478)
            {
                OtherCheck = Jump.OtherCheck,
            },
            //»ÃÏó³å
            MirageDive = new (7399)
            {
                BuffsNeed = new [] { ObjectStatus.DiveReady },

                OtherCheck = b =>
                {
                    if (Geirskogul.WillHaveOneChargeGCD(4)) return false;
                    if (inOpener && IsLastWeaponSkill(true, FangandClaw)) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },

            //ÎäÉñÇ¹
            Geirskogul = new (3555)
            {
                OtherCheck = b =>
                {
                    if (inOpener && IsLastWeaponSkill(true, ChaoticSpring)) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },

            //ËÀÕßÖ®°¶
            Nastrond = new (7400)
            {
                OtherCheck = b => JobGauge.IsLOTDActive,
            },

            //×¹ÐÇ³å
            Stardiver = new (16480)
            {
                OtherCheck = b => JobGauge.IsLOTDActive && JobGauge.LOTDTimer < 25000,
            },

            //ÌìÁúµã¾¦
            WyrmwindThrust = new (25773)
            {
                OtherCheck = b => JobGauge.FirstmindsFocusCount == 2 && !IsLastAction(true, Stardiver),
            },

            //Áú½£
            LifeSurge = new (83) 
            { 
                BuffsProvide = new [] { ObjectStatus.LifeSurge },

                OtherCheck = b => !IsLastAbility(true, LifeSurge),
            },

            //ÃÍÇ¹
            LanceCharge = new (85)
            {
                OtherCheck = b =>
                {
                    if (inOpener && IsLastWeaponSkill(true, TrueThrust)) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //¾ÞÁúÊÓÏß
            DragonSight = new (7398)
            {
                ChoiceTarget = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
                    b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0).ToArray();

                    var targets = TargetFilter.GetJobCategory(Targets, Role.½üÕ½);
                    if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                    targets = TargetFilter.GetJobCategory(Targets, Role.Ô¶³Ì);
                    if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                    targets = Targets;
                    if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                    return LocalPlayer;
                },

                BuffsNeed = new [] {ObjectStatus.PowerSurge},
                BuffsProvide = new[] { ObjectStatus.LanceCharge }, 

            },

            //Õ½¶·Á¬µ»
            BattleLitany = new (3557)
            {
                BuffsNeed = new[] { ObjectStatus.PowerSurge },
                BuffsProvide = new[] { ObjectStatus.LanceCharge },
            };
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("DRG_ShouldDelay", true, "ÑÓºóºìÁúÑª")
            .SetBool("DRG_Opener", true, "88¼¶ÆðÊÖ")
            .SetBool("DRG_SafeMove", true, "°²È«Î»ÒÆ");
    }

    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.ÒÆ¶¯, $"{Actions.SpineshatterDive.Action.Name}, {Actions.DragonfireDive.Action.Name}"},
    };

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain > 1)
        {
            if (Actions.SpineshatterDive.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            if (Actions.DragonfireDive.ShouldUse(out act, mustUse: true)) return true;
        }

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD.IsAnySameAction(true, Actions.FullThrust, Actions.CoerthanTorment)
            || LocalPlayer.HaveStatus(ObjectStatus.LanceCharge) && nextGCD == Actions.FangandClaw)
        {
            //Áú½£
            if (abilityRemain ==1 && Actions.LifeSurge.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //ÃÍÇ¹
            if (inOpener && abilityRemain == 1 && Actions.LanceCharge.ShouldUse(out act, mustUse: true)) return true;
            if (!inOpener && Actions.LanceCharge.ShouldUse(out act, mustUse: true)) return true;

            //¾ÞÁúÊÓÏß
            if (Actions.DragonSight.ShouldUse(out act, mustUse: true)) return true;

            //Õ½¶·Á¬µ»
            if (Actions.BattleLitany.ShouldUse(out act, mustUse: true)) return true;
        }

        //ËÀÕßÖ®°¶
        if (Actions.Nastrond.ShouldUse(out act, mustUse: true)) return true;

        //×¹ÐÇ³å
        if (Actions.Stardiver.ShouldUse(out act, mustUse: true)) return true;

        //¸ßÌø
        if (Actions.HighJump.EnoughLevel)
        {
            if (Actions.HighJump.ShouldUse(out act)) return true;
        }
        else
        {
            if (Actions.Jump.ShouldUse(out act)) return true;
        }

        //³¢ÊÔ½øÈëºìÁúÑª
        if (Actions.Geirskogul.ShouldUse(out act, mustUse: true)) return true;

        //ÆÆËé³å
        if (LocalPlayer.HaveStatus(ObjectStatus.RightEye) && Actions.SpineshatterDive.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Actions.SpineshatterDive.ShouldUse(out act)) return true;

        //»ÃÏó³å
        if (Actions.MirageDive.ShouldUse(out act)) return true;

        //ÁúÑ×³å
        if (Actions.DragonfireDive.ShouldUse(out act, mustUse: true)) return true;

        //ÌìÁúµã¾¦
        if (Actions.WyrmwindThrust.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        safeMove = Config.GetBoolByName("DRG_SafeMove");

        if (!InCombat && Config.GetBoolByName("DRG_Opener") && Service.ClientState.LocalPlayer!.Level >= 88)
        {
            inOpener = false;

            if (!Actions.LanceCharge.IsCoolDown && !Actions.BattleLitany.IsCoolDown)
            {
                inOpener = true;
            }
        }
        if (Actions.BattleLitany.IsCoolDown && !LocalPlayer.HaveStatus(ObjectStatus.LanceCharge))
        {
            inOpener = false;
        }
        #region ÈºÉË
        if (Actions.CoerthanTorment.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.SonicThrust.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.DoomSpike.ShouldUse(out act, lastComboActionID)) return true;

        #endregion

        #region µ¥Ìå
        if (Config.GetBoolByName("ShouldDelay"))
        {
            if (Actions.WheelingThrust.ShouldUse(out act)) return true;
            if (Actions.FangandClaw.ShouldUse(out act)) return true;
        }
        else
        {
            if (Actions.FangandClaw.ShouldUse(out act)) return true;
            if (Actions.WheelingThrust.ShouldUse(out act)) return true;
        }

        //¿´¿´ÊÇ·ñÐèÒªÐøBuff
        if (LocalPlayer.WillStatusEndGCD(5, 0, true, ObjectStatus.PowerSurge))
        {
            if (Actions.FullThrust.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.VorpalThrust.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.ChaosThrust.ShouldUse(out act, lastComboActionID)) return true;
        }
        else
        {
            if (Actions.Disembowel.ShouldUse(out act, lastComboActionID)) return true;
        }
        if (Actions.TrueThrust.ShouldUse(out act)) return true;

        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Actions.PiercingTalon.ShouldUse(out act)) return true;

        return false;

        #endregion
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //Ç£ÖÆ
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }
}
