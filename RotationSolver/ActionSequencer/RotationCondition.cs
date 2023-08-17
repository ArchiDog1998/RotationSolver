using ECommons.GameHelpers;
using RotationSolver.Localization;
using RotationSolver.UI;

namespace RotationSolver.ActionSequencer;

internal class RotationCondition : ICondition
{
    public ComboConditionType ComboConditionType;
    PropertyInfo _prop;
    public string PropertyName { get; set; } = string.Empty;

    MethodInfo _method;
    public string MethodName { get; set; } = string.Empty;

    BaseAction _action;

    public ActionID ID { get; set; } = ActionID.None;

    public int Condition;

    public int Param1;
    public float Param2;

    private void UpdateInfo(ICustomRotation rotation)
    {
        ConditionHelper.CheckBaseAction(rotation, ID, ref _action);
        ConditionHelper.CheckMemberInfo(rotation, PropertyName, ref _prop);
        ConditionHelper.CheckMemberInfo(rotation, MethodName, ref _method);
    }

    public bool IsTrue(ICustomRotation rotation)
    {
        if (!Player.Available) return false;
        UpdateInfo(rotation);

        switch (ComboConditionType)
        {
            case ComboConditionType.Bool:
                if (_prop == null) return false;
                if (_prop.GetValue(rotation) is bool b)
                {
                    return Condition > 0 ? !b : b;
                }
                return false;

            case ComboConditionType.Byte:
                if (_prop == null) return false;
                if (_prop.GetValue(rotation) is byte by)
                {
                    switch (Condition)
                    {
                        case 0:
                            return by > Param1;
                        case 1:
                            return by < Param1;
                        case 2:
                            return by == Param1;
                    }
                }
                return false;

            case ComboConditionType.Float:
                if (_prop == null) return false;
                if (_prop.GetValue(rotation) is float fl)
                {
                    switch (Condition)
                    {
                        case 0:
                            return fl > Param2;
                        case 1:
                            return fl < Param2;
                        case 2:
                            return fl == Param2;
                    }
                }
                return false;

            case ComboConditionType.Last:
                try
                {
                    if (_method?.Invoke(rotation, new object[] { Param1 > 0, new IAction[] { _action } }) is bool boo)
                    {
                        return Condition > 0 ? boo : !boo;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
        }

        return false;
    }

    string searchTxt = string.Empty;

    private readonly CollapsingHeaderGroup _actionsList = new()
    {
        HeaderSize = 12,
    };
    public void Draw(ICustomRotation rotation)
    {
        UpdateInfo(rotation);

        ConditionHelper.DrawByteEnum($"##Category{GetHashCode()}", ref ComboConditionType, EnumTranslations.ToName);

        switch (ComboConditionType)
        {
            case ComboConditionType.Bool:
                ImGui.SameLine();
                ConditionHelper.SearchItemsReflection($"##Comparation{GetHashCode()}", _prop?.GetMemberName(), ref searchTxt, rotation.AllBools, i =>
                {
                    _prop = i;
                    PropertyName = i.Name;
                });
                ImGui.SameLine();

                ImGuiHelper.SelectableCombo($"##IsOrNot{GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                }, ref Condition);

                break;

            case ComboConditionType.Byte:
                ImGui.SameLine();
                ConditionHelper.SearchItemsReflection($"##ByteChoice{GetHashCode()}", _prop?.GetMemberName(), ref searchTxt, rotation.AllBytes, i =>
                {
                    _prop = i;
                    PropertyName = i.Name;
                });

                ImGui.SameLine();

                ImGuiHelper.SelectableCombo($"##Comparation{GetHashCode()}", new string[] { ">", "<", "=" }, ref Condition);

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ImGui.DragInt($"##Value{GetHashCode()}", ref Param1);

                break;
            case ComboConditionType.Float:
                ImGui.SameLine();
                ConditionHelper.SearchItemsReflection($"##FloatChoice{GetHashCode()}", _prop?.GetMemberName(), ref searchTxt, rotation.AllFloats, i =>
                {
                    _prop = i;
                    PropertyName = i.Name;
                });

                ImGui.SameLine();
                ImGuiHelper.SelectableCombo($"##Comparation{GetHashCode()}", new string[] { ">", "<", "=" }, ref Condition);

                ImGui.SameLine();
                ImGui.SetNextItemWidth(50);

                ImGui.DragFloat($"##Value{GetHashCode()}", ref Param2);

                break;

            case ComboConditionType.Last:
                ImGui.SameLine();

                var names = new string[]
                    {
                        nameof(CustomRotation.IsLastGCD),
                        nameof(CustomRotation.IsLastAction),
                        nameof(CustomRotation.IsLastAbility),
                    };
                var index = Array.IndexOf(names, MethodName);
                ImGuiHelper.SelectableCombo($"##Last{GetHashCode()}", names, ref index);
                MethodName = names[index];

                ImGui.SameLine();

                ImGuiHelper.SelectableCombo($"##IsNot{GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                }, ref Condition);

                ImGui.SameLine();

                var name = _action?.Name ?? string.Empty;

                var popUpKey = "Rotation Condition Pop Up" + GetHashCode().ToString();

                ConditionHelper.ActionSelectorPopUp(popUpKey, _actionsList, rotation, item => ID = (ActionID)item.ID);

                if (_action?.GetTexture(out var icon) ?? false || IconSet.GetTexture(4, out icon))
                {
                    var cursor = ImGui.GetCursorPos();
                    if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * ConditionHelper.IconSize, GetHashCode().ToString()))
                    {
                        if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
                    }
                    ImGuiHelper.DrawActionOverlay(cursor, ConditionHelper.IconSize, 1);
                }

                ImGui.SameLine();
                ImGuiHelper.SelectableCombo($"##Adjust{GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Original,
                    LocalizationManager.RightLang.ActionSequencer_Adjusted,
                }, ref Param1);
                break;
        }
    }
}

public enum ComboConditionType : byte
{
    Bool,
    Byte,
    Float,
    Last,
}
