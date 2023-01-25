using RotationSolver.Actions;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Updaters;
using System.Collections.Generic;
using System.Linq;
using static RotationSolver.SigReplacers.Watcher;

namespace RotationSolver.Rotations.RangedMagicial.BLM;
internal sealed partial class BLM_Default : BLM_Base
{
    public override string GameVersion => "6.18";

    public override string RotationName => "Default";

    internal static float MpUpdateRemain => 3 - ActionUpdater.MPUpdateElapsed;
    internal static bool TargetHasThunder => Target.HasStatus(true, StatusID.Thunder, StatusID.Thunder2, StatusID.Thunder3, StatusID.Thunder4);
    internal static ActionRec[] RecordSpells => RecordActions.Where(b => b.Action.GetActinoType() == ActionCate.Spell).ToArray();
    internal static bool IsOldSpell(int count, IAction action) => RecordSpells[count].Action.RowId == action.ID;
    /// <summary>
    /// 目标身上的雷dot剩余时间
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    internal static bool TargetThunderWillEnd(float time) => Target.WillStatusEnd(time, false, StatusID.Thunder, StatusID.Thunder2, StatusID.Thunder3, StatusID.Thunder4);
    protected override bool CanHealSingleAbility => false;

    private static bool iceOpener = false;
    private static bool fireOpener = true;

