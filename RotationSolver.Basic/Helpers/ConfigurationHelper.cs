using Dalamud.Game.ClientState.Keys;
using ECommons.ExcelServices;

namespace RotationSolver.Basic.Helpers;

internal static class ConfigurationHelper
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

    public static float GetHealthAreaAbility(this Job job)
        => Service.Config.GetValue(job, Configuration.JobConfigFloat.HealthAreaAbility);

    public static float GetHealthAreaSpell(this Job job)
        => Service.Config.GetValue(job, Configuration.JobConfigFloat.HealthAreaSpell);

    public static float GetHealthAreaAbilityHot(this Job job)
        => Service.Config.GetValue(job, Configuration.JobConfigFloat.HealthAreaAbilityHot);

    public static float GetHealthAreaSpellHot(this Job job)
    => Service.Config.GetValue(job, Configuration.JobConfigFloat.HealthAreaSpellHot);

    public static float GetHealthSingleAbility(this Job job)
        => Service.Config.GetValue(job, Configuration.JobConfigFloat.HealthSingleAbility);

    public static float GetHealthSingleSpell(this Job job)
        => Service.Config.GetValue(job, Configuration.JobConfigFloat.HealthSingleSpell);

    public static float GetHealthSingleAbilityHot(this Job job)
    => Service.Config.GetValue(job, Configuration.JobConfigFloat.HealthSingleAbilityHot);

    public static float GetHealthSingleSpellHot(this Job job)
    => Service.Config.GetValue(job, Configuration.JobConfigFloat.HealthSingleSpellHot);

    public static float GetHealthForDyingTank(this Job job)
        => Service.Config.GetValue(job, Configuration.JobConfigFloat.HealthForDyingTanks);
}
