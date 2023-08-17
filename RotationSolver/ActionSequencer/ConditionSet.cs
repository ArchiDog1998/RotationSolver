using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.Havok;
using RotationSolver.Localization;
using RotationSolver.UI;
using RotationSolver.UI.SearchableConfigs;
using System.Drawing;
using System.Security.Policy;
using System.Windows.Forms;

namespace RotationSolver.ActionSequencer;

internal class ConditionSet : ICondition
{
    public bool IsTrue(ICustomRotation combo) => Conditions.Count != 0 && (IsAnd ? Conditions.All(c => c.IsTrue(combo))
                                : Conditions.Any(c => c.IsTrue(combo)));
    public List<ICondition> Conditions { get; set; } = new List<ICondition>();
    public bool IsAnd { get; set; }

    public void Draw(ICustomRotation rotation)
    {
        ImGui.BeginGroup();

        AddButton();

        ImGui.SameLine();

        if(ImGui.Button((IsAnd ? "&&" : "||") + $"##Rule{GetHashCode()}"))
        {
            IsAnd = !IsAnd;
        }

        ImGui.Spacing();

        for(int i = 0; i < Conditions.Count; i++)
        {
            ICondition condition = Conditions[i];

            void Delete() 
            {
                Conditions.RemoveAt(i);
            };

            void Up()
            {
                Conditions.RemoveAt(i);
                Conditions.Insert(Math.Max(0, i - 1), condition);
            };
            void Down()
            {
                Conditions.RemoveAt(i);
                Conditions.Insert(Math.Min(Conditions.Count, i + 1), condition);
            }

            var key = $"Condition Pop Up: {condition.GetHashCode()}";

            Searchable.DrawHotKeysPopup(key, string.Empty,
                (LocalizationManager.RightLang.ConfigWindow_List_Remove, Delete, new string[] { "Delete" }),
                (LocalizationManager.RightLang.ConfigWindow_Actions_MoveUp, Up, new string[] { "↑" }),
                (LocalizationManager.RightLang.ConfigWindow_Actions_MoveDown, Down, new string[] { "↓" }));

            DrawCondition(condition.IsTrue(rotation));

            Searchable.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true, 
                (Delete, new VirtualKey[] { VirtualKey.DELETE }),
                (Up, new VirtualKey[] { VirtualKey.UP }),
                (Down, new VirtualKey[] { VirtualKey.DOWN }));

            ImGui.SameLine();

            condition.Draw(rotation);
        }

        ImGui.EndGroup();
    }

    private void AddButton()
    {
        if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddButton" + GetHashCode().ToString()))
        {
            ImGui.OpenPopup("Popup" + GetHashCode().ToString());
        }

        if (ImGui.BeginPopup("Popup" + GetHashCode().ToString()))
        {
            AddOneCondition<ConditionSet>(LocalizationManager.RightLang.ActionSequencer_ConditionSet);
            AddOneCondition<ActionCondition>(LocalizationManager.RightLang.ActionSequencer_ActionCondition);
            AddOneCondition<TargetCondition>(LocalizationManager.RightLang.ActionSequencer_TargetCondition);
            AddOneCondition<RotationCondition>(LocalizationManager.RightLang.ActionSequencer_RotationCondition);

            ImGui.EndPopup();
        }
    }

    private void AddOneCondition<T>(string name) where T : ICondition
    {
        if (ImGui.Selectable(name))
        {
            Conditions.Add(Activator.CreateInstance<T>());
            ImGui.CloseCurrentPopup();
        }
    }

    internal static void DrawCondition(bool? tag)
    {
        float size = ConditionHelper.IconSize * (1 + 8 / 82);

        if (!tag.HasValue)
        {
            if (IconSet.GetTexture("ui/uld/image2.tex", out var texture) || IconSet.GetTexture(0u, out texture))
            {
                ImGui.Image(texture.ImGuiHandle, Vector2.One * size);
            }
        }
        else
        {
            if (IconSet.GetTexture("ui/uld/readycheck_hr1.tex", out var texture))
            {
                ImGui.Image(texture.ImGuiHandle, Vector2.One * size,
                    new Vector2(tag.Value ? 0 : 0.5f, 0),
                    new Vector2(tag.Value ? 0.5f : 1, 1));
            }
        }
    }
}
