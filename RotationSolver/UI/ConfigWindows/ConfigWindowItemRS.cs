using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using XIVConfigUI;

namespace RotationSolver.UI.ConfigWindows;
public abstract class ConfigWindowItemRS : ConfigWindowItem
{
    public abstract uint Icon { get; }
    protected static float Scale => ImGuiHelpers.GlobalScale;
    public sealed override bool GetIcon(out IDalamudTextureWrap texture)
    {
        return ImageLoader.GetTexture(Icon, out texture);
    }
}