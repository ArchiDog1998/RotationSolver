using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Healer;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Melee;

internal class DRGCombo : JobGaugeCombo<DRGGauge>
{
    internal override uint JobID => 22;

    internal struct Actions
    {
        public static readonly BaseAction
            //精准刺
            TrueThrust = new (75),

            //贯通刺
            VorpalThrust = new (78) { OtherIDsCombo = new [] { 16479u } },

            //直刺
            FullThrust = new (84),

            //开膛枪
            Disembowel = new (87) { OtherIDsCombo = new [] { 16479u } },

            //樱花怒放
            ChaosThrust = new (88),

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

            //贯穿尖
            PiercingTalon = new (90),

            //死天枪
            DoomSpike = new (86),

            //音速刺
            SonicThrust = new (7397) { OtherIDsCombo = new [] { 25770u } },

            //山境酷刑
            CoerthanTorment = new (16477),

            //破碎冲
            SpineshatterDive = new (95),

            //龙炎冲
            DragonfireDive = new (96),

            //跳跃
            Jump = new (92),
            HighJump = new (16478),
            //幻象冲
            MirageDive = new (7399)
            {
                BuffsNeed = new [] { ObjectStatus.DiveReady },
            },

            //武神枪
            Geirskogul = new (3555),

            //死者之岸
            Nastrond = new (7400),

            //坠星冲
            Stardiver = new (16480),

            //天龙点睛
            WyrmwindThrust = new (25773),

            //龙剑
            LifeSurge = new (83) { BuffsProvide = new [] { ObjectStatus.LifeSurge } },

            //猛枪
            LanceCharge = new (85),

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

                    return null;
                },

                BuffsNeed = new [] {ObjectStatus.BattleLitany},
            },

            //战斗连祷
            BattleLitany = new (3557);
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("ShouldDelay", true, "延后红龙血");
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
        if (nextGCD.ID == Actions.FullThrust.ID || nextGCD.ID == Actions.CoerthanTorment.ID || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge) && nextGCD == Actions.WheelingThrust)
        {
            if (abilityRemain == 1 && Actions.LifeSurge.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //战斗连祷
        if (Actions.BattleLitany.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        // 2 Buffs
        if (Actions.LanceCharge.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.DragonSight.ShouldUseAction(out act, mustUse: true)) return true;

        if (JobGauge.IsLOTDActive && Actions.Nastrond.ShouldUseAction(out act, mustUse: true)) return true;

        //尝试进入红龙血
        if (Actions.Geirskogul.ShouldUseAction(out act, mustUse: true)) return true;

        if (!IsMoving)
        {
            if (JobGauge.IsLOTDActive && JobGauge.LOTDTimer < 25000 && Actions.Stardiver.ShouldUseAction(out act, mustUse: true)) return true;

            if (Service.ClientState.LocalPlayer.Level >= Actions.HighJump.Level)
            {
                if (Actions.HighJump.ShouldUseAction(out act) && Vector3.Distance(LocalPlayer.Position, Actions.HighJump.Target.Position) - Actions.HighJump.Target.HitboxRadius < 2) return true;
            }
            else
            {
                if (Actions.Jump.ShouldUseAction(out act) && Vector3.Distance(LocalPlayer.Position, Actions.Jump.Target.Position) - Actions.Jump.Target.HitboxRadius < 2) return true;
            }
            if (Actions.MirageDive.ShouldUseAction(out act) && Vector3.Distance(LocalPlayer.Position, Actions.MirageDive.Target.Position) - Actions.MirageDive.Target.HitboxRadius < 2) return true;

            if (Actions.DragonfireDive.ShouldUseAction(out act, mustUse: true) && TargetFilter.DistanceToPlayer(Actions.DragonfireDive.Target) < 2) return true;
            if (Actions.SpineshatterDive.ShouldUseAction(out act, emptyOrSkipCombo: true) && TargetFilter.DistanceToPlayer(Actions.SpineshatterDive.Target) < 2) return true;
        }


        if (JobGauge.FirstmindsFocusCount == 2 && Actions.WyrmwindThrust.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
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
