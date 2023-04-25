using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Timeline;

internal class BaseStatus : ITexture
{
    public Status _status;
    public uint IconID => _status.Icon;
    public StatusID ID => (StatusID)_status.RowId;
    public string Name => $"{_status.Name} ({_status.RowId})";

    public string Description => string.Empty;

    public bool IsEnabled { get; set; }

    public BaseStatus(StatusID id)
        : this((uint)id)
    {
    }

    public BaseStatus(uint id)
     : this(Service.GetSheet<Status>().GetRow(id))
    {
    }

    public BaseStatus(Status status)
    {
        _status = status;
    }
}
