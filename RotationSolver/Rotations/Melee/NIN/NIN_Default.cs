using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Updaters;
using System.Collections.Generic;

namespace RotationSolver.Rotations.Melee.NINCombos;

internal sealed class NIN_Default : NIN_Base
{
    public override string GameVersion => "6.0";

    public override string RotationName => "Default";

    private static INinAction _ninactionAim = null;

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseHide", true, "脱战隐身恢复忍术").SetBool("AutoUnhide", true, "自动取消隐身");
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.DefenseSingle, $"{ShadeShift}"},
        {DescType.MoveAction, $"{Shukuchi}，目标为面向夹角小于30°内最远目标。"},
    };

    private static void SetNinjustus(INinAction act)
    {
        if (_ninactionAim != null && IsLastAction(false, Ten, Jin, Chi, FumaShurikenTen, FumaShurikenJin)) return;
        _ninactionAim = act;
    }

    private bool ChoiceNinjutsus(out IAction act)
    {
        act = null;
        if (Service.IconReplacer.OriginalHook(2260) != 2260) return false;

        //在GCD快转完的时候再判断是否调整非空忍术
        if (_ninactionAim != null && ActionUpdater.WeaponRemain < 0.2) return false;

        //有生杀予夺
        if (Player.HasStatus(true, StatusID.Kassatsu))
        {
            if (GokaMekkyaku.ShouldUse(out _))
            {
                SetNinjustus(GokaMekkyaku);
                return false;
            }
            if (HyoshoRanryu.ShouldUse(out _))
            {
                SetNinjustus(HyoshoRanryu);
                return false;
            }

            if (Katon.ShouldUse(out _))
            {
                SetNinjustus(Katon);
                return false;
            }

            if (Raiton.ShouldUse(out _))
            {
                SetNinjustus(Raiton);
                return false;
            }
        }
        else
        {
            bool empty = Ten.ShouldUse(out _, mustUse: true);
            bool haveDoton = Player.HasStatus(true, StatusID.Doton);

            //加状态
            if (Huraijin.ShouldUse(out act)) return true;

            if (InHuton && _ninactionAim?.ID == Huton.ID)
            {
                ClearNinjutsus();
                return false;
            }

            if (empty && (!InCombat || !Huraijin.EnoughLevel) && Huton.ShouldUse(out _))
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
            if (Katon.ShouldUse(out _))
            {
                if (!haveDoton && !IsMoving && TenChiJin.WillHaveOneChargeGCD(0, 1)) _ninactionAim = Doton;
                else SetNinjustus(Katon);
                return true;
            }
            //背刺
            if (SettingBreak && Suiton.ShouldUse(out _))
            {
                SetNinjustus(Suiton);
            }
        }
        //常规单体忍术
        if (Ten.ShouldUse(out _) && (!TenChiJin.EnoughLevel || TenChiJin.IsCoolDown))
        {
            if (Raiton.ShouldUse(out _))
            {
                SetNinjustus(Raiton);
                return true;
            }

            if (!Chi.EnoughLevel && FumaShuriken.ShouldUse(out _))
            {
                SetNinjustus(FumaShuriken);
                return true;
            }
        }
        return false;
    }

    private static void ClearNinjutsus()
    {
        _ninactionAim = null;
    }

    private bool DoNinjutsus(out IAction act)
    {
        act = null;

        //有天地人
        if (Player.HasStatus(true, StatusID.TenChiJin))
        {
            uint tenId = Service.IconReplacer.OriginalHook(Ten.ID);
            uint chiId = Service.IconReplacer.OriginalHook(Chi.ID);
            uint jinId = Service.IconReplacer.OriginalHook(Jin.ID);

            //第一个
            if (tenId == FumaShurikenTen.ID)
            {
                //AOE
                if (Katon.ShouldUse(out _))
                {
                    if (FumaShurikenJin.ShouldUse(out act)) return true;
                }
                //Single
                if (FumaShurikenTen.ShouldUse(out act)) return true;
            }

            //第二击杀AOE
            else if (tenId == KatonTen.ID)
            {
                if (KatonTen.ShouldUse(out act, mustUse: true)) return true;
            }
            //其他几击
            else if (chiId == RaitonChi.ID)
            {
                if (RaitonChi.ShouldUse(out act, mustUse: true)) return true;
            }
            else if (chiId == DotonChi.ID)
            {
                if (DotonChi.ShouldUse(out act, mustUse: true)) return true;
            }
            else if (jinId == SuitonJin.ID)
            {
                if (SuitonJin.ShouldUse(out act, mustUse: true)) return true;
            }
        }

        if (_ninactionAim == null) return false;

        uint id = Service.IconReplacer.OriginalHook(2260);

        //没开始，释放第一个
        if (id == 2260)
        {
            //重置
            if (!Player.HasStatus(true, StatusID.Kassatsu, StatusID.TenChiJin)
                && !Ten.ShouldUse(out _, mustUse: true))
            {
                return false;
            }
            act = _ninactionAim.Ninjutsus[0];
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
        else if (id == _ninactionAim.ID)
        {
            if (_ninactionAim.ShouldUse(out act, mustUse: true)) return true;
            if (_ninactionAim.ID == Doton.ID && !InCombat)
            {
                act = _ninactionAim;
                return true;
            }
        }
        //释放第二个
        else if (id == FumaShuriken.ID)
        {
            if (_ninactionAim.Ninjutsus.Length > 1)
            {
                act = _ninactionAim.Ninjutsus[1];
                return true;
            }
        }
        //释放第三个
        else if (id == Katon.ID || id == Raiton.ID || id == Hyoton.ID)
        {
            if (_ninactionAim.Ninjutsus.Length > 2)
            {
                act = _ninactionAim.Ninjutsus[2];
                return true;
            }
        }
        //ClearNinjutsus();
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
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
        if (Configs.GetBool("AutoUnhide") && Player.HasStatus(true, StatusID.Hidden))
        {
            StatusHelper.StatusOff(StatusID.Hidden);
        }
        //用隐匿恢复忍术数量
        if (!InCombat && _ninactionAim == null && Configs.GetBool("UseHide")
            && Ten.IsCoolDown && Hide.ShouldUse(out act)) return true;

        var replace = Service.IconReplacer.OriginalHook(2260);
        //无忍术或者忍术中途停了
        if (_ninactionAim == null || replace != 2260 && replace != _ninactionAim.ID)
        {
            //大招
            if (FleetingRaiju.ShouldUse(out act)) return true;
            if (ForkedRaiju.ShouldUse(out act))
            {
                if (ForkedRaiju.Target.DistanceToPlayer() < 2)
                {
                    return true;
                }
            }

            if (PhantomKamaitachi.ShouldUse(out act)) return true;

            if (Huraijin.ShouldUse(out act)) return true;

            //AOE
            if (HakkeMujinsatsu.ShouldUse(out act)) return true;
            if (DeathBlossom.ShouldUse(out act)) return true;

            //Single
            if (ArmorCrush.ShouldUse(out act)) return true;
            if (AeolianEdge.ShouldUse(out act)) return true;
            if (GustSlash.ShouldUse(out act)) return true;
            if (SpinningEdge.ShouldUse(out act)) return true;

            //飞刀
            if (RSCommands.SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
            if (ThrowingDagger.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool MoveGCD(out IAction act)
    {
        if (ForkedRaiju.ShouldUse(out act)) return true;
        return base.MoveGCD(out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (ShadeShift.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        if (!InCombat || Service.IconReplacer.OriginalHook(2260) != 2260) return false;

        //夺取
        if (SettingBreak && Mug.ShouldUse(out act)) return true;


        //解决Buff
        if (TrickAttack.ShouldUse(out act)) return true;
        if (Meisui.ShouldUse(out act)) return true;

        if (!IsMoving && TenChiJin.ShouldUse(out act)) return true;
        if (Kassatsu.ShouldUse(out act)) return true;
        if (UseBreakItem(out act)) return true;

        if (Bunshin.ShouldUse(out act)) return true;
        if (HellfrogMedium.ShouldUse(out act)) return true;
        if (Bhavacakra.ShouldUse(out act)) return true;

        if (!DreamWithinaDream.EnoughLevel)
        {
            if (Assassinate.ShouldUse(out act)) return true;
        }
        else
        {
            if (DreamWithinaDream.ShouldUse(out act)) return true;
        }
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (Feint.ShouldUse(out act)) return true;
        return false;
    }
}
