using RotationSolver.Actions;
using RotationSolver.Data;
using System.Collections.Generic;
using RotationSolver.Helpers;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Melee.RPR;

internal sealed class RPR_Default : RPR_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("EnshroudPooling", false, "双附体循环爆发(低于88级不会生效)**推荐**");
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

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.DefenseSingle, $"能力: {ArcaneCrest}"},
        {DescType.MoveAction, $"能力: {HellsIngress}, "},
    };
    private protected override IAction CountDownAction(float remainTime)
    {
        //倒数收获月
        if (remainTime <= 30 && Soulsow.ShouldUse(out _)) return Soulsow;
        //提前2s勾刃
        if (remainTime <= 2 && Harpe.ShouldUse(out _)) return Harpe;
        return base.CountDownAction(remainTime);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //非战斗收获月
        if (Soulsow.ShouldUse(out act)) return true;

        //上Debuff
        if (WhorlofDeath.ShouldUse(out act)) return true;
        if (ShadowofDeath.ShouldUse(out act)) return true;

        //补蓝
        if (SoulReaver)
        {
            if (Guillotine.ShouldUse(out act)) return true;
            if (Player.HasStatus(true, StatusID.EnhancedGibbet))
            {
                if (Gibbet.ShouldUse(out act)) return true;
            }
            else
            {
                if (Gallows.ShouldUse(out act)) return true;
            }
        }

        //夜游魂变身状态
        if (Enshrouded)
        {
            //补DoT
            if (ShadowofDeath.ShouldUse(out act)) return true;

            if (LemureShroud > 1)
            {
                if (Configs.GetBool("EnshroudPooling") && PlentifulHarvest.EnoughLevel && ArcaneCircle.WillHaveOneCharge(9) &&
                   (LemureShroud == 4 && Target.WillStatusEnd(30, true, StatusID.DeathsDesign) || LemureShroud == 3 && Target.WillStatusEnd(50, true, StatusID.DeathsDesign))) //双附体窗口期 
                {
                    if (ShadowofDeath.ShouldUse(out act, mustUse: true)) return true;
                }

                //夜游魂衣-虚无/交错收割 阴冷收割
                if (GrimReaping.ShouldUse(out act)) return true;
                if (Player.HasStatus(true, StatusID.EnhancedCrossReaping) || !Player.HasStatus(true, StatusID.EnhancedVoidReaping))
                {
                    if (CrossReaping.ShouldUse(out act)) return true;
                }
                else
                {
                    if (VoidReaping.ShouldUse(out act)) return true;
                }
            }
            if (LemureShroud == 1)
            {
                if (Communio.EnoughLevel)
                {
                    if (!IsMoving && Communio.ShouldUse(out act, mustUse: true))
                    {
                        return true;
                    }
                    //跑机制来不及读条？补个buff混一下
                    else
                    {
                        if (ShadowofDeath.ShouldUse(out act, mustUse: IsMoving)) return true;
                    }
                }
                else
                {
                    //夜游魂衣-虚无/交错收割 阴冷收割
                    if (GrimReaping.ShouldUse(out act)) return true;
                    if (Player.HasStatus(true, StatusID.EnhancedCrossReaping) || !Player.HasStatus(true, StatusID.EnhancedVoidReaping))
                    {
                        if (CrossReaping.ShouldUse(out act)) return true;
                    }
                    else
                    {
                        if (VoidReaping.ShouldUse(out act)) return true;
                    }
                }
            }
        }

        //大丰收
        if (PlentifulHarvest.ShouldUse(out act, mustUse: true)) return true;

        //灵魂钐割
        if (SoulScythe.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        //灵魂切割
        if (SoulSlice.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //群体二连
        if (NightmareScythe.ShouldUse(out act)) return true;
        if (SpinningScythe.ShouldUse(out act)) return true;

        //单体三连
        if (InfernalSlice.ShouldUse(out act)) return true;
        if (WaxingSlice.ShouldUse(out act)) return true;
        if (Slice.ShouldUse(out act)) return true;

        //摸不到怪 先花掉收获月
        if (HarvestMoon.ShouldUse(out act, mustUse: true)) return true;
        if (Harpe.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //神秘环
            if (ArcaneCircle.ShouldUse(out act)) return true;

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
                if (Enshroud.ShouldUse(out act)) return true;
            }
        }
        if (Enshrouded)
        {
            //夜游魂衣-夜游魂切割 夜游魂钐割
            if (LemuresScythe.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            if (LemuresSlice.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }

        //暴食
        //大丰收期间延后暴食
        if (PlentifulHarvest.EnoughLevel && !Player.HasStatus(true, StatusID.ImmortalSacrifice) && !Player.HasStatus(true, StatusID.BloodsownCircle) || !PlentifulHarvest.EnoughLevel)
        {
            if (Gluttony.ShouldUse(out act, mustUse: true)) return true;
        }

        //AOE
        if (GrimSwathe.ShouldUse(out act)) return true;
        //单体
        if (BloodStalk.ShouldUse(out act)) return true;
        return false;
    }


    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (!SoulReaver && !Enshrouded)
        {
            if (Feint.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //神秘纹
        if (!SoulReaver && !Enshrouded)
        {
            if (ArcaneCrest.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }
}