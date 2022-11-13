using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedMagicial.BLMCombo_Default.BLMCombo_Default;


namespace XIVAutoAttack.Combos.RangedMagicial.BLMCombo_Default;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/tree/main/XIVAutoAttack/Combos/RangedMagicial/BLMComboDefault")]
internal sealed partial class BLMCombo_Default : BLMCombo_Base<CommandType>
{
    /// <summary>
    /// 作者
    /// </summary>
    public override string Author => "汐ベMoon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };


    protected override bool CanHealSingleAbility => false;

    private static bool iceOpener = false;
    private static bool fireOpener = true;

    enum LoopName
    {
        Standard,           //标准循环          
        StandardEx,         //标准循环改
        NewShortFour,       //新短4
        NewShortFive,       //新短5
        LongSingleF3Three,  //长单3
        LongSingleF3Four,   //长单4
        LongSingleF3Five,   //长单5
        LongSingleF2Four,   //长单4(F2)
        SwiftSingleThree,   //瞬单3
        SwiftSingleFour,    //瞬单4
        LongDoubleTwo,      //长双2
        LongDoubleThree,    //长双3
        SwiftDoubleTwo,     //瞬双2
        SwiftDoubleThree,   //瞬双3
        FireDoubleFour,     //火双4
    }
    /// <summary>
    /// 标准循环
    /// </summary>
    private bool StandardLoop => Config.GetComboByName("UseLoop") == 0;
    /// <summary>
    /// 双星灵循环
    /// </summary>
    private bool DoubleTranspose => Config.GetComboByName("UseLoop") == 1;
    /// <summary>
    /// 进阶压冰循环
    /// </summary>
    private bool FewBlizzard => Config.GetComboByName("UseLoop") == 2;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        { DescType.单体治疗, $"{BetweenTheLines}, {Leylines}, 这个很特殊！" },
        { DescType.单体防御, $"{Manaward}" },
        { DescType.移动技能, $"{AetherialManipulation}，目标为面向夹角小于30°内最远目标。" },
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
                    .SetCombo("UseLoop", 1, "循环管理",  "标准循环", "星灵循环", "压冰循环")
                    .SetBool("AutoLeylines", true, "自动上黑魔纹");
    }

    private bool CommandManager(out IAction act)
    {
        act = null;
        return false;
    }

    private protected override void UpdateInfo()
    {
        //跳蓝判定点计算

        MPYuCe(2);

        base.UpdateInfo();
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        //战斗前激情
        if (remainTime <= 21 && Sharpcast.ShouldUse(out var act)) return act;
        return base.CountDownAction(remainTime);
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (BetweenTheLines.ShouldUse(out act)) return true;
        if (Leylines.ShouldUse(out act, mustUse: true)) return true;

        return base.HealSingleAbility(abilityRemain, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //加个魔罩
        if (Manaward.ShouldUse(out act)) return true;

        return base.DefenceSingleAbility(abilityRemain, out act);
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //混乱
        if (Addle.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //以太步
        if (AetherialManipulation.ShouldUse(out act, mustUse: true)) return true;

        return base.MoveAbility(abilityRemain, out act);
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {

        act = null;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //起手
        if (OpenerManager(out act)) return true;

        //AOE
        if (LoopManagerArea(out act)) return true;

        //低级适配
        if (Level < 90 && LoopManagerSingleNOMax(out act)) return true;
        //满级循环
        if (Level == 90 && UseLoopManager(out act)) return true;

        if (IsMoving && InCombat && HaveHostilesInRange)
        {
            if (Xenoglossy.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            if (Triplecast.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        if (!HaveHostilesInRange && Maintence(out act)) return true;
        //act = null;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        if (!HaveHostilesInRange && Maintence(out act)) return true;
        return base.GeneralAbility(abilityRemain, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //三连咏唱
        if (CanUseTriplecast(out act)) return true;

        //魔泉
        if (Manafont.ShouldUse(out act) && IsLastSpell(true, Despair, Xenoglossy) && Player.CurrentMp == 0 && JobGauge.InAstralFire) return true;

        //星灵移位
        if (CanUseTranspose(abilityRemain, out act)) return true;

        //醒梦
        if (CanUseLucidDreaming(out act)) return true;

        //即刻
        if (CanUseSwiftcast(out act)) return true;

        //激情咏唱
        if (CanUseSharpcast(out act)) return true;

        //黑魔纹
        if (Config.GetBoolByName("AutoLeylines") && Leylines.ShouldUse(out act))
        {
            if (Player.HaveStatus(true, StatusID.Triplecast) && Player.FindStatusStack(true, StatusID.Triplecast) <= 1) return true;

            if (!Player.HaveStatus(true, StatusID.Triplecast) && JobGauge.InUmbralIce && IsLastSpell(true, Thunder, Xenoglossy)) return true;

            if (!Player.HaveStatus(true, StatusID.Triplecast) && JobGauge.InAstralFire) return true;
        }

        //详述
        if (Amplifier.ShouldUse(out act)) return true;

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
            if (Sharpcast.ShouldUse(out act) && HaveHostilesInRange) return true;
        }
        if (iceOpener)
        {
            //三连
            if (Triplecast.ShouldUse(out act, emptyOrSkipCombo: true) && Triplecast.CurrentCharges == 2)
            {
                if (IsLastAction(true, Fire3) || IsLastSpell(true, Xenoglossy)) return true;
            }
        }

        if (fireOpener)
        {
            //三连
            if (IsLastAction(true, Fire4) && Triplecast.ShouldUse(out act, emptyOrSkipCombo: true) && Triplecast.CurrentCharges == 2) return true;
        }

        act = null;
        return false;
    }

    /// <summary>
    /// 循环管理
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseLoopManager(out IAction act)
    {
        if (JobGauge.InUmbralIce)
        {
            //雷
            if (CanUseThunder(out act)) return true;
            //冰阶段
            if (JobGauge.UmbralIceStacks == 3 && Blizzard4.ShouldUse(out act)) return true;
            //异言
            if (CanUseXenoglossy(out act)) return true;
            //悖论
            if (CanUseParadox(out act)) return true;
            //火3
            if (CanUseFire3(out act)) return true;

        }
        else if (JobGauge.InAstralFire)
        {
            //悖论
            if (CanUseParadox(out act)) return true;
            //火3
            if (CanUseFire3(out act)) return true;
            //雷
            if (CanUseThunder(out act)) return true;
            //异言
            if (CanUseXenoglossy(out act)) return true;
            //火4
            if (CanUseFire4(out act)) return true;
            //绝望
            if (CanUseDespair(out act)) return true;
            //冰3转冰
            if (CanUseBlizzard3(out act)) return true;
        }
        else
        {
            //火3
            if (CanUseFire3(out act)) return true;
            //冰3
            if (Blizzard3.ShouldUse(out act)) return true;
        }
        act = null;
        return false;
    }

    private bool LoopManagerSingleNOMax(out IAction act)
    {
        if (JobGauge.InUmbralIce)
        {
            if (Transpose.ShouldUse(out act))
            {
                if (!Fire4.EnoughLevel && Player.CurrentMp >= 9600) return true;
            }
            //雷
            if (CanUseThunder(out act)) return true;
            //冰阶段
            if (!Fire3.EnoughLevel && Blizzard.ShouldUse(out act)) return true;
            if (JobGauge.UmbralIceStacks == 3 && Blizzard4.ShouldUse(out act)) return true;
            //异言
            if (CanUseXenoglossy(out act)) return true;
            //悖论
            if (CanUseParadox(out act)) return true;
            //火3
            if (CanUseFire3(out act)) return true;
        }
        else if (JobGauge.InAstralFire)
        {
            //火3
            if (CanUseFire3(out act)) return true;
            //雷
            if (CanUseThunder(out act)) return true;
            //火1
            if (!Paradox.EnoughLevel && Fire.ShouldUse(out act))
            {
                if (JobGauge.ElementTimeRemaining <= CalcSpellTime(2500) * 2) return true;
                if (Level < 60) return true;
            }
            //异言
            if (CanUseXenoglossy(out act)) return true;
            //火4
            if (CanUseFire4(out act)) return true;
            //绝望
            if (CanUseDespair(out act)) return true;
            //冰3转冰
            if (CanUseBlizzard3(out act)) return true;
            if (Transpose.ShouldUse(out act))
            {
                if (!Fire3.EnoughLevel && Player.CurrentMp < 1600) return true;
            }
        }
        else
        {
            if (!Paradox.EnoughLevel && Fire.ShouldUse(out act))
            {
                if (JobGauge.ElementTimeRemaining <= CalcSpellTime(2500) * 2) return true;
                if (Level < 60) return true;
            }
            //冰3
            if (Blizzard3.ShouldUse(out act)) return true;
            if (Blizzard.ShouldUse(out act)) return true;
            //火3
            if (CanUseFire3(out act)) return true;
        }
        act = null;
        return false;
    }

    private bool LoopManagerArea(out IAction act)
    {
        //if (!Blizzard2.ShouldUse(out _)) return false;

        if (Foul.ShouldUse(out act) && IsPolyglotStacksMaxed) return true;


        if (Freeze.ShouldUse(out act) && !IsLastSpell(true, Freeze))
        {
            if (JobGauge.UmbralIceStacks == 3 && JobGauge.UmbralHearts != 3) return true;
        }
        if (Thunder2.ShouldUse(out act) && !IsLastSpell(true, Thunder2))
        {
            if (HasThunder || !Thunder2.Target.HaveStatus(true, StatusID.Thunder, StatusID.Thunder2, StatusID.Thunder3, StatusID.Thunder4)) return true;
        }

        if (Fire2.ShouldUse(out act) && Level >= 20)
        {
            if (!JobGauge.InUmbralIce && !Freeze.EnoughLevel) return true;

            if (JobGauge.InUmbralIce && ((!Freeze.EnoughLevel && Player.CurrentMp >= 9000) || JobGauge.UmbralHearts == 3)) return true;

            if (JobGauge.InAstralFire && (!Player.HaveStatus(true, StatusID.EnhancedFlare) || JobGauge.UmbralHearts > 1)) return true;
        }
        if (Flare.ShouldUse(out act))
        {
            if (JobGauge.InAstralFire && Player.HaveStatus(true, StatusID.EnhancedFlare)) return true;
            //return true;
        }
        if (Blizzard2.ShouldUse(out act) && JobGauge.UmbralIceStacks != 3)
        {
            //if (!JobGauge.IsEnochianActive) return true;
            //if (JobGauge.InAstralFire) return true;
            return true;
        }
        act = null;
        return false;
    }

    private bool Maintence(out IAction act)
    {
        if (UmbralSoul.ShouldUse(out act)) return true;
        if (Transpose.ShouldUse(out act)) return true;

        return false;
    }
}
