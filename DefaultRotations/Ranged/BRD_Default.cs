namespace DefaultRotations.Ranged;

[SourceCode(Path = "main/DefaultRotations/Ranged/BRD_Default.cs")]
public sealed class BRD_Default : BRD_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    protected override IRotationConfigSet CreateConfiguration() => base.CreateConfiguration()
            .SetBool(CombatType.PvE, "BindWAND", false, @"Use Raging Strikes on ""Wanderer's Minuet""")
            .SetCombo(CombatType.PvE, "FirstSong", 0, "First Song", "Wanderer's Minuet", "Mage's Ballad", "Army's Paeon")
            .SetFloat( RotationSolver.Basic.Configuration.ConfigUnitType.Seconds, CombatType.PvE, "WANDTime", 43, "Wanderer's Minuet Uptime", min: 0, max: 45, speed: 1)
            .SetFloat(RotationSolver.Basic.Configuration.ConfigUnitType.Seconds, CombatType.PvE,"MAGETime", 34, "Mage's Ballad Uptime", min: 0, max: 45, speed: 1)
            .SetFloat(RotationSolver.Basic.Configuration.ConfigUnitType.Seconds, CombatType.PvE, "ARMYTime", 43, "Army's Paeon Uptime", min: 0, max: 45, speed: 1);

    public override string Description => "Please make sure that the three song times add up to 120 seconds!";

    private bool BindWAND => Configs.GetBool("BindWAND") && WanderersMinuet.EnoughLevel;
    private int FirstSong => Configs.GetCombo("FirstSong");
    private float WANDRemainTime => 45 - Configs.GetFloat("WANDTime");
    private float MAGERemainTime => 45 - Configs.GetFloat("MAGETime");
    private float ARMYRemainTime => 45 - Configs.GetFloat("ARMYTime");

    protected override bool GeneralGCD(out IAction act)
    {
        if (IronJaws.CanUse(out act)) return true;
        if (IronJaws.CanUse(out act, CanUseOption.MustUse) && IronJaws.Target.WillStatusEnd(30, true, IronJaws.TargetStatus))
        {
            if (Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEndGCD(1, 0, true, StatusID.RagingStrikes)) return true;
        }

        if (CanUseApexArrow(out act)) return true;

        if (BlastArrow.CanUse(out act, CanUseOption.MustUse))
        {
            if (!Player.HasStatus(true, StatusID.RagingStrikes)) return true;
            if (Player.HasStatus(true, StatusID.RagingStrikes) && Barrage.IsCoolingDown) return true;
        }

        if (ShadowBite.CanUse(out act)) return true;
        if (QuickNock.CanUse(out act)) return true;

        if (WindBite.CanUse(out act)) return true;
        if (VenomousBite.CanUse(out act)) return true;

        if (StraitShoot.CanUse(out act)) return true;

        if (HeavyShoot.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (nextGCD.IsTheSameTo(true, StraitShoot, VenomousBite, WindBite, IronJaws))
        {
            return base.EmergencyAbility(nextGCD, out act);
        }
        else if ((!RagingStrikes.EnoughLevel || Player.HasStatus(true, StatusID.RagingStrikes)) && (!BattleVoice.EnoughLevel || Player.HasStatus(true, StatusID.BattleVoice)))
        {
            if ((EmpyrealArrow.IsCoolingDown && !EmpyrealArrow.WillHaveOneChargeGCD(1) || !EmpyrealArrow.EnoughLevel) && Repertoire != 3)
            {
                if (!Player.HasStatus(true, StatusID.StraightShotReady) && Barrage.CanUse(out act)) return true;
            }
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        act = null;

        if (Song == Song.NONE)
        {
            if (FirstSong == 0 && WanderersMinuet.CanUse(out act)) return true;
            if (FirstSong == 1 && MagesBallad.CanUse(out act)) return true;
            if (FirstSong == 2 && ArmysPaeon.CanUse(out act)) return true;
            if (WanderersMinuet.CanUse(out act)) return true;
            if (MagesBallad.CanUse(out act)) return true;
            if (ArmysPaeon.CanUse(out act)) return true;
        }

        if (IsBurst && Song != Song.NONE && MagesBallad.EnoughLevel)
        {
            if (RagingStrikes.CanUse(out act))
            {
                if (BindWAND && Song == Song.WANDERER && WanderersMinuet.EnoughLevel) return true;
                if (!BindWAND) return true;
            }

            if (RadiantFinale.CanUse(out act, CanUseOption.MustUse))
            {
                if (Player.HasStatus(true, StatusID.RagingStrikes) && RagingStrikes.ElapsedOneChargeAfterGCD(1)) return true;
            }

            if (BattleVoice.CanUse(out act, CanUseOption.MustUse))
            {
                if (IsLastAction(true, RadiantFinale)) return true;

                if (Player.HasStatus(true, StatusID.RagingStrikes) && RagingStrikes.ElapsedOneChargeAfterGCD(1)) return true;
            }
        }

        if (RadiantFinale.EnoughLevel && RadiantFinale.IsCoolingDown && BattleVoice.EnoughLevel && !BattleVoice.IsCoolingDown) return false;

        if (WanderersMinuet.CanUse(out act, CanUseOption.OnLastAbility))
        {
            if (SongEndAfter(ARMYRemainTime) && (Song != Song.NONE || Player.HasStatus(true, StatusID.ArmysEthos))) return true;
        }

        if (Song != Song.NONE && EmpyrealArrow.CanUse(out act)) return true;

        if (PitchPerfect.CanUse(out act))
        {
            if (SongEndAfter(3) && Repertoire > 0) return true;

            if (Repertoire == 3) return true;

            if (Repertoire == 2 && EmpyrealArrow.WillHaveOneChargeGCD(1) && NextAbilityToNextGCD < PitchPerfect.AnimationLockTime + Ping) return true;

            if (Repertoire == 2 && EmpyrealArrow.WillHaveOneChargeGCD() && NextAbilityToNextGCD > PitchPerfect.AnimationLockTime + Ping) return true;
        }

        if (MagesBallad.CanUse(out act))
        {
            if (Song == Song.WANDERER && SongEndAfter(WANDRemainTime) && Repertoire == 0) return true;
            if (Song == Song.ARMY && SongEndAfterGCD(2) && WanderersMinuet.IsCoolingDown) return true;
        }

        if (ArmysPaeon.CanUse(out act))
        {
            if (WanderersMinuet.EnoughLevel && SongEndAfter(MAGERemainTime) && Song == Song.MAGE) return true;
            if (WanderersMinuet.EnoughLevel && SongEndAfter(2) && MagesBallad.IsCoolingDown && Song == Song.WANDERER) return true;
            if (!WanderersMinuet.EnoughLevel && SongEndAfter(2)) return true;
        }

        if (Sidewinder.CanUse(out act))
        {
            if (Player.HasStatus(true, StatusID.BattleVoice) && (Player.HasStatus(true, StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

            if (!BattleVoice.WillHaveOneCharge(10) && !RadiantFinale.WillHaveOneCharge(10)) return true;

            if (RagingStrikes.IsCoolingDown && !Player.HasStatus(true, StatusID.RagingStrikes)) return true;
        }

        if (EmpyrealArrow.IsCoolingDown || !EmpyrealArrow.WillHaveOneChargeGCD() || Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
        {
            if (RainOfDeath.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;

            if (Bloodletter.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }

        return base.AttackAbility(out act);
    }

    private static bool CanUseApexArrow(out IAction act)
    {
        if (!ApexArrow.CanUse(out act, CanUseOption.MustUse)) return false;

        if (QuickNock.CanUse(out _) && SoulVoice == 100) return true;

        if (SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

        if (SoulVoice >= 80 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEnd(10, false, StatusID.RagingStrikes)) return true;

        if (SoulVoice == 100 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.HasStatus(true, StatusID.BattleVoice)) return true;

        if (Song == Song.MAGE && SoulVoice >= 80 && SongEndAfter(22) && SongEndAfter(18)) return true;

        if (!Player.HasStatus(true, StatusID.RagingStrikes) && SoulVoice == 100) return true;

        return false;
    }
}
