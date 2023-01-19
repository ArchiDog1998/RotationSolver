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
