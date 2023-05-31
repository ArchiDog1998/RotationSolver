using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.UI;

internal static class PainterManager
{
    static XIVPainter.XIVPainter _painter;


    public static void Init()
    {
        _painter = Svc.PluginInterface.Create<XIVPainter.XIVPainter>("RotationSolverOverlay");
    }

    public static void Dispose()
    {
        _painter?.Dispose();
    }
}
