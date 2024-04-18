using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Basic.Helpers
{
    internal static class ActionManagerHelper
    {
        public static unsafe float GetCurrentAnimationLock()
        {
            var actionManager = ActionManager.Instance();
            if (actionManager == null) return 0.6f;

            var animationLockRaw = ((IntPtr)actionManager + 8);
            return *(float*)animationLockRaw;
        }

        public static unsafe float GetRecastTime(ActionType type, uint id)
        {
            var actionManager = ActionManager.Instance();
            if (actionManager == null) return 0;

            return actionManager->GetRecastTime(type, id);
        }

        public static unsafe float GetDefaultRecastTime()
        {
            return GetRecastTime(ActionType.Action, 11);
        }

        public static unsafe float GetRecastTimeElapsed(ActionType type, uint id)
        {
            var actionManager = ActionManager.Instance();
            if (actionManager == null) return 0;

            return actionManager->GetRecastTimeElapsed(type, id);
        }

        public static unsafe float GetDefaultRecastTimeElapsed()
        {
            return GetRecastTimeElapsed(ActionType.Action, 11);
        }
    }
}
