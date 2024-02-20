using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using RotationSolver.Basic.Configuration;
using RotationSolver.Updaters;
using XIVPainter;
using XIVPainter.Element3D;
using XIVPainter.ElementSpecial;
using static Dalamud.Interface.Utility.Raii.ImRaii;

namespace RotationSolver.UI;

internal static class PainterManager
{
    class BeneficialPositionDrawing : Drawing3DPoly
    {
        const float beneficialRadius = 0.6f;
        public override void UpdateOnFrame(XIVPainter.XIVPainter painter)
        {
            SubItems = [];

            if (!Service.Config.ShowBeneficialPositions) return;

            if (Svc.ClientState == null) return;
            if (!Player.Available) return;

            if (!OtherConfiguration.BeneficialPositions.TryGetValue(Svc.ClientState.TerritoryType, out var pts)) return;

            var d = DateTime.Now.Millisecond / 1000f;
            var ratio = (float)DrawingExtensions.EaseFuncRemap(EaseFuncType.None, EaseFuncType.Cubic)(d);
            List<IDrawing3D> subItems = [];

            var color = ImGui.GetColorU32(Service.Config.BeneficialPositionColor);
            var hColor = ImGui.GetColorU32(Service.Config.HoveredBeneficialPositionColor);

            foreach (var p in pts)
            {
                if (Vector3.Distance(Player.Object.Position, p) > 80) continue;

                subItems.Add(new Drawing3DCircularSector(p, beneficialRadius * ratio, p == RotationConfigWindow.HoveredPosition ? hColor : color, 3)
                {
                    IsFill = false,
                });
            }

            SubItems = [.. subItems];

            base.UpdateOnFrame(painter);
        }

    }

    class TargetsDrawing : Drawing3DPoly
    {
        public override unsafe void UpdateOnFrame(XIVPainter.XIVPainter painter)
        {
            SubItems = [];

            if (!Service.Config.ShowHostilesIcons) return;

            List<IDrawing3D> subItems = [];

            if (IconSet.GetTexture(61510, out var hostileIcon))
            {
                foreach (var hostile in DataCenter.AllHostileTargets)
                {
                    subItems.Add(new Drawing3DImage(hostileIcon, hostile.Position + new Vector3(0,
                        Service.Config.HostileIconHeight, 0),
                        Service.Config.HostileIconSize)
                    {
                        DrawWithHeight = false,
                        MustInViewRange = true,
                    });
                }
            }

            SubItems = [.. subItems];

            base.UpdateOnFrame(painter);
        }
    }

    class TargetDrawing : Drawing3DPoly
    {
        readonly Drawing3DCircularSector _target;
        readonly Drawing3DImage _targetImage;
        public TargetDrawing()
        {
            var TColor = ImGui.GetColorU32(Service.Config.TargetColor);
            _target = new Drawing3DCircularSector(default, 0, TColor, 3)
            {
                IsFill = false,
            };
            _targetImage = new Drawing3DImage(null, default, 0)
            {
                MustInViewRange = true,
            };
        }

        const float targetRadius = 0.15f;
        public override void UpdateOnFrame(XIVPainter.XIVPainter painter)
        {
            SubItems = [];

            if (!Service.Config.ShowTarget) return;

            if (ActionUpdater.NextAction is not BaseAction act) return;

            if (act.Target == null) return;

            var d = DateTime.Now.Millisecond / 1000f;
            var ratio = (float)DrawingExtensions.EaseFuncRemap(EaseFuncType.None, EaseFuncType.Cubic)(d);
            List<IDrawing3D> subItems = [];

            if (Service.Config.TargetIconSize > 0)
            {
                _targetImage.Position = act.Target?.Position ?? Player.Object.Position;
                if (act.GetTexture(out var texture, true))
                {
                    _targetImage.Image = texture;
                    _targetImage.Size = Service.Config.TargetIconSize;
                    subItems.Add(_targetImage);
                }
            }
            else
            {
                _target.Color = ImGui.GetColorU32(Service.Config.TargetColor);
                _target.Center = act.Target?.Position ?? Player.Object.Position;
                _target.Radius = targetRadius * ratio;
                subItems.Add(_target);
            }

            if (act.Target.HasValue && (DataCenter.AllHostileTargets.Contains(act.Target?.Target) || act.Target?.Target == Player.Object && !act.Setting.IsFriendly))
            {
                var SColor = ImGui.GetColorU32(Service.Config.SubTargetColor);

                foreach (var t in act.Target!.Value.AffectedTargets)
                {
                    if (t == act.Target?.Target) continue;
                    subItems.Add(new Drawing3DCircularSector(t.Position, targetRadius * ratio, SColor, 3)
                    {
                        IsFill = false,
                    });
                }
            }

            SubItems = [.. subItems];

            base.UpdateOnFrame(painter);
        }
    }

