namespace RotationSolver.Default.Melee;

[RotationDesc(ActionID.Mug)]
[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Melee/NIN_Default.cs")]
[LinkDescription("https://www.thebalanceffxiv.com/img/jobs/nin/earlymug3.png")]
[LinkDescription("https://docs.google.com/spreadsheets/u/0/d/1BZZrqWMRrugCeiBICEgjCz2vRNXt_lRTxPnSQr24Em0/htmlview#",
    "Under the “Planner (With sample section)”")]
public sealed class NIN_Default : NIN_Base
{
    public override string GameVersion => "6.35";

    public override string RotationName => "Standard";

    private static INinAction _ninActionAim = null;
    private static bool InBurstStatus => TrickAttack.IsCoolingDown && !TrickAttack.ElapsedAfter(14) 
        || Mug.IsCoolingDown && !Mug.ElapsedAfter(19);

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseHide", true, "Use hide")
            .SetBool("AutoUnhide", true, "Auto Unhide.");
    }

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime > 10) ClearNinjutsu();

        var realInHuton = (InHuton || IsLastAction(false, Huton));
        if (realInHuton && _ninActionAim == Huton) ClearNinjutsu();

        if (DoNinjutsu(out var act))
        {
            if (act == Suiton && remainTime > Service.Config.CountDownAhead) return null;
            return act;
        }

        else if (remainTime < 5)
        {
            SetNinjutsu(Suiton);
        }
        else if (remainTime < 10)
        {
            if (_ninActionAim == null && Ten.IsCoolingDown && Hide.CanUse(out act)) return act;
            if (!realInHuton)
            {
                SetNinjutsu(Huton);
            }
        }
        return base.CountDownAction(remainTime);
    }

    #region Ninjutsu
    private static void SetNinjutsu(INinAction act)
    {
        if (act == null || AdjustId(ActionID.Ninjutsu) == ActionID.RabbitMedium) return;
        if (_ninActionAim != null && IsLastAction(false, Ten, Jin, Chi, FumaShurikenTen, FumaShurikenJin)) return;
        if(_ninActionAim != act)
        {
            _ninActionAim = act;
#if DEBUG
            Service.ChatGui.Print("Set " + act.Name);
#endif
        }
    }
    private static void ClearNinjutsu()
    {
        if (_ninActionAim != null)
        {
#if DEBUG
            Service.ChatGui.Print("Clear " + _ninActionAim.Name);
#endif
            _ninActionAim = null;
        }
    }

    private bool ChoiceNinjutsu(out IAction act)
    {
        act = null;
        if (AdjustId(ActionID.Ninjutsu) != ActionID.Ninjutsu) return false;
        if (_ninActionAim != null && WeaponRemain < 0.2) return false;

        //Kassatsu
        if (Player.HasStatus(true, StatusID.Kassatsu))
        {
            if (GokaMekkyaku.CanUse(out _))
            {
                SetNinjutsu(GokaMekkyaku);
                return false;
            }
            if (HyoshoRanryu.CanUse(out _))
            {
                SetNinjutsu(HyoshoRanryu);
                return false;
            }

            if (Katon.CanUse(out _))
            {
                SetNinjutsu(Katon);
                return false;
            }

            if (Raiton.CanUse(out _))
            {
                SetNinjutsu(Raiton);
                return false;
            }
        }
        else
        {
            //Buff
            if (Huraijin.CanUse(out act)) return true;
            if (InHuton && _ninActionAim?.ID == Huton.ID)
            {
                ClearNinjutsu();
                return false;
            }
            if (Ten.CanUse(out _, CanUseOption.EmptyOrSkipCombo)
               && (!InCombat || !Huraijin.EnoughLevel) && Huton.CanUse(out _)
               && !IsLastAction(false, Huton))
            {
                SetNinjutsu(Huton);
                return false;
            }

            //Aoe
            if (Katon.CanUse(out _))
            {
                if (!Player.HasStatus(true, StatusID.Doton) && !IsMoving && !TenChiJin.WillHaveOneCharge(10))
                    SetNinjutsu(Doton);
                else SetNinjutsu(Katon);
                return false;
            }

            //Vulnerable
            if (InBurst && TrickAttack.WillHaveOneCharge(18) && Suiton.CanUse(out _))
            {
                SetNinjutsu(Suiton);
                return false;
            }

            //Single
            if (Ten.CanUse(out _, InBurstStatus && !Player.HasStatus(false, StatusID.RaijuReady) ? CanUseOption.EmptyOrSkipCombo : CanUseOption.None))
            {
                if (Raiton.CanUse(out _))
                {
                    SetNinjutsu(Raiton);
                    return false;
                }

                if (!Chi.EnoughLevel && FumaShuriken.CanUse(out _))
                {
                    SetNinjutsu(FumaShuriken);
                    return false;
                }
            }
        }

        if (IsLastAction(false, DotonChi, SuitonJin,
            RabbitMedium, FumaShuriken, Katon, Raiton,
            Hyoton, Huton, Doton, Suiton, GokaMekkyaku, HyoshoRanryu))
        {
            ClearNinjutsu();
        }
        return false;
    }

    private bool DoNinjutsu(out IAction act)
    {
        act = null;

        //TenChiJin
        if (Player.HasStatus(true, StatusID.TenChiJin))
        {
            uint tenId = AdjustId(Ten.ID);
            uint chiId = AdjustId(Chi.ID);
            uint jinId = AdjustId(Jin.ID);

            //第一个
            if (tenId == FumaShurikenTen.ID)
            {
                //AOE
                if (Katon.CanUse(out _))
                {
                    if (FumaShurikenJin.CanUse(out act)) return true;
                }
                //Single
                if (FumaShurikenTen.CanUse(out act)) return true;
            }

            //第二击杀AOE
            else if (tenId == KatonTen.ID)
            {
                if (KatonTen.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            //其他几击
            else if (chiId == RaitonChi.ID)
            {
                if (RaitonChi.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            else if (chiId == DotonChi.ID)
            {
                if (DotonChi.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            else if (jinId == SuitonJin.ID)
            {
                if (SuitonJin.CanUse(out act, CanUseOption.MustUse)) return true;
            }
        }

        //Keep Kassatsu in Burst.
        if (Player.HasStatus(false, StatusID.Kassatsu) && !InBurstStatus) return false;
        if (_ninActionAim == null) return false;

        var id = AdjustId(ActionID.Ninjutsu);

        //First
        if (id == ActionID.Ninjutsu)
        {
            //Can't use.
            if (!Player.HasStatus(true, StatusID.Kassatsu, StatusID.TenChiJin)
                && !Ten.CanUse(out _, CanUseOption.EmptyOrSkipCombo))
            {
                return false;
            }
            act = _ninActionAim.Ninjutsu[0];
            return true;
        }
        //Failed
        else if ((uint)id == RabbitMedium.ID)
        {
            ClearNinjutsu();
            act = null;
            return false;
        }
        //Finished
        else if ((uint)id == _ninActionAim.ID)
        {
            if (_ninActionAim.CanUse(out act, CanUseOption.MustUse)) return true;
            if (_ninActionAim.ID == Doton.ID && !InCombat)
            {
                act = _ninActionAim;
                return true;
            }
        }
        //Second
        else if ((uint)id == FumaShuriken.ID)
        {
            if (_ninActionAim.Ninjutsu.Length > 1)
            {
                act = _ninActionAim.Ninjutsu[1];
                return true;
            }
        }
        //Third
        else if ((uint)id == Katon.ID || (uint)id == Raiton.ID || (uint)id == Hyoton.ID)
        {
            if (_ninActionAim.Ninjutsu.Length > 2)
            {
                act = _ninActionAim.Ninjutsu[2];
                return true;
            }
        }
        return false;
    }
    #endregion

    protected override bool GeneralGCD(out IAction act)
    {
        if (ChoiceNinjutsu(out act)) return true;
        if ((!InCombat || !CombatElapsedLess(9)) && DoNinjutsu(out act)) return true;

        //No Ninjutsu
        if (AdjustId(ActionID.Ninjutsu) is ActionID.Ninjutsu or ActionID.RabbitMedium)
        {
            if (!CombatElapsedLess(16) && FleetingRaiju.CanUse(out act)) return true;
            if (Player.HasStatus(true, StatusID.RaijuReady)) return false;

            if (InBurstStatus && PhantomKamaitachi.CanUse(out act)) return true;

            if (Huraijin.CanUse(out act)) return true;

            //AOE
            if (HakkeMujinsatsu.CanUse(out act)) return true;
            if (DeathBlossom.CanUse(out act)) return true;

            //Single
            if (ArmorCrush.CanUse(out act)) return true;
            if (AeolianEdge.CanUse(out act)) return true;
            if (GustSlash.CanUse(out act)) return true;
            if (SpinningEdge.CanUse(out act)) return true;

            //Range
            if (SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
            if (ThrowingDagger.CanUse(out act)) return true;
        }

        if (Configs.GetBool("AutoUnhide"))
        {
            StatusHelper.StatusOff(StatusID.Hidden);
        }
        if (!InCombat && _ninActionAim == null && Configs.GetBool("UseHide")
            && Ten.IsCoolingDown && Hide.CanUse(out act)) return true;

        act = null;
        return false;
    }

    [RotationDesc(ActionID.ForkedRaiju)]
    protected override bool MoveForwardGCD(out IAction act)
    {
        if (ForkedRaiju.CanUse(out act)) return true;
        return base.MoveForwardGCD(out act);
    }

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (AdjustId(2260) != 2260 || !InCombat) return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);

        if (Kassatsu.CanUse(out act)) return true;
        if (UseBurstMedicine(out act)) return true;

        if (InBurst && !CombatElapsedLess(5) && Mug.CanUse(out act)) return true;

        //Use Suiton
        if (!CombatElapsedLess(6))
        {
            if (TrickAttack.CanUse(out act)) return true;
            if (TrickAttack.IsCoolingDown && !TrickAttack.WillHaveOneCharge(19)
                && Meisui.CanUse(out act)) return true;
        }

        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;
        if (!InCombat || AdjustId(2260) != 2260) return false;

        if (!IsMoving && InBurstStatus && !Ten.ElapsedAfter(30) &&  TenChiJin.CanUse(out act)) return true;

        if (!CombatElapsedLess(5) && Bunshin.CanUse(out act)) return true;
        if (HellfrogMedium.CanUse(out act)) return true;
        if (Bhavacakra.CanUse(out act)) return true;

        if (InBurstStatus)
        {
            if (!DreamWithinADream.EnoughLevel)
            {
                if (Assassinate.CanUse(out act)) return true;
            }
            else
            {
                if (DreamWithinADream.CanUse(out act)) return true;
            }
        }
        return false;
    }
}
