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
#if DEBUG
    internal static IEnumerable<BattleChara> AllTargets { get; set; } = new BattleChara[0];
#else
    private static IEnumerable<BattleChara> AllTargets { get; set; } = new BattleChara[0];
#endif

    /// <summary>
    /// 敌人
    /// </summary>
    internal static IEnumerable<BattleChara> HostileTargets { get; private set; } = new BattleChara[0];

    internal static IEnumerable<BattleChara> TarOnMeTargets { get; private set; } = new BattleChara[0];

    internal static IEnumerable<BattleChara> CanInterruptTargets { get; private set; } = new BattleChara[0];

    internal static bool HasHostilesInRange { get; private set; } = false;

    internal static bool IsHostileCastingAOE { get; private set; } = false;

    internal static bool IsHostileCastingToTank { get; private set; } = false;

    internal static unsafe ushort Infate
    {
        get
        {
            try
            {
                if( Service.Configuration.ChangeTargetForFate && (IntPtr)FateManager.Instance() != IntPtr.Zero 
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

    internal unsafe static void UpdateHostileTargets()
    {
        var inFate = Infate;

        AllTargets = TargetFilter.GetTargetable(TargetFilter.GetObjectInRadius(Service.ObjectTable, 30).Where(obj =>
        {
            if (obj is BattleChara c && c.CurrentHp != 0)
            {
                if (c.StatusList.Any(StatusHelper.IsInvincible)) return false;

                if (!c.IsTargetable()) return false;

                if (obj.CanAttack()) return true;
            }
            return false;
        }).Cast<BattleChara>());

        uint[] ids = GetEnemies();

        if (AllTargets != null)
        {
            HostileTargets = CountDown.CountDownTime > 0 ? AllTargets : inFate > 0 ?
                 AllTargets.Where(t => t.FateId() == inFate) :
                AllTargets.Where(t => (t.TargetObject is BattleChara || ids.Contains(t.ObjectId)) && t.FateId() == 0
                || t.TargetObject == Service.ClientState.LocalPlayer);

            switch (IconReplacer.RightNowTargetToHostileType)
            {
                case TargetHostileType.AllTargetsCanAttack:
                    HostileTargets = AllTargets;
                    break;

                default:
                case TargetHostileType.TargetsHaveTargetOrAllTargetsCanAttack:
                    if (!HostileTargets.Any())
                        HostileTargets = AllTargets;
                    break;

                case TargetHostileType.TargetsHaveTarget:
                    break;
            }

            CanInterruptTargets = HostileTargets.Where(tar =>
            {
                var baseCheck = tar.IsCasting && tar.IsCastInterruptible && tar.TotalCastTime >= 2
                    && tar.CurrentCastTime >= Service.Configuration.InterruptibleTime;

                if(!baseCheck) return false;

                var act = Service.DataManager.GetExcelSheet<Action>().GetRow(tar.CastActionId);
                if (act == null) return false;

                //跳过扇形圆型
                if (act.CastType is 3 or 4) return false;
                if (ActionManager.GetActionRange(tar.CastActionId) < 8) return false;
                return true;
            });

            TarOnMeTargets = HostileTargets.Where(tar => tar.TargetObjectId == Service.ClientState.LocalPlayer.ObjectId);

            float radius = 25;
            switch (Service.DataManager.GetExcelSheet<ClassJob>().GetRow(
                Service.ClientState.LocalPlayer.ClassJob.Id).GetJobRole())
            {
                case JobRole.Tank:
                case JobRole.Melee:
                    radius = 3;
                    break;
            }
            HasHostilesInRange = TargetFilter.GetObjectInRadius(HostileTargets, radius).Any();
        }
        else
        {
            AllTargets = HostileTargets = CanInterruptTargets = new BattleChara[0];
            HasHostilesInRange = false;
        }

        if (HostileTargets.Count() == 1)
        {
            var tar = HostileTargets.FirstOrDefault();

            IsHostileCastingToTank = IsHostileCastingTank(tar);
            IsHostileCastingAOE = IsHostileCastingArea(tar);
        }
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
