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

    public static readonly uint[] BadStatus =
    [
        583, //No items.
        581, //Unable to use.
        579, //Between Area
        574, //Job
        573, //没学会 ?
    ];

    public static VirtualKey ToVirtual(this ConsoleModifiers modifiers)
    {
        return modifiers switch
        {
            ConsoleModifiers.Alt => VirtualKey.MENU,
            ConsoleModifiers.Shift => VirtualKey.SHIFT,
            _ => VirtualKey.CONTROL,
        };
    }
}
