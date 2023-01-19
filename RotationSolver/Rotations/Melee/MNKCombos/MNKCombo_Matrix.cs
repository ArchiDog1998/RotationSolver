using Dalamud.Game.ClientState.JobGauge.Enums;
using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Rotations.Melee.MNKCombos;

internal sealed class MNKCombo_Matrix : MNKRotation_Base
{
    public override string GameVersion => "6.0";

    public override string RotationName => "Matrix - 工整/阴阳起手";

    private uint balanceType; //0 阴       、 1-阳
    private int balanceStep; // 1 第一段震脚 、 2 第二段震脚 、 0 小爆发震脚

    // 有阴
    bool HaveLunar => (Nadi & Nadi.LUNAR) != 0;

    // 有阳
    bool HaveSolar => (Nadi & Nadi.SOLAR) != 0;

    // 是否拥有三种查克拉
    private bool HaveBeastChakra1 => BeastChakras.Contains(BeastChakra.OPOOPO);
    private bool HaveBeastChakra2 => BeastChakras.Contains(BeastChakra.RAPTOR);
    private bool HaveBeastChakra3 => BeastChakras.Contains(BeastChakra.COEURL);

    // 查克拉的数量
    private int BeastChakraNum => BeastChakras.ToList().FindAll(it => it != BeastChakra.NONE).Count;
    private int BeastChakra1Num => BeastChakras.ToList().FindAll(it => it == BeastChakra.OPOOPO).Count;
    private int BeastChakra2Num => BeastChakras.ToList().FindAll(it => it == BeastChakra.RAPTOR).Count;
    private int BeastChakra3Num => BeastChakras.ToList().FindAll(it => it == BeastChakra.COEURL).Count;

    // 无相身形/三种型
    private bool In0 => Player.HasStatus(true, StatusID.FormlessFist);
    private bool In1 => Player.HasStatus(true, StatusID.OpoOpoForm);
    private bool In2 => Player.HasStatus(true, StatusID.RaptorForm);
    private bool In3 => Player.HasStatus(true, StatusID.CoerlForm);


