using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Commands;
using RotationSolver.Updaters;
using XIVPainter;
using XIVPainter.Element3D;

namespace RotationSolver.UI;

internal static class PainterManager
{
    class PositionalDrawing : Drawing3DPoly
    {
        Drawing3DCircularSectorO _noneCir, _flankCir, _rearCir;

        public EnemyPositional Positional { get; set; }

        public GameObject Target { get; set; }

        public PositionalDrawing()
        {
            var right = ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 0.15f));
            var wrong = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.8f, 0.2f, 0.15f));

            _noneCir = new Drawing3DCircularSectorO(null, 3, wrong, 2);

            _flankCir = new Drawing3DCircularSectorO(null, 3, wrong, 2, XIVPainter.Enum.RadiusInclude.IncludeBoth,
                new Vector2(MathF.PI * 0.25f, MathF.PI / 2), new Vector2(MathF.PI * 1.25f, MathF.PI / 2));
            _rearCir = new Drawing3DCircularSectorO(null, 3, wrong, 2, XIVPainter.Enum.RadiusInclude.IncludeBoth,
                new Vector2(MathF.PI * 0.75f, MathF.PI / 2));

            _noneCir.InsideColor = _flankCir.InsideColor = _rearCir.InsideColor = right;

            SubItems = new IDrawing3D[] { _noneCir, _flankCir, _rearCir };
        }

        public override void UpdateOnFrame(XIVPainter.XIVPainter painter)
        {
            if (Target == null || !Target.IsNPCEnemy())
            {
                _flankCir.Target = null;
                _rearCir.Target = null;
                _noneCir.Target = null;
            }
            else
            {
                var pos = Positional;
                if (!Target.HasPositional() || Player.Available && Player.Object.HasStatus(true, CustomRotation.TrueNorth.StatusProvide))
                {
                    pos = EnemyPositional.None;
                }
                switch (pos)
                {
                    case EnemyPositional.Flank:
                        _flankCir.Target = Target;
                        _rearCir.Target = null;
                        _noneCir.Target = null;
                        break;

                    case EnemyPositional.Rear:
                        _flankCir.Target = null;
                        _rearCir.Target = Target;
                        _noneCir.Target = null;
                        break;

                    default:
                        _flankCir.Target = null;
                        _rearCir.Target = null;
                        _noneCir.Target = Target;
                        break;
                }
            }

            base.UpdateOnFrame(painter);
        }
    }

    class TargetDrawing : Drawing3DPoly
    {
        
    }

    static XIVPainter.XIVPainter _painter;
    static PositionalDrawing _positional;

    public static void Init()
    {
        _painter = Svc.PluginInterface.Create<XIVPainter.XIVPainter>("RotationSolverOverlay");
        _painter.UseTaskForAccelerate = false;

        var annulus = new Drawing3DAnnulusO(Player.Object, 3, 3 + Service.Config.MeleeRangeOffset,
            ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0.8f, 0.75f, 0)), 2);
        annulus.InsideColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.8f, 0.3f, 0.2f, 0.15f));

        annulus.UpdateEveryFrame = () =>
        {
            if (Player.Available && (Player.Object.IsJobCategory(JobRole.Tank) || Player.Object.IsJobCategory(JobRole.Melee)) && (Svc.Targets.Target?.IsNPCEnemy() ?? false) && Service.Config.DrawMeleeOffset
            && ActionUpdater.NextGCDAction == null)
            {
                annulus.Target = Svc.Targets.Target;
            }
            else
            {
                annulus.Target = null;
            }
        };

        _positional = new PositionalDrawing();

        var c = Service.Config.MovingTargetColor;
        var color = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));
        var movingTarget = new Drawing3DHighlightLine(default, default, 0, color, 3);
        movingTarget.UpdateEveryFrame = () =>
        {
            var tar = CustomRotation.MoveTarget;

            if (!Service.Config.ShowMoveTarget
            || tar == null || !Player.Available || tar == Player.Object )
            {
                movingTarget.Radius = 0;
                return;
            }
            var dir = Player.Object.Position - tar.Position;
            var length = dir.Length();
            if (!dir.Normalize())
            {
                movingTarget.Radius = 0;
                return;
            }

            movingTarget.Radius = 0.5f;

            var c = Service.Config.MovingTargetColor;
            movingTarget.Color = ImGui.GetColorU32(new Vector4(c.X, c.Y, c.Z, 1));

            movingTarget.From = Player.Object.Position;

            movingTarget.To = tar.Position + dir * MathF.Min(length, Player.Object.HitboxRadius + tar.HitboxRadius);
        };

        var targetDrawing = new TargetDrawing();

        _painter.AddDrawings(_positional, annulus, movingTarget, targetDrawing);

#if DEBUG
        //_painter.AddDrawings(new Drawing3DCircularSectorO(Player.Object, 3, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0.5f, 0.4f, 0.15f)), 5));
        //var color = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0.5f, 0.4f, 0.15f));

        //var p = new Drawing3DCircularSector(Player.Object.Position, 5, color, 5);
        //p.ClosestPtDis = 0.5f;
        //_painter.AddDrawings(p);
#endif
    }

    public static void UpdatePositional(EnemyPositional positional, GameObject target)
    {
        _positional.Target = target;
        _positional.Positional = positional;
    }

    public static void ClearPositional()
    {
        _positional.Target = null;
    }

    public static void Dispose()
    {
        _painter?.Dispose();
    }
}
