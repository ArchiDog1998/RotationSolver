using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static IEnumerable<BattleChara> TarOnMeTargets { get; private set; } = new BattleChara[0];

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static IEnumerable<BattleChara> CanInterruptTargets { get; private set; } = new BattleChara[0];

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static bool HaveHostilesInRange { get; private set; } = false;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static bool IsHostileAOE { get; private set; } = false;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static bool IsHostileTank { get; private set; } = false;

    internal unsafe static void UpdateHostileTargets()
    {
        //能打的目标
        AllTargets = TargetFilter.GetTargetable(TargetFilter.GetObjectInRadius(Service.ObjectTable, 30).Where(obj =>
        {
            if (obj is BattleChara c && c.CurrentHp != 0)
            {
                if (c.StatusList.Any(status => Service.DataManager.GetExcelSheet<Status>()
                    .GetRow(status.StatusId).Icon == 15024)) return false;

                try
                {
                    var gameObj = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)c.Address;

                    //不可选中
                    if (!gameObj->GetIsTargetable()) return false;

                    //不在fate中，不打fate怪。
                    if (FateManager.Instance()->FateJoined == 0
                        && gameObj->FateId > 0) return false;
                }
                catch
                {
                    return false;
                }

                if (obj.CanAttack()) return true;
            }
            return false;
        }).Cast<BattleChara>());

        //Filter the fate objects.
        bool inFate = Service.Configuration.ChangeTargetForFate && FateManager.Instance()->FateJoined > 0;
        uint[] ids = GetEnemies() ?? new uint[0];

        if (AllTargets != null)
        {
            HostileTargets = CountDown.CountDownTime > 0 ? AllTargets : inFate ?
                 AllTargets.Where(t =>
                 {
                     try
                     {
                         return t.TargetObject == Service.ClientState.LocalPlayer ||
                            ((FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)t.Address)->FateId == FateManager.Instance()->CurrentFate->FateId
                            && Service.ClientState.LocalPlayer.Level <= FateManager.Instance()->CurrentFate->MaxLevel;
                     }
                     catch
                     {
                         return false;
                     }
                 }) :
                AllTargets.Where(t => t.TargetObject is BattleChara || ids.Contains(t.ObjectId));

            switch (IconReplacer.RightNowTargetToHostileType)
            {
                case  TargetHostileType.AllTargetsCanAttack:
                    HostileTargets = AllTargets;
                    break;

                default:
                case  TargetHostileType.TargetsHaveTargetOrAllTargetsCanAttack:
                    if (!HostileTargets.Any())
                        HostileTargets = AllTargets;
                    break;

                case  TargetHostileType.TargetsHaveTarget:
                    break;
            }

            CanInterruptTargets = HostileTargets.Where(tar => tar.IsCasting && tar.IsCastInterruptible && tar.TotalCastTime >= 2
            && tar.CurrentCastTime >= Service.Configuration.InterruptibleTime);

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
            HaveHostilesInRange = TargetFilter.GetObjectInRadius(HostileTargets, radius).Any();
        }
        else
        {
            AllTargets = HostileTargets = CanInterruptTargets = new BattleChara[0];
            HaveHostilesInRange = false;
        }

        if (HostileTargets.Count() == 1)
        {
            var tar = HostileTargets.FirstOrDefault();

            IsHostileTank = IsHostileCastingTank(tar);
            IsHostileAOE = IsHostileCastingArea(tar);
        }
    }

    private static unsafe uint[] GetEnemies()
    {
        if (!Service.Configuration.AddEnemyListToHostile) return new uint[0];

        var addonByName = Service.GameGui.GetAddonByName("_EnemyList", 1);
        if (addonByName != IntPtr.Zero)
        {
            var addon = (AddonEnemyList*)addonByName;
            var numArray = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetRaptureAtkModule()->AtkModule.AtkArrayDataHolder.NumberArrays[19];
            List<uint> list = new List<uint>(addon->EnemyCount);
            for (var i = 0; i < addon->EnemyCount; i++)
            {
                list.Add((uint)numArray->IntArray[8 + i * 6]);
            }
            return list.ToArray();
        }
        return new uint[0];
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
