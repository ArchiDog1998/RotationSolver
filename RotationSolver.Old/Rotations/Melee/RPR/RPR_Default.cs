using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;

namespace RotationSolver.Rotations.Melee.RPR;

[DefaultRotation]
internal sealed class RPR_Default : RPR_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("EnshroudPooling", false, "Enshroud Pooling");
    }
    public RPR_Default()
    {
        //保留红条不第一时间打出去,保证暴食不空转 同时保证不延后大丰收
        BloodStalk.RotationCheck = b => !Player.HasStatus(true, StatusID.BloodsownCircle) && !Player.HasStatus(true, StatusID.ImmortalSacrifice) && (Gluttony.EnoughLevel && !Gluttony.WillHaveOneChargeGCD(4) || !Gluttony.EnoughLevel || Soul == 100);
        GrimSwathe.RotationCheck = BloodStalk.RotationCheck;

        //必须有dot
        ArcaneCircle.RotationCheck = b => Target.HasStatus(true, StatusID.DeathsDesign);

        //必须进战
        HarvestMoon.RotationCheck = b => InCombat;
    }


    protected override IAction CountDownAction(float remainTime)
    {
        //倒数收获月
        if (remainTime <= 30 && Soulsow.CanUse(out _)) return Soulsow;
        //提前2s勾刃
        if (remainTime <= Harpe.CastTime + Service.Configuration.CountDownAhead
            && Harpe.CanUse(out _)) return Harpe;
        return base.CountDownAction(remainTime);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        //非战斗收获月
        if (Soulsow.CanUse(out act)) return true;

        //上Debuff
        if (WhorlofDeath.CanUse(out act)) return true;
        if (ShadowofDeath.CanUse(out act)) return true;

        //补蓝
        if (SoulReaver)
        {
            if (Guillotine.CanUse(out act)) return true;
            if (Player.HasStatus(true, StatusID.EnhancedGibbet))
            {
                if (Gibbet.CanUse(out act)) return true;
            }
            else
            {
                if (Gallows.CanUse(out act)) return true;
            }
        }

        //夜游魂变身状态
        if (Enshrouded)
        {
            //补DoT
            if (ShadowofDeath.CanUse(out act)) return true;

            if (LemureShroud > 1)
            {
                if (Configs.GetBool("EnshroudPooling") && PlentifulHarvest.EnoughLevel && ArcaneCircle.WillHaveOneCharge(9) &&
                   (LemureShroud == 4 && Target.WillStatusEnd(30, true, StatusID.DeathsDesign) || LemureShroud == 3 && Target.WillStatusEnd(50, true, StatusID.DeathsDesign))) //双附体窗口期 
                {
                    if (ShadowofDeath.CanUse(out act, mustUse: true)) return true;
                }

                //夜游魂衣-虚无/交错收割 阴冷收割
                if (GrimReaping.CanUse(out act)) return true;
                if (Player.HasStatus(true, StatusID.EnhancedCrossReaping) || !Player.HasStatus(true, StatusID.EnhancedVoidReaping))
                {
                    if (CrossReaping.CanUse(out act)) return true;
                }
                else
                {
                    if (VoidReaping.CanUse(out act)) return true;
                }
            }
            if (LemureShroud == 1)
            {
                if (Communio.EnoughLevel)
                {
                    if (!IsMoving && Communio.CanUse(out act, mustUse: true))
                    {
                        return true;
                    }
                    //跑机制来不及读条？补个buff混一下
                    else
                    {
                        if (ShadowofDeath.CanUse(out act, mustUse: IsMoving)) return true;
                    }
                }
                else
                {
                    //夜游魂衣-虚无/交错收割 阴冷收割
                    if (GrimReaping.CanUse(out act)) return true;
                    if (Player.HasStatus(true, StatusID.EnhancedCrossReaping) || !Player.HasStatus(true, StatusID.EnhancedVoidReaping))
                    {
                        if (CrossReaping.CanUse(out act)) return true;
                    }
                    else
                    {
                        if (VoidReaping.CanUse(out act)) return true;
                    }
                }
            }
        }

        //大丰收
        if (PlentifulHarvest.CanUse(out act, mustUse: true)) return true;

        //灵魂钐割
        if (SoulScythe.CanUse(out act, emptyOrSkipCombo: true)) return true;
        //灵魂切割
        if (SoulSlice.CanUse(out act, emptyOrSkipCombo: true)) return true;

        //群体二连
        if (NightmareScythe.CanUse(out act)) return true;
        if (SpinningScythe.CanUse(out act)) return true;

        //单体三连
        if (InfernalSlice.CanUse(out act)) return true;
        if (WaxingSlice.CanUse(out act)) return true;
        if (Slice.CanUse(out act)) return true;

        //摸不到怪 先花掉收获月
        if (HarvestMoon.CanUse(out act, mustUse: true)) return true;
        if (Harpe.CanUse(out act)) return true;

        return false;
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (InBurst)
        {
            //神秘环
            if (ArcaneCircle.CanUse(out act)) return true;

            if (IsTargetBoss && IsTargetDying || //资源倾泻
               !Configs.GetBool("EnshroudPooling") && Shroud >= 50 ||//未开启双附体
               Configs.GetBool("EnshroudPooling") && Shroud >= 50 &&
               (!PlentifulHarvest.EnoughLevel || //等级不足以双附体
               Player.HasStatus(true, StatusID.ArcaneCircle) || //在神秘环期间附体
               ArcaneCircle.WillHaveOneCharge(8) || //双附体起手
               !Player.HasStatus(true, StatusID.ArcaneCircle) && ArcaneCircle.WillHaveOneCharge(65) && !ArcaneCircle.WillHaveOneCharge(50) || //奇数分钟不用攒附体
               !Player.HasStatus(true, StatusID.ArcaneCircle) && Shroud >= 90)) //攒蓝条为双附体
            {
                //夜游魂衣
                if (Enshroud.CanUse(out act)) return true;
            }
        }
        if (Enshrouded)
        {
            //夜游魂衣-夜游魂切割 夜游魂钐割
            if (LemuresScythe.CanUse(out act, emptyOrSkipCombo: true)) return true;
            if (LemuresSlice.CanUse(out act, emptyOrSkipCombo: true)) return true;
        }

        //暴食
        //大丰收期间延后暴食
        if (PlentifulHarvest.EnoughLevel && !Player.HasStatus(true, StatusID.ImmortalSacrifice) && !Player.HasStatus(true, StatusID.BloodsownCircle) || !PlentifulHarvest.EnoughLevel)
        {
            if (Gluttony.CanUse(out act, mustUse: true)) return true;
        }

        //AOE
        if (GrimSwathe.CanUse(out act)) return true;
        //单体
        if (BloodStalk.CanUse(out act)) return true;
        return false;
    }
}