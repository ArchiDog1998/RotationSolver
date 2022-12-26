using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Data;


namespace XIVAutoAttack.Helpers
{
    internal static class StatusHelper
    {
        public record LocationInfo(EnemyLocation Loc, byte[] Tags);
        public static readonly SortedList<ActionID, LocationInfo> ActionLocations = new SortedList<ActionID, LocationInfo>()
        {
            {ActionID.FangandClaw, new( EnemyLocation.Side, new byte[] { 13, 10 })},
            {ActionID.WheelingThrust, new(EnemyLocation.Back, new byte[] { 10, 13 }) },
            {ActionID.ChaosThrust, new(EnemyLocation.Back, new byte[] { 61, 28 }) },
            {ActionID.ChaoticSpring, new(EnemyLocation.Back, new byte[] { 66, 28 }) },
            {ActionID.Demolish, new(EnemyLocation.Back, new byte[] { 46, 60 }) },
            {ActionID.SnapPunch, new(EnemyLocation.Side, new byte[] { 19, 21 }) },
            {ActionID.TrickAttack, new(EnemyLocation.Back, new byte[] { 25 }) },
            {ActionID.AeolianEdge,new( EnemyLocation.Back, new byte[] { 30, 68 }) },
            {ActionID.ArmorCrush, new(EnemyLocation.Side, new byte[] { 30, 66 }) },
            {ActionID.Suiton, new(EnemyLocation.Back, new byte[] { }) },
            {ActionID.Gibbet, new(EnemyLocation.Side , new byte[] { 11 })},
            {ActionID.Gallows, new(EnemyLocation.Back, new byte[] { 11 }) },
            {ActionID.Gekko, new(EnemyLocation.Back , new byte[] { 68, 29, 72 })},
            {ActionID.Kasha, new(EnemyLocation.Side, new byte[] { 29, 68, 72 }) },
        };

        public static StatusID[] SheildStatus { get; } = new StatusID[]
        {
            StatusID.Grit, StatusID.RoyalGuard, StatusID.IronWill, StatusID.Defiance
        };

        public static StatusID[] NoNeedHealingStatus { get; } = new StatusID[]
        {
            StatusID.Holmgang, StatusID.WillDead, StatusID.WalkingDead,
        };

        public static bool NeedHealing(BattleChara p) => p.WillStatusEndGCD(2, 0, false, NoNeedHealingStatus);

        /// <summary>
        /// 状态是否在下几个GCD转好后消失(列表中剩余时间最小的)。
        /// </summary>
        /// <param name="gcdCount">要隔着多少个完整的GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <returns>这个时间点状态是否已经消失</returns>
        internal static bool WillStatusEndGCD(this BattleChara obj, uint gcdCount = 0, uint abilityCount = 0, bool isFromSelf = true, params StatusID[] effectIDs)
        {
            var remain = obj.StatusTime(isFromSelf, effectIDs);
            return CooldownHelper.RecastAfterGCD(remain, gcdCount, abilityCount);
        }

        /// <summary>
        /// 状态是否在几秒后消失。
        /// </summary>
        /// <param name="remainWant">要多少秒呢</param>
        /// <returns>这个时间点状态是否已经消失</returns>
        internal static bool WillStatusEnd(this BattleChara obj, float remainWant, bool isFromSelf = true, params StatusID[] effectIDs)
        {
            var remain = obj.StatusTime(isFromSelf, effectIDs);
            return CooldownHelper.RecastAfter(remain, remainWant);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static float StatusTime(this BattleChara obj, bool isFromSelf, params StatusID[] effectIDs)
        {
            var times = obj.StatusTimes(isFromSelf, effectIDs);
            if (times == null || !times.Any()) return 0;
            return times.Min();
        }

        private static IEnumerable<float> StatusTimes(this BattleChara obj, bool isFromSelf, params StatusID[] effectIDs)
        {
            return obj.GetStatus(isFromSelf, effectIDs).Select(status => status.RemainingTime == 0 ? float.MaxValue : status.RemainingTime);
        }

        internal static byte StatusStack(this BattleChara obj, bool isFromSelf, params StatusID[] effectIDs)
        {
            var stacks = obj.StatusStacks(isFromSelf, effectIDs);
            if (stacks == null || !stacks.Any()) return 0;
            return stacks.Min();
        }

        private static IEnumerable<byte> StatusStacks(this BattleChara obj, bool isFromSelf, params StatusID[] effectIDs)
        {
            return obj.GetStatus(isFromSelf, effectIDs).Select(status => status.StackCount == 0 ? byte.MaxValue : status.StackCount);
        }

        /// <summary>
        /// 表示角色<paramref name="obj"/>是否存在任何人或自己赋予的参数<paramref name="effectIDs"/>中的任何一个
        /// </summary>
        /// <param name="obj">检查对象</param>
        /// <param name="effectIDs">状态</param>
        /// <returns>是否拥有任何一个状态</returns>
        internal static bool HasStatus(this BattleChara obj, bool isFromSelf, params StatusID[] effectIDs)
        {
            return obj.GetStatus(isFromSelf, effectIDs).Any();
        }

        internal static void StatusOff(StatusID status)
        {
            CommandController.SubmitToChat($"/statusoff {GetStatusName(status)}");
        }

        /// <summary>
        /// 获得状态的名字
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static string GetStatusName(StatusID id)
        {
            return Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>().GetRow((uint)id).Name.ToString();
        }

        private static IEnumerable<Status> GetStatus(this BattleChara obj, bool isFromSelf, params StatusID[] effectIDs)
        {
            var newEffects = effectIDs.Select(a => (uint)a);
            return obj.GetAllStatus(isFromSelf).Where(status => newEffects.Contains(status.StatusId));
        }

        private static IEnumerable<Status> GetAllStatus(this BattleChara obj, bool isFromSelf)
        {
            if (obj == null) return new Status[0];

            return obj.StatusList.Where(status => isFromSelf ? status.SourceId == Service.ClientState.LocalPlayer.ObjectId
            || status.SourceObject?.OwnerId == Service.ClientState.LocalPlayer.ObjectId : true);
        }
    }
}
