namespace DefaultRotations.Magical;

[SourceCode(Path = "main/DefaultRotations/Magical/BLM_Default.cs")]
public class BLM_Default : BLM_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.31";

    public override string RotationName => "Default";

    private static bool NeedToGoIce
    {
        get
        {
            //Can use Despair.
            if (Despair.EnoughLevel && CurrentMp >= Despair.MPNeed) return false;

            //Can use Fire1
            if (Fire.EnoughLevel && CurrentMp >= Fire.MPNeed) return false;

            return true;
        }
    }

    private static bool NeedToTransposeGoIce(bool usedOne)
    {
        if (!NeedToGoIce) return false;
        if (!Paradox.EnoughLevel) return false;
        var compare = usedOne ? -1 : 0;
        var count = PolyglotStacks;
        if (count == compare++) return false;
        if (count == compare++ && !EnchinaEndAfterGCD(2)) return false;
        if (count >= compare && (HasFire || Swiftcast.WillHaveOneChargeGCD(2) || TripleCast.WillHaveOneChargeGCD(2))) return true;
        if (!HasFire && !Swiftcast.WillHaveOneChargeGCD(2) && !TripleCast.CanUse(out _, gcdCountForAbility: 8)) return false;
        return true;
    }

    protected override IRotationConfigSet CreateConfiguration()
        => base.CreateConfiguration()
        .SetBool(CombatType.PvE, "UseTransposeForParadox", true, "Use Transpose to Astral Fire before Paradox")
        .SetBool(CombatType.PvE, "ExtendTimeSafely", false, "Extend Astral Fire Time Safely")
        .SetBool(CombatType.PvE, "UseN15", false, @"Use ""Double Paradox"" rotation [N15]");

    protected override IAction CountDownAction(float remainTime)
    {
        IAction act;
        if (remainTime < Fire3.CastTime + CountDownAhead)
        {
            if (Fire3.CanUse(out act)) return act;
        }
        if (remainTime <= 12 && SharpCast.CanUse(out act, CanUseOption.EmptyOrSkipCombo | CanUseOption.IgnoreClippingCheck)) return act;
        return base.CountDownAction(remainTime);
    }

    protected override bool AttackAbility(out IAction act)
    {
        if (IsBurst && UseBurstMedicine(out act)) return true;
        if (InUmbralIce)
        {
            if (UmbralIceStacks == 2 && !HasFire
                && !IsLastGCD(ActionID.Paradox))
            {
                if (Swiftcast.CanUse(out act)) return true;
                if (TripleCast.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
            }

            if (UmbralIceStacks < 3 && LucidDreaming.CanUse(out act)) return true;
            if (SharpCast.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }
        if (InAstralFire)
        {
            if (!CombatElapsedLess(6) && CombatElapsedLess(9) && LeyLines.CanUse(out act)) return true;
            if (TripleCast.CanUse(out act, gcdCountForAbility: 5)) return true;
        }
        if (Amplifier.CanUse(out act)) return true;
        return base.AttackAbility(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        //To Fire
        if (CurrentMp >= 7200 && UmbralIceStacks == 2 && Paradox.EnoughLevel)
        {
            if ((HasFire || HasSwift) && Transpose.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        }
        if (nextGCD.IsTheSameTo(false, Fire3) && HasFire)
        {
            if (Transpose.CanUse(out act)) return true;
        }

        //Using Manafont
        if (InAstralFire)
        {
            if (CurrentMp == 0 && Manafont.CanUse(out act)) return true;
            //To Ice
            if (NeedToTransposeGoIce(true) && Transpose.CanUse(out act)) return true;
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if (InFireOrIce(out act, out var mustGo)) return true;
        if (mustGo) return false;
        //Triplecast for moving.
        if (IsMoving && HasHostilesInRange && TripleCast.CanUse(out act, CanUseOption.EmptyOrSkipCombo | CanUseOption.IgnoreClippingCheck)) return true;

        if (AddElementBase(out act)) return true;
        if (Scathe.CanUse(out act)) return true;
        if (MaintainStatus(out act)) return true;

        return base.GeneralGCD(out act);
    }

    private bool InFireOrIce(out IAction act, out bool mustGo)
    {
        act = null;
        mustGo = false;
        if (InUmbralIce)
        {
            if (GoFire(out act)) return true;
            if (MaintainIce(out act)) return true;
            if (DoIce(out act)) return true;
        }
        if (InAstralFire)
        {
            if (GoIce(out act)) return true;
            if (MaintainFire(out act)) return true;
            if (DoFire(out act)) return true;
        }
        return false;
    }

    private static bool GoIce(out IAction act)
    {
        act = null;

        if (!NeedToGoIce) return false;

        //Use Manafont or transpose.
        if ((!Manafont.IsCoolingDown || NeedToTransposeGoIce(false))
            && UseInstanceSpell(out act)) return true;

        //Go to Ice.
        if (Blizzard2.CanUse(out act)) return true;
        if (Blizzard3.CanUse(out act)) return true;
        if (Transpose.CanUse(out act)) return true;
        if (Blizzard.CanUse(out act)) return true;
        return false;
    }

    private static bool MaintainIce(out IAction act)
    {
        act = null;
        if (UmbralIceStacks == 1)
        {
            if (Blizzard2.CanUse(out act)) return true;

            if (Player.Level == 90 && Blizzard.CanUse(out act)) return true;
            if (Blizzard3.CanUse(out act)) return true;
        }
        if (UmbralIceStacks == 2 && Player.Level < 90)
        {
            if (Blizzard2.CanUse(out act)) return true;
            if (Blizzard.CanUse(out act)) return true;
        }
        return false;
    }

    private static bool DoIce(out IAction act)
    {
        if (IsLastAction(ActionID.UmbralSoul, ActionID.Transpose)
            && IsParadoxActive && Blizzard.CanUse(out act)) return true;

        if (UmbralIceStacks == 3 && UsePolyglot(out act)) return true;

        //Add Hearts
        if (UmbralIceStacks == 3 &&
            Blizzard4.EnoughLevel && UmbralHearts < 3 && !IsLastGCD
            (ActionID.Blizzard4, ActionID.Freeze))
        {
            if (Freeze.CanUse(out act)) return true;
            if (Blizzard4.CanUse(out act)) return true;
        }

        if (AddThunder(out act, 5)) return true;
        if (UmbralIceStacks == 2 && UsePolyglot(out act, 0)) return true;

        if (IsParadoxActive)
        {
            if (Blizzard.CanUse(out act)) return true;
        }

        if (Blizzard2.CanUse(out act)) return true;
        if (Blizzard4.CanUse(out act)) return true;
        if (Blizzard.CanUse(out act)) return true;
        return false;
    }

    private static bool GoFire(out IAction act)
    {
        act = null;

        //Transpose line
        if (UmbralIceStacks < 3) return false;

        //Need more MP
        if (CurrentMp < 9600) return false;

        if (IsParadoxActive)
        {
            if (Blizzard.CanUse(out act)) return true;
        }

        //Go to Fire.
        if (Fire2.CanUse(out act)) return true;
        if (Fire3.CanUse(out act)) return true;
        if (Transpose.CanUse(out act)) return true;
        if (Fire.CanUse(out act)) return true;

        return false;
    }

    private bool MaintainFire(out IAction act)
    {
        switch (AstralFireStacks)
        {
            case 1:
                if (Fire2.CanUse(out act)) return true;
                if (Configs.GetBool("UseN15"))
                {
                    if (HasFire && Fire3.CanUse(out act)) return true;
                    if (IsParadoxActive && Fire.CanUse(out act)) return true;
                }
                if (Fire3.CanUse(out act)) return true;
                break;
            case 2:
                if (Fire2.CanUse(out act)) return true;
                if (Fire.CanUse(out act)) return true;
                break;
        }

        if (ElementTimeEndAfterGCD(Configs.GetBool("ExtendTimeSafely") ? 3u : 2u))
        {
            if (CurrentMp >= Fire.MPNeed * 2 + 800 && Fire.CanUse(out act)) return true;
            if (Flare.CanUse(out act)) return true;
            if (Despair.CanUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private static bool DoFire(out IAction act)
    {
        if (UsePolyglot(out act)) return true;

        // Add thunder only at combat start.
        if (CombatElapsedLess(5))
        {
            if (AddThunder(out act, 0)) return true;
        }

        if (TripleCast.CanUse(out act, CanUseOption.IgnoreClippingCheck)) return true;

        if (AddThunder(out act, 0) && Player.WillStatusEndGCD(1, 0, true,
            StatusID.Thundercloud)) return true;

        if (UmbralHearts < 2 && Flare.CanUse(out act)) return true;
        if (Fire2.CanUse(out act)) return true;

        if (CurrentMp >= Fire.MPNeed + 800)
        {
            if (Fire4.EnoughLevel)
            {
                if (Fire4.CanUse(out act)) return true;
            }
            else if (HasFire)
            {
                if (Fire3.CanUse(out act)) return true;
            }
            if (Fire.CanUse(out act)) return true;
        }

        if (Despair.CanUse(out act)) return true;

        return false;
    }

    private static bool UseInstanceSpell(out IAction act)
    {
        if (UsePolyglot(out act)) return true;
        if (HasThunder && AddThunder(out act, 1)) return true;
        if (UsePolyglot(out act, 0)) return true;
        return false;
    }

    private static bool AddThunder(out IAction act, uint gcdCount = 3)
    {
        act = null;
        //Return if just used.
        if (IsLastGCD(ActionID.Thunder, ActionID.Thunder2, ActionID.Thunder3, ActionID.Thunder4)) return false;

        //So long for thunder.
        if (Thunder.CanUse(out _) && !Thunder.Target.WillStatusEndGCD(gcdCount, 0, true,
            StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv))
            return false;

        if (Thunder2.CanUse(out act)) return true;
        if (Thunder.CanUse(out act)) return true;

        return false;
    }

    private static bool AddElementBase(out IAction act)
    {
        if (CurrentMp >= 7200)
        {
            if (Fire2.CanUse(out act)) return true;
            if (Fire3.CanUse(out act)) return true;
            if (Fire.CanUse(out act)) return true;
        }
        else
        {
            if (Blizzard2.CanUse(out act)) return true;
            if (Blizzard3.CanUse(out act)) return true;
            if (Blizzard.CanUse(out act)) return true;
        }
        return false;
    }

    private static bool UsePolyglot(out IAction act, uint gcdCount = 3)
    {
        if (gcdCount == 0 || IsPolyglotStacksMaxed && EnchinaEndAfterGCD(gcdCount))
        {
            if (Foul.CanUse(out act)) return true;
            if (Xenoglossy.CanUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private bool MaintainStatus(out IAction act)
    {
        act = null;
        if (CombatElapsedLess(6)) return false;
        if (UmbralSoul.CanUse(out act)) return true;
        if (InAstralFire && Transpose.CanUse(out act)) return true;
        if (Configs.GetBool("UseTransposeForParadox") &&
            InUmbralIce && !IsParadoxActive && UmbralIceStacks == 3
            && Transpose.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.BetweenTheLines, ActionID.LeyLines)]
    protected override bool HealSingleAbility(out IAction act)
    {
        if (BetweenTheLines.CanUse(out act)) return true;
        if (LeyLines.CanUse(out act)) return true;

        return base.HealSingleAbility(out act);
    }
}
