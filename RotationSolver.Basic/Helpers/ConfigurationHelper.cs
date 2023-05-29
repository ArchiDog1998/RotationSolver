using Dalamud.Game.ClientState.Keys;
using ECommons.ExcelServices;

namespace RotationSolver.Basic.Helpers;

public static class ConfigurationHelper
{
    public static readonly SortedList<ActionID, EnemyPositional> ActionPositional = new()
    {
        {ActionID.FangandClaw, EnemyPositional.Flank},
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

    public static float GetHealAreaAbility(this Job job)
        => Service.Config.HealthAreaAbilities.TryGetValue(job, out var value) ? value : Service.Config.HealthAreaAbility;

    public static float GetHealAreaSpell(this Job job)
        => Service.Config.HealthAreaSpells.TryGetValue(job, out var value) ? value : Service.Config.HealthAreaSpell;

    public static float GetHealingOfTimeSubtractArea(this Job job)
        => Service.Config.HealingOfTimeSubtractAreas.TryGetValue(job, out var value) ? value : HealingOfTimeSubtractAreasDefault;

    public static float GetHealSingleAbility(this Job job)
        => Service.Config.HealthSingleAbilities.TryGetValue(job, out var value) ? value : Service.Config.HealthSingleAbility;

    public static float GetHealSingleSpell(this Job job)
        => Service.Config.HealthSingleSpells.TryGetValue(job, out var value) ? value : Service.Config.HealthSingleSpell;

    public static float GetHealingOfTimeSubtractSingle(this Job job)
        => Service.Config.HealingOfTimeSubtractSingles.TryGetValue(job, out var value) ? value : HealingOfTimeSubtractSinglesDefault;

    public static float GetHealthForDyingTank(this Job job)
        => Service.Config.HealthForDyingTanks.TryGetValue(job, out var value) ? value : HealthForDyingTanksDefault;

    public const float HealingOfTimeSubtractAreasDefault = 0.2f;
    public const float HealingOfTimeSubtractSinglesDefault = 0.2f;
    public const float HealthForDyingTanksDefault = 0.15f;
}
