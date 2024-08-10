using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using NRender;
using RotationSolver.Updaters;
using RotationSolver.Vfx;
using XIVConfigUI;
using XIVConfigUI.Overlay;

namespace RotationSolver.UI;

internal static class PainterManager
{
    private class TargetsDrawingItem : TargetDrawingItem
    {
        public override bool Show => Service.Config.ShowHostilesIcons;

        public override IGameObject[] Targets => DataCenter.AllHostileTargets;

        public override float Height => Service.Config.HostileIconHeight;

        public override float Size => Service.Config.HostileIconSize;

        protected override uint GetIcon(IGameObject IGameObject)
        {
            if (ActionUpdater.NextAction is IBaseAction act)
            {
                if (act.Config.CantAttack(IGameObject)) return 61502;
                if (act.Config.IsTopPriority(IGameObject)) return 61480;
            }
            else
            {
                if (IGameObject.IsNoTarget()) return 61502;
                if (IGameObject.IsTopPriority()) return 61480;
            }

            return 61510;
        }
    }

    private class AllianceDrawingItem : TargetDrawingItem
    {
        public override bool Show => Service.Config.ShowAllianceIcons;

        public override IGameObject[] Targets => DataCenter.AllianceMembers.Where(i => i.EntityId != Player.Object?.EntityId).ToArray();

        public override float Height => Service.Config.AllianceIconHeight;

        public override float Size => Service.Config.AllianceIconSize;

        protected override uint GetIcon(IGameObject IGameObject) => 61515;
    }

    private class UsersDrawingItem : TargetDrawingItem
    {
        public override bool Show => Service.Config.ShowUsersIcons;

        public override IGameObject[] Targets => [.. SocialUpdater._users];

        public override float Height => Service.Config.UserIconHeight;

        public override float Size => Service.Config.UserIconSize;

        protected override uint GetIcon(IGameObject IGameObject) => 61501;
    }

    private static DrawingHighlightHotbar? _highLight;
    private static Drawing3DImage? _stateImage;
    public static HashSet<HotbarID> HotbarIDs => _highLight?.HotbarIDs ?? [];

    public static Vector4 HighlightColor
    {
        get => _highLight?.Color ?? Vector4.One;
        set
        {
            if (_highLight == null) return;
            _highLight.Color = value;
        }
    }

    private const float omenHeight = 1;

    private static StaticVfx? _meleeWarning, _movePosition;
    private readonly static Drawing3DText[] TargetTexts = new Drawing3DText[64];
    private readonly static List<TargetDrawingItem> TargetDrawings = [];

    public static void Init()
    {
        NRenderMain.Init(Svc.PluginInterface, "RotationSolverVfxDraw");

        for (int i = 0; i < TargetTexts.Length; i++)
        {
            TargetTexts[i] = new Drawing3DText(string.Empty, default);
        }

        TargetDrawings.Add(new AllianceDrawingItem());
        TargetDrawings.Add(new TargetsDrawingItem());
        TargetDrawings.Add(new UsersDrawingItem());

        _highLight = new();
        Update();

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
                    _stateImage.Size = Service.Config.StateIconSize;
                }
                else
                {
                    _stateImage.Size = 0;
                }
            },
        };
    }

    public static void Update()
    {
        XIVConfigUIMain.UseOverlay = !Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent] && Service.Config.UseOverlayWindow;
        DrawingExtensions.ViewPadding = Service.Config.WindowPadding;

        HighlightColor = Service.Config.TeachingModeColor;

        foreach (var target in TargetDrawings)
        {
            target.Update();
        }

        if (_stateImage != null)
        {
            if (ImageLoader.GetTexture(61516, out var texture))
            {
                _stateImage.Image = texture;
            }
        }

        UpdateTargetTexts();
        UpdateTarget();
        UpdateMeleeOffset();
        UpdateMoveTarget();
    }

    private static void UpdateMoveTarget()
    {
        var tar = CustomRotation.MoveTarget;

        var playerPos = Player.Object?.Position ?? default;

        if (!Service.Config.ShowMoveTarget || !Player.Available
            || !tar.HasValue || Vector3.Distance(tar.Value, playerPos) < 0.01f)
        {
            tar = null;
        }

        if (tar == null && _movePosition == null) return;
        if (tar != null && _movePosition != null)
        {
            _movePosition.Position = tar.Value;
            return;
        }

        _movePosition?.Dispose();
        _movePosition = null;

        if (tar == null) return;

        _movePosition = new StaticVfx(StaticOmen.Circle, new Vector3(0.5f, omenHeight, 0.5f), tar.Value, Service.Config.MoveTargetColor, 0);
    }

    private static void UpdateMeleeOffset()
    {
        IBattleChara? tar = DataCenter.HostileTarget;
        if (Player.Available && (Player.Object.IsJobCategory(JobRole.Tank) || Player.Object.IsJobCategory(JobRole.Melee))
            && (tar?.IsEnemy() ?? false) && Service.Config.DrawMeleeOffset
            && ActionUpdater.NextGCDAction == null
            && tar.DistanceToPlayer() > 3 && tar.DistanceToPlayer() < 3 + Service.Config.MeleeRangeOffset)
        {
        }
        else
        {
            tar = null;
        }

        if (tar != null)
        {
            if (!tar.IsValid()) tar = null;
            else if (!tar.IsTargetable) tar = null;
        }

        if (tar == null && _meleeWarning == null) return;
        if (tar != null && _meleeWarning != null) return;

        _meleeWarning?.Dispose();
        _meleeWarning = null;

        if (tar == null) return;

        var radius = tar.HitboxRadius + 3 + Player.Object.HitboxRadius;
        var bigRadius = radius + Service.Config.MeleeRangeOffset;

        _meleeWarning = new(StaticOmen.Donut, new Vector3(bigRadius, omenHeight, bigRadius), tar, Service.Config.MeleeOffsetColor)
        {
            Radian = radius / bigRadius,
        };
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
        foreach (IGameObject t in DataCenter.AllHostileTargets.OrderBy(ObjectHelper.DistanceToPlayer))
        {
            if (t is not IBattleChara b) continue;
            if (t is IPlayerCharacter) continue;

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

    private readonly static Drawing3DImage _targetImage = new(null, default, 0)
    {
        MustInViewRange = true,
        Enable = false,
    };
    private static void UpdateTarget()
    {
        _targetImage.Enable = false;
        if (!Service.Config.ShowTarget) return;

        if (ActionUpdater.NextAction is not BaseAction act) return;

        if (Service.Config.TargetIconSize <= 0) return;

        _targetImage.Enable = true;
        _targetImage.Position = act.Target.Position ?? Player.Object.Position;
        if (act.GetTexture(out var texture, true))
        {
            _targetImage.Image = texture;
            _targetImage.Size = Service.Config.TargetIconSize;
        }
    }

    public static void Dispose()
    {
        NRenderMain.Dispose();
    }
}
