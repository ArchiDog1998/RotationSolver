namespace RotationSolver.Data;

[Flags]
public enum DownloadOption : byte
{
    Local = 0,
    Download = 1 << 0,
    MustDownload = Download | 1 << 1,
    ShowList = 1 << 2,
}
