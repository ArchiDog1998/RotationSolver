using System.ComponentModel;
using XIVConfigUI;

namespace RotationSolver.UI.ConfigWindows;
[Description("UI")]
public class UIItem : ConfigWindowItemRS
{
    private CollapsingHeaderGroup? _UIHeader;

    public override uint Icon => 42;
    public override string Description => UiString.Item_UI.Local();

    public override void Draw(ConfigWindow window)
    {
        _UIHeader ??= window.Collection.GetGroups<UiString>([
                UiString.ConfigWindow_UI_Information,
                    UiString.ConfigWindow_UI_Overlay,
                    UiString.ConfigWindow_UI_Windows,
                ]);
        _UIHeader?.Draw();
    }
}