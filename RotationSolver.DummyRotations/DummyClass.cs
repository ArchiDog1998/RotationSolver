using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.DummyRotations
{
    [Rotation("Dummy Rotation", CombatType.PvE, GameVersion = "6.58")]
    [Api(1)]
    public class DummyClass : SummonerRotation
    {
        protected override bool GeneralGCD(out IAction? act)
        {
            return base.GeneralGCD(out act);
        }

        protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
        {
            return base.GeneralAbility(nextGCD, out act);
        }

        protected override IAction? CountDownAction(float remainTime)
        {
            return base.CountDownAction(remainTime);
        }

        protected override bool AttackAbility(IAction nextGCD, out IAction? act)
        {
            return base.AttackAbility(nextGCD, out act);
        }

        protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
        {
            return base.EmergencyAbility(nextGCD, out act);
        }

        public override void DisplayStatus()
        {

        }
    }
}
