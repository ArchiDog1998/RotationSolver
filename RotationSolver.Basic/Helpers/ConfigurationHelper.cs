using Dalamud.Game.ClientState.Keys;

namespace RotationSolver.Basic.Helpers;

internal static class ConfigurationHelper
{
    public static readonly SortedList<ActionID, EnemyPositional> ActionPositional = new()
    {
        {ActionID.FangAndClaw, EnemyPositional.Flank},
        {ActionID.WheelingThrust, EnemyPositional.Rear},
        {ActionID.ChaosThrust, EnemyPositional.Rear },
        {ActionID.ChaoticSpring, EnemyPositional.Rear },
        {ActionID.Demolish, EnemyPositional.Rear },
        {ActionID.SnapPunch, EnemyPositional.Flank },
        {ActionID.TrickAttack, EnemyPositional.Rear },
        {ActionID.AeolianEdge,EnemyPositional.Rear },
        {ActionID.ArmorCrush, EnemyPositional.Flank },
        {ActionID.Gibbet, EnemyPositional.Flank},
        {ActionID.Gallows, EnemyPositional.Rear },
        {ActionID.Gekko, EnemyPositional.Rear},
        {ActionID.Kasha, EnemyPositional.Flank },
    };

    public static readonly uint[] BadStatus = new uint[]
    {
        583, //No items.
        581, //Unable to use.
        579, //Between Area
        574, //Job
        573, //没学会 ?
    };

    public static readonly VirtualKey[] Keys = new VirtualKey[] { VirtualKey.CONTROL, VirtualKey.SHIFT, VirtualKey.MENU };
}
