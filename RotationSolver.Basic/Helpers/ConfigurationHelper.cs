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

    public static readonly VirtualKey[] Keys = new VirtualKey[] { VirtualKey.CONTROL, VirtualKey.SHIFT, VirtualKey.MENU, VirtualKey.LBUTTON, VirtualKey.MBUTTON, VirtualKey.RBUTTON };

    public static float GetHealthAreaAbility(this Job job)
        => Service.Config.HealthAreaAbilities.TryGetValue(job, out var value) ? value : Service.Config.HealthAreaAbility;

    public static float GetHealthAreaSpell(this Job job)
        => Service.Config.HealthAreaSpells.TryGetValue(job, out var value) ? value : Service.Config.HealthAreaSpell;

    public static float GetHealthAreaAbilityHot(this Job job)
        => Service.Config.HealthAreaAbilitiesHot.TryGetValue(job, out var value) ? value : Service.Config.HealthAreaAbilityHot;

    public static float GetHealthAreaSpellHot(this Job job)
    => Service.Config.HealthAreaSpellsHot.TryGetValue(job, out var value) ? value : Service.Config.HealthAreaSpellHot;

    public static float GetHealthSingleAbility(this Job job)
        => Service.Config.HealthSingleAbilities.TryGetValue(job, out var value) ? value : Service.Config.HealthSingleAbility;

    public static float GetHealthSingleSpell(this Job job)
        => Service.Config.HealthSingleSpells.TryGetValue(job, out var value) ? value : Service.Config.HealthSingleSpell;

    public static float GetHealthSingleAbilityHot(this Job job)
    => Service.Config.HealthSingleAbilitiesHot.TryGetValue(job, out var value) ? value : Service.Config.HealthSingleAbilityHot;

    public static float GetHealthSingleSpellHot(this Job job)
    => Service.Config.HealthSingleSpellsHot.TryGetValue(job, out var value) ? value : Service.Config.HealthSingleSpellHot;


    public static float GetHealthForDyingTank(this Job job)
        => Service.Config.HealthForDyingTanks.TryGetValue(job, out var value) ? value : HealthForDyingTanksDefault;

    public const float HealthForDyingTanksDefault = 0.15f;
}
