using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;

namespace RotationSolver.Rotations.RangedMagicial.BLM;

internal class BLM_Default : BLM_Base
{
    public override string GameVersion => "6.31";

    public override string RotationName => "Default";

    //public override SortedList<DescType, string> DescriptionDict => new SortedList<DescType, string>() 
    //{
    //    {DescType.HealSingle, $"{BetweenTheLines}, {Leylines}" }
    //};

    private static bool NeedToGoIce
    {
        get
        {
            //Can use Despair.
            if (Despair.EnoughLevel && Player.CurrentMp >= Despair.MPNeed) return false;

            //Can use Fire1
            if (Fire.EnoughLevel && Player.CurrentMp >= Fire.MPNeed) return false;

            return true;
        }
    }

    private static bool NeedToTransposeGoIce(bool usedOne)
    {
        if (!NeedToGoIce) return false;
        var compare = usedOne ? -1 : 0;
        var count = PolyglotStacks;
        if (count == compare++) return false;
        if (count == compare++ && !EnchinaEndAfterGCD(2)) return false;
        if (count >= compare && (HasFire || Swiftcast.WillHaveOneChargeGCD(2) || Triplecast.WillHaveOneChargeGCD(2))) return true;
        if (!HasFire && !Swiftcast.WillHaveOneChargeGCD(2) && !Triplecast.CanUse(out _, gcdCountForAbility: 8)) return false;
        return true;
    }

    private protected override IRotationConfigSet CreateConfiguration()
        => base.CreateConfiguration()
        .SetBool("UseTransposeForParadox", true, "Use Transpose to Fire for Paradox")
        .SetBool("ExtendTimeSafely", false, "Extend Fire Element Time Safely")
        .SetBool("UseN15", false, "Use N15");

    private protected override IAction CountDownAction(float remainTime)
    {
        IAction act;
        if(remainTime < Fire3.CastTime + Service.Configuration.CountDownAhead)
        {
            if (Fire3.CanUse(out act)) return act;
        }
        if (remainTime <= 12 && Sharpcast.CanUse(out act, emptyOrSkipCombo: true)) return act;
        return base.CountDownAction(remainTime);
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (InBurst && UseTincture(out act)) return true;
        if (InUmbralIce)
        {
            if (UmbralIceStacks == 2 && !HasFire
                && !IsLastGCD(ActionID.Paradox))
            {
                if (Swiftcast.CanUse(out act)) return true;
                if (Triplecast.CanUse(out act, emptyOrSkipCombo: true)) return true;
            }

            if (UmbralIceStacks < 3 && LucidDreaming.CanUse(out act)) return true;
            if (Sharpcast.CanUse(out act, emptyOrSkipCombo: true)) return true;
        }
        if (InAstralFire)
        {
            if (!CombatElapsedLess(6) && CombatElapsedLess(9) && Leylines.CanUse(out act)) return true;
            if (Triplecast.CanUse(out act, gcdCountForAbility: 5)) return true;
        }
        if (Amplifier.CanUse(out act)) return true;
        return false;
    }

    private protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        //To Fire
        if(Player.CurrentMp >= 7200 && UmbralIceStacks == 2 && Player.Level == 90)
        {
            if ((HasFire || HasSwift) && abilitiesRemaining == 1 && Transpose.CanUse(out act)) return true;
        }
        if(nextGCD.IsTheSameTo(false, Fire3) && HasFire && abilitiesRemaining == 1)
        {
            if(Transpose.CanUse(out act)) return true;
        }

        //Using Manafont
        if (InAstralFire)
        {
            if (Player.CurrentMp == 0 && abilitiesRemaining == 2 && Manafont.CanUse(out act)) return true;
            //To Ice
            if (NeedToTransposeGoIce(true) && Transpose.CanUse(out act)) return true;
        }

        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        if (InFireOrIce(out act, out var mustGo)) return true;
        if (mustGo) return false;
        //Triplecast for moving.
        if (IsMoving && Triplecast.CanUse(out act, emptyOrSkipCombo: true)) return true;
        
        if (AddElementBase(out act)) return true;
        if (Scathe.CanUse(out act)) return true;
        if (MaintainceStatus(out act)) return true;

        return false;
    }

    private bool InFireOrIce(out IAction act, out bool mustGo)
    {
        act = null;
        mustGo = false;
        if (InUmbralIce)
        {
            if (GoFire(out act)) return true;
            if (MaintainceIce(out act)) return true;
            if (DoIce(out act)) return true;
        }
        if (InAstralFire)
        {
            if (GoIce(out act)) return true;
            if (MaintainceFire(out act)) return true;
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

    private static bool MaintainceIce(out IAction act)
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
        if(IsLastAction(ActionID.UmbralSoul, ActionID.Transpose)
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
        if (Player.CurrentMp < 9600) return false;

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

    private bool MaintainceFire(out IAction act)
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
            if (Player.CurrentMp >= Fire.MPNeed * 2 + 800 && Fire.CanUse(out act)) return true;
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

        if (Triplecast.CanUse(out act)) return true;

        if (AddThunder(out act, 0) && Player.WillStatusEndGCD(1, 0, true, 
            StatusID.Thundercloud)) return true;

        if (UmbralHearts < 2 && Flare.CanUse(out act)) return true;
        if (Fire2.CanUse(out act)) return true;

        if (Player.CurrentMp >= Fire.MPNeed + 800)
        {
            if (Fire4.CanUse(out act)) return true;
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
        act= null;
        //Return if just used.
        if(IsLastGCD(ActionID.Thunder, ActionID.Thunder2, ActionID.Thunder3, ActionID.Thunder4)) return false;

        //So long for thunder.
        if(Thunder.CanUse(out _) && !Thunder.Target.WillStatusEndGCD(gcdCount, 0, true,
            StatusID.Thunder, StatusID.Thunder2, StatusID.Thunder3, StatusID.Thunder4))
            return false;

        if (Thunder2.CanUse(out act)) return true;
        if (Thunder.CanUse(out act)) return true;

        return false;
    }

    private static bool AddElementBase(out IAction act)
    {
        if (Player.CurrentMp >= 7200)
        {
            if (Fire2.CanUse(out act)) return true;
            if (Fire3.CanUse(out act)) return true;
            if (Fire.CanUse(out act)) return true;
        }
        else
        {
            if(Blizzard2.CanUse(out act)) return true;
            if(Blizzard3.CanUse(out act)) return true;
            if(Blizzard.CanUse(out act)) return true;
        }
        return false;
    }

    private static bool UsePolyglot(out IAction act, uint gcdCount = 3)
    {
        if(gcdCount == 0 || IsPolyglotStacksMaxed && EnchinaEndAfterGCD(gcdCount))
        {
            if (Foul.CanUse(out act)) return true;
            if (Xenoglossy.CanUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private bool MaintainceStatus(out IAction act)
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

    [RotationDesc(ActionID.BetweenTheLines, ActionID.Leylines)]
    private protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (BetweenTheLines.CanUse(out act)) return true;
        if (Leylines.CanUse(out act, mustUse: true)) return true;

        return base.HealSingleAbility(abilitiesRemaining, out act);
    }
}
