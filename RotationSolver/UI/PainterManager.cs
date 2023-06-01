using ECommons.DalamudServices;
using ECommons.GameHelpers;
using XIVPainter.Element3D;

namespace RotationSolver.UI;

internal static class PainterManager
{
    static XIVPainter.XIVPainter _painter;
    static Drawing3DCircularSectorFO _noneCir, _flankCir1, _flankCir2, _rearCir;
    static Drawing3DAnnulusFO _annulus;

    public static void Init()
    {
        _painter = Svc.PluginInterface.Create<XIVPainter.XIVPainter>("RotationSolverOverlay");
        _painter.UseTaskForAccelerate = false;

        var right = ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 0.15f));
        var wrong = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.8f, 0.2f, 0.15f));

        _noneCir = new Drawing3DCircularSectorFO(null, 3, wrong, 2);
        _flankCir1 = new Drawing3DCircularSectorFO(null, 3, wrong, 2, MathF.PI * 0.25f, MathF.PI / 2);
        _flankCir2 = new Drawing3DCircularSectorFO(null, 3, wrong, 2, MathF.PI * 1.25f, MathF.PI / 2);
        _rearCir = new Drawing3DCircularSectorFO(null, 3, wrong, 2, MathF.PI * 0.75f, MathF.PI / 2);

        _noneCir.InsideColor = _flankCir1.InsideColor = _flankCir2.InsideColor
            = _rearCir.InsideColor = right;

        _annulus = new Drawing3DAnnulusFO(null, 3, 3 + Service.Config.MeleeRangeOffset, right, 2);
        _annulus.InsideColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.8f, 0.3f, 0.2f, 0.15f));

        _annulus.UpdateEveryFrame = () =>
        {
            if (Player.Available && (Player.Object.IsJobCategory(JobRole.Tank) || Player.Object.IsJobCategory(JobRole.Melee)) && (Svc.Targets.Target?.IsNPCEnemy() ?? false) && Service.Config.DrawMeleeOffset
            && DataCenter.StateType != StateCommandType.Cancel)
            {
                _annulus.Target = Svc.Targets.Target;
            }
            else
            {
                _annulus.Target = null;
            }
        };

        _painter.AddDrawings(_noneCir, _flankCir1, _flankCir2, _rearCir, _annulus);

        if (Player.Available)
        {
            _painter.AddDrawings(new Drawing3DCircularSectorF(Player.Object.Position, 5, right, 2)
            {
                DeadTime = DateTime.Now.AddSeconds(5),
                InsideColor = wrong,
            });
        }
    }

    public static void UpdateOffset(GameObject target)
    {
        _annulus.Target = target;
    }

    public static void UpdatePositional(EnemyPositional positional, GameObject target)
    {
        switch(positional)
        {
            case EnemyPositional.Flank:
                _flankCir1.Target = _flankCir2.Target = target;
                _rearCir.Target = null;
                _noneCir.Target = null;
                break;

            case EnemyPositional.Rear:
                _flankCir1.Target = _flankCir2.Target = null;
                _rearCir.Target = target;
                _noneCir.Target = null;
                break;

            default:
                _flankCir1.Target = _flankCir2.Target = null;
                _rearCir.Target = null;
                _noneCir.Target = target;
                break;
        }
    }

    public static void ClearPositional()
    {
        _flankCir1.Target = _flankCir2.Target = null;
        _rearCir.Target = null;
        _noneCir.Target = null;
    }

    public static void Dispose()
    {
        _painter?.Dispose();
    }
}
