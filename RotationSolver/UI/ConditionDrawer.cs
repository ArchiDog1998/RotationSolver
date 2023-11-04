using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration.Conditions;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using System.Xml.Linq;
using Action = System.Action;

namespace RotationSolver.UI;

internal static class ConditionDrawer
{
    internal static void DrawMain(this ConditionSet conditionSet, ICustomRotation rotation)
    {
        if (conditionSet == null) return;

        DrawCondition(conditionSet.IsTrue(rotation));
        ImGui.SameLine();
        conditionSet.Draw(rotation);
    }

    internal static void DrawCondition(bool? tag)
    {
        float size = IconSize * (1 + 8 / 82);

        if (!tag.HasValue)
        {
            if (IconSet.GetTexture("ui/uld/image2.tex", out var texture, true) || IconSet.GetTexture(0u, out texture))
            {
                ImGui.Image(texture.ImGuiHandle, Vector2.One * size);
            }
        }
        else
        {
            if (IconSet.GetTexture("ui/uld/readycheck_hr1.tex", out var texture, true))
            {
                ImGui.Image(texture.ImGuiHandle, Vector2.One * size,
                    new Vector2(tag.Value ? 0 : 0.5f, 0),
                    new Vector2(tag.Value ? 0.5f : 1, 1));
            }
        }
    }

    public static void CheckMemberInfo<T>(ICustomRotation rotation, ref string name, ref T value) where T : MemberInfo
    {
        if (!string.IsNullOrEmpty(name) && (value == null || value.Name != name))
        {
            var memberName = name;
            if (typeof(T).IsAssignableFrom(typeof(PropertyInfo)))
            {
                value = (T)rotation.GetType().GetAllMethods(RuntimeReflectionExtensions.GetRuntimeProperties).FirstOrDefault(m => m.Name == memberName);
            }
            else if (typeof(T).IsAssignableFrom(typeof(MethodInfo)))
            {
                value = (T)rotation.GetType().GetAllMethods(RuntimeReflectionExtensions.GetRuntimeMethods).FirstOrDefault(m => m.Name == memberName);
            }
        }
    }

    private static IEnumerable<MemberInfo> GetAllMethods(this Type type, Func<Type, IEnumerable<MemberInfo>> getFunc)
    {
        if (type == null || getFunc == null) return Array.Empty<MemberInfo>();

        var methods = getFunc(type);
        return methods.Union(type.BaseType.GetAllMethods(getFunc));
    }

    public static void DrawByteEnum<T>(string name, ref T value, Func<T, string> function) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        var index = Array.IndexOf(values, value);
        var names = values.Select(function).ToArray();

