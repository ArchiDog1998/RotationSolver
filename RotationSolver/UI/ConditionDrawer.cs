using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration.Condition;
using RotationSolver.UI.ConfigWindows;
using RotationSolver.Updaters;
using XIVConfigUI;
using XIVConfigUI.Attributes;
using Action = System.Action;
using TargetType = RotationSolver.Basic.Configuration.Condition.TargetType;

namespace RotationSolver.UI;

internal static class ConditionDrawer
{
    internal static void DrawMain(this ConditionSet? conditionSet, ICustomRotation? rotation)
    {
        if (conditionSet == null) return;
        if (rotation == null) return;

        DrawCondition(conditionSet.IsTrue(rotation), conditionSet.GetHashCode().ToString(), ref conditionSet.Not);
        ImGui.SameLine();
        conditionSet.Draw(rotation);
    }

    internal static void DrawCondition(bool? tag, string id, ref bool isNot)
    {
        float size = IconSize * (1 + (8 / 82));
        if (!tag.HasValue)
        {
            if (ImageLoader.GetTexture("ui/uld/image2.tex", out var texture) || IconSet.GetTexture(0u, out texture))
            {
                if (ImGuiHelper.SilenceImageButton(texture.ImGuiHandle, Vector2.One * size, false, id))
                {
                    isNot = !isNot;
                }

                ImGuiHelper.HoveredTooltip(string.Format(UiString.ActionSequencer_NotDescription.Local(), isNot));
            }
        }
        else
        {
            if (ImageLoader.GetTexture("ui/uld/readycheck_hr1.tex", out var texture))
            {
                if (ImGuiHelper.SilenceImageButton(texture.ImGuiHandle, Vector2.One * size,
                    new Vector2(tag.Value ? 0 : 0.5f, 0),
                    new Vector2(tag.Value ? 0.5f : 1, 1), isNot ? ImGui.ColorConvertFloat4ToU32(new Vector4(1, 0.8f, 0.5f, 0.2f)) : 0, id))
                {
                    isNot = !isNot;
                }
                ImGuiHelper.HoveredTooltip(string.Format(UiString.ActionSequencer_NotDescription.Local(), isNot));
            }
        }
    }

    internal static void DrawCondition(bool? tag)
    {
        float size = IconSize * (1 + (8 / 82));

        if (!tag.HasValue)
        {
            if (ImageLoader.GetTexture("ui/uld/image2.tex", out var texture) || IconSet.GetTexture(0u, out texture))
            {
                ImGui.Image(texture.ImGuiHandle, Vector2.One * size);
            }
        }
        else
        {
            if (ImageLoader.GetTexture("ui/uld/readycheck_hr1.tex", out var texture))
            {
                ImGui.Image(texture.ImGuiHandle, Vector2.One * size,
                    new Vector2(tag.Value ? 0 : 0.5f, 0),
                    new Vector2(tag.Value ? 0.5f : 1, 1));
            }
        }
    }

    private static IEnumerable<MemberInfo> GetAllMethods(this Type? type, Func<Type, IEnumerable<MemberInfo>> getFunc)
    {
        if (type == null || getFunc == null) return [];

        var methods = getFunc(type);
        return methods.Union(type.BaseType.GetAllMethods(getFunc));
    }

    public static bool DrawByteEnum<T>(string name, ref T value) where T : struct, Enum
    {
        var values = Enum.GetValues<T>().Where(i => i.GetAttribute<ObsoleteAttribute>() == null).ToHashSet().ToArray();
        var index = Array.IndexOf(values, value);
        var names = values.Select(v =>  v.Local()).ToArray();

        if (ImGuiHelperRS.SelectableCombo(name, names, ref index))
        {
            value = values[index];
            return true;
        }
        return false;
    }
    public static bool DrawDragFloat2(ConfigUnitType type, string name, ref Vector2 value, string id, string name1, string name2)
    {
        ImGui.Text(name);
        id = "##" + id;
        var result = DrawDragFloat(type, name1 + id, ref value.X);
        result |= DrawDragFloat(type, name2 + id, ref value.Y);
        return result;
    }

