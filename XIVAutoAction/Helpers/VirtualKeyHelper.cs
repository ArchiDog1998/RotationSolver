using Dalamud.Game.ClientState.Keys;

namespace AutoAction.Helpers
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
