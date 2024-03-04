using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Basic.Configuration;
using RotationSolver.Updaters;
using XIVPainter;
using XIVPainter.Element3D;
using XIVPainter.ElementSpecial;
using XIVPainter.Vfx;

namespace RotationSolver.UI;

internal static class PainterManager
{
    class BeneficialPositionDrawing : Drawing3DPoly
    {
        const float beneficialRadius = 0.6f;
        protected override void UpdateOnFrame()
        {
            SubItems = [];

            if (!Service.Config.ShowBeneficialPositions) return;

            if (Svc.ClientState == null) return;
            if (!Player.Available) return;

            if (!OtherConfiguration.BeneficialPositions.TryGetValue(Svc.ClientState.TerritoryType, out var pts)) return;

            var d = DateTime.Now.Millisecond / 1000f;
            var ratio = (float)DrawingExtensions.EaseFuncRemap(EaseFuncType.None, EaseFuncType.Cubic)(d);
            List<Drawing3D> subItems = [];

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

            base.UpdateOnFrame();
        }
    }

    class TargetsDrawing : Drawing3DPoly
    {
        public TargetsDrawing()
        {
            SubItems = new Drawing3D[64];

            for (int i = 0; i < SubItems.Length; i++)
            {
                SubItems[i] = new Drawing3DImage(null, Vector3.Zero)
                {
                    Enable = false,
                    MustInViewRange = true,
                };
            }
        }

        protected override unsafe void UpdateOnFrame()
        {
            if (!Service.Config.ShowHostilesIcons)
            {
                foreach (var item in SubItems)
                {
                    item.Enable = false;
                }
                return;
            }

            if (!IconSet.GetTexture(61510, out var hostileIcon)) return;

            for (int i = 0; i < Math.Min(SubItems.Length, DataCenter.AllHostileTargets.Length); i++)
            {
                if (SubItems[i] is not Drawing3DImage item) continue;

                var ojb = DataCenter.AllHostileTargets[i];

                item.Enable = true;
                item.Image = hostileIcon;
                item.Position = ojb.Position + new Vector3(0,
                        Service.Config.HostileIconHeight, 0);
                item.Size = Service.Config.HostileIconSize;
            }

            base.UpdateOnFrame();
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
                Enable = false,
            };
            _targetImage = new Drawing3DImage(null, default, 0)
            {
                MustInViewRange = true,
                Enable = false,
            };
        }

        const float targetRadius = 0.15f;
        protected override void UpdateOnFrame()
        {
            _target.Enable = _targetImage.Enable = false;
            if (!Service.Config.ShowTarget) return;

            if (ActionUpdater.NextAction is not BaseAction act) return;

            if (act.Target == null) return;

            var d = DateTime.Now.Millisecond / 1000f;
            var ratio = (float)DrawingExtensions.EaseFuncRemap(EaseFuncType.None, EaseFuncType.Cubic)(d);

            if (Service.Config.TargetIconSize > 0)
            {
                _targetImage.Enable = true;
                _targetImage.Position = act.Target?.Position ?? Player.Object.Position;
                if (act.GetTexture(out var texture, true))
                {
                    _targetImage.Image = texture;
                    _targetImage.Size = Service.Config.TargetIconSize;
                }
            }
            else
            {
                _target.Enable = true;
                _target.Color = ImGui.GetColorU32(Service.Config.TargetColor);
                _target.Center = act.Target?.Position ?? Player.Object.Position;
                _target.Radius = targetRadius * ratio;
            }

            base.UpdateOnFrame();
        }
    }

    class TargetText : Drawing3DPoly
    {
        const int ItemsCount = 16;

        public TargetText()
        {
            SubItems = new Drawing3D[ItemsCount];
            for (int i = 0; i < ItemsCount; i++)
            {
                SubItems[i] = new Drawing3DText(string.Empty, default);
            }
        }

        protected override void UpdateOnFrame()
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
            base.UpdateOnFrame();
        }
    }

    static DrawingHighlightHotbar? _highLight;
    static Drawing3DImage? _stateImage;
    public static HashSet<uint> ActionIds => _highLight?.ActionIds ?? [];

    public static Vector4 HighlightColor
    {
        get => _highLight?.Color ?? Vector4.One;
        set
        {
            if (_highLight == null) return;
            _highLight.Color = value;
        }
    }

    public static void Init()
    {
        XIVPainterMain.Init(Svc.PluginInterface, "RotationSolverOverlay");

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

        var movingTarget = new VfxHighlightLine(default, default, 0);
        movingTarget.UpdateEveryFrame = () =>
        {
            var tar = CustomRotation.MoveTarget;

            var playerPos = Player.Object?.Position ?? default;

            movingTarget.From = playerPos;

            if (!Service.Config.ShowMoveTarget || !Player.Available || !tar.HasValue || Vector3.Distance(tar.Value, playerPos) < 0.01f)
            {
                movingTarget.Radius = 0;
                movingTarget.To = playerPos;
                return;
            }

            movingTarget.Radius = 0.5f;
            movingTarget.To = tar.Value;
        };

        _stateImage = new Drawing3DImage(null, default, 0)
        {
            MustInViewRange = true,
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

        _ = new TargetDrawing();
        _ = new TargetsDrawing();
        _ = new TargetText();
        _ = new BeneficialPositionDrawing();
    }

    public static void UpdateSettings()
    {
        XIVPainterMain.SampleLength = Service.Config.SampleLength;
        XIVPainterMain.UseTaskToAccelerate = Service.Config.UseTasksForOverlay;
        XIVPainterMain.Enable = !Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent] && Service.Config.UseOverlayWindow;
    }

    public static void Dispose()
    {
        XIVPainterMain.Dispose();
    }
}
