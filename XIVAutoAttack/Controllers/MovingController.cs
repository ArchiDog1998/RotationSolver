using Dalamud.Game.Gui;
using Dalamud.Hooking;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Controllers
{
    internal class MovingController : IDisposable
    {
        bool PosLocker = false;
        private Hook<ActorMoveHook> actorMoveHook;
        private delegate IntPtr ActorMoveHook(IntPtr address, float x, float y, float z);
        public MovingController()
        {
            actorMoveHook ??= Hook<ActorMoveHook>.FromAddress(Service.Address.ActorMove, new ActorMoveHook(ActorMoveDetour));
            actorMoveHook.Enable();
        }
        public void Dispose()
        {
            actorMoveHook.Disable();
        }
        private IntPtr ActorMoveDetour(IntPtr address, float x, float y, float z)
        {
            if (Service.Configuration.PoslockCasting && PosLocker && address == Service.ObjectTable[0].Address)
            {
                var pos = Service.ClientState.LocalPlayer.Position;
                return actorMoveHook.Original(address, pos.X, pos.Y, pos.Z);
            }
            return actorMoveHook.Original(address, x, y, z);
        }
        internal bool IsMoving
        {
            get => Marshal.ReadByte(Service.Address.IsMoving) == 1;
            set => PosLocker = !value;
        }
    }
}
