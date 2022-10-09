using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Actions
{
    internal static class StatusHelper
    {
        internal static bool HaveStatusSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusSelfFromSelf(effectIDs).Length > 0;
        }
        internal static float[] FindStatusSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusFromSelf(Service.ClientState.LocalPlayer, effectIDs);
        }

        internal static float[] FindStatusFromSelf(BattleChara obj, params ushort[] effectIDs)
        {
            uint[] newEffects = effectIDs.Select(a => (uint)a).ToArray();
            return FindStatusFromSelf(obj).Where(status => newEffects.Contains(status.StatusId)).Select(status => status.RemainingTime).ToArray();
        }

        internal static Status[] FindStatusFromSelf(BattleChara obj)
        {
            if (obj == null) return new Status[0];

            return obj.StatusList.Where(status => status.SourceID == Service.ClientState.LocalPlayer.ObjectId).ToArray();
        }

        internal static float FindStatusTimeFromSelf(BattleChara obj, params ushort[] effectIDs)
        {
            var times = FindStatusFromSelf(obj, effectIDs);
            if (times == null || times.Length == 0) return 0;
            return times.Max();
        }
    }
}
