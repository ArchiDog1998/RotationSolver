using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using XIVAutoAttack.Combos.Melee;

namespace XIVAutoAttack.Actions
{
    internal static class StatusHelper
    {
        public static readonly SortedList<uint, EnemyLocation> ActionLocations = new SortedList<uint, EnemyLocation>()
        {
            {DRGCombo.Actions.FangandClaw.ID, EnemyLocation.Side },
            {DRGCombo.Actions.WheelingThrust.ID, EnemyLocation.Back },
            {DRGCombo.Actions.ChaosThrust.ID, EnemyLocation.Back },
            {25772, EnemyLocation.Back },
            {MNKCombo.Actions.Demolish.ID, EnemyLocation.Back },
            {MNKCombo.Actions.SnapPunch.ID, EnemyLocation.Side },
            {NINCombo.Actions.TrickAttack.ID, EnemyLocation.Back },
            {NINCombo.Actions.AeolianEdge.ID, EnemyLocation.Back },
            {NINCombo.Actions.ArmorCrush.ID, EnemyLocation.Side },
            {NINCombo.Actions.Suiton.ID, EnemyLocation.Back },
            {RPRCombo.Actions.Gibbet.ID, EnemyLocation.Side },
            {RPRCombo.Actions.Gallows.ID, EnemyLocation.Back },
            {SAMCombo.Actions.Gekko.ID, EnemyLocation.Back },
            {SAMCombo.Actions.Kasha.ID, EnemyLocation.Side },
        };

        [Obsolete("该方法已过时，请使用LocalPlayer.HaveStatus")]
        internal static bool HaveStatusFromSelf(params ushort[] effectIDs)
        {
            return FindStatusTimesSelfFromSelf(effectIDs).Length > 0;
        }

        [Obsolete("该方法已过时，请使用LocalPlayer.FindStatusTimes")]

        internal static float[] FindStatusTimesSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusTimes(Service.ClientState.LocalPlayer, effectIDs);
        }

        [Obsolete("该方法已过时，请使用LocalPlayer.FindStatusTime")]
        internal static float FindStatusTimeSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusTime(Service.ClientState.LocalPlayer, effectIDs);
        }

        internal static float FindStatusTime(this BattleChara obj, params ushort[] effectIDs)
        {
            var times = FindStatusTimes(obj, effectIDs);
            if (times == null || times.Length == 0) return 0;
            return times.Max();
        }

        internal static float[] FindStatusTimes(this BattleChara obj, params ushort[] effectIDs)
        {
            return FindStatus(obj, effectIDs).Select(status => status.RemainingTime).ToArray();
        }

        internal static byte FindStatusStack(this BattleChara obj, params ushort[] effectIDs)
        {
            var stacks = FindStatusStacks(obj, effectIDs);
            if (stacks == null || stacks.Length == 0) return 0;
            return stacks.Max();
        }

        internal static byte[] FindStatusStacks(this BattleChara obj, params ushort[] effectIDs)
        {
            return FindStatus(obj, effectIDs).Select(status => Math.Max(status.StackCount, (byte)1)).ToArray();
        }

        internal static bool HaveStatus(this BattleChara obj, params ushort[] effectIDs)
        {
            return obj.FindStatus(effectIDs).Length > 0;
        }

        private static Status[] FindStatus(this BattleChara obj, params ushort[] effectIDs)
        {
            uint[] newEffects = effectIDs.Select(a => (uint)a).ToArray();
            return FindAllStatus(obj).Where(status => newEffects.Contains(status.StatusId)).ToArray();
        }

        [Obsolete("这个API未来将不会开放！")]
        internal static Status[] FindAllStatus(this BattleChara obj)
        {
            if (obj == null) return new Status[0];

            return obj.StatusList.Where(status => status.SourceID == Service.ClientState.LocalPlayer.ObjectId).ToArray();
        }
    }
}
