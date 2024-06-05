using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration.Drawing;
using RotationSolver.Basic.Configuration.TerritoryAction;
using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using RotationSolver.UI.ConfigWindows;
using XIVConfigUI;
using XIVConfigUI.Attributes;
using XIVDrawer.Vfx;

namespace RotationSolver.UI;

internal static class TerritoryActionDrawer
{
    private static float Scale => ImGuiHelpers.GlobalScale;

    private static readonly CollapsingHeaderGroup _territoryActionsList = new()
    {
        HeaderSize = FontSize.Fifth,
    };

    private static IDisposable[]? _previewItems = null;

    public static void DrawTerritoryAction(ITerritoryAction action, uint[] actionIds)
    {
        if (action is ActionAction actionAction)
        {
            if (DataCenter.RightNowRotation != null)
            {
                var popUpKey = $"Action Finder{actionAction.GetHashCode()}";
                ConditionDrawer.ActionSelectorPopUp(popUpKey, _territoryActionsList, DataCenter.RightNowRotation, item => actionAction.ID = (ActionID)item.ID);

                if (actionAction.ID.GetTexture(out var icon) || ImageLoader.GetTexture(4, out icon))
                {
                    ImGui.SameLine();
                    var cursor = ImGui.GetCursorPos();
                    if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * ConditionDrawer.IconSize, actionAction.GetHashCode().ToString()))
                    {
                        if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
                    }
                    ImGuiHelper.DrawActionOverlay(cursor, ConditionDrawer.IconSize, 1);
                }

                ImGui.SameLine();

                var type = actionAction.TargetType;
                if(ConditionDrawer.DrawByteEnum("##Target Type" + actionAction.GetHashCode(), ref type))
                {
                    actionAction.TargetType = type;
                }
            }
        }
        else if (action is StateAction stateItem)
        {
            var state = stateItem.State;
            ImGui.SameLine();
            if (ConditionDrawer.DrawByteEnum($"##AutoState{stateItem.GetHashCode()}", ref state))
            {
                stateItem.State = state;
            }
        }
        else if (action is MacroAction macroAction)
        {
            ImGui.SameLine();

            var macro = macroAction.Macro;
            if (ImGui.InputTextMultiline(UiString.ConfigWindow_About_Macros.Local() + ": ##" + macroAction.GetHashCode(), ref macro, 500, new Vector2(-1, 50)))
            {
                macroAction.Macro = macro;
            }
        }
        else if (action is MoveAction moveAction)
        {
            ImGui.SameLine();
            if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddPoint" + moveAction.GetHashCode())
                && Player.Available)
            {
                moveAction.Points.Add(Player.Object.Position);
            }

            for (int i = 0; i < moveAction.Points.Count; i++)
            {
                var point = moveAction.Points[i];

                void Delete()
                {
                    moveAction.Points.RemoveAt(i);
                }

                void Up()
                {
                    moveAction.Points.RemoveAt(i);
                    moveAction.Points.Insert(Math.Max(0, i - 1), point);
                }

                void Down()
                {
                    moveAction.Points.RemoveAt(i);
                    moveAction.Points.Insert(Math.Min(moveAction.Points.Count, i + 1), point);
                }

                var key = $"Point Pop Up: {moveAction.Points.GetHashCode()}";

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                    (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]),
                    (UiString.ConfigWindow_Actions_MoveUp.Local(), Up, ["↑"]),
                    (UiString.ConfigWindow_Actions_MoveDown.Local(), Down, ["↓"]));

                if (ImageLoader.GetTexture(10, out var texture))
                {
                    if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, ConditionDrawer.IconSize * Vector2.One, "Position " + moveAction.GetHashCode() + i))
                    {
                        if (_previewItems == null)
                        {
                            _previewItems = [new StaticVfx(GroundOmenFriendly.BasicCircle.Omen(), point, 0, Vector3.One)];
                        }
                        else
                        {
                            foreach (var preview in _previewItems)
                            {
                                preview.Dispose();
                            }
                            _previewItems = null;
                        }
                    }
                }

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                    (Delete, [VirtualKey.DELETE]),
                    (Up, [VirtualKey.UP]),
                    (Down, [VirtualKey.DOWN]));

                ImGui.SameLine();

                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePosition.Local(), ref point, "Pos" + moveAction.GetHashCode() + i, "X", "Y", "Z", () => Player.Object?.Position ?? default))
                {
                    moveAction.Points[i] = point;
                }
            }
        }
        else if (action is PathfindAction pathAction)
        {
            ImGui.SameLine();

            var point = pathAction.Destination;
            if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePosition.Local(), ref point, "Pos" + pathAction.GetHashCode(), "X", "Y", "Z", () => Player.Object?.Position ?? default))
            {
                pathAction.Destination = point;
            }
        }
        else if (action is DrawingAction drawingAction)
        {
            DrawDrawingTimeline(drawingAction, actionIds);
        }
    }

    private static void DrawDrawingTimeline(DrawingAction drawingItem, uint[] actionIds)
    {
        ImGui.SameLine();

        AddButton();

        for (int i = 0; i < drawingItem.DrawingGetters.Count; i++)
        {
            if (i != 0)
            {
                ImGui.Spacing();
            }
            var item = drawingItem.DrawingGetters[i];

            void Delete()
            {
                drawingItem.DrawingGetters.RemoveAt(i);
            }

            void Up()
            {
                drawingItem.DrawingGetters.RemoveAt(i);
                drawingItem.DrawingGetters.Insert(Math.Max(0, i - 1), item);
            }

            void Down()
            {
                drawingItem.DrawingGetters.RemoveAt(i);
                drawingItem.DrawingGetters.Insert(Math.Min(drawingItem.DrawingGetters.Count, i + 1), item);
            }

            void Copy()
            {
                var str = JsonConvert.SerializeObject(drawingItem.DrawingGetters[i], Formatting.Indented);
                ImGui.SetClipboardText(str);
            }

            var key = $"DrawingItem Pop Up: {item.GetHashCode()}";

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]),
                (UiString.ConfigWindow_Actions_MoveUp.Local(), Up, ["↑"]),
                (UiString.ConfigWindow_Actions_MoveDown.Local(), Down, ["↓"]),
                (UiString.ConfigWindow_Actions_Copy.Local(), Copy, ["Ctrl"]));

            if (ImageLoader.GetTexture(30, out var texture))
            {
                if (ImGuiHelper.SilenceImageButton(texture.ImGuiHandle, Vector2.One * ConditionDrawer.IconSize, false, $"Icon :{item.GetHashCode()}"))
                {
                    if (_previewItems == null)
                    {
                        _previewItems = item.GetDrawing();
                    }
                    else
                    {
                        foreach (var preview in _previewItems)
                        {
                            preview.Dispose();
                        }
                        _previewItems = null;
                    }
                }
            }

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                (Delete, [VirtualKey.DELETE]),
                (Up, [VirtualKey.UP]),
                (Down, [VirtualKey.DOWN]),
                (Copy, [VirtualKey.CONTROL]));

            ImGui.SameLine();

            using var grp = ImRaii.Group();

            DrawingGetterDraw(item, actionIds);
        }

        void AddButton()
        {
            if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddDrawingButton" + drawingItem.GetHashCode()))
            {
                ImGui.OpenPopup("PopupDrawingButton" + drawingItem.GetHashCode());
            }
            ImguiTooltips.HoveredTooltip(UiString.AddDrawingTimelineButton.Local());

            using var popUp = ImRaii.Popup("PopupDrawingButton" + drawingItem.GetHashCode());
            if (popUp)
            {
                AddOneCondition<StaticDrawingGetter>();
                AddOneCondition<ObjectDrawingGetter>();

                if (actionIds.Length > 0)
                {
                    AddOneCondition<ActionDrawingGetter>();
                }
                if (ImGui.Selectable(UiString.ActionSequencer_FromClipboard.Local()))
                {
                    var str = ImGui.GetClipboardText();
                    try
                    {
                        var s = JsonConvert.DeserializeObject<BaseDrawingGetter>
                            (str, new BaseDrawingGetterConverter())!;
                        drawingItem.DrawingGetters.Add(s);
                    }
                    catch (Exception ex)
                    {
                        Svc.Log.Warning(ex, "Failed to load the condition.");
                    }
                    ImGui.CloseCurrentPopup();
                }
            }

            void AddOneCondition<T>() where T : BaseDrawingGetter
            {
                if (ImGui.Selectable(typeof(T).Local()))
                {
                    drawingItem.DrawingGetters.Add(Activator.CreateInstance<T>());
                    ImGui.CloseCurrentPopup();
                }
            }
        }
    }

    static readonly IEnumerable<FieldInfo> _omenInfo = typeof(GroundOmenHostile).GetRuntimeFields()
    .Concat(typeof(GroundOmenNone).GetRuntimeFields())
    .Concat(typeof(GroundOmenFriendly).GetRuntimeFields());

    static readonly string[] _omenNames = _omenInfo
        .Select(f => f.GetValue(null))
        .OfType<string>().ToArray();

    static readonly string[] _omenShowNames = _omenInfo
    .Select(f => f.Name).ToArray();

    static readonly IEnumerable<FieldInfo> _actorInfo = typeof(LockOnOmen).GetRuntimeFields().Union(typeof(ChannelingOmen).GetRuntimeFields());
    static readonly string[] _actorNames = _actorInfo
        .Select(f => f.GetValue(null))
        .OfType<string>().ToArray();

    static readonly string[] _actorShowNames = _actorInfo
        .Select(f => f.Name).ToArray();

    internal static int _openedTab = 0;

    private static void DrawingGetterDraw(BaseDrawingGetter drawing, uint[] actionIds)
    {
        var enable = drawing.Enable;
        if (ImGui.Checkbox("##Enable" + drawing.GetHashCode(), ref enable))
        {
            drawing.Enable = enable;
        }

        ImGui.SameLine();

        var name = drawing.Name;
        ImGui.SetNextItemWidth(300 * Scale);
        if (ImGui.InputText("##" + drawing.GetHashCode(), ref name, 256))
        {
            drawing.Name = name;
        }

        ImGui.SameLine();

        var isOpen = _openedTab == drawing.GetHashCode();
        if (ImGuiEx.IconButton(FontAwesomeIcon.Cog, "Config icon." + drawing.GetHashCode()))
        {
            _openedTab = isOpen ? 0 : drawing.GetHashCode();
        }
        if (!isOpen) return;

        if (drawing is StaticDrawingGetter staticDrawing)
        {
            var index = Array.IndexOf(_omenNames, staticDrawing.Path.UnOmen());
            if (ImGuiHelperRS.SelectableCombo("##PathName" + drawing.GetHashCode(), _omenShowNames, ref index))
            {
                staticDrawing.Path = _omenNames[index].Omen();
            }

            ImGui.SameLine();

            var isTarget = staticDrawing.PlaceOnObject;
            if (ImGui.Checkbox(UiString.TimelinePlaceOnTarget.Local() + "##" + drawing.GetHashCode(), ref isTarget))
            {
                staticDrawing.PlaceOnObject = isTarget;
            }

            if (isTarget)
            {
                var pos = staticDrawing.Position;
                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePositionOffset.Local() + ":　", ref pos, drawing.GetHashCode().ToString() + "PositionOffset", "X", "Y", "Z"))
                {
                    staticDrawing.Position = pos;
                }
            }
            else
            {
                var pos = staticDrawing.Position;
                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePosition.Local() + ":　", ref pos, drawing.GetHashCode().ToString() + "Position", "X", "Y", "Z", () => Player.Object?.Position ?? default))
                {
                    staticDrawing.Position = pos;
                }
            }

            var rot = staticDrawing.Rotation / MathF.PI * 180f;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Degree, "Rotation: ##" + drawing.GetHashCode(), ref rot))
            {
                staticDrawing.Rotation = rot * MathF.PI / 180f;
            }

            var scale = staticDrawing.Scale;
            if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelineScale.Local() + ":　", ref scale, drawing.GetHashCode().ToString() + "Scale", "X", "Y", "Z"))
            {
                staticDrawing.Scale = scale;
            }

            if (isTarget)
            {
                DrawObjectGetter(staticDrawing.ObjectGetter, UiString.TimelineObjectGetter.Local());
            }

            DrawTextDrawing(staticDrawing.Text, UiString.TimelineShowText.Local() + ": ");
        }
        else if (drawing is ObjectDrawingGetter objectDrawing)
        {
            var index = objectDrawing.IsActorEffect ? 1 : 0;
            if (ImGuiHelperRS.SelectableCombo("##ActorType" + drawing.GetHashCode(), [UiString.TimelineGround.Local(), UiString.TimelineActor.Local()], ref index))
            {
                objectDrawing.IsActorEffect = index != 0;
            }

            ImGui.SameLine();
            if (objectDrawing.IsActorEffect)
            {
                index = Array.IndexOf(_actorNames.Select(n => n.StartsWith("chn_") ? n.Channeling() : n.LockOn()).ToArray(), objectDrawing.Path);
                if (ImGuiHelperRS.SelectableCombo("##PathName" + drawing.GetHashCode(), _actorShowNames, ref index))
                {
                    var actorName = _actorNames[index];
                    objectDrawing.Path = actorName.StartsWith("chn_") ? actorName.Channeling() : actorName.LockOn();
                }
            }
            else
            {
                index = Array.IndexOf(_omenNames, objectDrawing.Path.UnOmen());
                if (ImGuiHelperRS.SelectableCombo("##PathName" + drawing.GetHashCode(), _omenShowNames, ref index))
                {
                    objectDrawing.Path = _omenNames[index].Omen();
                }

                var rot = objectDrawing.Rotation / MathF.PI * 180f;
                if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Degree, UiString.TimelineRotation.Local() + ": ##" + drawing.GetHashCode(), ref rot))
                {
                    objectDrawing.Rotation = rot * MathF.PI / 180f;
                }

                var pos = objectDrawing.Position;
                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePosition.Local() + ":　", ref pos, drawing.GetHashCode().ToString() + "Position", "X", "Y", "Z", () => Player.Object?.Position ?? default))
                {
                    objectDrawing.Position = pos;
                }

                var scale = objectDrawing.Scale;
                if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelineScale.Local() + ":　", ref scale, drawing.GetHashCode().ToString() + "Scale", "X", "Y", "Z"))
                {
                    objectDrawing.Scale = scale;
                }
            }

            DrawObjectGetter(objectDrawing.ObjectGetter, UiString.TimelineObjectGetter.Local());
            DrawTextDrawing(objectDrawing.ObjectText, UiString.TimelineShowText.Local());

            var check = objectDrawing.GetATarget;
            if (ImGui.Checkbox(UiString.TimelineNeedATarget.Local() + ": ##" + drawing.GetHashCode(), ref check))
            {
                objectDrawing.GetATarget = check;
            }

            if (!check) return;

            ImGui.SameLine();
            check = objectDrawing.IsTargetByTarget;
            if (ImGui.Checkbox(UiString.TimelineTargetByTarget.Local() + ": ##" + drawing.GetHashCode(), ref check))
            {
                objectDrawing.IsTargetByTarget = check;
            }

            if (!check)
            {
                DrawObjectGetter(objectDrawing.TargetGetter, UiString.TimelineTargetGetter.Local());
            }

            DrawTextDrawing(objectDrawing.TargetText, UiString.TimelineShowText.Local());
        }
        else if (drawing is ActionDrawingGetter actionDrawing)
        {
            var index = Array.IndexOf(_omenNames, actionDrawing.Path.UnOmen());
            if (ImGuiHelperRS.SelectableCombo("##PathName" + drawing.GetHashCode(), _omenShowNames, ref index))
            {
                actionDrawing.Path = _omenNames[index].Omen();
            }

            index = Array.IndexOf(actionIds, actionDrawing.ActionID);
            var actionNames = actionIds.Select(i => Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()?.GetRow(i)?.Name.RawString ?? "Unnamed Action").ToArray();

            ImGui.SameLine();
            if (ImGuiHelperRS.SelectableCombo("Action ##Select Action" + drawing.GetHashCode(), actionNames, ref index))
            {
                actionDrawing.ActionID = actionIds[index];
            }

            var rot = actionDrawing.Rotation / MathF.PI * 180f;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Degree, UiString.TimelineRotation.Local() + ": ##" + drawing.GetHashCode(), ref rot))
            {
                actionDrawing.Rotation = rot * MathF.PI / 180f;
            }

            var pos = actionDrawing.Position;
            if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePosition.Local() + ": ", ref pos, drawing.GetHashCode().ToString(), "X", "Y", "Z", () => Player.Object?.Position ?? default))
            {
                actionDrawing.Position = pos;
            }

            var scale = actionDrawing.X;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Yalms, UiString.TimelineScale.Local() + " X:　##" + drawing.GetHashCode(), ref scale))
            {
                actionDrawing.X = scale;
            }

            scale = actionDrawing.Y;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Yalms, UiString.TimelineScale.Local() + " Y:　##" + drawing.GetHashCode(), ref scale))
            {
                actionDrawing.Y = scale;
            }

            DrawObjectGetter(actionDrawing.ObjectGetter, UiString.TimelineObjectGetter.Local());
        }
    }
    static string _statusSearching = string.Empty;
    internal static void DrawObjectGetter(ObjectGetter getter, string getterName)
    {
        if (ImGui.Button(getterName + "##" + getter.GetHashCode()))
        {
            if (_previewItems == null)
            {
                _previewItems = [.. Svc.Objects.Where(getter.CanGet).Select(b => new StaticVfx(GroundOmenNone.Circle.Omen(), b, Vector3.One * b.HitboxRadius))];
            }
            else
            {
                foreach (var preview in _previewItems)
                {
                    preview.Dispose();
                }
                _previewItems = null;
            }
        }

        ImGui.SameLine();

        var check = getter.Type;

        if (ConditionDrawer.DrawByteEnum("Object Type: ##" + getter.GetHashCode(), ref check))
        {
            getter.Type = check;
        }

        using var indent = ImRaii.PushIndent();

        ImGui.SameLine();

        var key = "Status PopUp" + getter.GetHashCode();
        var status = Svc.Data.GetExcelSheet<Status>()?.GetRow(getter.Status);
        if (ImageLoader.GetTexture(getter.Status == 0 ? 16220 : status?.Icon ?? 10100, out var texture, 10100))
        {
            if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, new Vector2(24, 32) * Scale, "Status" + getter.Status.ToString()))
            {
                ImGui.OpenPopup(key);
            }

            ImGuiHelper.ExecuteHotKeysPopup(key + "Edit", string.Empty, $"{status?.Name ?? "Unknown"} ({getter.Status})", false,
                (() => getter.Status = 0, [VirtualKey.DELETE]));
        }

        switch (check)
        {
            case ObjectType.Myself:
                return;

            case ObjectType.PlayerCharactor:

                var size = Vector2.One * 24 * Scale;

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_tank.png", out var overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Tank, "Tank##" + getter.GetHashCode()))
                    {
                        getter.Tank = !getter.Tank;
                    }
                }

                ImGui.SameLine();

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_healer.png", out overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Healer, "Healer##" + getter.GetHashCode()))
                    {
                        getter.Healer = !getter.Healer;
                    }
                }

                ImGui.SameLine();

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_dps.png", out overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Melee, "Melee##" + getter.GetHashCode()))
                    {
                        getter.Melee = !getter.Melee;
                    }
                }

                ImGui.SameLine();

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_ranged.png", out overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Range, "Range##" + getter.GetHashCode()))
                    {
                        getter.Range = !getter.Range;
                    }
                }

                ImGui.SameLine();

                if (ImageLoader.GetTexture("https://xivapi.com/cj/misc/clear_dps_magic.png", out overlay))
                {
                    if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, size,
                        getter.Caster, "Caster##" + getter.GetHashCode()))
                    {
                        getter.Caster = !getter.Caster;
                    }
                }
                break;

            case ObjectType.GameObject:
            case ObjectType.BattleCharactor:
                ImGui.SetNextItemWidth(150 * Scale);
                var name = getter.DataID;
                if (ImGui.InputText("Data ID## " + getter.GetHashCode(), ref name, 256))
                {
                    getter.DataID = name;
                }

                break;
        }

        RotationConfigWindow.StatusPopUp(key, RotationConfigWindow.BadStatus, ref _statusSearching, s => getter.Status = s.RowId);

        if (getter.Status != 0)
        {
            var time = getter.StatusTime;
            if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Seconds, UiString.ConfigWindow_Timeline_Time.Local() + ": ##" + getter.GetHashCode(), ref time))
            {
                getter.StatusTime = time;
            }
        }

        var duration = getter.TimeDuration;
        if (ConditionDrawer.DrawDragFloat2(ConfigUnitType.Seconds, UiString.TimelineEffectDuration.Local(), ref duration, "Effect duration" + getter.GetHashCode(), "Start", "End"))
        {
            getter.TimeDuration = duration;
        }

        if (!ImGui.CollapsingHeader("Effect##" + getter.GetHashCode())) return;

        var vfx = getter.VfxPath;
        ImGui.SetNextItemWidth(300 * Scale);
        if (ImGui.InputText("Vfx ## " + getter.GetHashCode(), ref vfx, 256))
        {
            getter.VfxPath = vfx;
        }

        ImGui.NewLine();
        var param = (int)getter.ObjectEffect1;
        if (ConditionDrawer.DrawDragInt("Param1##" + getter.GetHashCode(), ref param))
        {
            getter.ObjectEffect1 = (ushort)param;
        }

        param = getter.ObjectEffect2;
        if (ConditionDrawer.DrawDragInt("Param2##" + getter.GetHashCode(), ref param))
        {
            getter.ObjectEffect2 = (ushort)param;
        }
    }

    private static void DrawTextDrawing(TextDrawing textDrawing, string name)
    {
        var text = textDrawing.Text;
        ImGui.SetNextItemWidth(300 * Scale);
        if (ImGui.InputText(name + "##" + textDrawing.GetHashCode(), ref text, 256))
        {
            textDrawing.Text = text;
        }

        if (string.IsNullOrEmpty(text)) return;

        using var indent = ImRaii.PushIndent();

        var positionOffset = textDrawing.PositionOffset;
        if (ConditionDrawer.DrawDragFloat3(ConfigUnitType.Yalms, UiString.TimelinePositionOffset.Local() + ": ", ref positionOffset, textDrawing.GetHashCode().ToString(), "X", "Y", "Z"))
        {
            textDrawing.PositionOffset = positionOffset;
        }

        ImGui.SetNextItemWidth(Scale * 150);
        var scale = textDrawing.Scale;
        if (ImGui.DragFloat($"{UiString.TimelineScale.Local()}##{textDrawing.GetHashCode()}", ref scale, 0.1f, 0.1f, 20, $"{scale:F2}{ConfigUnitType.Percent.ToSymbol()}"))
        {
            textDrawing.Scale = scale;
        }

        var corner = textDrawing.Corner;
        if (ConditionDrawer.DrawDragFloat(ConfigUnitType.Pixels, UiString.TimelineCorner.Local() + "##" + textDrawing.GetHashCode(), ref corner))
        {
            textDrawing.Corner = corner;
        }

        var padding = textDrawing.Padding;
        if (ConditionDrawer.DrawDragFloat2(ConfigUnitType.Pixels, UiString.TimelinePadding.Local() + ": ", ref padding, textDrawing.GetHashCode().ToString(), "X", "Y"))
        {
            textDrawing.Padding = padding;
        }

        var value = textDrawing.Color;
        ImGui.SetNextItemWidth(150 * 1.5f * Scale);
        if (ImGui.ColorEdit4($"{UiString.TimelineColor.Local()}##{textDrawing.GetHashCode()}", ref value))
        {
            textDrawing.Color = value;
        }

        value = textDrawing.BackgroundColor;
        ImGui.SetNextItemWidth(150 * 1.5f * Scale);
        if (ImGui.ColorEdit4($"{UiString.TimelineBackgroundColor.Local()}##{textDrawing.GetHashCode()}", ref value))
        {
            textDrawing.BackgroundColor = value;
        }
    }
}
