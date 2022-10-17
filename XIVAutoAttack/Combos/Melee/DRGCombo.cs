using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Healer;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Melee;

internal class DRGCombo : JobGaugeCombo<DRGGauge>
{
    internal override uint JobID => 22;
    private static bool inOpener = true;
    private static bool safeMove = false;

    internal struct Actions
    {
        public static readonly BaseAction
            //精准刺
            TrueThrust = new (75),

            //贯通刺
            VorpalThrust = new (78) { OtherIDsCombo = new [] { 16479u } },

            //直刺
            FullThrust = new (84),

            //苍穹刺
            HeavensThrust = new (25771),

            //开膛枪
            Disembowel = new (87) { OtherIDsCombo = new [] { 16479u } },

            //樱花怒放
            ChaosThrust = new (88),

            //樱花怒放
            ChaoticSpring = new(25772),

            //龙牙龙爪
            FangandClaw = new (3554)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SharperFangandClaw },
            },

            //龙尾大回旋
            WheelingThrust = new (3556)
            {
                BuffsNeed = new ushort[] { ObjectStatus.EnhancedWheelingThrust },
            },

            //龙眼雷电
            RaidenThrust = new (16479),

            //贯穿尖
            PiercingTalon = new (90),

            //死天枪
            DoomSpike = new (86),

            //音速刺
            SonicThrust = new (7397) { OtherIDsCombo = new [] { 25770u } },

            //山境酷刑
            CoerthanTorment = new (16477),

            //破碎冲
            SpineshatterDive = new (95)
            {
                OtherCheck = b =>
                {

                    if (safeMove && TargetFilter.DistanceToPlayer(Target) > 2) return false;
                    if (LastAction == SpineshatterDive.ID) return false;

                    if (inOpener && (LastWeaponskill == FangandClaw.ID || LastWeaponskill == HeavensThrust.ID)) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //龙炎冲
            DragonfireDive = new (96)
            {
                OtherCheck = b =>
                {
                    if (safeMove && TargetFilter.DistanceToPlayer(Target) > 2) return false;

                    if (inOpener && LastWeaponskill == RaidenThrust.ID) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //跳跃
            Jump = new (92)
            {
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.DiveReady)) return false;

                    if (safeMove && Vector3.Distance(LocalPlayer.Position, b.Position) - b.HitboxRadius > 2) return false;

                    if (inOpener && LastWeaponskill == ChaoticSpring.ID) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },
            //高跳
            HighJump = new (16478)
            {
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.DiveReady)) return false;

                    if (safeMove && Vector3.Distance(LocalPlayer.Position, b.Position) - b.HitboxRadius > 2) return false;

                    if (inOpener && LastWeaponskill == ChaoticSpring.ID) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },
            //幻象冲
            MirageDive = new (7399)
            {
                BuffsNeed = new [] { ObjectStatus.DiveReady },

                OtherCheck = b =>
                {
                    if (Geirskogul.RecastTimeRemain < 12) return false;
                    if (inOpener && LastWeaponskill == FangandClaw.ID) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },

            //武神枪
            Geirskogul = new (3555)
            {
                OtherCheck = b =>
                {
                    if (inOpener && LastWeaponskill == ChaoticSpring.ID) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },

            //死者之岸
            Nastrond = new (7400)
            {
                OtherCheck = b => JobGauge.IsLOTDActive,
            },

            //坠星冲
            Stardiver = new (16480)
            {
                OtherCheck = b => JobGauge.IsLOTDActive && JobGauge.LOTDTimer < 25000,
            },

            //天龙点睛
            WyrmwindThrust = new (25773)
            {
                OtherCheck = b => JobGauge.FirstmindsFocusCount == 2 && LastAction != Stardiver.ID,
            },

            //龙剑
            LifeSurge = new (83) 
            { 
                BuffsProvide = new [] { ObjectStatus.LifeSurge },

                OtherCheck = b => LastAbility != LifeSurge.ID,
            },

            //猛枪
            LanceCharge = new (85)
            {
                OtherCheck = b =>
                {
                    if (inOpener && LastWeaponskill == TrueThrust.ID) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //巨龙视线
            DragonSight = new (7398)
            {
                ChoiceTarget = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
                    b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0).ToArray();

                    var targets = TargetFilter.GetJobCategory(Targets, Role.近战);
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    targets = TargetFilter.GetJobCategory(Targets, Role.远程);
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    targets = Targets;
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    return LocalPlayer;
                },

                BuffsNeed = new [] {ObjectStatus.PowerSurge},


                OtherCheck = b =>
                {
                    //增伤buff下
                    if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge)) return false;

                    return true;
                }

            },

            //战斗连祷
            BattleLitany = new (3557)
            {
                BuffsNeed = new[] { ObjectStatus.PowerSurge },

                OtherCheck = b =>
                {
                    //增伤buff下
                    if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge)) return false;

                    return true;
                }
            };
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("DRG_ShouldDelay", true, "延后红龙血")
            .SetBool("DRG_Opener", true, "88级起手")
            .SetBool("DRG_SafeMove", true, "安全位移");
    }

    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.移动, $"{Actions.SpineshatterDive.Action.Name}, {Actions.DragonfireDive.Action.Name}"},
    };

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain > 1)
        {
            if (Actions.SpineshatterDive.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
            if (Actions.DragonfireDive.ShouldUseAction(out act, mustUse: true)) return true;
        }

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD.ID == Actions.FullThrust.ID || nextGCD.ID == Actions.CoerthanTorment.ID || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge) && nextGCD == Actions.FangandClaw)
        {
            //龙剑
            if (abilityRemain ==1 && Actions.LifeSurge.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //猛枪
        if (inOpener && abilityRemain == 1 && Actions.LanceCharge.ShouldUseAction(out act, mustUse: true)) return true;
        if (!inOpener && Actions.LanceCharge.ShouldUseAction(out act, mustUse: true)) return true;

        //巨龙视线
        if (Actions.DragonSight.ShouldUseAction(out act, mustUse: true)) return true;

        //战斗连祷
        if (Actions.BattleLitany.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //死者之岸
        if (Actions.Nastrond.ShouldUseAction(out act, mustUse: true)) return true;

        //坠星冲
        if (Actions.Stardiver.ShouldUseAction(out act, mustUse: true)) return true;

        //高跳
        if (Service.ClientState.LocalPlayer.Level >= Actions.HighJump.Level)
        {
            if (Actions.HighJump.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.Jump.ShouldUseAction(out act)) return true;
        }

        //尝试进入红龙血
        if (Actions.Geirskogul.ShouldUseAction(out act, mustUse: true)) return true;

        //破碎冲
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RightEye) && Actions.SpineshatterDive.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        if (Actions.SpineshatterDive.ShouldUseAction(out act)) return true;

        //幻象冲
        if (Actions.MirageDive.ShouldUseAction(out act)) return true;

        //龙炎冲
        if (Actions.DragonfireDive.ShouldUseAction(out act, mustUse: true)) return true;

        //天龙点睛
        if (Actions.WyrmwindThrust.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        safeMove = Config.GetBoolByName("DRG_SafeMove");

        if (!TargetHelper.InBattle && Config.GetBoolByName("DRG_Opener") && Service.ClientState.LocalPlayer!.Level >= 88)
        {
            inOpener = false;

            if (!Actions.LanceCharge.IsCoolDown && !Actions.BattleLitany.IsCoolDown)
            {
                inOpener = true;
            }
        }
        if (Actions.BattleLitany.IsCoolDown && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge))
        {
            inOpener = false;
        }
        #region 群伤
        if (Actions.CoerthanTorment.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SonicThrust.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.DoomSpike.ShouldUseAction(out act, lastComboActionID)) return true;

        #endregion

        #region 单体
        if (Config.GetBoolByName("ShouldDelay"))
        {
            if (Actions.WheelingThrust.ShouldUseAction(out act)) return true;
            if (Actions.FangandClaw.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.FangandClaw.ShouldUseAction(out act)) return true;
            if (Actions.WheelingThrust.ShouldUseAction(out act)) return true;
        }

        //看看是否需要续Buff
        var time = StatusHelper.FindStatusSelfFromSelf(ObjectStatus.PowerSurge);
        if (time.Length > 0 && time[0] > 13)
        {
            if (Actions.FullThrust.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.VorpalThrust.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.ChaosThrust.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        else
        {
            if (Actions.Disembowel.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (Actions.TrueThrust.ShouldUseAction(out act)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.PiercingTalon.ShouldUseAction(out act)) return true;

        return false;

        #endregion
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }
}