    /// <summary>
    /// 标准循环
    /// </summary>
    //private bool StandardLoop => Config.GetComboByName("UseLoop") == 0;
    private bool StandardLoop => true;
    /// <summary>
    /// 双星灵循环
    /// </summary>
    private bool DoubleTranspose => Configs.GetCombo("UseLoop") == 1;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        { DescType.HealSingle, $"{BetweenTheLines}, {Leylines}" },
        { DescType.DefenseSingle, $"{Manaward}" },
        { DescType.MoveAction, $"{AetherialManipulation}" },
    };

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
                    .SetBool("AutoLeylines", true, "Auto use Leylines");
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        //战斗前激情
        if (remainTime <= 21 && Sharpcast.CanUse(out var act)) return act;
        return base.CountDownAction(remainTime);
    }

    private protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (BetweenTheLines.CanUse(out act)) return true;
        if (Leylines.CanUse(out act, mustUse: true)) return true;

        return base.HealSingleAbility(abilitiesRemaining, out act);
    }

    private protected override bool DefenseSingleGCD(out IAction act)
    {
        //加个魔罩
        if (Manaward.CanUse(out act)) return true;
        return base.DefenseSingleGCD(out act);
    }

    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //混乱
        if (Addle.CanUse(out act)) return true;
        return false;
    }

    private protected override bool MoveGCD(out IAction act)
    {
        //以太步
        if (AetherialManipulation.CanUse(out act, mustUse: true)) return true;
        return base.MoveGCD(out act);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //AOE
        if (LoopManagerArea(out act)) return true;

        //低级适配
        if (Level < 90 && LoopManagerSingleNOMax(out act)) return true;

        //起手
        if (Level == 90 && OpenerManager(out act)) return true;
        //满级循环
        if (Level == 90 && LoopManagerMax(out act)) return true;

        //移动时
        if (IsMoving && InCombat && HasHostilesInRange && !IsLastAction(true, AetherialManipulation))
        {
            if (Foul.CanUse(out act)) return true;
            if (Xenoglossy.CanUse(out act)) return true;
            if (HasThunder && Thunder2.CanUse(out act)) return true;
            if (HasThunder && Thunder.CanUse(out act)) return true;
            if (HasFire && Fire3.CanUse(out act)) return true;
            if (Triplecast.CanUse(out act, emptyOrSkipCombo: true)) return true;
            //if (!Player.HasStatus(true, StatusID.Triplecast) && Swiftcast.ShouldUse(out act)) return true;
        }

        //保持天语
        if (Maintence(out act)) return true;


        return false;
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        //三连咏唱
        if (UseTriplecast(out act)) return true;

        //魔泉
        if (Manafont.CanUse(out act))
        {
            if (IsLastGCD(true, Despair, Xenoglossy) && Player.CurrentMp == 0 && InAstralFire) return true;

            if (!Paradox.EnoughLevel && Player.CurrentMp <= 7000 && InAstralFire && !LoopManagerArea(out _)) return true;
        }

        //星灵移位
        if (UseTranspose(abilitiesRemaining, out act)) return true;

        //醒梦
        if (Level == 90 && UseLucidDreaming(out act)) return true;

        //即刻
        if (UseSwiftcast(out act)) return true;

        //激情咏唱
        if (UseSharpcast(out act)) return true;

        //黑魔纹
        if (Configs.GetBool("AutoLeylines") && Target.IsBoss() && Leylines.CanUse(out act))
        {
            if (Player.HasStatus(true, StatusID.Triplecast) && Player.StatusStack(true, StatusID.Triplecast) <= 1) return true;

            if (!Player.HasStatus(true, StatusID.Triplecast) && InUmbralIce && IsLastGCD(true, Thunder, Xenoglossy)) return true;

            if (!Player.HasStatus(true, StatusID.Triplecast) && InAstralFire) return true;
        }

        //详述
        if (Amplifier.CanUse(out act)) return true;

        act = null;
        return false;
    }

    /// <summary>
    /// 起手管理
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool OpenerManager(out IAction act)
    {
        if (!InCombat)
        {
            //激情咏唱
            if (Sharpcast.CanUse(out act) && HasHostilesInRange) return true;
        }
        if (iceOpener)
        {
            //三连
            if (Triplecast.CanUse(out act, emptyOrSkipCombo: true) && Triplecast.CurrentCharges == 2)
            {
                if (IsLastAction(true, Fire3) || IsLastGCD(true, Xenoglossy)) return true;
            }
        }

        if (fireOpener)
        {
            //三连
            if (IsLastAction(true, Fire4) && Triplecast.CanUse(out act, emptyOrSkipCombo: true) && Triplecast.CurrentCharges == 2) return true;
        }

        act = null;
        return false;
    }

    /// <summary>
    /// 满级循环管理
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool LoopManagerMax(out IAction act)
    {
        if (InUmbralIce)
        {
            //雷
            if (UseThunder(out act)) return true;
            //冰4
            if (UmbralIceStacks == 3 && Blizzard4.CanUse(out act)) return true;
            //悖论
            if (UseParadox(out act)) return true;
            //火3
            if (UseFire3(out act)) return true;
            //异言
            if (UseXenoglossy(out act)) return true;
        }
        else if (InAstralFire)
        {
            //悖论
            if (UseParadox(out act)) return true;
            //火3
            if (UseFire3(out act)) return true;
            //雷
            if (UseThunder(out act)) return true;
            //异言
            if (UseXenoglossy(out act)) return true;
            //火4
            if (UseFire4(out act)) return true;
            //绝望
            if (UseDespair(out act)) return true;
            //冰3转冰
            if (UseBlizzard3(out act)) return true;
        }
        else
        {
            //火3
            if (UseFire3(out act)) return true;
            //冰3
            if (Blizzard3.CanUse(out act)) return true;
        }
        act = null;
        return false;
    }

    /// <summary>
    /// 低级循环
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool LoopManagerSingleNOMax(out IAction act)
    {

        if (InUmbralIce)
        {
            //星灵
            if (!Fire3.EnoughLevel && Player.CurrentMp >= 9600 && Transpose.CanUse(out act, mustUse: true)) return true;
            //雷
            if (Thunder.CanUse(out act) && Target.IsBoss() && !Target.IsDying())
            {
                //没雷dot
                if (!TargetHasThunder || TargetThunderWillEnd(3)) return true;
                if (HasThunder && Player.WillStatusEnd(3, true, StatusID.Thundercloud)) return true;
            }
            //冰3
            if (UmbralIceStacks != 3 && Blizzard3.CanUse(out act)) return true;
            //冰1
            if (Blizzard.CanUse(out act))
            {
                if (!Blizzard4.EnoughLevel && (Player.CurrentMp < 9600 || UmbralIceStacks != 3)) return true;

                if (ElementTimeEndAfterGCD(1) && UmbralIceStacks != 3) return true;
            }
            //冰4
            if (UmbralHearts != 3 && Blizzard4.CanUse(out act)) return true;
            //火3
            if (UseFire3(out act)) return true;
            //悖论
            if (UseParadox(out act)) return true;
            //异言
            if (UseXenoglossy(out act)) return true;
        }
        if (InAstralFire)
        {
            //火3
            if (UseFire3(out act)) return true;
            //雷
            if (Thunder.CanUse(out act) && Target.IsBoss() && Target.IsDying())
            {
                //没雷dot
                if (!TargetHasThunder || TargetThunderWillEnd(3)) return true;
                if (HasThunder && Player.WillStatusEnd(3, true, StatusID.Thundercloud)) return true;
            }
            //火1
            if (!Paradox.EnoughLevel && Fire.CanUse(out act))
            {
                if (Fire4.EnoughLevel && ElementTimeEndAfterGCD(2)) return true;
                if (!Fire4.EnoughLevel) return true;
            }
            //异言
            if (UseXenoglossy(out act)) return true;
            //火4
            if (UseFire4(out act)) return true;
            //绝望
            if (UseDespair(out act)) return true;
            //冰3转冰
            if (UseBlizzard3(out act)) return true;
            //星灵
            if (!Fire3.EnoughLevel && Player.CurrentMp < 1600 && Transpose.CanUse(out act)) return true;
        }
        else if (!IsEnochianActive)
        {
            //火3
            if (Player.CurrentMp >= 6000 && Fire3.CanUse(out act)) return true;
            //冰3
            if (Blizzard3.CanUse(out act)) return true;

            if (Level < 60 && !InUmbralIce && !InAstralFire)
            {
                //火1
                if (Player.CurrentMp >= 6800 && Fire.CanUse(out act)) return true;
                //冰1
                if (Player.CurrentMp < 6800 && Blizzard.CanUse(out act)) return true;
            }

        }
        act = null;
        return false;
    }

    /// <summary>
    /// 范围技能循环
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool LoopManagerArea(out IAction act)
    {
        //if (!Blizzard2.ShouldUse(out _)) return false;
        //魔泉
        if (Manafont.CanUse(out act) && Blizzard2.CanUse(out _))
        {
            if (!Paradox.EnoughLevel && Player.CurrentMp == 0 && InAstralFire && IsLastGCD(true, Flare)) return true;
        }

        if (Foul.CanUse(out act) && IsPolyglotStacksMaxed) return true;


        if (Freeze.CanUse(out act) && !IsLastGCD(true, Freeze))
        {
            if (Blizzard4.EnoughLevel && UmbralIceStacks == 3 && UmbralHearts != 3) return true;
            if (!Blizzard4.EnoughLevel && Player.CurrentMp < 9000) return true;
        }
        if (Thunder2.CanUse(out act) && !IsLastGCD(true, Thunder2))
        {
            if (HasThunder || !Thunder2.Target.HasStatus(true, StatusID.Thunder, StatusID.Thunder2, StatusID.Thunder3, StatusID.Thunder4)) return true;
        }

        if (Flare.CanUse(out act))
        {
            if (Blizzard4.EnoughLevel && Player.HasStatus(true, StatusID.EnhancedFlare) && UmbralHearts <= 1) return true;
            if (!Blizzard4.EnoughLevel && Player.CurrentMp < 1000) return true;
        }

        if (Fire2.CanUse(out act) && Level >= 20)
        {
            if (!InUmbralIce && !Freeze.EnoughLevel) return true;

            if (InUmbralIce && (!Freeze.EnoughLevel && Player.CurrentMp >= 9000 || UmbralHearts == 3)) return true;

            if (InAstralFire) return true;
        }

        if (Blizzard2.CanUse(out act) && UmbralIceStacks != 3)
        {
            //if (!JobGauge.IsEnochianActive) return true;
            //if (JobGauge.InAstralFire) return true;
            return true;
        }
        act = null;
        return false;
    }

    /// <summary>
    /// 保持天语状态
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool Maintence(out IAction act)
    {
        act = null;
        if (HasHostilesInRange && InCombat || IsEnochianActive) return false;

        if (UmbralSoul.CanUse(out act) && UmbralIceStacks == 3 && ElementTimeEndAfterGCD(2)) return true;
        if (UmbralSoul.CanUse(out act) && (UmbralIceStacks != 3 || UmbralHearts != 3)) return true;
        if (Transpose.CanUse(out act) && ElementTimeEndAfterGCD(1) && Foul.EnoughLevel) return true;
        if (Transpose.CanUse(out act) && ElementTimeEndAfterGCD(1) && InAstralFire && !Foul.EnoughLevel) return true;

        return false;
    }

    /// <summary>
    /// 星灵移位
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseTranspose(byte abilitiesRemaining, out IAction act)
    {
        if (!Transpose.CanUse(out act, mustUse: true)) return false;

        //没有满级时
        if (!Paradox.EnoughLevel)
        {
            if (InUmbralIce && !Fire3.EnoughLevel && Player.CurrentMp >= 9600) return true;
            if (InAstralFire && !Fire3.EnoughLevel && Player.CurrentMp < 1600) return true;
            return false;
        }

        //标准循环
        if (StandardLoop)
        {
            if (InUmbralIce && HasFire && Player.CurrentMp >= 9600 && UmbralHearts == 3 && !IsParadoxActive) return true;
            return false;
        }

        //星灵转火
        if (InUmbralIce)
        {
            //标准循环改
            if (HasFire && Player.CurrentMp >= 9600 && UmbralHearts == 3 && !IsParadoxActive) return true;

            if (!IsParadoxActive && UmbralHearts != 3)
            {
                //火双4,最佳
                if (HasFire && Player.CurrentMp >= 7200) return true;

                //瞬双n(2/3-1)
                if (!HasFire && HasSwift && Player.CurrentMp >= 8000) return true;
            }
        }

        //星灵转冰
        if (InAstralFire && abilitiesRemaining == 2 && (Manafont.ElapsedAfter(3) || !Manafont.IsCoolingDown))
        {
            if (IsLastGCD(true, Despair) || IsOldSpell(1, Despair) && IsLastGCD(true, Xenoglossy, Thunder)) return true;
        }

        act = null;
        return false;
    }

    /// <summary>
    /// 醒梦
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseLucidDreaming(out IAction act)
    {
        if (!LucidDreaming.CanUse(out act)) return false;
        if (Blizzard2.CanUse(out _)) return false;
        if (StandardLoop) return false;
        if (InUmbralIce && UmbralIceStacks < 3 && !IsParadoxActive)
        {
            if (HasFire || Player.HasStatus(true, StatusID.LeyLines)) return false;
            if (Transpose.IsCoolingDown && (UseThunder(out _) || UseXenoglossy(out _))) return true;
        }

        //if (fireOpener && Leylines.IsCoolDown) return true;
        act = null;
        return false;
    }

    /// <summary>
    /// 即刻
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseSwiftcast(out IAction act)
    {
        if (!Swiftcast.CanUse(out act)) return false;
        if (StandardLoop) return false;
        if (InUmbralIce && UmbralIceStacks < 3 && !IsParadoxActive)
        {
            if (HasFire) return false;
            //if (IsOldSpell(1, Actions.Thunder3)) return false;
            if (Player.HasStatus(true, StatusID.LucidDreaming)) return true;
        }

        act = null;
        return false;
    }

    /// <summary>
    /// 三连
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseTriplecast(out IAction act)
    {
        if (!Triplecast.CanUse(out act, emptyOrSkipCombo: true)) return false;

        if (Level != 90) return false;

        if (StandardLoop)
        {
            if (InAstralFire && IsLastGCD(true, Xenoglossy, Thunder)) return true;
            return false;
        }

        if (InUmbralIce && UmbralIceStacks < 3 && !IsParadoxActive)
        {
            //if (IsOldSpell(1, Actions.Thunder3)) return false;
            //if (Player.HaveStatus(ObjectStatus.LucidDreaming) && !HasFire) return true;
        }

        if (!InAstralFire) return false;

        //if (!IsParadoxActive)
        //{
        //    if (fireOpener && Leylines.IsCoolDown && !Leylines.ElapsedAfterGCD(1) && !Manafont.IsCoolDown) return true;

        //    if (F4RemainingNumber() == 3 && Triplecast.CurrentCharges == 1 && Level == 90) return false;

        //    if (Player.CurrentMp == 0) return false;

        //    if (IsLastGCD(true, Xenoglossy, Thunder) && F4RemainingNumber() < 2) return true;
        //}



        act = null;
        return false;
    }

    /// <summary>
    /// 激情咏唱
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseSharpcast(out IAction act)
    {
        if (!Sharpcast.CanUse(out act, emptyOrSkipCombo: true)) return false;

        if (Level != 90) return true;

        if (StandardLoop)
        {
            if (InUmbralIce && !TargetThunderWillEnd(20)) return true;
            return true;
        }

        if (Triplecast.IsCoolingDown && Triplecast.CurrentCharges == 1 && Player.HasStatus(true, StatusID.Triplecast)) return false;
        //if (!IsLastSpell(true, Actions.Thunder)) return true;
        return true;

        //act = null;
        //return false;
    }

    /// <summary>
    /// 悖论
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseParadox(out IAction act)
    {
        if (!Paradox.CanUse(out act)) return false;

        //在冰
        if (InUmbralIce && (Transpose.IsCoolingDown && UmbralIceStacks >= 1 || UmbralIceStacks == 3 && UmbralHearts == 3)) return true;

        //在火
        if (InAstralFire && ElementTimeEndAfterGCD(2)) return true;

        act = null;
        return false;
    }

    /// <summary>
    /// 火3
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseFire3(out IAction act)
    {
        if (!Fire3.CanUse(out act)) return false;
        if (IsLastGCD(true, Fire3)) return false;

        //冰阶段进火
        if (InUmbralIce)
        {
            if (Paradox.EnoughLevel && IsParadoxActive) return false;

            if (!Fire4.EnoughLevel && Player.CurrentMp >= 9600) return true;

            //标准循环
            if (UmbralHearts == 3) return true;
            //没有火苗
            if (HasFire || StandardLoop) return false;

            //瞬单3/4
            if (HasSwift && Player.CurrentMp >= 5600) return true;

            //长单3,4,5
            if (!HasSwift && !UseThunder(out _) && !UseXenoglossy(out _)) return true;
        }

        if (InAstralFire)
        {
            if (Level < 90 && HasFire) return true;
            //进火后火3
            if (IsLastAction(true, Transpose) || AstralFireStacks < 3) return true;
        }

        //火起手
        if (Level == 90 && fireOpener && !IsEnochianActive && !InUmbralIce) return true;

        return false;
    }

    /// <summary>
    /// 火4
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseFire4(out IAction act)
    {
        if (!Fire4.CanUse(out act)) return false;
        if (Player.CurrentMp < 2400) return false;

        //能瞬发时判断
        if (HasSwift && ElementTimeEndAfterGCD(1, 1)) return true;
        ////当前火状态还能打几个火4
        //if (F4RemainingNumber() >= 1) return true;

        return false;
    }

    /// <summary>
    /// 绝望
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseDespair(out IAction act)
    {
        if (!Despair.CanUse(out act)) return false;
        //有悖论不放
        if (IsParadoxActive || UmbralHearts > 0) return false;
        //能放火4时不放
        if (UseFire4(out _)) return false;
        //正常判断,绝望收尾
        return true;
    }

    /// <summary>
    /// 冰3
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseBlizzard3(out IAction act)
    {
        if (!Blizzard3.CanUse(out act)) return false;
        if (IsLastGCD(true, Blizzard3)) return false;

        if (Level < 90 && Player.CurrentMp < 1600) return true;

        //标准循环
        if (StandardLoop && (Player.CurrentMp == 0 || !UseFire4(out _) && !UseDespair(out _))) return true;

        //双星灵
        if (InAstralFire && !UseFire4(out _) && !UseDespair(out _) && !IsParadoxActive && (Manafont.ElapsedAfter(3) || !Manafont.IsCoolingDown)) return true;
        return false;
    }

    /// <summary>
    /// 雷3
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseThunder(out IAction act)
    {
        if (!Thunder.CanUse(out act)) return false;

        if (IsLastGCD(true, Thunder)) return false;

        if (InUmbralIce)
        {
            if (UmbralIceStacks == 3 || StandardLoop)
            {
                if (!TargetHasThunder || TargetThunderWillEnd(3)) return true;
                if (HasThunder && Player.WillStatusEnd(4, true, StatusID.Thundercloud)) return true;
                return false;
            }
            //有雷dot,但10秒结束不了,不释放
            if (TargetHasThunder && !TargetThunderWillEnd(10)) return false;
            //有悖论时不释放
            if (IsParadoxActive) return false;
            //没雷dot
            if (!TargetHasThunder) return true;
            //有雷云
            if (HasThunder) return true;
        }

        //在火
        if (InAstralFire)
        {
            if (StandardLoop)
            {
                if (!TargetHasThunder || TargetHasThunder && TargetThunderWillEnd(3)) return true;
                return false;
            }
            //上个技能是异言pass
            if (IsLastGCD(true, Xenoglossy)) return false;
            //魔泉时
            if (!Manafont.ElapsedAfter(3) && Manafont.IsCoolingDown) return false;

            if (TargetHasThunder && !TargetThunderWillEnd(8)) return false;

            if (TargetHasThunder && TargetThunderWillEnd(3)) return true;

            if (HasThunder && Player.WillStatusEnd(4, true, StatusID.Thundercloud)) return true;

            //if (!HasThunder && (!TargetHasThunder || TargetHasThunder && TargetThunderWillEnd(3))) return true;

        }
        return false;
    }

    /// <summary>
    /// 异言
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseXenoglossy(out IAction act)
    {
        if (!Xenoglossy.CanUse(out act)) return false;

        //标准循环
        if (StandardLoop)
        {
            if (InUmbralIce && UmbralHearts != 3) return false;
            if (EnchinaEndAfterGCD(3) && IsPolyglotStacksMaxed) return true;
            if (!Manafont.IsCoolingDown && IsLastGCD(true, Despair)) return true;

            return false;
        }

        //在冰
        if (InUmbralIce)
        {
            if (UmbralHearts == 3 || IsParadoxActive) return false;

            if (IsOldSpell(1, Thunder3)) return false;
            if (PolyglotStacks == 2) return true;
            if (HasFire) return true;
            if (!HasFire && (HasSwift || !Swiftcast.IsCoolingDown) && !Player.HasStatus(true, StatusID.LeyLines)) return true;

            //if ( (3 - ActionUpdater.MPUpdateElapsed) - ActionUpdater.WeaponRemain < 1.5)
        }

        //在火
        if (InAstralFire)
        {
            if (IsLastGCD(true, Xenoglossy) || HasSwift) return false;
            //起手
            if (iceOpener && !IsParadoxActive && Player.CurrentMp <= 1200) return true;
            //魔泉时
            if (!Manafont.ElapsedAfter(3) && Manafont.IsCoolingDown) return false;

            //未来观测卡跳蓝(三连咏唱)

            if (IsLastGCD(true, Despair))
            {
                //火双n
                if (HasFire) return true;
                if (!HasFire) return true;
            }
        }

        if (ElementTimeEndAfterGCD(2)) return false;
        if (EnchinaEndAfterGCD(1) && IsPolyglotStacksMaxed) return true;
        act = null;
        return false;
    }
}