        if (ImGuiHelper.SelectableCombo(name, names, ref index))
        {
            value = values[index];
        }
    }

    public static bool DrawDragFloat(string name, ref float value)
    {
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        return ImGui.DragFloat(name, ref value);
    }

    public static bool DrawDragInt(string name, ref int value)
    {
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        return ImGui.DragInt(name, ref value);
    }

    public static bool DrawCheckBox(string name, ref int value, string desc = "")
    {
        ImGui.SameLine();

        var @bool = value != 0;

        var result = false;
        if (ImGui.Checkbox(name, ref @bool))
        {
            value = @bool ? 1 : 0;
            result = true;
        }

        ImguiTooltips.HoveredTooltip(desc);

        return result;
    }

    internal static void SearchItemsReflection<T>(string popId, string name, ref string searchTxt, T[] actions, Action<T> selectAction) where T : MemberInfo
    {
        SearchItems(popId, name, ref searchTxt, actions, ImGuiHelper.GetMemberName, selectAction, LocalizationManager.RightLang.ConfigWindow_Actions_MemberName);
    }

    internal static void SearchItems<T>(string popId, string name, ref string searchTxt, T[] items, Func<T, string> getSearchName, Action<T> selectAction, string searchingHint)
    {
        if (ImGuiHelper.SelectableButton(name + "##" + popId))
        {
            if (!ImGui.IsPopupOpen(popId)) ImGui.OpenPopup(popId);
        }

        using var popUp = ImRaii.Popup(popId);
        if (!popUp.Success) return;

        if (items == null || items.Length == 0)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, LocalizationManager.RightLang.ConfigWindow_Condition_NoItemsWarning);
            return;
        }

        var searchingKey = searchTxt;

        var members = items.Select(m => (m, getSearchName(m)))
            .OrderByDescending(s => RotationConfigWindow.Similarity(s.Item2, searchingKey));

        ImGui.SetNextItemWidth(Math.Max(50 * ImGuiHelpers.GlobalScale, members.Max(i => ImGuiHelpers.GetButtonSize(i.Item2).X)));
        ImGui.InputTextWithHint("##Searching the member", searchingHint, ref searchTxt, 128);

        ImGui.Spacing();

        ImRaii.IEndObject child = null;
        if (members.Count() >= 15)
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(0, 300), new Vector2(500, 300));
            child = ImRaii.Child(popId);
            if (!child) return;
        }

        foreach (var member in members)
        {
            if (ImGui.Selectable(member.Item2))
            {
                selectAction?.Invoke(member.m);
                ImGui.CloseCurrentPopup();
            }
        }
        child?.Dispose();
    }

    public static float IconSizeRaw => ImGuiHelpers.GetButtonSize("H").Y;
    public static float IconSize => IconSizeRaw * ImGuiHelpers.GlobalScale;
    private const int count = 8;
    public static void ActionSelectorPopUp(string popUpId, CollapsingHeaderGroup group, ICustomRotation rotation, Action<IAction> action, Action others = null)
    {
        if (group == null) return;

        using var popUp = ImRaii.Popup(popUpId);

        if (!popUp.Success) return;

        others?.Invoke();

        group.ClearCollapsingHeader();

        foreach (var pair in RotationUpdater.GroupActions(rotation.AllBaseActions))
        {
            group.AddCollapsingHeader(() => pair.Key, () =>
            {
                var index = 0;
                foreach (var item in pair.OrderBy(t => t.ID))
                {
                    if (!item.GetTexture(out var icon)) continue;

                    if (index++ % count != 0)
                    {
                        ImGui.SameLine();
                    }

                    using (var group = ImRaii.Group())
                    {
                        var cursor = ImGui.GetCursorPos();
                        if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, group.GetHashCode().ToString()))
                        {
                            action?.Invoke(item);
                            ImGui.CloseCurrentPopup();
                        }
                        ImGuiHelper.DrawActionOverlay(cursor, IconSize, 1);
                    }

                    var name = item.Name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        ImguiTooltips.HoveredTooltip(name);
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
            ImGui.TextColored(ImGuiColors.DalamudRed, LocalizationManager.RightLang.ConfigWindow_Condition_RotationNullWarning);
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
        const float MIN = 0, MAX = 60;

        ImGui.SetNextItemWidth(80 * ImGuiHelpers.GlobalScale);
        if (ImGui.DragFloatRange2($"##Random Delay {condition.GetHashCode()}", ref condition.DelayMin, ref condition.DelayMax, 0.1f, MIN, MAX))
        {
            condition.DelayMin = Math.Max(Math.Min(condition.DelayMin, condition.DelayMax), MIN);
            condition.DelayMax = Math.Min(Math.Max(condition.DelayMin, condition.DelayMax), MAX);
        }
        ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ActionSequencer_Delay_Description);
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
        SearchItems($"##Comparation{namedCondition.GetHashCode()}", namedCondition.ConditionName, ref searchTxt,
            DataCenter.RightSet.NamedConditions.Select(p => p.Name).ToArray(), i => i.ToString(), i =>
            {
                namedCondition.ConditionName = i;
            }, LocalizationManager.RightLang.ConfigWindow_Condition_ConditionName);

        ImGui.SameLine();

        ImGuiHelper.SelectableCombo($"##IsOrNot{namedCondition.GetHashCode()}", new string[]
        {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
        }, ref namedCondition.Condition);
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
                        ImguiTooltips.HoveredTooltip(tooltip);
                    }
                }
            }
        }

        if (traitCondition._trait?.GetTexture(out var icon) ?? false || IconSet.GetTexture(4, out icon))
        {
            var cursor = ImGui.GetCursorPos();
            if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, traitCondition.GetHashCode().ToString()))
            {
                if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
            }
            ImGuiHelper.DrawActionOverlay(cursor, IconSize, -1);
            ImguiTooltips.HoveredTooltip(name);
        }

        ImGui.SameLine();
        var i = 0;
        ImGuiHelper.SelectableCombo($"##Category{traitCondition.GetHashCode()}", new string[]
        {
            LocalizationManager.RightLang.ActionConditionType_EnoughLevel
        }, ref i);
        ImGui.SameLine();

        var condition = traitCondition.Condition ? 1 : 0;
        if (ImGuiHelper.SelectableCombo($"##Comparation{traitCondition.GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                }, ref condition))
        {
            traitCondition.Condition = condition > 0;
        }
    }

    private static readonly CollapsingHeaderGroup _actionsList = new()
    {
        HeaderSize = 12,
    };

    static string searchTxt = string.Empty;

    private static void DrawAfter(this ActionCondition actionCondition, ICustomRotation rotation)
    {
        var name = actionCondition._action?.Name ?? string.Empty;
        var popUpKey = "Action Condition Pop Up" + actionCondition.GetHashCode().ToString();

        ActionSelectorPopUp(popUpKey, _actionsList, rotation, item => actionCondition.ID = (ActionID)item.ID);

        if (actionCondition._action?.GetTexture(out var icon) ?? false || IconSet.GetTexture(4, out icon))
        {
            var cursor = ImGui.GetCursorPos();
            if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, actionCondition.GetHashCode().ToString()))
            {
                if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
            }
            ImGuiHelper.DrawActionOverlay(cursor, IconSize, 1);
            ImguiTooltips.HoveredTooltip(name);
        }

        ImGui.SameLine();

        DrawByteEnum($"##Category{actionCondition.GetHashCode()}", ref actionCondition.ActionConditionType, EnumTranslations.ToName);

        var condition = actionCondition.Condition ? 1 : 0;
        var combos = Array.Empty<string>();
        switch (actionCondition.ActionConditionType)
        {
            case ActionConditionType.ElapsedGCD:
            case ActionConditionType.RemainGCD:
            case ActionConditionType.Elapsed:
            case ActionConditionType.Remain:
            case ActionConditionType.CurrentCharges:
            case ActionConditionType.MaxCharges:
                combos = new string[] { ">", "<=" };
                break;

            case ActionConditionType.CanUse:
                combos = new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Can,
                    LocalizationManager.RightLang.ActionSequencer_Cannot,
                };
                break;

            case ActionConditionType.EnoughLevel:
            case ActionConditionType.IsCoolDown:
                combos = new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                };
                break;
        }
        ImGui.SameLine();

        if (ImGuiHelper.SelectableCombo($"##Comparation{actionCondition.GetHashCode()}", combos, ref condition))
        {
            actionCondition.Condition = condition > 0;
        }

        switch (actionCondition.ActionConditionType)
        {
            case ActionConditionType.Elapsed:
            case ActionConditionType.Remain:
                DrawDragFloat($"s##Seconds{actionCondition.GetHashCode()}", ref actionCondition.Time);
                break;

            case ActionConditionType.ElapsedGCD:
            case ActionConditionType.RemainGCD:
                if (DrawDragInt($"GCD##GCD{actionCondition.GetHashCode()}", ref actionCondition.Param1))
                {
                    actionCondition.Param1 = Math.Max(0, actionCondition.Param1);
                }
                if (DrawDragInt($"{LocalizationManager.RightLang.ActionSequencer_TimeOffset}##Ability{actionCondition.GetHashCode()}", ref actionCondition.Param2))
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
                            if (ImGui.Checkbox(value.ToString(), ref b))
                            {
                                option ^= value;
                                actionCondition.Param1 = (int)option;
                            }
                        }
                    }
                }

                if (DrawDragInt($"{LocalizationManager.RightLang.ActionSequencer_AOECount}##AOECount{actionCondition.GetHashCode()}", ref actionCondition.Param2))
                {
                    actionCondition.Param2 = Math.Max(0, actionCondition.Param2);
                }
                break;

            case ActionConditionType.CurrentCharges:
            case ActionConditionType.MaxCharges:
                if (DrawDragInt($"{LocalizationManager.RightLang.ActionSequencer_Charges}##Charges{actionCondition.GetHashCode()}", ref actionCondition.Param1))
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

        DrawByteEnum($"##Rule{conditionSet.GetHashCode()}", ref conditionSet.Type, t => t switch
        {
            LogicalType.And => "&&",
            LogicalType.Or => " | | ",
            LogicalType.NotAnd => "! &&",
            LogicalType.NotOr => "!  | | ",
            _ => string.Empty,
        });

        ImGui.Spacing();

        for (int i = 0; i < conditionSet.Conditions.Count; i++)
        {
            var condition = conditionSet.Conditions[i];

            void Delete()
            {
                conditionSet.Conditions.RemoveAt(i);
            };

            void Up()
            {
                conditionSet.Conditions.RemoveAt(i);
                conditionSet.Conditions.Insert(Math.Max(0, i - 1), condition);
            };

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
                (LocalizationManager.RightLang.ConfigWindow_List_Remove, Delete, new string[] { "Delete" }),
                (LocalizationManager.RightLang.ConfigWindow_Actions_MoveUp, Up, new string[] { "↑" }),
                (LocalizationManager.RightLang.ConfigWindow_Actions_MoveDown, Down, new string[] { "↓" }),
                (LocalizationManager.RightLang.ConfigWindow_Actions_Copy, Copy, new string[] { "Ctrl" }));

            DrawCondition(condition.IsTrue(rotation));

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                (Delete, new VirtualKey[] { VirtualKey.DELETE }),
                (Up, new VirtualKey[] { VirtualKey.UP }),
                (Down, new VirtualKey[] { VirtualKey.DOWN }),
                (Copy, new VirtualKey[] { VirtualKey.CONTROL }));
            
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
                AddOneCondition<ConditionSet>(LocalizationManager.RightLang.ActionSequencer_ConditionSet);
                AddOneCondition<ActionCondition>(LocalizationManager.RightLang.ActionSequencer_ActionCondition);
                AddOneCondition<TraitCondition>(LocalizationManager.RightLang.ActionSequencer_TraitCondition);
                AddOneCondition<TargetCondition>(LocalizationManager.RightLang.ActionSequencer_TargetCondition);
                AddOneCondition<RotationCondition>(LocalizationManager.RightLang.ActionSequencer_RotationCondition);
                AddOneCondition<NamedCondition>(LocalizationManager.RightLang.ActionSequencer_NamedCondition);
                AddOneCondition<TerritoryCondition>(LocalizationManager.RightLang.ActionSequencer_TerritoryCondition);
                if (ImGui.Selectable(LocalizationManager.RightLang.ActionSequencer_FromClipboard))
                {
                    var str = ImGui.GetClipboardText();
                    try
                    {
                        var set = JsonConvert.DeserializeObject<ICondition>(str, new IConditionConverter());
                        conditionSet.Conditions.Add(set);
                    }
                    catch (Exception ex)
                    {
                        Svc.Log.Warning(ex, "Failed to load the condition.");
                    }
                    ImGui.CloseCurrentPopup();
                }
            }

            void AddOneCondition<T>(string name) where T : ICondition
            {
                if (ImGui.Selectable(name))
                {
                    conditionSet.Conditions.Add(Activator.CreateInstance<T>());
                    ImGui.CloseCurrentPopup();
                }
            }
        }
    }

    private static void DrawAfter(this RotationCondition rotationCondition, ICustomRotation rotation)
    {
        DrawByteEnum($"##Category{rotationCondition.GetHashCode()}", ref rotationCondition.ComboConditionType, EnumTranslations.ToName);

        switch (rotationCondition.ComboConditionType)
        {
            case ComboConditionType.Bool:
                ImGui.SameLine();
                SearchItemsReflection($"##Comparation{rotationCondition.GetHashCode()}", rotationCondition._prop.GetMemberName(), ref searchTxt, rotation.AllBools, i =>
                {
                    rotationCondition._prop = i;
                    rotationCondition.PropertyName = i.Name;
                });
                ImGui.SameLine();

                ImGuiHelper.SelectableCombo($"##IsOrNot{rotationCondition.GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                }, ref rotationCondition.Condition);

                break;

            case ComboConditionType.Integer:
                ImGui.SameLine();
                SearchItemsReflection($"##ByteChoice{rotationCondition.GetHashCode()}", rotationCondition._prop.GetMemberName(), ref searchTxt, rotation.AllBytesOrInt, i =>
                {
                    rotationCondition._prop = i;
                    rotationCondition.PropertyName = i.Name;
                });

                ImGui.SameLine();

                ImGuiHelper.SelectableCombo($"##Comparation{rotationCondition.GetHashCode()}", new string[] { ">", "<", "=" }, ref rotationCondition.Condition);

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ImGui.DragInt($"##Value{rotationCondition.GetHashCode()}", ref rotationCondition.Param1);

                break;
            case ComboConditionType.Float:
                ImGui.SameLine();
                SearchItemsReflection($"##FloatChoice{rotationCondition.GetHashCode()}", rotationCondition._prop.GetMemberName(), ref searchTxt, rotation.AllFloats, i =>
                {
                    rotationCondition._prop = i;
                    rotationCondition.PropertyName = i.Name;
                });

                ImGui.SameLine();
                ImGuiHelper.SelectableCombo($"##Comparation{rotationCondition.GetHashCode()}", new string[] { ">", "<", "=" }, ref rotationCondition.Condition);

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ImGui.DragFloat($"##Value{rotationCondition.GetHashCode()}", ref rotationCondition.Param2);

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
                if (ImGuiHelper.SelectableCombo($"##Last{rotationCondition.GetHashCode()}", names, ref index))
                {
                    rotationCondition.MethodName = names[index];
                }

                ImGui.SameLine();

                ImGuiHelper.SelectableCombo($"##IsNot{rotationCondition.GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                }, ref rotationCondition.Condition);

                ImGui.SameLine();

                var name = rotationCondition._action?.Name ?? string.Empty;

                var popUpKey = "Rotation Condition Pop Up" + rotationCondition.GetHashCode().ToString();

                ActionSelectorPopUp(popUpKey, _actionsList, rotation, item => rotationCondition.ID = (ActionID)item.ID);

                if (rotationCondition._action?.GetTexture(out var icon) ?? false || IconSet.GetTexture(4, out icon))
                {
                    var cursor = ImGui.GetCursorPos();
                    if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, rotationCondition.GetHashCode().ToString()))
                    {
                        if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
                    }
                    ImGuiHelper.DrawActionOverlay(cursor, IconSize, 1);
                }

                ImGui.SameLine();
                ImGuiHelper.SelectableCombo($"##Adjust{rotationCondition.GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Original,
                    LocalizationManager.RightLang.ActionSequencer_Adjusted,
                }, ref rotationCondition.Param1);
                break;
        }
    }

    private static Status[] _allStatus = null;
    private static Status[] AllStatus => _allStatus ??= Enum.GetValues<StatusID>()
        .Select(id => Service.GetSheet<Status>().GetRow((uint)id)).ToArray();

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
            if (ImGui.Selectable(LocalizationManager.RightLang.ActionSequencer_Target))
            {
                targetCondition._action = null;
                targetCondition.ID = ActionID.None;
                targetCondition.IsTarget = true;
            }

            if (ImGui.Selectable(LocalizationManager.RightLang.ActionSequencer_Player))
            {
                targetCondition._action = null;
                targetCondition.ID = ActionID.None;
                targetCondition.IsTarget = false;
            }
        });

        if (targetCondition._action != null ? targetCondition._action.GetTexture(out var icon) || IconSet.GetTexture(4, out icon)
            : IconSet.GetTexture(targetCondition.IsTarget ? 16u : 18u, out icon))
        {
            var cursor = ImGui.GetCursorPos();
            if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, targetCondition.GetHashCode().ToString()))
            {
                if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
            }
            ImGuiHelper.DrawActionOverlay(cursor, IconSize, 1);

            var description = targetCondition._action != null ? string.Format(LocalizationManager.RightLang.ActionSequencer_ActionTarget, targetCondition._action.Name)
                : targetCondition.IsTarget
                ? LocalizationManager.RightLang.ActionSequencer_Target
                : LocalizationManager.RightLang.ActionSequencer_Player;
            ImguiTooltips.HoveredTooltip(description);
        }

        ImGui.SameLine();
        DrawByteEnum($"##Category{targetCondition.GetHashCode()}", ref targetCondition.TargetConditionType, EnumTranslations.ToName);

        var condition = targetCondition.Condition ? 1 : 0;
        var combos = Array.Empty<string>();
        switch (targetCondition.TargetConditionType)
        {
            case TargetConditionType.HasStatus:
                combos = new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Has,
                    LocalizationManager.RightLang.ActionSequencer_HasNot,
                };
                break;
            case TargetConditionType.IsDying:
            case TargetConditionType.IsBoss:
            case TargetConditionType.InCombat:
            case TargetConditionType.CastingAction:
            case TargetConditionType.TargetName:
            case TargetConditionType.ObjectEffect:
            case TargetConditionType.Vfx:
                combos = new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                };
                break;

            case TargetConditionType.CastingActionTimeUntil:
            case TargetConditionType.Distance:
            case TargetConditionType.StatusEnd:
            case TargetConditionType.TimeToKill:
            case TargetConditionType.HP:
            case TargetConditionType.HPRatio:
            case TargetConditionType.MP:
                combos = new string[] { ">", "<=" };
                break;
        }

        ImGui.SameLine();

        if (ImGuiHelper.SelectableCombo($"##Comparation{targetCondition.GetHashCode()}", combos, ref condition))
        {
            targetCondition.Condition = condition > 0;
        }

        var popupId = "Status Finding Popup" + targetCondition.GetHashCode().ToString();

        RotationConfigWindow.StatusPopUp(popupId, AllStatus, ref searchTxt, status =>
        {
            targetCondition.Status = status;
            targetCondition.StatusId = (StatusID)targetCondition.Status.RowId;
        }, size: IconSizeRaw);

        void DrawStatusIcon()
        {
            if (IconSet.GetTexture(targetCondition.Status?.Icon ?? 16220, out var icon)
                || IconSet.GetTexture(16220, out icon))
            {
                if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(IconSize * 3 / 4, IconSize) * ImGuiHelpers.GlobalScale, targetCondition.GetHashCode().ToString()))
                {
                    if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
                }
                ImguiTooltips.HoveredTooltip(targetCondition.Status?.Name ?? string.Empty);
            }
        }

        switch (targetCondition.TargetConditionType)
        {
            case TargetConditionType.HasStatus:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                var check = targetCondition.FromSelf ? 1 : 0;
                if (ImGuiHelper.SelectableCombo($"From Self {targetCondition.GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_StatusAll,
                    LocalizationManager.RightLang.ActionSequencer_StatusSelf,
                }, ref check))
                {
                    targetCondition.FromSelf = check != 0;
                }
                break;

            case TargetConditionType.StatusEnd:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                check = targetCondition.FromSelf ? 1 : 0;
                if (ImGuiHelper.SelectableCombo($"From Self {targetCondition.GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_StatusAll,
                    LocalizationManager.RightLang.ActionSequencer_StatusSelf,
                }, ref check))
                {
                    targetCondition.FromSelf = check != 0;
                }

                DrawDragFloat($"s##Seconds{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime);
                break;


            case TargetConditionType.StatusEndGCD:
                ImGui.SameLine();
                DrawStatusIcon();

                ImGui.SameLine();

                check = targetCondition.FromSelf ? 1 : 0;
                if (ImGuiHelper.SelectableCombo($"From Self {targetCondition.GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_StatusAll,
                    LocalizationManager.RightLang.ActionSequencer_StatusSelf,
                }, ref check))
                {
                    targetCondition.FromSelf = check != 0;
                }

                DrawDragInt($"GCD##GCD{targetCondition.GetHashCode()}", ref targetCondition.GCD);
                DrawDragFloat($"{LocalizationManager.RightLang.ActionSequencer_TimeOffset}##Ability{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime);
                break;

            case TargetConditionType.Distance:
                if (DrawDragFloat($"yalm##yalm{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime))
                {
                    targetCondition.DistanceOrTime = Math.Max(0, targetCondition.DistanceOrTime);
                }
                break;

            case TargetConditionType.CastingAction:
                ImGui.SameLine();
                ImGuiHelper.SetNextWidthWithName(targetCondition.CastingActionName);
                ImGui.InputText($"Ability Name##CastingActionName{targetCondition.GetHashCode()}", ref targetCondition.CastingActionName, 128);
                break;

            case TargetConditionType.CastingActionTimeUntil:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(150 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(targetCondition.DistanceOrTime.ToString()).X));
                ImGui.DragFloat($"s##CastingActionTimeUntil{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime, .1f);
                break;

            case TargetConditionType.HPRatio:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(150 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(targetCondition.DistanceOrTime.ToString()).X));
                ImGui.DragFloat($"##HPRatio{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime, .1f);
                break;

            case TargetConditionType.MP:
            case TargetConditionType.HP:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(150 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(targetCondition.GCD.ToString()).X));
                ImGui.DragInt($"##HPorMP{targetCondition.GetHashCode()}", ref targetCondition.GCD, .1f);
                break;

            case TargetConditionType.TimeToKill:
                ImGui.SameLine();
                ImGui.SetNextItemWidth(Math.Max(150 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(targetCondition.DistanceOrTime.ToString()).X));
                ImGui.DragFloat($"##TimeToKill{targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime, .1f);
                break;

            case TargetConditionType.TargetName:
                ImGui.SameLine();
                ImGuiHelper.SetNextWidthWithName(targetCondition.CastingActionName);
                ImGui.InputText($"Name##TargetName{targetCondition.GetHashCode()}", ref targetCondition.CastingActionName, 128);
                break;

            case TargetConditionType.ObjectEffect:
                ImGui.SameLine();

                ImGui.Text("P1:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(40 * ImGuiHelpers.GlobalScale);
                ImGui.DragInt($"##Param1{targetCondition.GetHashCode()}", ref targetCondition.GCD, .1f);

                ImGui.SameLine();

                ImGui.Text("P2:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(40 * ImGuiHelpers.GlobalScale);
                ImGui.DragInt($"##Param2{targetCondition.GetHashCode()}", ref targetCondition.Param2, .1f);

                ImGui.SameLine();

                ImGui.Text("Time Offset:");
                ImGui.SameLine();
                const float MIN = 0, MAX = 60;

                ImGui.SetNextItemWidth(80 * ImGuiHelpers.GlobalScale);
                if (ImGui.DragFloatRange2($"##TimeOffset {targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime, ref targetCondition.TimeEnd, 0.1f, MIN, MAX))
                {
                    targetCondition.DistanceOrTime = Math.Max(Math.Min(targetCondition.DistanceOrTime, targetCondition.TimeEnd), MIN);
                    targetCondition.TimeEnd = Math.Min(Math.Max(targetCondition.DistanceOrTime, targetCondition.TimeEnd), MAX);
                }

                ImGui.SameLine();
                check = targetCondition.FromSelf ? 1 : 0;
                if (ImGuiHelper.SelectableCombo($"From Self {targetCondition.GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_StatusAll,
                    LocalizationManager.RightLang.ActionSequencer_StatusSelf,
                }, ref check))
                {
                    targetCondition.FromSelf = check != 0;
                }
                break;

            case TargetConditionType.Vfx:
                ImGui.SameLine();

                ImGui.Text("Vfx Path:");
                ImGui.SameLine();

                ImGuiHelper.SetNextWidthWithName(targetCondition.CastingActionName);
                ImGui.InputText($"Name##TargetName{targetCondition.GetHashCode()}", ref targetCondition.CastingActionName, 128);

                ImGui.SameLine();

                ImGui.Text("Time Offset:");
                ImGui.SameLine();

                ImGui.SetNextItemWidth(80 * ImGuiHelpers.GlobalScale);
                if (ImGui.DragFloatRange2($"##TimeOffset {targetCondition.GetHashCode()}", ref targetCondition.DistanceOrTime, ref targetCondition.TimeEnd, 0.1f, MIN, MAX))
                {
                    targetCondition.DistanceOrTime = Math.Max(Math.Min(targetCondition.DistanceOrTime, targetCondition.TimeEnd), MIN);
                    targetCondition.TimeEnd = Math.Min(Math.Max(targetCondition.DistanceOrTime, targetCondition.TimeEnd), MAX);
                }

                ImGui.SameLine();
                check = targetCondition.FromSelf ? 1 : 0;
                if (ImGuiHelper.SelectableCombo($"From Self {targetCondition.GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_StatusAll,
                    LocalizationManager.RightLang.ActionSequencer_StatusSelf,
                }, ref check))
                {
                    targetCondition.FromSelf = check != 0;
                }
                break;
        }

        if (targetCondition._action == null && targetCondition.IsTarget)
        {
            using var style = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Condition_TargetWarning);
        }
    }

    private static string[] _territoryNames = null;
    public static string[] TerritoryNames => _territoryNames ??= Service.GetSheet<TerritoryType>()?
        .Select(t => t?.PlaceName?.Value?.Name?.RawString ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToArray();

    private static string[] _dutyNames = null;

    public static string[] DutyNames => _dutyNames ??= new HashSet<string>(Service.GetSheet<ContentFinderCondition>()?
        .Select(t => t?.Name?.RawString ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).Reverse()).ToArray();

    private static void DrawAfter(this TerritoryCondition territoryCondition, ICustomRotation _)
    {
        DrawByteEnum($"##Category{territoryCondition.GetHashCode()}", ref territoryCondition.TerritoryConditionType, EnumTranslations.ToName);

        ImGui.SameLine();

        ImGuiHelper.SelectableCombo($"##IsNot{territoryCondition.GetHashCode()}", new string[]
        {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
        }, ref territoryCondition.Condition);

        switch (territoryCondition.TerritoryConditionType)
        {
            case TerritoryConditionType.TerritoryContentType:
                ImGui.SameLine();

                var type = (TerritoryContentType)territoryCondition.Param1;
                DrawByteEnum($"##TerritoryContentType{territoryCondition.GetHashCode()}", ref type, i => i.ToString());
                territoryCondition.Param1 = (int)type;
                break;

            case TerritoryConditionType.TerritoryName:
                ImGui.SameLine();

                SearchItems($"##TerritoryName{territoryCondition.GetHashCode()}", territoryCondition.Name, ref searchTxt,
                TerritoryNames, i => i.ToString(), i =>
                {
                    territoryCondition.Name = i;
                }, LocalizationManager.RightLang.ConfigWindow_Condition_TerritoryName);
                break;

            case TerritoryConditionType.DutyName:
                ImGui.SameLine();

                SearchItems($"##DutyName{territoryCondition.GetHashCode()}", territoryCondition.Name, ref searchTxt,
                DutyNames, i => i.ToString(), i =>
                {
                    territoryCondition.Name = i;
                }, LocalizationManager.RightLang.ConfigWindow_Condition_DutyName);
                break;

            case TerritoryConditionType.MapEffect:
                ImGui.SameLine();

                ImGui.Text("Pos:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(40 * ImGuiHelpers.GlobalScale);
                ImGui.DragInt($"##Position{territoryCondition.GetHashCode()}", ref territoryCondition.Position, .1f);

                ImGui.SameLine();

                ImGui.Text("P1:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(40 * ImGuiHelpers.GlobalScale);
                ImGui.DragInt($"##Param1{territoryCondition.GetHashCode()}", ref territoryCondition.Param1, .1f);

                ImGui.SameLine();

                ImGui.Text("P2:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(40 * ImGuiHelpers.GlobalScale);
                ImGui.DragInt($"##Param2{territoryCondition.GetHashCode()}", ref territoryCondition.Param2, .1f);

                ImGui.SameLine();

                ImGui.Text("Time Offset:");
                ImGui.SameLine();
                const float MIN = 0, MAX = 60;

                ImGui.SetNextItemWidth(80 * ImGuiHelpers.GlobalScale);
                if (ImGui.DragFloatRange2($"##TimeOffset {territoryCondition.GetHashCode()}", ref territoryCondition.TimeStart, ref territoryCondition.TimeEnd, 0.1f, MIN, MAX))
                {
                    territoryCondition.TimeStart = Math.Max(Math.Min(territoryCondition.TimeStart, territoryCondition.TimeEnd), MIN);
                    territoryCondition.TimeEnd = Math.Min(Math.Max(territoryCondition.TimeStart, territoryCondition.TimeEnd), MAX);
                }
                break;
        }
    }
    #endregion
}
