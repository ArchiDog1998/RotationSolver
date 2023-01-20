using Dalamud.Game.ClientState.Keys;
using Newtonsoft.Json.Linq;
using RotationSolver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Helpers;

internal static class ConfigurationHelper
{
    public record LocationInfo(EnemyPositional Loc, byte[] Tags);
    public static readonly SortedList<ActionID, LocationInfo> ActionLocations = new SortedList<ActionID, LocationInfo>()
    {
        {ActionID.FangandClaw, new( EnemyPositional.Side, new byte[] { 13, 10 })},
        {ActionID.WheelingThrust, new(EnemyPositional.Back, new byte[] { 10, 13 }) },
        {ActionID.ChaosThrust, new(EnemyPositional.Back, new byte[] { 61, 28 }) },
        {ActionID.ChaoticSpring, new(EnemyPositional.Back, new byte[] { 66, 28 }) },
        {ActionID.Demolish, new(EnemyPositional.Back, new byte[] { 46, 60 }) },
        {ActionID.SnapPunch, new(EnemyPositional.Side, new byte[] { 19, 21 }) },
        {ActionID.TrickAttack, new(EnemyPositional.Back, new byte[] { 25 }) },
        {ActionID.AeolianEdge,new( EnemyPositional.Back, new byte[] { 30, 68 }) },
        {ActionID.ArmorCrush, new(EnemyPositional.Side, new byte[] { 30, 66 }) },
        {ActionID.Suiton, new(EnemyPositional.Back, new byte[] { }) },
        {ActionID.Gibbet, new(EnemyPositional.Side , new byte[] { 11 })},
        {ActionID.Gallows, new(EnemyPositional.Back, new byte[] { 11 }) },
        {ActionID.Gekko, new(EnemyPositional.Back , new byte[] { 68, 29, 72 })},
        {ActionID.Kasha, new(EnemyPositional.Side, new byte[] { 29, 68, 72 }) },
    };


    public static readonly string[] AuthorKeys = new string[]
    {
        "Ig4lHXUohMZNIeheUtAtRg==",
    };

    public static readonly uint[] BadStatus = new uint[]
    {
        579, //状态限制
        573, //没学会
        572, //一些额外条件未满足
    };

    public static readonly VirtualKey[] Keys = new VirtualKey[] { VirtualKey.CONTROL, VirtualKey.SHIFT, VirtualKey.MENU };

    internal static float GetHealAreaAbility(ClassJobID job)
        => Service.Configuration.HealthAreaAbilities.TryGetValue(job, out var value) ? value : Service.Configuration.HealthAreaAbility;

    internal static float GetHealAreafSpell(ClassJobID job)
        => Service.Configuration.HealthAreafSpells.TryGetValue(job, out var value) ? value : Service.Configuration.HealthAreafSpell;

    internal static float GetHealingOfTimeSubtractArea(ClassJobID job)
        => Service.Configuration.HealingOfTimeSubtractAreas.TryGetValue(job, out var value) ? value : 0.2f;

    internal static float GetHealSingleAbility(ClassJobID job)
        => Service.Configuration.HealthSingleAbilities.TryGetValue(job, out var value) ? value : Service.Configuration.HealthSingleAbility;

    internal static float GetHealSingleSpell(ClassJobID job)
        => Service.Configuration.HealthSingleSpells.TryGetValue(job, out var value) ? value : Service.Configuration.HealthSingleSpell;

    internal static float GetHealingOfTimeSubtractSingle(ClassJobID job)
        => Service.Configuration.HealingOfTimeSubtractSingles.TryGetValue(job, out var value) ? value : 0.2f;

    internal static float GetHealthForDyingTank(ClassJobID job)
        => Service.Configuration.HealthForDyingTanks.TryGetValue(job, out var value) ? value : 0.15f;
}
