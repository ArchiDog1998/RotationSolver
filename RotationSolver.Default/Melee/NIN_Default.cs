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

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseHide", true, "Use hide")
            .SetBool("AutoUnhide", true, "Auto Unhide.");
    }

    private static void SetNinjustus(INinAction act)
    {
        if (_ninActionAim != null && IsLastAction(false, Ten, Jin, Chi, FumaShurikenTen, FumaShurikenJin)) return;
        _ninActionAim = act;
    }

    private bool ChoiceNinjutsus(out IAction act)
    {
        act = null;
        if (AdjustId(2260) != 2260) return false;

        //在GCD快转完的时候再判断是否调整非空忍术
        if (_ninActionAim != null && WeaponRemain < 0.2) return false;

        //有生杀予夺
        if (Player.HasStatus(true, StatusID.Kassatsu))
        {
            if (GokaMekkyaku.CanUse(out _))
            {
                SetNinjustus(GokaMekkyaku);
                return false;
            }
            if (HyoshoRanryu.CanUse(out _))
            {
                SetNinjustus(HyoshoRanryu);
                return false;
            }

            if (Katon.CanUse(out _))
            {
                SetNinjustus(Katon);
                return false;
            }

            if (Raiton.CanUse(out _))
            {
                SetNinjustus(Raiton);
                return false;
            }
        }
        else
        {
            bool empty = Ten.CanUse(out _, CanUseOption.MustUse);
            bool haveDoton = Player.HasStatus(true, StatusID.Doton);

            //加状态
            if (Huraijin.CanUse(out act)) return true;

            if (InHuton && _ninActionAim?.ID == Huton.ID)
            {
                ClearNinjutsus();
                return false;
            }

            if (empty && (!InCombat || !Huraijin.EnoughLevel) && Huton.CanUse(out _))
            {
                SetNinjustus(Huton);
                return false;
            }

            if (GeneralNinjutsus(empty, haveDoton)) return false;
        }
        return false;
    }

    private bool GeneralNinjutsus(bool empty, bool haveDoton)
    {
        //清空忍术
        if (empty)
        {
            if (Katon.CanUse(out _))
            {
                if (!haveDoton && !IsMoving && TenChiJin.WillHaveOneChargeGCD(0, 1)) _ninActionAim = Doton;
                else SetNinjustus(Katon);
                return true;
            }
            //背刺
            if (InBurst && Suiton.CanUse(out _))
            {
                SetNinjustus(Suiton);
            }
        }
        //常规单体忍术
        if (Ten.CanUse(out _) && (!TenChiJin.EnoughLevel || TenChiJin.IsCoolingDown))
        {
            if (Raiton.CanUse(out _))
            {
                SetNinjustus(Raiton);
                return true;
            }

            if (!Chi.EnoughLevel && FumaShuriken.CanUse(out _))
            {
                SetNinjustus(FumaShuriken);
                return true;
            }
        }
        return false;
    }

    private static void ClearNinjutsus()
    {
        _ninActionAim = null;
    }

    private bool DoNinjutsus(out IAction act)
    {
        act = null;

        //有天地人
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

        if (_ninActionAim == null) return false;

        uint id = AdjustId(2260);

        //没开始，释放第一个
        if (id == 2260)
        {
            //重置
            if (!Player.HasStatus(true, StatusID.Kassatsu, StatusID.TenChiJin)
                && !Ten.CanUse(out _, CanUseOption.MustUse))
            {
                return false;
            }
            act = _ninActionAim.Ninjutsus[0];
            return true;
        }
        //失败了
        else if (id == RabbitMedium.ID)
        {
            ClearNinjutsus();
            act = null;
            return false;
        }
        //结束了
        else if (id == _ninActionAim.ID)
        {
            if (_ninActionAim.CanUse(out act, CanUseOption.MustUse)) return true;
            if (_ninActionAim.ID == Doton.ID && !InCombat)
            {
                act = _ninActionAim;
                return true;
            }
        }
        //释放第二个
        else if (id == FumaShuriken.ID)
        {
            if (_ninActionAim.Ninjutsus.Length > 1)
            {
                act = _ninActionAim.Ninjutsus[1];
                return true;
            }
        }
        //释放第三个
        else if (id == Katon.ID || id == Raiton.ID || id == Hyoton.ID)
        {
            if (_ninActionAim.Ninjutsus.Length > 2)
            {
                act = _ninActionAim.Ninjutsus[2];
                return true;
            }
        }
        //ClearNinjutsus();
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if (IsLastAction(false, DotonChi, SuitonJin,
            RabbitMedium, FumaShuriken, Katon, Raiton,
            Hyoton, Huton, Doton, Suiton, GokaMekkyaku, HyoshoRanryu))
        {
            ClearNinjutsus();
        }
        if (ChoiceNinjutsus(out act)) return true;
        if (DoNinjutsus(out act)) return true;

        //用真北取消隐匿
        if (Configs.GetBool("AutoUnhide"))
        {
            StatusHelper.StatusOff(StatusID.Hidden);
        }
        //用隐匿恢复忍术数量
        if (!InCombat && _ninActionAim == null && Configs.GetBool("UseHide")
            && Ten.IsCoolingDown && Hide.CanUse(out act)) return true;

        var replace = AdjustId(2260);
        //无忍术或者忍术中途停了
        if (_ninActionAim == null || replace != 2260 && replace != _ninActionAim.ID)
        {
            //大招
            if (FleetingRaiju.CanUse(out act)) return true;
            if (ForkedRaiju.CanUse(out act))
            {
                if (ForkedRaiju.Target.DistanceToPlayer() < 2)
                {
                    return true;
                }
            }

            if (PhantomKamaitachi.CanUse(out act)) return true;

            if (Huraijin.CanUse(out act)) return true;

            //AOE
            if (HakkeMujinsatsu.CanUse(out act)) return true;
            if (DeathBlossom.CanUse(out act)) return true;

            //Single
            if (ArmorCrush.CanUse(out act)) return true;
            if (AeolianEdge.CanUse(out act)) return true;
            if (GustSlash.CanUse(out act)) return true;
            if (SpinningEdge.CanUse(out act)) return true;

            //飞刀
            if (SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
            if (ThrowingDagger.CanUse(out act)) return true;
        }

        act = null;
        return false;
    }

    [RotationDesc(ActionID.ForkedRaiju)]
    protected override bool MoveForwardGCD(out IAction act)
    {
        if (ForkedRaiju.CanUse(out act)) return true;
        return base.MoveForwardGCD(out act);
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;
        if (!InCombat || AdjustId(2260) != 2260) return false;

        //夺取
        if (InBurst && Mug.CanUse(out act)) return true;

        //解决Buff
        if (TrickAttack.CanUse(out act)) return true;
        if (Meisui.CanUse(out act)) return true;

        if (!IsMoving && TenChiJin.CanUse(out act)) return true;
        if (Kassatsu.CanUse(out act)) return true;
        if (UseBurstMedicine(out act)) return true;

        if (Bunshin.CanUse(out act)) return true;
        if (HellfrogMedium.CanUse(out act)) return true;
        if (Bhavacakra.CanUse(out act)) return true;

        if (!DreamWithinADream.EnoughLevel)
        {
            if (Assassinate.CanUse(out act)) return true;
        }
        else
        {
            if (DreamWithinADream.CanUse(out act)) return true;
        }
        return false;
    }
}