    public override SortedList<DescType, string> DescriptionDict => new()
    {
        { DescType.HealArea, $"{Mantra}" },
        { DescType.DefenseSingle, $"{RiddleofEarth}" },
        { DescType.MoveAction, $"{Thunderclap}" },
    };

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("AutoFormShift", true, "自动演武");
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Mantra.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (RiddleofEarth.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Feint.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool MoveForwardAbility(byte abilityRemain, out IAction act)
    {
        if (Thunderclap.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    /**
     * 军体拳
     */
    private bool JunTiQuan(out IAction act)
    {
        act = null;
        if (In3)
        {
            if (Form3(out act)) return true;
        }

        if (In2)
        {
            if (Form2(out act)) return true;
        }

        if (In1)
        {
            if (Form1(out act)) return true;
        }

        if (In0)
        {
            if (balanceType == 0)
            {
                if (Form1(out act)) return true;
            }

            if (balanceType == 1)
            {
                // 四面脚
                if (FourpointFury.ShouldUse(out act)) return true;
                // 双掌打
                if (FourpointFury.ShouldUse(out act)) return true;
                if (TwinSnakes.ShouldUse(out act)) return true;
            }
        }

        return false;
    }

    /**
     * 一型战技
     */
    private bool Form1(out IAction act)
    {
        // 破坏神冲 -> 破坏神脚
        if (ArmoftheDestroyer.ShouldUse(out act)) return true;
        // 双龙
        if (DragonKick.ShouldUse(out act)) return true;
        // 连击
        if (Bootshine.ShouldUse(out act)) return true;
        return false;
    }

    /**
     * 二型战技
     */
    private bool Form2(out IAction act)
    {
        // 四面脚
        if (FourpointFury.ShouldUse(out act)) return true;
        if (Player.HasStatus(true, StatusID.RiddleofFire)
            && !Player.HasStatus(true, StatusID.PerfectBalance))
        {
            if (Target.WillStatusEndGCD(2, 0, true, StatusID.Demolish))
            {
                if (TwinSnakes.ShouldUse(out act)) return true;
            }
        }
        // 双掌打（buff在3GCD内结束使用）
        if (Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist)
            && TwinSnakes.ShouldUse(out act)) return true;
        // 正拳
        if (TrueStrike.ShouldUse(out act)) return true;
        return false;
    }

    /**
     * 三型战技
     */
    private bool Form3(out IAction act)
    {
        // 地烈劲
        if (Rockbreaker.ShouldUse(out act)) return true;
        // 破碎拳
        if (!Target.HasStatus(true, StatusID.Demolish)
            || Target.WillStatusEndGCD(3, 0, true, StatusID.Demolish))
        {
            if (Demolish.ShouldUse(out act, mustUse: true)) return true;
        }

        // 崩拳
        if (SnapPunch.ShouldUse(out act)) return true;
        return false;
    }

    /**
     * 震脚攒阴阳
     */
    private bool Balance(out IAction act)
    {
        act = null;

        if (Player.HasStatus(true, StatusID.PerfectBalance))
        {
            if (balanceType == 0)
            {
                if (Balance4Lunar(out act)) return true;
            }

            if (balanceType == 1)
            {
                if (Balance4Solar(out act)) return true;
            }
        }

        return false;
    }

    /**
     * 阴震脚
     */
    private bool Balance4Lunar(out IAction act)
    {
        act = null;

        if (BeastChakraNum == 0)
        {
            // 破坏神脚
            if (ShadowoftheDestroyer.ShouldUse(out act)) return true;
            // 地烈劲 （破坏神冲还没有升级成破坏神脚时，在aoe时，使用地烈劲）
            if (Rockbreaker.ShouldUse(out act)) return true;
            // 双龙
            if (DragonKick.ShouldUse(out act)) return true;
            // 连击
            if (Bootshine.ShouldUse(out act)) return true;
        }

        if (BeastChakra1Num > 0)
        {
            if (Form1(out act)) return true;
        }

        if (BeastChakra2Num > 0)
        {
            if (Form2(out act)) return true;
        }

        if (BeastChakra3Num > 0)
        {
            if (Form3(out act)) return true;
        }

        return false;
    }

    /**
     * 阳震脚
     */
    private bool Balance4Solar(out IAction act)
    {
        // 没有2型查克拉，并且功力即将消失，补上功力
        if (!HaveBeastChakra2
            && Player.WillStatusEndGCD(1, 0, true, StatusID.DisciplinedFist)
            && TwinSnakes.ShouldUse(out act)) return true;

        // 没有3型查克拉，并且破碎即将消失，补上功力
        if (!HaveBeastChakra3
            && Target.WillStatusEndGCD(1, 0, true, StatusID.Demolish)
            && Demolish.ShouldUse(out act, mustUse: true)) return true;

        if (!HaveBeastChakra1)
        {
            if (Form1(out act)) return true;
        }
        else if (!HaveBeastChakra3)
        {
            if (Form3(out act)) return true;
        }
        else
        {
            if (Form2(out act)) return true;
        }

        return false;
    }

    /**
     * 必杀技
     */
    private bool BiShaJi(out IAction act)
    {
        act = null;

        // 查克拉满了的话，放必杀
        if (!BeastChakras.Contains(BeastChakra.NONE))
        {
            // 阴~阳斗气  -->  斗舞/斗魂脚
            if (HaveSolar && HaveLunar)
            {
                if (PhantomRush.ShouldUse(out act, mustUse: true))
                {
                    if (balanceStep == 1)
                    {
                        balanceStep = 2;
                    }

                    return true;
                }

                if (TornadoKick.ShouldUse(out act, mustUse: true))
                {
                    if (balanceStep == 1)
                    {
                        balanceStep = 2;
                    }

                    return true;
                }
            }

            // 三种查克拉  -->  凤凰舞/爆裂脚
            if (BeastChakra1Num == 1 && BeastChakra2Num == 1)
            {
                if (RisingPhoenix.ShouldUse(out act, mustUse: true))
                {
                    if (balanceStep == 1)
                    {
                        balanceStep = 2;
                    }

                    return true;
                }

                if (FlintStrike.ShouldUse(out act, mustUse: true))
                {
                    if (balanceStep == 1)
                    {
                        balanceStep = 2;
                    }

                    return true;
                }
            }

            // 一种查克拉 --> 苍气炮
            if (BeastChakra1Num == 3 || BeastChakra2Num == 3 || BeastChakra3Num == 3)
            {
                if (ElixirField.ShouldUse(out act, mustUse: true))
                {
                    if (balanceStep == 1)
                    {
                        balanceStep = 2;
                    }

                    return true;
                }
            }
            // 为武僧注入灵魂 --> 翻天脚
            else
            {
                if (CelestialRevolution.ShouldUse(out act, mustUse: true))
                {
                    if (balanceStep == 1)
                    {
                        balanceStep = 2;
                    }

                    return true;
                }
            }
        }

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        // 必杀技
        if (BiShaJi(out act)) return true;

        // 震脚中
        if (Balance(out act)) return true;

        // 军体拳
        if (JunTiQuan(out act)) return true;

        if (RSCommands.SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
        if (Chakra < 5 && Meditation.ShouldUse(out act)) return true;
        if (Configs.GetBool("AutoFormShift") && FormShift.ShouldUse(out act)) return true;

        return false;
    }

    /**
     * 爆发（红莲+金兰）
     */
    private bool ShouldBurst(byte abilityRemain, out IAction act)
    {
        act = null;

        // 有功力
        if (Player.HasStatus(true, StatusID.DisciplinedFist))
        {
            // 当前有震脚 或者 在1gcd内有震脚可以用
            if (Player.HasStatus(true, StatusID.PerfectBalance)
                || PerfectBalance.WillHaveOneChargeGCD(1))
            {
                // abilityRemain保持红莲在后半gcd窗口使用
                if (abilityRemain == 1)
                {
                    if (RiddleofFire.ShouldUse(out act)) return true;
                }
            }

            // 用了红莲再开金兰
            if (Player.HasStatus(true, StatusID.RiddleofFire))
            {
                if (Brotherhood.ShouldUse(out act)) return true;
            }
        }

        return false;
    }

    /**
     * 震脚
     */
    private bool ShouldPerfectBalance(out IAction act)
    {
        act = null;

        // 绑定在身型2型使用（即使用完1型战技后）
        if (!In2)
        {
            return false;
        }

        // 身上必须带有功力buff
        if (!Player.HasStatus(true, StatusID.DisciplinedFist))
        {
            return false;
        }

        // 判断是否处于金兰或者即将有金兰
        if (Player.HasStatus(true, StatusID.Brotherhood)
            || !Player.HasStatus(true, StatusID.Brotherhood) && Brotherhood.WillHaveOneChargeGCD(3))
        {
            if (PerfectBalanceFor120S(out act)) return true;
        }
        else
        {
            if (PerfectBalanceFor60S(out act)) return true;
        }

        return false;
    }

    /**
     * 120秒的大红莲爆发期
     */
    private bool PerfectBalanceFor120S(out IAction act)
    {
        act = null;

        // 处在红莲期间,
        if (Player.HasStatus(true, StatusID.RiddleofFire))
        {
            // 一段震脚(9GCD内红莲不结束, 宽松一点写成8GCD)
            if (!Player.WillStatusEndGCD(8, 0, true, StatusID.RiddleofFire))
            {
                // 3GCD内需要补功力
                if (Player.WillStatusEndGCD(4, 0, true, StatusID.DisciplinedFist))
                {
                    if (PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true))
                    {
                        // 阳震脚
                        balanceType = 1;
                        balanceStep = 1;
                        return true;
                    }
                }
                else
                {
                    if (PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true))
                    {
                        // 阴震脚
                        balanceType = 0;
                        balanceStep = 1;
                        return true;
                    }
                }
            }
            // 二段震脚
            else if (balanceStep == 2)
            {
                if (Player.WillStatusEndGCD(4, 0, true, StatusID.DisciplinedFist))
                {
                    if (PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true))
                    {
                        // 阳震脚
                        balanceType = 1;
                        balanceStep = 2;
                        return true;
                    }
                }
                else
                {
                    if (PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true))
                    {
                        // 阴震脚
                        balanceType = 0;
                        balanceStep = 2;
                        return true;
                    }
                }
            }
        }

