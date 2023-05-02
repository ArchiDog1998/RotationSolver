using Lumina.Excel.GeneratedSheets;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.TextureItems;

internal class ActionTexture : ITexture
{
    readonly Action _action;

    public uint IconID => _action.Icon;

    public string Name => $"{_action.Name} ({_action.RowId})";

    public uint ID => _action.RowId;

    public string Description => string.Empty;
    public bool IsEnabled { get; set; } = true;

    public ActionTexture(Action action)
    {
        _action = action;
    }
}
