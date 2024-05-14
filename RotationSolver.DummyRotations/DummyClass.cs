using ImGuiNET;
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
    public class DummyClass : MachinistRotation
    {
        public enum DummyClassConfig
        {
            DummyConfig1,
            DummyConfig2,
            DummyConfig3,
            DummyConfig4,
            DummyConfig5,
            DummyConfig6,
            DummyConfig7,
            DummyConfig8,
            DummyConfig9,
            DummyConfig10,
        }
        [RotationConfig(CombatType.PvE)]
        public DummyClassConfig DummyClassConfigValue { get; set; } = DummyClassConfig.DummyConfig1;
        protected override bool GeneralGCD(out IAction? act)
        {
            act = null;
            if (DrillPvE.CanUse(out act)) return true;
            return false;
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
            ImGui.Text("Dummy Rotation");
            ImGui.Text("Dummy Config: " + DummyClassConfigValue);
        }
    }
}