    public static bool DrawDragFloat3(ConfigUnitType type, string name, ref Vector3 value, string id, string name1, string name2, string name3, Func<Vector3>? func = null)
    {
        var result = false;
        if (func == null)
        {
            ImGui.Text(name);
        }
        else
        {
            if (ImGui.Button(name + "##" + id))
            {
                value = func();
                result = true;
            }
        }

        id = "##" + id;
        result |= DrawDragFloat(type, name1 + id, ref value.X);
        result |= DrawDragFloat(type, name2 + id, ref value.Y);
        result |= DrawDragFloat(type, name3 + id, ref value.Z);
        return result;
    }

    public static bool DrawDragFloat(ConfigUnitType type, string name, ref float value, string tooltip = "")
    {
        ImGui.SameLine();
        var show = type == ConfigUnitType.Percent ? $"{value * 100:F1}{type.ToSymbol()}" : $"{value:F2}{type.ToSymbol()}";

        ImGui.SetNextItemWidth(Math.Max(50 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(show).X));
        var result = type == ConfigUnitType.Percent ? ImGui.SliderFloat(name, ref value, 0, 1, show) 
            : ImGui.DragFloat(name, ref value, 0.1f, 0, 0, show);
        if (!string.IsNullOrEmpty(tooltip))
        {
            tooltip += "\n";
        }
        ImGuiHelper.HoveredTooltip(tooltip + type.Local());

        return result;
    }

    public static bool DrawDragInt(string name, ref int value)
    {
        ImGui.SameLine();
        ImGui.SetNextItemWidth(Math.Max(50 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(value.ToString()).X));
        return ImGui.DragInt(name, ref value);
    }

    public static bool DrawCondition(ICondition condition, ref int index)
    {
        ImGui.SameLine();

        return ImGuiHelperRS.SelectableCombo($"##Comparation{condition.GetHashCode()}", [">", "<", "="], ref index);
    }

    internal static void SearchItemsReflection(string popId, string name, ref string searchTxt, MemberInfo[] actions, Action<MemberInfo> selectAction)
    {
        ImGuiHelperRS.SearchCombo(popId, name, ref searchTxt, actions, i => LocalManager.Local(i), selectAction, UiString.ConfigWindow_Actions_MemberName.Local());
    }

