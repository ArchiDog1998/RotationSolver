using Dalamud.Game.ClientState.Objects.Types;
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
        internal static IEnumerable<GameObject> AllTargets { get; private set; }
        internal static void UpdateTarget()
        {
            AllTargets = Service.ObjectTable.GetObjectInRadius(30);
            var battles = AllTargets.OfType<BattleChara>();
            UpdateHostileTargets(battles);
            UpdateFriends(battles);
            UpdateNamePlate(Service.ObjectTable.OfType<BattleChara>());
        }
    }
}
