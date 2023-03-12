using Lumina.Excel.GeneratedSheets;
using RotationSolver.Data;

namespace RotationSolver.Timeline;

internal class BaseStatus : ITexture
{
    public Status _status;
    public uint IconID => _status.Icon;
    public StatusID ID => (StatusID)_status.RowId;
    public string Name => $"{_status.Name}[{_status.RowId}]";

    public string Description => string.Empty;

    public bool IsEnabled { get; set; }

    public BaseStatus(StatusID id)
    {
        _status = Service.DataManager.GetExcelSheet<Status>().GetRow((uint)id);
    }
}
