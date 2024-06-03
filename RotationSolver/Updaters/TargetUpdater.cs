using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using RotationSolver.Basic.Configuration;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Updaters;

internal static partial class TargetUpdater
{
    static readonly ObjectListDelay<BattleChara> 
        _raisePartyTargets = new(() => Service.Config.RaiseDelay),
        _raiseAllTargets = new(() => Service.Config.RaiseDelay);
    internal static void UpdateTarget()
    {
        UpdateTimeToKill();
    }

    private static DateTime _lastUpdateTimeToKill = DateTime.MinValue;
    private static readonly TimeSpan _timeToKillSpan = TimeSpan.FromSeconds(0.5);
    private static void UpdateTimeToKill()
    {
        var now = DateTime.Now;
        if (now - _lastUpdateTimeToKill < _timeToKillSpan) return;
        _lastUpdateTimeToKill = now;

        if (DataCenter.RecordedHP.Count >= DataCenter.HP_RECORD_TIME)
        {
            DataCenter.RecordedHP.Dequeue();
        }

        DataCenter.RecordedHP.Enqueue((now, new SortedList<uint, float>(DataCenter.AllTargets.Where(b => b != null && b.CurrentHp != 0).ToDictionary(b => b.ObjectId, b => b.GetHealthRatio()))));
    }
    
}
