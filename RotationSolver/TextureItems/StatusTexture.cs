using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.TextureItems;

internal class StatusTexture(Status status) : ITexture
{
    readonly Status _status = status;
    public uint IconID => _status.Icon;
    public StatusID ID => (StatusID)_status.RowId;
    public string Name => $"{_status.Name} ({_status.RowId})";

    public string Description => _status.Description?.ToString() ?? string.Empty;

    public bool IsEnabled { get; set; } = true;

    public StatusTexture(StatusID id)
        : this((uint)id)
    {
    }

    public StatusTexture(uint id)
     : this(Service.GetSheet<Status>().GetRow(id)!)
    {
    }
}
