using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal class DRGCombo : CustomComboJob<DRGGauge>
{
    internal override uint JobID => 22;

    protected override bool ShouldSayout => true;
    internal struct Actions
    {
        public static readonly BaseAction
            //¾«×¼´Ì
            TrueThrust = new BaseAction(75),

            //¹áÍ¨´Ì
            VorpalThrust = new BaseAction(78) { OtherIDsCombo = new uint[] { 16479 } },

            //Ö±´Ì
            FullThrust = new BaseAction(84),

            //¿ªÌÅÇ¹
            Disembowel = new BaseAction(87) { OtherIDsCombo = new uint[] { 16479 } },

            //Ó£»¨Å­·Å
            ChaosThrust = new BaseAction(88),

            //ÁúÑÀÁú×¦
            FangandClaw = new BaseAction(3554)
            {
                EnermyLocation = EnemyLocation.Side,
                BuffsNeed = new ushort[] { ObjectStatus.SharperFangandClaw },
            },

            //ÁúÎ²´ó»ØÐý
            WheelingThrust = new BaseAction(3556)
            {
                EnermyLocation = EnemyLocation.Back,
                BuffsNeed = new ushort[] { ObjectStatus.EnhancedWheelingThrust },
            },

            //¹á´©¼â
            PiercingTalon = new BaseAction(90),

            //ËÀÌìÇ¹
            DoomSpike = new BaseAction(86),

            //ÒôËÙ´Ì
            SonicThrust = new BaseAction(7397) { OtherIDsCombo = new uint[] { 25770 } },

            //É½¾³¿áÐÌ
            CoerthanTorment = new BaseAction(16477),

            //ÆÆËé³å
            SpineshatterDive = new BaseAction(95)
            {
            },

            //ÁúÑ×³å
            DragonfireDive = new BaseAction(96)
            {
            },

            //ÌøÔ¾
            Jump = new BaseAction(92)
            {
            },
            HighJump = new BaseAction(16478)
            {
            },
            //»ÃÏó³å
            MirageDive = new BaseAction(7399) 
            { 
                BuffsNeed = new ushort[] { ObjectStatus.DiveReady },
            },

            //ÎäÉñÇ¹
            Geirskogul = new BaseAction(3555),

            //ËÀÕßÖ®°¶
            Nastrond = new BaseAction(7400),

            //×¹ÐÇ³å
            Stardiver = new BaseAction(16480)
            {
            },

            //ÌìÁúµã¾¦
            WyrmwindThrust = new BaseAction(25773),

            //Áú½£
            LifeSurge = new BaseAction(83) { BuffsProvide = new ushort[] { ObjectStatus.LifeSurge } },

            //ÃÍÇ¹
            LanceCharge = new BaseAction(85),

            //¾ÞÁúÊÓÏß
            DragonSight = new BaseAction(7398)
            {
                ChoiceFriend = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
                    b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0).ToArray();

                    var targets = TargetHelper.GetJobCategory(Targets, Role.½üÕ½);
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    targets = TargetHelper.GetJobCategory(Targets, Role.Ô¶³Ì);
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    targets = Targets;
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    return null;
                },
            },

            //Õ½¶·Á¬µ»
            BattleLitany = new BaseAction(3557);
    }


    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if(abilityRemain > 1)
        {
            if (Actions.SpineshatterDive.ShouldUseAction(out act, Empty: true)) return true;
            if (Actions.DragonfireDive.ShouldUseAction(out act, mustUse: true)) return true;
        }
        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (nextGCD.ActionID == Actions.FullThrust.ActionID || nextGCD.ActionID == Actions.CoerthanTorment.ActionID || (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge) && nextGCD == Actions.WheelingThrust))
        {
            if (abilityRemain == 1 && Actions.LifeSurge.ShouldUseAction(out act, Empty: true)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        //3 BUff
        if (Actions.LanceCharge.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.DragonSight.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.BattleLitany.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        if (JobGauge.IsLOTDActive && Actions.Nastrond.ShouldUseAction(out act, mustUse: true)) return true;

        //³¢ÊÔ½øÈëºìÁúÑª
        if (Actions.Geirskogul.ShouldUseAction(out act, mustUse: true)) return true;

        if (abilityRemain > 1 && !IsMoving)
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

            if (Actions.DragonfireDive.ShouldUseAction(out act, mustUse: true) && BaseAction.DistanceToPlayer(Actions.DragonfireDive.Target, true) < 2) return true;
            if (Actions.SpineshatterDive.ShouldUseAction(out act, Empty: true) && BaseAction.DistanceToPlayer(Actions.SpineshatterDive.Target, true) < 2) return true;
        }


        if (JobGauge.FirstmindsFocusCount == 2 && Actions.WyrmwindThrust.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        #region ÈºÉË
        if (Actions.CoerthanTorment.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SonicThrust.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.DoomSpike.ShouldUseAction(out act, lastComboActionID)) return true;

        #endregion

        #region µ¥Ìå
        if (Actions.WheelingThrust.ShouldUseAction(out act)) return true;
        if (Actions.FangandClaw.ShouldUseAction(out act)) return true;

        //¿´¿´ÊÇ·ñÐèÒªÐøBuff
        var time = BaseAction.FindStatusSelfFromSelf(ObjectStatus.PowerSurge);
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

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //Ç£ÖÆ
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }
}
