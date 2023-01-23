using Dalamud.Game.ClientState.Keys;
using RotationSolver.Data;
using System.Collections.Generic;

namespace RotationSolver.Helpers;

internal static class ConfigurationHelper
{
    public record LocationInfo(EnemyPositional Loc, byte[] Tags);
    public static readonly SortedList<ActionID, LocationInfo> ActionPositionals = new SortedList<ActionID, LocationInfo>()
    {
        {ActionID.FangandClaw, new( EnemyPositional.Flank, new byte[] { 13, 10 })},
        {ActionID.WheelingThrust, new(EnemyPositional.Rear, new byte[] { 10, 13 }) },
        {ActionID.ChaosThrust, new(EnemyPositional.Rear, new byte[] { 61, 28 }) },
        {ActionID.ChaoticSpring, new(EnemyPositional.Rear, new byte[] { 66, 28 }) },
        {ActionID.Demolish, new(EnemyPositional.Rear, new byte[] { 46, 60 }) },
        {ActionID.SnapPunch, new(EnemyPositional.Flank, new byte[] { 19, 21 }) },
        {ActionID.TrickAttack, new(EnemyPositional.Rear, new byte[] { 25 }) },
        {ActionID.AeolianEdge,new(EnemyPositional.Rear, new byte[] { 30, 68 }) },
        {ActionID.ArmorCrush, new(EnemyPositional.Flank, new byte[] { 30, 66 }) },
        {ActionID.Suiton, new(EnemyPositional.Rear, new byte[] { }) },
        {ActionID.Gibbet, new(EnemyPositional.Flank , new byte[] { 11 })},
        {ActionID.Gallows, new(EnemyPositional.Rear, new byte[] { 11 }) },
        {ActionID.Gekko, new(EnemyPositional.Rear , new byte[] { 68, 29, 72 })},
        {ActionID.Kasha, new(EnemyPositional.Flank, new byte[] { 29, 68, 72 }) },
    };


    public static readonly string[] AuthorKeys = new string[]
    {
        "Ig4lHXUohMZNIeheUtAtRg==", //ArchiTed
    };

    public static readonly uint[] BadStatus = new uint[]
    {
        581, //Unable to use.
        579, //Between Area
        574, //Job
        573, //没学会 ?
        572, //一些额外条件未满足 ?
    };

    public static readonly VirtualKey[] Keys = new VirtualKey[] { VirtualKey.CONTROL, VirtualKey.SHIFT, VirtualKey.MENU };

    internal static float GetHealAreaAbility(this ClassJobID job)
        => Service.Configuration.HealthAreaAbilities.TryGetValue(job, out var value) ? value : Service.Configuration.HealthAreaAbility;

    internal static float GetHealAreafSpell(this ClassJobID job)
        => Service.Configuration.HealthAreafSpells.TryGetValue(job, out var value) ? value : Service.Configuration.HealthAreafSpell;

    internal static float GetHealingOfTimeSubtractArea(this ClassJobID job)
        => Service.Configuration.HealingOfTimeSubtractAreas.TryGetValue(job, out var value) ? value : 0.2f;

    internal static float GetHealSingleAbility(this ClassJobID job)
        => Service.Configuration.HealthSingleAbilities.TryGetValue(job, out var value) ? value : Service.Configuration.HealthSingleAbility;

    internal static float GetHealSingleSpell(this ClassJobID job)
        => Service.Configuration.HealthSingleSpells.TryGetValue(job, out var value) ? value : Service.Configuration.HealthSingleSpell;

    internal static float GetHealingOfTimeSubtractSingle(this ClassJobID job)
        => Service.Configuration.HealingOfTimeSubtractSingles.TryGetValue(job, out var value) ? value : 0.2f;

    internal static float GetHealthForDyingTank(this ClassJobID job)
        => Service.Configuration.HealthForDyingTanks.TryGetValue(job, out var value) ? value : 0.15f;
}