    class TargetText : Drawing3DPoly
    {
        const int ItemsCount = 16;

        public TargetText()
        {
            SubItems = new IDrawing3D[ItemsCount];
            for (int i = 0; i < ItemsCount; i++)
            {
                SubItems[i] = new Drawing3DText(string.Empty, default);
            }
        }

        public override void UpdateOnFrame(XIVPainter.XIVPainter painter)
        {
            for (int i = 0; i < ItemsCount; i++)
            {
                ((Drawing3DText)SubItems[i]).Text = string.Empty;
            }

            if (!Service.Config.ShowTargetTimeToKill) return;

            uint HealthRatioColor = ImGui.GetColorU32(Service.Config.TTKTextColor);

            int index = 0;
            foreach (GameObject t in DataCenter.AllHostileTargets.OrderBy(ObjectHelper.DistanceToPlayer))
            {
                if (t is not BattleChara b) continue;
                if (t is PlayerCharacter) continue;

                var item = (Drawing3DText)SubItems[index++];

                try
                {
                    item.Text = $"TTK: {b.GetTimeToKill():F2}s / {b.GetTimeToKill(true):F2}s";
                    item.Color = HealthRatioColor;
                    item.Position = b.Position;
                }
                catch
                {
                    continue;
                }

                if (index >= ItemsCount) break;
            }
            base.UpdateOnFrame(painter);
        }
    }

    static XIVPainter.XIVPainter? _painter;
    static DrawingHighlightHotbar _highLight = new();
    static Drawing3DImage? _stateImage;
    public static HashSet<uint> ActionIds => _highLight.ActionIds;

    public static Vector4 HighlightColor
    {
        get => _highLight.Color;
        set => _highLight.Color = value;
    }

    public static void Init()
    {
        _painter = XIVPainter.XIVPainter.Create(Svc.PluginInterface, "RotationSolverOverlay");

        _highLight = new();
        UpdateSettings();

        HighlightColor = Service.Config.TeachingModeColor;

        var annulus = new Drawing3DAnnulusO(Player.Object, 3, 3 + Service.Config.MeleeRangeOffset, 0, 2)
        {
            InsideColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.8f, 0.3f, 0.2f, 0.15f))
        };

        annulus.UpdateEveryFrame = () =>
        {
            if (Player.Available && (Player.Object.IsJobCategory(JobRole.Tank) || Player.Object.IsJobCategory(JobRole.Melee)) && (Svc.Targets.Target?.IsEnemy() ?? false) && Service.Config.DrawMeleeOffset
            && ActionUpdater.NextGCDAction == null)
            {
                annulus.Target = Svc.Targets.Target;
            }
            else
            {
                annulus.Target = null;
            }
        };

        var color = ImGui.GetColorU32(Service.Config.MovingTargetColor);
        var movingTarget = new Drawing3DHighlightLine(default, default, 0, color, 3);
        movingTarget.UpdateEveryFrame = () =>
        {
            var tar = CustomRotation.MoveTarget;

            if (!Service.Config.ShowMoveTarget || !Player.Available || !tar.HasValue || Vector3.Distance(tar.Value, Player.Object.Position) < 0.01f)
            {
                movingTarget.Radius = 0;
                return;
            }

            movingTarget.Radius = 0.5f;

            movingTarget.Color = ImGui.GetColorU32(Service.Config.MovingTargetColor);

            movingTarget.From = Player.Object.Position;
            movingTarget.To = tar.Value;
        };

        _stateImage = new Drawing3DImage(null, default, 0)
        {
            MustInViewRange = true,
            DrawWithHeight = false,
            UpdateEveryFrame = () =>
            {
                if (!Player.Available) return;
                if (_stateImage == null) return;

                unsafe
                {
                    _stateImage.Position = Player.Object.Position + new Vector3(0,
                                Service.Config.StateIconHeight, 0);
                }

                if (DataCenter.State && Service.Config.ShowStateIcon)
                {
                    if (IconSet.GetTexture(61516, out var texture))
                    {
                        _stateImage.Image = texture;
                        _stateImage.Size = Service.Config.StateIconSize;
                    }
                }
                else
                {
                    _stateImage.Size = 0;
                }
            },
        };

        _painter.AddDrawings(
            _highLight, _stateImage, new TargetDrawing(), annulus, movingTarget,
            new TargetsDrawing(), new TargetText(), new BeneficialPositionDrawing()
            );
    }

    public static void UpdateSettings()
    {
        if (_painter == null) return;
        _painter.DrawingHeight = Service.Config.DrawingHeight;
        _painter.SampleLength = Service.Config.SampleLength;
        _painter.Enable = !Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent] && Service.Config.UseOverlayWindow;
    }

    public static void Dispose()
    {
        _painter?.Dispose();
    }
}
