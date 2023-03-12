using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic;
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


    private static float JobRange
    {
        get
        {
            float radius = 25;
            if(Service.Player == null) return radius;
            switch (Service.GetSheet<ClassJob>().GetRow(
                Service.Player.ClassJob.Id).GetJobRole())
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
        DataCenter.AllHostileTargets = allTargets.Where(b =>
        {
            if (!b.IsNPCEnemy()) return false;

            //Dead.
            if (b.CurrentHp == 0) return false;

            if (!b.IsTargetable()) return false;

            if (b.StatusList.Any(StatusHelper.IsInvincible)) return false;

            if (Service.Config.OnlyAttackInView)
            {
                if(!Service.GameGui.WorldToScreen(b.Position, out _)) return false;
            }

            return true;
        });

        DataCenter.HostileTargets.Delay(GetHostileTargets(DataCenter.AllHostileTargets));

        DataCenter.CanInterruptTargets.Delay(DataCenter.HostileTargets.Where(ObjectHelper.CanInterrupt));

        DataCenter.TarOnMeTargets = DataCenter.HostileTargets.Where(tar => tar.TargetObjectId == Service.Player.ObjectId);

        DataCenter.HasHostilesInRange = TargetFilter.GetObjectInRadius(DataCenter.HostileTargets, JobRange).Any();

        if (DataCenter.HostileTargets.Count() == 1)
        {
            var tar = DataCenter.HostileTargets.FirstOrDefault();

            DataCenter.IsHostileCastingToTank = IsHostileCastingTank(tar);
            DataCenter.IsHostileCastingAOE = IsHostileCastingArea(tar);
        }
        else
        {
            DataCenter.IsHostileCastingToTank = DataCenter.IsHostileCastingAOE = false;
        }
    }

    private static IEnumerable<BattleChara> GetHostileTargets(IEnumerable<BattleChara> allattackableTargets)
    {
        var type = IconReplacer.RightNowTargetToHostileType;
        if (type == TargetHostileType.AllTargetsCanAttack || Service.CountDownTime > 0)
        {
            return allattackableTargets;
        }

        uint[] ids = GetEnemies();
        var fateId = DataCenter.FateId;

        var hostiles = allattackableTargets.Where(t =>
        {
            if (ids.Contains(t.ObjectId)) return true;
            if (t.TargetObject == Service.Player
            || t.TargetObject?.OwnerId == Service.Player.ObjectId) return true;

            //Remove other's treasure.
            if (t.IsOthersPlayers()) return false;

            if (t.IsTopPriorityHostile()) return true;

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
        if (!Service.Config.AddEnemyListToHostile) return new uint[0];

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
           
            if (!(h.TotalCastTime > 2.5 && 
                CooldownHelper.RecastAfterGCD(last, 2) && !CooldownHelper.RecastAfterGCD(last, 0))) return false;

            var action = Service.GetSheet<Action>().GetRow(h.CastActionId);
            return check?.Invoke(action) ?? false;
        }
        return false;
    }
}
