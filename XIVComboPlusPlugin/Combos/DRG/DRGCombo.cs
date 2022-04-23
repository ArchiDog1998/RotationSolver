using Dalamud.Game.ClientState.JobGauge.Types;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal abstract class DRGCombo : CustomComboJob<DRGGauge>
{
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
                SayoutText = EnemyLocation.Side,
                BuffsNeed = new ushort[] { ObjectStatus.SharperFangandClaw },
            },

            //ÁúÎ²´ó»ØÐý
            WheelingThrust = new BaseAction(3556)
            {
                SayoutText = EnemyLocation.Back,
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
            SpineshatterDive = new BaseAction(95),

            //ÁúÑ×³å
            DragonfireDive = new BaseAction(96),

            //ÌøÔ¾
            Jump = new BaseAction(92),

            //»ÃÏó³å
            MirageDive = new BaseAction(7399) { BuffsNeed = new ushort[] { ObjectStatus.DiveReady }, },

            //ÎäÉñÇ¹
            Geirskogul = new BaseAction(3555),

            //ËÀÕßÖ®°¶
            Nastrond = new BaseAction(7400),

            //×¹ÐÇ³å
            Stardiver = new BaseAction(16480),

            //ÌìÁúµã¾¦
            WyrmwindThrust = new BaseAction(25773),

            //Áú½£
            LifeSurge = new BaseAction(83) { BuffsProvide = new ushort[] { ObjectStatus.LifeSurge } },

            //ÃÍÇ¹
            LanceCharge = new BaseAction(85),

            //¾ÞÁúÊÓÏß
            DragonSight = new BaseAction(7398),

            //Õ½¶·Á¬µ»
            BattleLitany = new BaseAction(3557);
    }

    private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if(nextGCD == Actions.FullThrust || nextGCD == Actions.CoerthanTorment || (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge) && nextGCD == Actions.WheelingThrust))
        {
            if (Actions.LifeSurge.TryUseAction(level, out act, Empty:true)) return true;
        }
        return base.EmergercyAbility(level, abilityRemain, nextGCD, out act);
    }

    private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //3 BUff
        if (Actions.LanceCharge.TryUseAction(level, out act, mustUse:true)) return true;
        if (Actions.DragonSight.TryUseAction(level, out act, mustUse: true)) return true;
        if (Actions.BattleLitany.TryUseAction(level, out act, mustUse: true)) return true;

        if (JobGauge.IsLOTDActive)
        {
            if (abilityRemain > 1 && Actions.Stardiver.TryUseAction(level, out act, mustUse:true)) return true;
            if (JobGauge.FirstmindsFocusCount == 2 && Actions.WyrmwindThrust.TryUseAction(level, out act, mustUse: true)) return true;
            if (Actions.Nastrond.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //³¢ÊÔ½øÈëºìÁúÑª
        if (Actions.Geirskogul.TryUseAction(level, out act, mustUse:true)) return true;
        if (Actions.MirageDive.TryUseAction(level, out act, mustUse: true)) return true;
        if (abilityRemain > 1 && Vector3.Distance(LocalPlayer.Position, Target.Position) - Target.HitboxRadius < 1)
        {
            if (!Service.IconReplacer.GetCooldown(9).IsCooldown && Actions.Jump.TryUseAction(level, out act)) return true;
            if (Actions.SpineshatterDive.TryUseAction(level, out act, Empty: true)) return true;
            if (Actions.DragonfireDive.TryUseAction(level, out act, mustUse:true)) return true;
        }

        //»ØÑª
        if (GeneralActions.Bloodbath.TryUseAction(level, out act)) return true;
        if (GeneralActions.SecondWind.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        #region ÈºÉË
        if (Actions.CoerthanTorment.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.SonicThrust.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.DoomSpike.TryUseAction(level, out act, lastComboActionID)) return true;

        #endregion

        #region µ¥Ìå
        if (Actions.WheelingThrust.TryUseAction(level, out act)) return true;
        if (Actions.FangandClaw.TryUseAction(level, out act)) return true;

        //¿´¿´ÊÇ·ñÐèÒªÐøBuff
        var time = BaseAction.FindStatusSelfFromSelf(ObjectStatus.PowerSurge);
        if(time.Length > 0 && time[0] > 13)
        {
            if (Actions.FullThrust.TryUseAction(level, out act, lastComboActionID)) return true;
            if (Actions.VorpalThrust.TryUseAction(level, out act, lastComboActionID)) return true;
            if (Actions.ChaosThrust.TryUseAction(level, out act, lastComboActionID)) return true;
        }
        else
        {
            if (Actions.Disembowel.TryUseAction(level, out act, lastComboActionID)) return true;
        }


        if (Actions.TrueThrust.TryUseAction(level, out act)) return true;
        if (Actions.PiercingTalon.TryUseAction(level, out act)) return true;

        return false;

        #endregion
    }
}
