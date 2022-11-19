using Dalamud.Game.ClientState.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Helpers
{
    internal static class VirtualKeyHelper
    {
        internal static string ToName(this VirtualKey k)
        {
            return k switch
            {
                VirtualKey.SHIFT => "SHIFT",
                VirtualKey.CONTROL => "CTRL",
                VirtualKey.MENU => "ALT",
                _ => k.ToString(),
            };
        }
    }
}
