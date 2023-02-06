using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Updaters;

internal static partial class TargetUpdater
{
    /// <summary>
    /// 敌人
    /// </summary>
    internal static ObjectListDelay<BattleChara> HostileTargets { get; } = new ObjectListDelay<BattleChara>(
        () => (Service.Configuration.HostileDelayMin, Service.Configuration.HostileDelayMax));

    internal static IEnumerable<BattleChara> TarOnMeTargets { get; private set; } = new BattleChara[0];

    internal static ObjectListDelay<BattleChara> CanInterruptTargets { get;} = new ObjectListDelay<BattleChara>(
        () => (Service.Configuration.InterruptDelayMin, Service.Configuration.InterruptDelayMax));
    internal static bool HasHostilesInRange { get; private set; } = false;

    internal static bool IsHostileCastingAOE { get; private set; } = false;

    internal static bool IsHostileCastingToTank { get; private set; } = false;

    internal static unsafe ushort FateId
    {
        get
        {
            try
            {
                if(Service.Configuration.ChangeTargetForFate && (IntPtr)FateManager.Instance() != IntPtr.Zero 
                    && (IntPtr)FateManager.Instance()->CurrentFate != IntPtr.Zero
                    && Service.ClientState.LocalPlayer.Level <= FateManager.Instance()->CurrentFate->MaxLevel)
                {
                    return FateManager.Instance()->CurrentFate->FateId;
                }
            }
            catch(Exception ex)
            {
                PluginLog.Error(ex.StackTrace);
            }
            return 0;
        }
    }

    private static float JobRange
    {
        get
        {
            float radius = 25;
            switch (Service.DataManager.GetExcelSheet<ClassJob>().GetRow(
                Service.ClientState.LocalPlayer.ClassJob.Id).GetJobRole())
            {
                case JobRole.Tank:
                case JobRole.Melee:
                    radius = 3;
                    break;
            }
            return radius;
        }
    }

    private unsafe static void UpdateHostileTargets(IEnumerable<BattleChara> allTargets)
    {
        var allAttackableTargets = allTargets.Where(b =>
        {
            if (!b.IsTargetable()) return false;

            if (b.StatusList.Any(StatusHelper.IsInvincible)) return false;

            return b.CanAttack();
        });

        HostileTargets.Delay(GetHostileTargets(allAttackableTargets));

        CanInterruptTargets.Delay(HostileTargets.Where(ObjectHelper.CanInterrupt));

        TarOnMeTargets = HostileTargets.Where(tar => tar.TargetObjectId == Service.ClientState.LocalPlayer.ObjectId);

        HasHostilesInRange = TargetFilter.GetObjectInRadius(HostileTargets, JobRange).Any();

        if (HostileTargets.Count() == 1)
        {
            var tar = HostileTargets.FirstOrDefault();

            IsHostileCastingToTank = IsHostileCastingTank(tar);
            IsHostileCastingAOE = IsHostileCastingArea(tar);
        }
        else
        {
            IsHostileCastingToTank = IsHostileCastingAOE = false;
        }
    }

    private static IEnumerable<BattleChara> GetHostileTargets(IEnumerable<BattleChara> allattackableTargets)
    {
        var type = IconReplacer.RightNowTargetToHostileType;
        if (type == TargetHostileType.AllTargetsCanAttack || CountDown.CountDownTime > 0)
        {
            return allattackableTargets;
        }

        uint[] ids = GetEnemies();
        var fateId = FateId;

        var hostiles = allattackableTargets.Where(t =>
        {
            if (ids.Contains(t.ObjectId)) return true;
            if (t.TargetObject == Service.ClientState.LocalPlayer) return true;

            //Remove other's treasure.
            if (t.IsOthersTreasure()) return false;

            return fateId > 0 ? t.FateId() == fateId : t.TargetObject is BattleChara;
        });

        if (type == TargetHostileType.TargetsHaveTargetOrAllTargetsCanAttack)
        {
            if (!hostiles.Any()) hostiles = allattackableTargets;
        }

        return hostiles;
    }

    private static unsafe uint[] GetEnemies()
    {
        if (!Service.Configuration.AddEnemyListToHostile) return new uint[0];

        var addonByName = Service.GameGui.GetAddonByName("_EnemyList", 1);
        if (addonByName == IntPtr.Zero) return new uint[0];

        var addon = (AddonEnemyList*)addonByName;
        var numArray = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetRaptureAtkModule()->AtkModule.AtkArrayDataHolder.NumberArrays[19];
        List<uint> list = new List<uint>(addon->EnemyCount);
        for (var i = 0; i < addon->EnemyCount; i++)
        {
            list.Add((uint)numArray->IntArray[8 + i * 6]);
        }
        return list.ToArray();
    }

    private static bool IsHostileCastingTank(BattleChara h)
    {
        return IsHostileCastingBase(h, (act) =>
        {
            return h.CastTargetObjectId == h.TargetObjectId;
        });
    }

    private static bool IsHostileCastingArea(BattleChara h)
    {
        return IsHostileCastingBase(h, (act) =>
        {
            if (h.CastTargetObjectId == h.TargetObjectId) return false;
            if ((act.CastType == 1 || act.CastType == 2) &&
                act.Range == 0 &&
                act.EffectRange >= 40)
                return true;
            return false;
        });
    }

    private static bool IsHostileCastingBase(BattleChara h, Func<Action, bool> check)
    {
        if (h.IsCasting)
        {
            if (h.IsCastInterruptible) return false;
            var last = h.TotalCastTime - h.CurrentCastTime;

            if (!(h.TotalCastTime > 2 && last < 6 && last > 0.5)) return false;

            var action = Service.DataManager.GetExcelSheet<Action>().GetRow(h.CastActionId);
            return check?.Invoke(action) ?? false;
        }
        return false;
    }
}
