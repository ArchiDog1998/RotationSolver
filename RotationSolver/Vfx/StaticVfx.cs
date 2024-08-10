using ECommons.DalamudServices;
using NRender;
using NRender.Vfx;
using System.Runtime.CompilerServices;

namespace RotationSolver.Vfx;

internal class StaticVfx : OmenElement, IDisposable
{
    private long realInitTime = 0;

    public bool ShowColorChange { get; set; }

    public Action? UpdateEveryFrame { get; set; }

    public bool Forever { get; set; } = true;

    public StaticVfx(string path, Vector3 scale, IGameObject Owner, Vector4 color)
        : base(path.Omen(), scale, Owner, color)
    {
        Init();
    }

    public StaticVfx(string path, Vector3 scale, Vector3 position, Vector4 color, float facing)
        : base(path.Omen(), scale, position, color, facing)
    {
        Init();
    }

    private void Init()
    {
        Svc.Framework.Update += Framework_Update;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "initTime")]
    extern static ref long GetSetPrivateField(OmenElement element);

    private void Framework_Update(Dalamud.Plugin.Services.IFramework framework)
    {
        if (!VfxManager.drawOmenElementList.Contains(this))
        {
            Svc.Framework.Update -= Framework_Update;
            return;
        }

        long initTime = GetSetPrivateField(this);
        if (realInitTime == 0 && initTime != 0)
        {
            realInitTime = initTime;
        }

        if (realInitTime != 0 && !Forever
            && (ShowColorChange || Environment.TickCount64 - realInitTime > DestoryAt))
        {
            GetSetPrivateField(this) = realInitTime;
        }
        else
        {
            GetSetPrivateField(this) = Environment.TickCount64;
        }

        UpdateEveryFrame?.Invoke();
    }
}
