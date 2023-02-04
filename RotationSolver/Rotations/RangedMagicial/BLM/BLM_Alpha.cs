using RotationSolver.Actions;
using RotationSolver.Rotations.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Rotations.RangedMagicial.BLM;

internal class BLM_Alpha : BLM_Base
{
    public override string GameVersion => "6.31";

    public override string RotationName => "Alpha";

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        act = null;
        return false;
    }
}
