using ECommons.DalamudServices;
using ECommons.EzIpcManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Basic.IPC
{
    internal class IPCProvider
    {
        internal IPCProvider()
        {
            EzIPC.Init(this, prefix: "RotationSolverReborn");
        }

        [EzIPC]
        public void Test(string param)
        {
            Svc.Log.Debug($"IPC Test! Param:{param}");
        }

        [EzIPC]
        public void AddPriorityNameID(uint nameId)
        {
            DataCenter.PrioritizedNameIds.Add(nameId);

            Svc.Log.Debug($"IPC AddPriorityNameID was called. NameID:{nameId}");
        }

        [EzIPC]
        public void RemovePriorityNameID(uint nameId)
        {
            if (DataCenter.PrioritizedNameIds.Contains(nameId))
                DataCenter.PrioritizedNameIds.Remove(nameId);
            else
                Svc.Log.Warning($"IPC RemovePriorityNameID was called but NameID:{nameId} was not found.");

            Svc.Log.Debug($"IPC RemovePriorityNameID was called. NameID:{nameId}");
        }

        [EzIPC]
        public void AddBlacklistNameID(uint nameId)
        {
            DataCenter.BlacklistedNameIds.Add(nameId);

            Svc.Log.Debug($"IPC AddBlacklistNameID was called. NameID:{nameId}");
        }

        [EzIPC]
        public void RemoveBlacklistNameID(uint nameId)
        {
            if (DataCenter.BlacklistedNameIds.Contains(nameId))
                DataCenter.BlacklistedNameIds.Remove(nameId);
            else
                Svc.Log.Warning($"IPC RemoveBlacklistNameID was called but NameID:{nameId} was not found.");

            Svc.Log.Debug($"IPC RemoveBlacklistNameID was called. NameID:{nameId}");
        }

        [EzIPC]
        public void ChangeOperatingMode(StateCommandType stateCommand)
        {
            switch (stateCommand)
            {
                case StateCommandType.Auto:
                    DataCenter.IsManual = false;
                    DataCenter.State = true;
                    break;
                case StateCommandType.Manual:
                    DataCenter.IsManual = true;
                    DataCenter.State = true;
                    break;
                case StateCommandType.Off:
                    DataCenter.State = false;
                    DataCenter.IsManual = false;
                    break;
            }
            Svc.Log.Debug($"IPC ChangeOperatingMode was called. StateCommand:{stateCommand}");
        }
    }
}
