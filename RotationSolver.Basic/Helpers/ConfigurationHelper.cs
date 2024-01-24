using Dalamud.Game.ClientState.Keys;

namespace RotationSolver.Basic.Helpers;

internal static class ConfigurationHelper
{
    public static readonly SortedList<ActionID, EnemyPositional> ActionPositional = new()
    {
        {ActionID.FangAndClawPvE, EnemyPositional.Flank},
        {ActionID.WheelingThrustPvE, EnemyPositional.Rear},
        {ActionID.ChaosThrustPvE, EnemyPositional.Rear },
        {ActionID.ChaoticSpringPvE, EnemyPositional.Rear },
        {ActionID.DemolishPvE, EnemyPositional.Rear },
        {ActionID.SnapPunchPvE, EnemyPositional.Flank },
        {ActionID.TrickAttackPvE, EnemyPositional.Rear },
        {ActionID.AeolianEdgePvE,EnemyPositional.Rear },
        {ActionID.ArmorCrushPvE, EnemyPositional.Flank },
        {ActionID.GibbetPvE, EnemyPositional.Flank},
        {ActionID.GallowsPvE, EnemyPositional.Rear },
        {ActionID.GekkoPvE, EnemyPositional.Rear},
        {ActionID.KashaPvE, EnemyPositional.Flank },
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
