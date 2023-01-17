using Dalamud.Game.ClientState.JobGauge.Enums;
using RotationSolver.Actions;
using RotationSolver.Combos.Basic;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Commands;
using RotationSolver.Configuration;
using RotationSolver.Data;
using RotationSolver.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Combos.Melee.MNKCombos;
internal sealed class MNKCombo_Default : MNKRotation_Base
{
    public override string GameVersion => "6.0";

    public override string Author => "无";


    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealArea, $"{Mantra}"},
        {DescType.DefenseSingle, $"{RiddleofEarth}"},
        {DescType.MoveAction, $"{Thunderclap}"},
    };

    private protected override ActionConfiguration CreateConfiguration()
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


    private bool OpoOpoForm(out IAction act)
    {
        if (ArmoftheDestroyer.ShouldUse(out act)) return true;
        if (DragonKick.ShouldUse(out act)) return true;
        if (Bootshine.ShouldUse(out act)) return true;
        return false;
    }


    private bool RaptorForm(out IAction act)
    {
        if (FourpointFury.ShouldUse(out act)) return true;

        //确认Buff
        if (Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist) && TwinSnakes.ShouldUse(out act)) return true;

        if (TrueStrike.ShouldUse(out act)) return true;
        return false;
    }

    private bool CoerlForm(out IAction act)
    {
        if (Rockbreaker.ShouldUse(out act)) return true;
        if (Demolish.ShouldUse(out act)) return true;
        if (SnapPunch.ShouldUse(out act)) return true;
        return false;
    }

    private bool LunarNadi(out IAction act)
    {
        if (OpoOpoForm(out act)) return true;
        return false;
    }

    private bool SolarNadi(out IAction act)
    {
        if (!BeastChakras.Contains(BeastChakra.RAPTOR))
        {
            if (RaptorForm(out act)) return true;
        }
        else if (!BeastChakras.Contains(BeastChakra.OPOOPO))
        {
            if (OpoOpoForm(out act)) return true;
        }
        else
        {
            if (CoerlForm(out act)) return true;
        }

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        bool havesolar = (Nadi & Nadi.SOLAR) != 0;
        bool havelunar = (Nadi & Nadi.LUNAR) != 0;

        //满了的话，放三个大招
        if (!BeastChakras.Contains(BeastChakra.NONE))
        {
            if (havesolar && havelunar)
            {
                if (PhantomRush.ShouldUse(out act, mustUse: true)) return true;
                if (TornadoKick.ShouldUse(out act, mustUse: true)) return true;
            }
            if (BeastChakras.Contains(BeastChakra.RAPTOR))
            {
                if (RisingPhoenix.ShouldUse(out act, mustUse: true)) return true;
                if (FlintStrike.ShouldUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (ElixirField.ShouldUse(out act, mustUse: true)) return true;
            }
        }
        //有震脚就阴阳
        else if (Player.HasStatus(true, StatusID.PerfectBalance))
        {
            if (havesolar && LunarNadi(out act)) return true;
            if (SolarNadi(out act)) return true;
        }

        if (Player.HasStatus(true, StatusID.CoerlForm))
        {
            if (CoerlForm(out act)) return true;
        }
        else if (Player.HasStatus(true, StatusID.RaptorForm))
        {
            if (RaptorForm(out act)) return true;
        }
        else
        {
            if (OpoOpoForm(out act)) return true;
        }

        if (RSCommands.SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
        if (Chakra < 5 && Meditation.ShouldUse(out act)) return true;
        if (Config.GetBoolByName("AutoFormShift") && FormShift.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            if (RiddleofFire.ShouldUse(out act)) return true;
            if (Brotherhood.ShouldUse(out act)) return true;
        }

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

        return false;
    }
}
