using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Basic;
using RotationSolver.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Updaters
{
    internal static partial class TargetUpdater
    {
        internal static void UpdateTarget()
        {
            DataCenter.AllTargets = Service.ObjectTable.GetObjectInRadius(30);
            var battles = DataCenter.AllTargets.OfType<BattleChara>();
            UpdateHostileTargets(battles);
            UpdateFriends(battles);
            UpdateNamePlate(Service.ObjectTable.OfType<BattleChara>());
        }
    }
}