    public static float IconSize => XIVConfigUI.ConditionConfigs.ConditionDrawer.IconSize;
    private const int count = 8;
    public static void ActionSelectorPopUp(string popUpId, CollapsingHeaderGroup group, ICustomRotation rotation, Action<IAction> action, Action? others = null)
    {
        if (group == null) return;

        using var popUp = ImRaii.Popup(popUpId);

        if (!popUp.Success) return;

        others?.Invoke();

        group.ClearCollapsingHeader();

        foreach (var pair in RotationUpdater.GroupActions(rotation.AllBaseActions.Where(i => i.Action.IsInJob()))!)
        {
            group.AddCollapsingHeader(() => pair.Key, () =>
            {
                var index = 0;
                foreach (var item in pair.OrderBy(t => t.ID))
                {
                    if (!IconSet.GetTexture((ActionID)item.ID, out var icon)) continue;

                    if (index++ % count != 0)
                    {
                        ImGui.SameLine();
                    }

                    using (var group = ImRaii.Group())
                    {
                        var cursor = ImGui.GetCursorPos();
                        if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, item.GetHashCode().ToString()))
                        {
                            action?.Invoke(item);
                            ImGui.CloseCurrentPopup();
                        }
                        ImGuiHelper.DrawActionOverlay(cursor, IconSize, 1);
                    }

                    var name = item.Name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        ImGuiHelper.HoveredTooltip(name);
                    }
                }
            });
        }
        group.Draw();
    }

    #region Draw
    public static void Draw(this ICondition condition, ICustomRotation rotation)
    {
        if (rotation == null)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, UiString.ConfigWindow_Condition_RotationNullWarning.Local());
            return;
        }

        condition.CheckBefore(rotation);

        condition.DrawBefore();

        if (condition is DelayCondition delay) delay.DrawDelay();

        ImGui.SameLine();

        condition.DrawAfter(rotation);
    }

    private static void DrawDelay(this DelayCondition condition)
    {
        const float MIN = 0, MAX = 600;

        ImGui.SetNextItemWidth(80 * ImGuiHelpers.GlobalScale);
        if (ImGui.DragFloatRange2($"##Random Delay {condition.GetHashCode()}", ref condition.DelayMin, ref condition.DelayMax, 0.1f, MIN, MAX,
            $"{condition.DelayMin:F1}{ConfigUnitType.Seconds.ToSymbol()}", $"{condition.DelayMax:F1}{ConfigUnitType.Seconds.ToSymbol()}"))
        {
            condition.DelayMin = Math.Max(Math.Min(condition.DelayMin, condition.DelayMax), MIN);
            condition.DelayMax = Math.Min(Math.Max(condition.DelayMin, condition.DelayMax), MAX);
        }
        ImGuiHelper.HoveredTooltip(UiString.ActionSequencer_Delay_Description.Local() +
            "\n" + ConfigUnitType.Seconds.Local());

        ImGui.SameLine();

        ImGui.SetNextItemWidth(40 * ImGuiHelpers.GlobalScale);
        ImGui.DragFloat($"##Offset Delay {condition.GetHashCode()}", ref condition.DelayOffset, 0.1f, MIN, MAX,
            $"{condition.DelayOffset:F1}{ConfigUnitType.Seconds.ToSymbol()}");

        ImGuiHelper.HoveredTooltip(UiString.ActionSequencer_Offset_Description.Local() +
    "\n" + ConfigUnitType.Seconds.Local());
    }

    private static void DrawBefore(this ICondition condition)
    {
        if (condition is ConditionSet)
        {
            ImGui.BeginGroup();
        }
    }

    private static void DrawAfter(this ICondition condition, ICustomRotation rotation)
    {
        switch (condition)
        {
            case TraitCondition traitCondition:
                traitCondition.DrawAfter(rotation);
                break;

            case ActionCondition actionCondition:
                actionCondition.DrawAfter(rotation);
                break;

            case ConditionSet conditionSet:
                conditionSet.DrawAfter(rotation);
                break;

            case RotationCondition rotationCondition:
                rotationCondition.DrawAfter(rotation);
                break;

            case TargetCondition targetCondition:
                targetCondition.DrawAfter(rotation);
                break;

            case NamedCondition namedCondition:
                namedCondition.DrawAfter(rotation);
                break;

            case TerritoryCondition territoryCondition:
                territoryCondition.DrawAfter(rotation);
                break;
        }
    }

    private static void DrawAfter(this NamedCondition namedCondition, ICustomRotation _)
    {
        ImGuiHelperRS.SearchCombo($"##Comparation{namedCondition.GetHashCode()}", namedCondition.ConditionName, ref searchTxt,
            DataCenter.RightSet.NamedConditions.Select(p => p.Name).ToArray(), i => i.ToString(), i =>
            {
                namedCondition.ConditionName = i;
            }, UiString.ConfigWindow_Condition_ConditionName.Local());

        ImGui.SameLine();
    }

    private static void DrawAfter(this TraitCondition traitCondition, ICustomRotation rotation)
    {
        var name = traitCondition._trait?.Name ?? string.Empty;
        var popUpKey = "Trait Condition Pop Up" + traitCondition.GetHashCode().ToString();

        using (var popUp = ImRaii.Popup(popUpKey))
        {
            if (popUp.Success)
            {
                var index = 0;
                foreach (var trait in rotation.AllTraits)
                {
                    if (!trait.GetTexture(out var traitIcon)) continue;

                    if (index++ % count != 0)
                    {
                        ImGui.SameLine();
                    }

                    using (var group = ImRaii.Group())
                    {
                        if (group.Success)
                        {
                            var cursor = ImGui.GetCursorPos();
                            if (ImGuiHelper.NoPaddingNoColorImageButton(traitIcon.ImGuiHandle, Vector2.One * IconSize, trait.GetHashCode().ToString()))
                            {
                                traitCondition.TraitID = trait.ID;
                                ImGui.CloseCurrentPopup();
                            }
                            ImGuiHelper.DrawActionOverlay(cursor, IconSize, -1);
                        }
                    }

                    var tooltip = trait.Name;
                    if (!string.IsNullOrEmpty(tooltip))
                    {
                        ImGuiHelper.HoveredTooltip(tooltip);
                    }
                }
            }
        }

        if (traitCondition._trait?.GetTexture(out var icon) ?? false || ImageLoader.GetTexture(4, out icon))
        {
            var cursor = ImGui.GetCursorPos();
            if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, traitCondition.GetHashCode().ToString()))
            {
                if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
            }
            ImGuiHelper.DrawActionOverlay(cursor, IconSize, -1);
            ImGuiHelper.HoveredTooltip(name);
        }

        ImGui.SameLine();
        var i = 0;
        ImGuiHelperRS.SelectableCombo($"##Category{traitCondition.GetHashCode()}",
        [
            UiString.ActionConditionType_EnoughLevel.Local()
        ], ref i);
        ImGui.SameLine();
    }

    private static readonly CollapsingHeaderGroup _actionsList = new()
    {
        HeaderSize = FontSize.Fifth,
    };

    static string searchTxt = string.Empty;

    private static void DrawAfter(this ActionCondition actionCondition, ICustomRotation rotation)
    {
        var name = actionCondition._action?.Name ?? string.Empty;
        var popUpKey = "Action Condition Pop Up" + actionCondition.GetHashCode().ToString();

        ActionSelectorPopUp(popUpKey, _actionsList, rotation, item => actionCondition.ID = (ActionID)item.ID);

        if ((actionCondition._action?.GetTexture(out var icon) ?? false) || ImageLoader.GetTexture(4, out icon))
        {
            var cursor = ImGui.GetCursorPos();
            if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, actionCondition.GetHashCode().ToString()))
            {
                if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
            }
            ImGuiHelper.DrawActionOverlay(cursor, IconSize, 1);
            ImGuiHelper.HoveredTooltip(name);
        }

        ImGui.SameLine();

        DrawByteEnum($"##Category{actionCondition.GetHashCode()}", ref actionCondition.ActionConditionType);

        switch (actionCondition.ActionConditionType)
        {
            case ActionConditionType.Elapsed:
            case ActionConditionType.Remain:
                DrawDragFloat(ConfigUnitType.Seconds, $"##Seconds{actionCondition.GetHashCode()}", ref actionCondition.Time);
                break;

            case ActionConditionType.ElapsedGCD:
            case ActionConditionType.RemainGCD:
                if (DrawDragInt($"GCD##GCD{actionCondition.GetHashCode()}", ref actionCondition.Param1))
                {
                    actionCondition.Param1 = Math.Max(0, actionCondition.Param1);
                }
                if (DrawDragInt($"{UiString.ActionSequencer_TimeOffset.Local()}##Ability{actionCondition.GetHashCode()}", ref actionCondition.Param2))
                {
                    actionCondition.Param2 = Math.Max(0, actionCondition.Param2);
                }
                break;

            case ActionConditionType.CanUse:
                var popUpId = "Can Use Id" + actionCondition.GetHashCode().ToString();
                var option = (CanUseOption)actionCondition.Param1;

                if (ImGui.Selectable($"{option}##CanUse{actionCondition.GetHashCode()}"))
                {
                    if (!ImGui.IsPopupOpen(popUpId)) ImGui.OpenPopup(popUpId);
                }

                using (var popUp = ImRaii.Popup(popUpId))
                {
                    if (popUp.Success)
                    {
                        var showedValues = Enum.GetValues<CanUseOption>().Where(i => i.GetAttribute<JsonIgnoreAttribute>() == null);

                        foreach (var value in showedValues)
                        {
                            var b = option.HasFlag(value);
                            if (ImGui.Checkbox(value.Local(), ref b))
                            {
                                option ^= value;
                                actionCondition.Param1 = (int)option;
                            }
                        }
                    }
                }
                break;

            case ActionConditionType.CurrentCharges:
            case ActionConditionType.MaxCharges:
                DrawCondition(actionCondition, ref actionCondition.Param2);

                if (DrawDragInt($"{UiString.ActionSequencer_Charges.Local()}##Charges{actionCondition.GetHashCode()}", ref actionCondition.Param1))
                {
                    actionCondition.Param1 = Math.Max(0, actionCondition.Param1);
                }
                break;
        }
    }

    private static void DrawAfter(this ConditionSet conditionSet, ICustomRotation rotation)
    {
        AddButton();

        ImGui.SameLine();

        DrawByteEnum($"##Rule{conditionSet.GetHashCode()}", ref conditionSet.Type);

        ImGui.Spacing();

        for (int i = 0; i < conditionSet.Conditions.Count; i++)
        {
            var condition = conditionSet.Conditions[i];

            void Delete()
            {
                conditionSet.Conditions.RemoveAt(i);
            }

            void Up()
            {
                conditionSet.Conditions.RemoveAt(i);
                conditionSet.Conditions.Insert(Math.Max(0, i - 1), condition);
            }

            void Down()
            {
                conditionSet.Conditions.RemoveAt(i);
                conditionSet.Conditions.Insert(Math.Min(conditionSet.Conditions.Count, i + 1), condition);
            }

            void Copy()
            {
                var str = JsonConvert.SerializeObject(conditionSet.Conditions[i], Formatting.Indented);
                ImGui.SetClipboardText(str);
            }

            var key = $"Condition Pop Up: {condition.GetHashCode()}";

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                (LocalString.Remove.Local(), Delete, ["Delete"]),
                (LocalString.MoveUp.Local(), Up, ["↑"]),
                (LocalString.MoveDown.Local(), Down, ["↓"]),
                (LocalString.CopyToClipboard.Local(), Copy, ["Ctrl"]));

            if (condition is DelayCondition delay)
            {
                DrawCondition(delay.IsTrue(rotation), delay.GetHashCode().ToString(), ref delay.Not);
            }
            else
            {
                DrawCondition(condition.IsTrue(rotation));
            }

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                (Delete, [VirtualKey.DELETE]),
                (Up, [VirtualKey.UP]),
                (Down, [VirtualKey.DOWN]),
                (Copy, [VirtualKey.CONTROL]));

            ImGui.SameLine();

            condition.Draw(rotation);
        }

        ImGui.EndGroup();

        void AddButton()
        {
            if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddButton" + conditionSet.GetHashCode().ToString()))
            {
                ImGui.OpenPopup("Popup" + conditionSet.GetHashCode().ToString());
            }

            using var popUp = ImRaii.Popup("Popup" + conditionSet.GetHashCode().ToString());
            if (popUp)
            {
                AddOneCondition<ConditionSet>();
                AddOneCondition<ActionCondition>();
                AddOneCondition<TraitCondition>();
                AddOneCondition<TargetCondition>();
                AddOneCondition<RotationCondition>();
                AddOneCondition<NamedCondition>();
                AddOneCondition<TerritoryCondition>();
                if (ImGui.Selectable(LocalString.FromClipboard.Local()))
                {
                    var str = ImGui.GetClipboardText();
                    try
                    {
                        var set = JsonConvert.DeserializeObject<ICondition>(str, new IConditionConverter())!;
                        conditionSet.Conditions.Add(set);
                    }
                    catch (Exception ex)
                    {
                        Svc.Log.Warning(ex, "Failed to load the condition.");
                    }
                    ImGui.CloseCurrentPopup();
                }
            }

            void AddOneCondition<T>() where T : ICondition
            {
                if (ImGui.Selectable(typeof(T).Local()))
                {
                    conditionSet.Conditions.Add(Activator.CreateInstance<T>());
                    ImGui.CloseCurrentPopup();
                }
            }
        }
    }

    private static void DrawAfter(this RotationCondition rotationCondition, ICustomRotation rotation)
    {
        DrawByteEnum($"##Category{rotationCondition.GetHashCode()}", ref rotationCondition.ComboConditionType);

        switch (rotationCondition.ComboConditionType)
        {
            case ComboConditionType.Bool:
                ImGui.SameLine();
                SearchItemsReflection($"##Comparation{rotationCondition.GetHashCode()}", rotationCondition._prop?.Local() ?? "No Property", ref searchTxt, rotation.AllBools, i =>
                {
                    rotationCondition._prop = (PropertyInfo)i;
                    rotationCondition.PropertyName = i.Name;
                });

                break;

            case ComboConditionType.Integer:
                ImGui.SameLine();
                SearchItemsReflection($"##ByteChoice{rotationCondition.GetHashCode()}", rotationCondition._prop?.Local() ?? "No Property", ref searchTxt, rotation.AllBytesOrInt, i =>
                {
                    rotationCondition._prop = (PropertyInfo)i;
                    rotationCondition.PropertyName = i.Name;
                });

                DrawCondition(rotationCondition, ref rotationCondition.Condition);

                DrawDragInt($"##Value{rotationCondition.GetHashCode()}", ref rotationCondition.Param1);

                break;

            case ComboConditionType.Float:
                ImGui.SameLine();
                SearchItemsReflection($"##FloatChoice{rotationCondition.GetHashCode()}", rotationCondition._prop?.Local() ?? "No Property", ref searchTxt, rotation.AllFloats, i =>
                {
                    rotationCondition._prop = (PropertyInfo)i;
                    rotationCondition.PropertyName = i.Name;
                });

                DrawCondition(rotationCondition, ref rotationCondition.Condition);

                DrawDragFloat(ConfigUnitType.None, $"##Value{rotationCondition.GetHashCode()}", ref rotationCondition.Param2);
                break;

            case ComboConditionType.Last:
                ImGui.SameLine();

                var names = new string[]
                    {
                        nameof(CustomRotation.IsLastGCD),
                        nameof(CustomRotation.IsLastAction),
                        nameof(CustomRotation.IsLastAbility),
                    };
                var index = Math.Max(0, Array.IndexOf(names, rotationCondition.MethodName));
                if (ImGuiHelperRS.SelectableCombo($"##Last{rotationCondition.GetHashCode()}", names, ref index))
                {
                    rotationCondition.MethodName = names[index];
                }

                ImGui.SameLine();

                var name = rotationCondition._action?.Name ?? string.Empty;

                var popUpKey = "Rotation Condition Pop Up" + rotationCondition.GetHashCode().ToString();

                ActionSelectorPopUp(popUpKey, _actionsList, rotation, item => rotationCondition.ID = (ActionID)item.ID);

                if (rotationCondition._action?.GetTexture(out var icon) ?? false || ImageLoader.GetTexture(4, out icon))
                {
                    var cursor = ImGui.GetCursorPos();
                    if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, rotationCondition.GetHashCode().ToString()))
                    {
                        if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
                    }
                    ImGuiHelper.DrawActionOverlay(cursor, IconSize, 1);
                }

                ImGui.SameLine();
                ImGuiHelperRS.SelectableCombo($"##Adjust{rotationCondition.GetHashCode()}",
                [
                    UiString.ActionSequencer_Original.Local(),
                    UiString.ActionSequencer_Adjusted.Local(),
                ], ref rotationCondition.Param1);
                break;
        }
    }

    private static Status[]? _allStatus = null;
    private static Status[] AllStatus => _allStatus ??= Enum.GetValues<StatusID>()
        .Select(id => Service.GetSheet<Status>().GetRow((uint)id)).ToArray()!;

    private static void DrawAfter(this TargetCondition targetCondition, ICustomRotation rotation)
    {
        DelayCondition.CheckBaseAction(rotation, targetCondition.ID, ref targetCondition._action);

        if (targetCondition.StatusId != StatusID.None &&
            (targetCondition.Status == null || targetCondition.Status.RowId != (uint)targetCondition.StatusId))
        {
            targetCondition.Status = AllStatus.FirstOrDefault(a => a.RowId == (uint)targetCondition.StatusId);
        }

        var popUpKey = "Target Condition Pop Up" + targetCondition.GetHashCode().ToString();

        ActionSelectorPopUp(popUpKey, _actionsList, rotation, item => targetCondition.ID = (ActionID)item.ID, () =>
        {
            if (ImGui.Selectable(TargetType.HostileTarget.Local()))
            {
                targetCondition._action = null;
                targetCondition.ID = ActionID.None;
                targetCondition.TargetType = TargetType.HostileTarget;
            }

            if (ImGui.Selectable(TargetType.Target.Local()))
            {
                targetCondition._action = null;
                targetCondition.ID = ActionID.None;
                targetCondition.TargetType = TargetType.Target;
            }

            if (ImGui.Selectable(TargetType.Player.Local()))
            {
                targetCondition._action = null;
                targetCondition.ID = ActionID.None;
                targetCondition.TargetType = TargetType.Player;
            }
        });

        if (targetCondition._action != null ? targetCondition._action.GetTexture(out var icon) || ImageLoader.GetTexture(4, out icon)
            : ImageLoader.GetTexture(targetCondition.TargetType switch
            {
                TargetType.Target => 16u,
                TargetType.HostileTarget => 15u,
                TargetType.Player => 18u,
                _ => 0,
            }, out icon))
        {
            var cursor = ImGui.GetCursorPos();
            if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, targetCondition.GetHashCode().ToString()))
            {
                if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
            }
            ImGuiHelper.DrawActionOverlay(cursor, IconSize, 1);

            var description = targetCondition._action != null ? string.Format(UiString.ActionSequencer_ActionTarget.Local(), targetCondition._action.Name)
                : targetCondition.TargetType.Local();
            ImGuiHelper.HoveredTooltip(description);
        }

        ImGui.SameLine();
        DrawByteEnum($"##Category{targetCondition.GetHashCode()}", ref targetCondition.TargetConditionType);

        var popupId = "Status Finding Popup" + targetCondition.GetHashCode().ToString();

        RotationConfigWindow.StatusPopUp(popupId, AllStatus, ref searchTxt, status =>
        {
            targetCondition.Status = status;
            targetCondition.StatusId = (StatusID)targetCondition.Status.RowId;
        }, size: IconSize);

        void DrawStatusIcon()
        {
            if (ImageLoader.GetTexture(targetCondition.Status?.Icon ?? 16220, out var icon)
                || ImageLoader.GetTexture(16220, out icon))
            {
                if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(IconSize * 3 / 4, IconSize) * ImGuiHelpers.GlobalScale, targetCondition.GetHashCode().ToString()))
                {
                    if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
                }
                ImGuiHelper.HoveredTooltip(targetCondition.Status?.Name ?? string.Empty);
            }
        }

        switch (targetCondition.TargetConditionType)
        {
            case TargetConditionType.HasStatus:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                var check = targetCondition.FromSelf ? 1 : 0;
                if (ImGuiHelperRS.SelectableCombo($"From Self {targetCondition.GetHashCode()}",
                [
                    UiString.ActionSequencer_StatusAll.Local(),
                    UiString.ActionSequencer_StatusSelf.Local(),
                ], ref check))
                {
                    targetCondition.FromSelf = check != 0;
                }
                break;

            case TargetConditionType.StatusEnd:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                check = targetCondition.FromSelf ? 1 : 0;
                if (ImGuiHelperRS.SelectableCombo($"From Self {targetCondition.GetHashCode()}",
                [
                    UiString.ActionSequencer_StatusAll.Local(),
                    UiString.ActionSequencer_StatusSelf.Local(),
                ], ref check))
                {
                    targetCondition.FromSelf = check != 0;
                }

                DrawCondition(targetCondition, ref targetCondition.Param2);

                DrawDragFloat(ConfigUnitType.Seconds, $"s##Seconds{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime);
                break;

            case TargetConditionType.StatusEndGCD:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                check = targetCondition.FromSelf ? 1 : 0;
                if (ImGuiHelperRS.SelectableCombo($"From Self {targetCondition.GetHashCode()}",
                [
                    UiString.ActionSequencer_StatusAll.Local(),
                    UiString.ActionSequencer_StatusSelf.Local(),
                ], ref check))
                {
                    targetCondition.FromSelf = check != 0;
                }

                DrawDragInt($"GCD##GCD{targetCondition.GetHashCode()}", ref targetCondition.GCD);
                DrawDragFloat(ConfigUnitType.Seconds, $"{UiString.ActionSequencer_TimeOffset.Local()}##Ability{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime);
                break;

            case TargetConditionType.Distance:
                DrawCondition(targetCondition, ref targetCondition.Param2);

                if (DrawDragFloat(ConfigUnitType.Yalms, $"##yalm{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime))
                {
                    targetCondition.DistanceOrTime = Math.Max(0, targetCondition.DistanceOrTime);
                }
                break;

            case TargetConditionType.CastingAction:
                ImGui.SameLine();
                ImGuiHelperRS.SetNextWidthWithName(targetCondition.CastingActionName);
                ImGui.InputText($"Ability Name##CastingActionName{targetCondition.GetHashCode()}", ref targetCondition.CastingActionName, 128);
                break;

            case TargetConditionType.CastingActionTime:
                DrawCondition(targetCondition, ref targetCondition.Param2);
                DrawDragFloat(ConfigUnitType.Seconds, $"##CastingActionTimeUntil{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime);
                break;

            case TargetConditionType.HPRatio:
                DrawCondition(targetCondition, ref targetCondition.Param2);

                DrawDragFloat(ConfigUnitType.Percent, $"##HPRatio{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime);
                break;

            case TargetConditionType.MP:
            case TargetConditionType.HP:
                DrawCondition(targetCondition, ref targetCondition.Param2);

                DrawDragInt($"##HPorMP{targetCondition.GetHashCode()}", ref targetCondition.GCD);
                break;

            case TargetConditionType.TimeToKill:
                DrawCondition(targetCondition, ref targetCondition.Param2);

                DrawDragFloat(ConfigUnitType.Seconds, $"##TimeToKill{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime);
                break;

            case TargetConditionType.TargetName:
                ImGui.SameLine();
                ImGuiHelperRS.SetNextWidthWithName(targetCondition.CastingActionName);
                ImGui.InputText($"Name##TargetName{targetCondition.GetHashCode()}", ref targetCondition.CastingActionName, 128);
                break;
        }

        if (targetCondition._action == null && targetCondition.TargetType == TargetType.Target)
        {
            using var style = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
            ImGui.TextWrapped(UiString.ConfigWindow_Condition_TargetWarning.Local());
        }
    }

    private static string[]? _territoryNames = null;
    public static string[] TerritoryNames => _territoryNames ??= Service.GetSheet<TerritoryType>()?
        .Select(t => t?.PlaceName?.Value?.Name?.RawString ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToArray()!;

    private static string[]? _dutyNames = null;

    public static string[] DutyNames => _dutyNames ??= new HashSet<string>(Service.GetSheet<ContentFinderCondition>()?
        .Select(t => t?.Name?.RawString ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).Reverse()!).ToArray();

    private static void DrawAfter(this TerritoryCondition territoryCondition, ICustomRotation _)
    {
        DrawByteEnum($"##Category{territoryCondition.GetHashCode()}", ref territoryCondition.TerritoryConditionType);

        switch (territoryCondition.TerritoryConditionType)
        {
            case TerritoryConditionType.TerritoryContentType:
                ImGui.SameLine();

                var type = (TerritoryContentType)territoryCondition.TerritoryId;
                DrawByteEnum($"##TerritoryContentType{territoryCondition.GetHashCode()}", ref type);
                territoryCondition.TerritoryId = (int)type;
                break;

            case TerritoryConditionType.TerritoryName:
                ImGui.SameLine();

                ImGuiHelperRS.SearchCombo($"##TerritoryName{territoryCondition.GetHashCode()}", territoryCondition.Name, ref searchTxt,
                TerritoryNames, i => i.ToString(), i =>
                {
                    territoryCondition.Name = i;
                }, UiString.ConfigWindow_Condition_TerritoryName.Local());
                break;

            case TerritoryConditionType.DutyName:
                ImGui.SameLine();

                ImGuiHelperRS.SearchCombo($"##DutyName{territoryCondition.GetHashCode()}", territoryCondition.Name, ref searchTxt,
                DutyNames, i => i.ToString(), i =>
                {
                    territoryCondition.Name = i;
                }, UiString.ConfigWindow_Condition_DutyName.Local());
                break;
        }
    }
    #endregion
}
