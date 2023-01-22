using Dalamud.Game.ClientState.JobGauge.Enums;
using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Rotations.Melee.MNK;
internal sealed class MNK_Default : MNK_Base
{
    public override string GameVersion => "6.0";

    public override string RotationName => "Default";


    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealArea, $"{Mantra}"},
        {DescType.DefenseSingle, $"{RiddleofEarth}"},
        {DescType.MoveAction, $"{Thunderclap}"},
    };

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("AutoFormShift", true, "Auto use FormShift");
    }

    private protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Mantra.CanUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (RiddleofEarth.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }


    private bool OpoOpoForm(out IAction act)
    {
        if (ArmoftheDestroyer.CanUse(out act)) return true;
        if (DragonKick.CanUse(out act)) return true;
        if (Bootshine.CanUse(out act)) return true;
        return false;
    }


    private bool RaptorForm(out IAction act)
    {
        if (FourpointFury.CanUse(out act)) return true;

        //确认Buff
        if (Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist) && TwinSnakes.CanUse(out act)) return true;

        if (TrueStrike.CanUse(out act)) return true;
        return false;
    }

    private bool CoerlForm(out IAction act)
    {
        if (Rockbreaker.CanUse(out act)) return true;
        if (Demolish.CanUse(out act)) return true;
        if (SnapPunch.CanUse(out act)) return true;
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
                if (PhantomRush.CanUse(out act, mustUse: true)) return true;
                if (TornadoKick.CanUse(out act, mustUse: true)) return true;
            }
            if (BeastChakras.Contains(BeastChakra.RAPTOR))
            {
                if (RisingPhoenix.CanUse(out act, mustUse: true)) return true;
                if (FlintStrike.CanUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (ElixirField.CanUse(out act, mustUse: true)) return true;
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
        if (Chakra < 5 && Meditation.CanUse(out act)) return true;
        if (Configs.GetBool("AutoFormShift") && FormShift.CanUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (SettingBreak)
        {
            if (RiddleofFire.CanUse(out act)) return true;
            if (Brotherhood.CanUse(out act)) return true;
        }

        //震脚
        if (BeastChakras.Contains(BeastChakra.NONE))
        {
            //有阳斗气
            if ((Nadi & Nadi.SOLAR) != 0)
            {
                //两种Buff都在6s以上
                var dis = Player.WillStatusEndGCD(3, 0, true, StatusID.DisciplinedFist);

                Demolish.CanUse(out _);
                var demo = Demolish.Target.WillStatusEndGCD(3, 0, true, StatusID.Demolish);

                if (!dis && (!demo || !PerfectBalance.IsCoolDown))
                {
                    if (PerfectBalance.CanUse(out act, emptyOrSkipCombo: true)) return true;
                }
            }
            else
            {
                if (PerfectBalance.CanUse(out act, emptyOrSkipCombo: true)) return true;
            }
        }

        if (RiddleofWind.CanUse(out act)) return true;

        if (HowlingFist.CanUse(out act)) return true;
        if (SteelPeak.CanUse(out act)) return true;
        if (HowlingFist.CanUse(out act, mustUse: true)) return true;

        return false;
    }
}
