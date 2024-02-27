using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace RotationSolver.Basic.Helpers;

/// <summary>
/// The filter for target.
/// </summary>
public static class TargetFilter
{
    #region Find one target
    /// <summary>
    /// Get the deadth ones in the list.
    /// </summary>
    /// <param name="charas"></param>
    /// <returns></returns>
    public unsafe static IEnumerable<BattleChara> GetDeath(this IEnumerable<BattleChara> charas) => charas.Where(item =>
        {
            if (item == null) return false;
            if (!item.IsDead) return false;
            if (item.CurrentHp != 0) return false;

            if (!item.IsTargetable) return false;

            if (item.HasStatus(false, StatusID.Raise)) return false;

            if (!Service.Config.RaiseBrinkOfDeath && item.HasStatus(false, StatusID.BrinkOfDeath)) return false;

            if (DataCenter.AllianceMembers.Any(c => c.CastTargetObjectId == item.ObjectId)) return false;

            return true;
        });

    /// <summary>
    /// Get the specific roles members.
    /// </summary>
    /// <param name="objects"></param>
    /// <param name="roles"></param>
    /// <returns></returns>
    public static IEnumerable<BattleChara> GetJobCategory(this IEnumerable<BattleChara> objects, params JobRole[] roles)
        => roles.SelectMany(role => objects.Where(obj => obj.IsJobCategory(role)));

    /// <summary>
    /// Is the target the role.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public static bool IsJobCategory(this GameObject obj, JobRole role)
    {
        SortedSet<byte> validJobs = new(Service.GetSheet<ClassJob>()
            .Where(job => role == job.GetJobRole())
            .Select(job => (byte)job.RowId));

        return obj.IsJobs(validJobs);
    }

    /// <summary>
    /// Is the target in the jobs.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="validJobs"></param>
    /// <returns></returns>
    public static bool IsJobs(this GameObject obj, params Job[] validJobs)
    {
        return obj.IsJobs(new SortedSet<byte>( validJobs.Select(j => (byte)(uint)j)));
    }

    private static bool IsJobs(this GameObject obj, SortedSet<byte> validJobs)
    {
        if(obj is not BattleChara b) return false;
        return validJobs.Contains((byte?)b.ClassJob.GameData?.RowId ?? 0);
    }
    #endregion

    /// <summary>
    /// Get the <paramref name="objects"/> in <paramref name="radius"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objects"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static IEnumerable<T> GetObjectInRadius<T>(this IEnumerable<T> objects, float radius) where T : GameObject
        => objects.Where(o => o.DistanceToPlayer() <= radius);
}
