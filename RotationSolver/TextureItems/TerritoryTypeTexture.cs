using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.TextureItems;

internal class TerritoryTypeTexture : ITexture
{
    readonly TerritoryType _territory;
    public uint IconID => _territory?.ContentFinderCondition?.Value?.ContentType?.Value?.Icon ?? 0;
    public uint ID => _territory?.RowId ?? 0;
    public string Name => $"{_territory.PlaceName?.Value?.Name} ({_territory.RowId})";

    public string Description => string.Empty;
    public bool IsEnabled { get; set; } = true;

    public TerritoryTypeTexture(uint id)
        : this(Service.GetSheet<TerritoryType>().GetRow(id))
    {
    }

    public TerritoryTypeTexture(TerritoryType territory)
    {
        _territory = territory;
    }
}