        return false;
    }


    /**
     * 60秒的小红莲爆发期
     */
    private bool PerfectBalanceFor60S(out IAction act)
    {
        act = null;

        // 处在红莲期间,
        if (Player.HasStatus(true, StatusID.RiddleofFire))
        {
            if (!Player.WillStatusEndGCD(5, 0, true, StatusID.DisciplinedFist)
                && !Target.WillStatusEndGCD(7, 0, true, StatusID.Demolish))
            {
                // 3GCD内需要补dot
                if (PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true))
                {
                    // 阴震脚
                    balanceType = 0;
                    balanceStep = 0;
                    return true;
                }
            }
        }

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null;

        if (SettingBreak)
        {
            // 震脚
            if (ShouldPerfectBalance(out act)) return true;

            // 红莲 + 金兰
            if (ShouldBurst(abilityRemain, out act)) return true;

            if (Player.HasStatus(true, StatusID.DisciplinedFist))
            {
                if (!(Brotherhood.IsCoolDown && PerfectBalance.CurrentCharges == PerfectBalance.MaxCharges &&
                     RiddleofFire.IsCoolDown))
                {
                    if (RiddleofWind.ShouldUse(out act)) return true;

                    if (HowlingFist.ShouldUse(out act)) return true;
                    if (SteelPeak.ShouldUse(out act)) return true;
                    if (HowlingFist.ShouldUse(out act, mustUse: true)) return true;
                }
            }
        }
        /*else
        {
            if (Player.HasStatus(true, StatusID.DisciplinedFist))
            {
                //震脚
                if (BeastChakras.Contains(BeastChakra.NONE))
                {
                    //有阳斗气
                    if ((Nadi & Nadi.SOLAR) != 0)
                    {
                        //两种Buff都在6s以上
                        var dis = Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist);

                        Demolish.ShouldUse(out _);
                        var demo = Demolish.Target.WillStatusEndGCD(3, 0, true, StatusID.Demolish);

                        if (!dis && (!demo || !PerfectBalance.IsCoolDown))
                        {
                            if (PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                        }
                    }
                    else
                    {
                        if (PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                    }
                }
                
                if (RiddleofWind.ShouldUse(out act)) return true;

                if (HowlingFist.ShouldUse(out act)) return true;
                if (SteelPeak.ShouldUse(out act)) return true;
                if (HowlingFist.ShouldUse(out act, mustUse: true)) return true;
            }
        }*/

        return false;
    }
}