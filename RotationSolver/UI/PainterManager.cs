using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Basic.Configuration;
using RotationSolver.Updaters;
using XIVConfigUI;
using XIVDrawer;
using XIVDrawer.Element3D;
using XIVDrawer.ElementSpecial;
using XIVDrawer.Vfx;

namespace RotationSolver.UI;

internal static class PainterManager
{
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

    readonly static Drawing3DCircularSector _target = new (default, 0, 0, 3)
    {
        IsFill = false,
        Enable = false,
    };
    readonly static Drawing3DImage _targetImage = new (null, default, 0)
    {
        MustInViewRange = true,
        Enable = false,
    };
    const float targetRadius = 0.15f;
    const float beneficialRadius = 0.6f;

    readonly static Drawing3DCircularSector[] BeneficialItems = new Drawing3DCircularSector[64];
    readonly static Drawing3DText[] TargetTexts = new Drawing3DText[64];
    readonly static Drawing3DImage[] TargetsDrawings = new Drawing3DImage[64];
    readonly static Drawing3DImage[] UsersDrawings = new Drawing3DImage[64];

    public static void Init()
    {
        XIVDrawerMain.Init(Svc.PluginInterface, "RotationSolverOverlay");
        for (int i = 0; i < TargetTexts.Length; i++)
        {
            TargetTexts[i] = new Drawing3DText(string.Empty, default);
        }
        for (int i = 0; i < TargetsDrawings.Length; i++)
        {
            TargetsDrawings[i] = new Drawing3DImage(null, Vector3.Zero)
            {
                Enable = false,
                MustInViewRange = true,
            };
        }
        for (int i = 0; i < UsersDrawings.Length; i++)
        {
            UsersDrawings[i] = new Drawing3DImage(null, Vector3.Zero)
            {
                Enable = false,
                MustInViewRange = true,
            };
        }
        for (int i = 0; i < BeneficialItems.Length; i++)
        {
            BeneficialItems[i] = new Drawing3DCircularSector(default, 0, 0, 3)
            {
                Enable = false,
                IsFill = false,
            };
        }

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
                    if (ImageLoader.GetTexture(61516, out var texture))
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
    }

    public static void UpdateSettings()
    {
        XIVDrawerMain.SampleLength = Service.Config.SampleLength;
        XIVDrawerMain.UseTaskToAccelerate = Service.Config.UseTasksForOverlay;
        XIVDrawerMain.Enable = !Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent] && Service.Config.UseOverlayWindow;
        XIVDrawerMain.ViewPadding = Service.Config.WindowPadding;

        UpdateTargetTexts();
        UpdateTarget();
        UpdateHostileIcons();
        UpdateUsersIcons();
        UpdateBeneficial();
    }

    private static void UpdateTargetTexts()
    {
        for (int i = 0; i < TargetTexts.Length; i++)
        {
            TargetTexts[i].Text = string.Empty;
        }

        if (!Service.Config.ShowTargetTimeToKill) return;

        uint HealthRatioColor = ImGui.GetColorU32(Service.Config.TTKTextColor);

        int index = 0;
        foreach (GameObject t in DataCenter.AllHostileTargets.OrderBy(ObjectHelper.DistanceToPlayer))
        {
            if (t is not BattleChara b) continue;
            if (t is PlayerCharacter) continue;

            var item = TargetTexts[index++];

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

            if (index >= TargetTexts.Length) break;
        }
    }
    private static void UpdateTarget()
    {
        _target.Enable = _targetImage.Enable = false;
        if (!Service.Config.ShowTarget) return;

        if (ActionUpdater.NextAction is not BaseAction act) return;

        var d = DateTime.Now.Millisecond / 1000f;
        var ratio = (float)DrawingExtensions.EaseFuncRemap(EaseFuncType.None, EaseFuncType.Cubic)(d);

        if (Service.Config.TargetIconSize > 0)
        {
            _targetImage.Enable = true;
            _targetImage.Position = act.Target.Position ?? Player.Object.Position;
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
            _target.Center = act.Target.Position ?? Player.Object.Position;
            _target.Radius = targetRadius * ratio;
        }
    }

    private static unsafe void UpdateHostileIcons()
    {
        foreach (var item in TargetsDrawings)
        {
            item.Enable = false;
        }

        if (!Service.Config.ShowHostilesIcons)
        {
            return;
        }

        if (!ImageLoader.GetTexture(61510, out var hostileIcon)) return;

        for (int i = 0; i < Math.Min(TargetsDrawings.Length, DataCenter.AllHostileTargets.Length); i++)
        {
            if (TargetsDrawings[i] is not Drawing3DImage item) continue;

            var obj = DataCenter.AllHostileTargets[i];

            item.Enable = true;
            item.Image = hostileIcon;
            item.Position = obj.Position + new Vector3(0,
                    Service.Config.HostileIconHeight, 0);
            item.Size = Service.Config.HostileIconSize;
        }
    }

    private static unsafe void UpdateUsersIcons()
    {
        foreach (var item in UsersDrawings)
        {
            item.Enable = false;
        }

        if (!Service.Config.ShowUsersIcons)
        {
            return;
        }

        if (!ImageLoader.GetTexture(61551, out var hostileIcon)) return;

        for (int i = 0; i < Math.Min(UsersDrawings.Length, SocialUpdater._users.Count); i++)
        {
            if (UsersDrawings[i] is not Drawing3DImage item) continue;

            var obj = SocialUpdater._users[i];

            item.Enable = true;
            item.Image = hostileIcon;
            item.Position = obj.Position + new Vector3(0,
                    Service.Config.UserIconHeight, 0);
            item.Size = Service.Config.UserIconSize;
        }
    }

    private static void UpdateBeneficial()
    {
        if (!Service.Config.ShowBeneficialPositions
            || Svc.ClientState == null || !Player.Available
            || !OtherConfiguration.BeneficialPositions.TryGetValue(Svc.ClientState.TerritoryType, out var pts))
        {
            foreach (var item in BeneficialItems)
            {
                item.Enable = false;
            }
            return;
        }

        var d = DateTime.Now.Millisecond / 1000f;
        var ratio = (float)DrawingExtensions.EaseFuncRemap(EaseFuncType.None, EaseFuncType.Cubic)(d);

        var color = ImGui.GetColorU32(Service.Config.BeneficialPositionColor);
        var hColor = ImGui.GetColorU32(Service.Config.HoveredBeneficialPositionColor);

        int index = 0;
        foreach (var p in pts)
        {
            if (Vector3.Distance(Player.Object.Position, p) > 80) continue;

            var item = BeneficialItems[index++];

            item!.Center = p;
            item!.Radius = beneficialRadius * ratio;
            item!.Color = p == RotationConfigWindow.HoveredPosition ? hColor : color;

            if (index >= BeneficialItems.Length) break;
        }
    }

    public static void Dispose()
    {
        XIVDrawerMain.Dispose();
    }
}
