using Dalamud.Game.ClientState.Keys;
using RotationSolver.Basic;
using RotationSolver.Data;
using System.Collections.Generic;

namespace RotationSolver.Helpers;

public static class ConfigurationHelper
{
    public record PositionalInfo(EnemyPositional Pos, byte[] Tags);
    public static readonly SortedList<ActionID, PositionalInfo> ActionPositional = new SortedList<ActionID, PositionalInfo>()
    {
        {ActionID.FangandClaw, new(EnemyPositional.Flank, new byte[] { 13, 10 })},
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
        583, //No items.
        581, //Unable to use.
        579, //Between Area
        574, //Job
        573, //没学会 ?
        572, //一些额外条件未满足 ?
    };

    public static readonly VirtualKey[] Keys = new VirtualKey[] { VirtualKey.CONTROL, VirtualKey.SHIFT, VirtualKey.MENU };

    public static float GetHealAreaAbility(this ClassJobID job)
        => Service.Config.HealthAreaAbilities.TryGetValue(job, out var value) ? value : Service.Config.HealthAreaAbility;

    public static float GetHealAreaSpell(this ClassJobID job)
        => Service.Config.HealthAreaSpells.TryGetValue(job, out var value) ? value : Service.Config.HealthAreaSpell;

    public static float GetHealingOfTimeSubtractArea(this ClassJobID job)
        => Service.Config.HealingOfTimeSubtractAreas.TryGetValue(job, out var value) ? value : 0.2f;

    public static float GetHealSingleAbility(this ClassJobID job)
        => Service.Config.HealthSingleAbilities.TryGetValue(job, out var value) ? value : Service.Config.HealthSingleAbility;

    public static float GetHealSingleSpell(this ClassJobID job)
        => Service.Config.HealthSingleSpells.TryGetValue(job, out var value) ? value : Service.Config.HealthSingleSpell;

    public static float GetHealingOfTimeSubtractSingle(this ClassJobID job)
        => Service.Config.HealingOfTimeSubtractSingles.TryGetValue(job, out var value) ? value : 0.2f;

    public static float GetHealthForDyingTank(this ClassJobID job)
        => Service.Config.HealthForDyingTanks.TryGetValue(job, out var value) ? value : 0.15f;
}
