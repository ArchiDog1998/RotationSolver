using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Basic.Configuration;
using RotationSolver.UI.ConfigWindows;
using RotationSolver.Updaters;
using XIVConfigUI;
using XIVDrawer;
using XIVDrawer.Element3D;
using XIVDrawer.ElementSpecial;
using XIVDrawer.Vfx;

namespace RotationSolver.UI;

internal static class PainterManager
{
    private class TargetsDrawingItem : TargetDrawingItem
    {
        public override bool Show => Service.Config.ShowHostilesIcons;

        public override GameObject[] Targets => DataCenter.AllHostileTargets;

        public override float Height => Service.Config.HostileIconHeight;

        public override float Size => Service.Config.HostileIconSize;

        protected override uint GetIcon(GameObject gameObject)
        {
            if (ActionUpdater.NextAction is IBaseAction act)
            {
                if (act.Config.CantAttack(gameObject)) return 61502;
                if (act.Config.IsTopPriority(gameObject)) return 61480;
            }
            else
            {
                if (gameObject.IsNoTarget()) return 61502;
                if (gameObject.IsTopPriority()) return 61480;
            }

            return 61510;
        }
    }

    private class AllianceDrawingItem : TargetDrawingItem
    {
        public override bool Show => Service.Config.ShowAllianceIcons;

        public override GameObject[] Targets => DataCenter.AllianceMembers.Where(i => i.EntityId != Player.Object?.EntityId).ToArray();

        public override float Height => Service.Config.AllianceIconHeight;

        public override float Size => Service.Config.AllianceIconSize;

        protected override uint GetIcon(GameObject gameObject) => 61515;
    }

    private class UsersDrawingItem : TargetDrawingItem
    {
        public override bool Show => Service.Config.ShowUsersIcons;

        public override GameObject[] Targets => SocialUpdater._users.ToArray();

        public override float Height => Service.Config.UserIconHeight;

        public override float Size => Service.Config.UserIconSize;

        protected override uint GetIcon(GameObject gameObject) => 61501;
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
    readonly static List<TargetDrawingItem> TargetDrawings = [];

    public static void Init()
    {
        XIVDrawerMain.Init(Svc.PluginInterface, "RotationSolverOverlay");
        for (int i = 0; i < TargetTexts.Length; i++)
        {
            TargetTexts[i] = new Drawing3DText(string.Empty, default);
        }

        for (int i = 0; i < BeneficialItems.Length; i++)
        {
            BeneficialItems[i] = new Drawing3DCircularSector(default, 0, 0, 3)
            {
                Enable = false,
                IsFill = false,
            };
        }

        TargetDrawings.Add(new AllianceDrawingItem());
        TargetDrawings.Add(new TargetsDrawingItem());
        TargetDrawings.Add(new UsersDrawingItem());

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

        foreach (var target in TargetDrawings)
        {
            target.Update();
        }

        UpdateTargetTexts();
        UpdateTarget();
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

    private static void UpdateBeneficial()
    {
        foreach (var item in BeneficialItems)
        {
            item.Enable = false;
        }

        if (!Service.Config.ShowBeneficialPositions
            || Svc.ClientState == null || !Player.Available)
        {
            return;
        }

        var d = DateTime.Now.Millisecond / 1000f;
        var ratio = (float)DrawingExtensions.EaseFuncRemap(EaseFuncType.None, EaseFuncType.Cubic)(d);

        var color = ImGui.GetColorU32(Service.Config.BeneficialPositionColor);
        var hColor = ImGui.GetColorU32(Service.Config.HoveredBeneficialPositionColor);

        var pts = OtherConfiguration.TerritoryConfig.BeneficialPositions;

        for (int i = 0; i < Math.Min(BeneficialItems.Length, pts.Count); i++)
        {
            var item = BeneficialItems[i];
            var p = pts[i];

            item.Center = p;
            item.Radius = beneficialRadius * ratio;
            item.Color = p == ListItem.HoveredPosition ? hColor : color;
            item.Enable = true;
        }
    }

    public static void Dispose()
    {
        XIVDrawerMain.Dispose();
    }
}
