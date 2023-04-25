using Lumina.Excel.GeneratedSheets;
using RotationSolver.Localization;
using RotationSolver.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.UI;

internal partial class RotationConfigWindow
{
    private static BaseStatus[] _allStatus = null;
    public static BaseStatus[] AllStatus
    {
        get
        {
            if (_allStatus == null)
            {
                _allStatus = Service.GetSheet<Status>()
                    .Where(s => s.CanDispel)
                    .Select(s => new BaseStatus(s))
                    .ToArray();
            }
            return _allStatus;
        }
    }

    private void DrawListTab()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("List Items"))
        {
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Hostile, DrawParamHostile);

            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Invinsibility, DrawInvinsibility);

            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatus, DrawDangerousStatus);

            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private void DrawParamHostile()
    {
        if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_AddHostileCondition))
        {
            Service.Config.TargetingTypes.Add(TargetingType.Big);
        }
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Param_HostileDesc);

        for (int i = 0; i < Service.Config.TargetingTypes.Count; i++)
        {
            ImGui.Separator();

            var names = Enum.GetNames(typeof(TargetingType));
            var targingType = (int)Service.Config.TargetingTypes[i];
            if (ImGui.Combo(LocalizationManager.RightLang.ConfigWindow_Param_HostileCondition + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
            {
                Service.Config.TargetingTypes[i] = (TargetingType)targingType;
                Service.Config.Save();
            }

            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_ConditionUp + "##HostileUp" + i.ToString()))
            {
                if (i != 0)
                {
                    var value = Service.Config.TargetingTypes[i];
                    Service.Config.TargetingTypes.RemoveAt(i);
                    Service.Config.TargetingTypes.Insert(i - 1, value);
                }
            }
            ImGui.SameLine();
            ImGuiHelper.Spacing();
            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_ConditionDown + "##HostileDown" + i.ToString()))
            {
                if (i < Service.Config.TargetingTypes.Count - 1)
                {
                    var value = Service.Config.TargetingTypes[i];
                    Service.Config.TargetingTypes.RemoveAt(i);
                    Service.Config.TargetingTypes.Insert(i + 1, value);
                }
            }

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_ConditionDelete + "##HostileDelete" + i.ToString()))
            {
                Service.Config.TargetingTypes.RemoveAt(i);
            }
        }
    }

    private void DrawDangerousStatus()
    {
        if (ImGuiHelper.IconButton(FontAwesomeIcon.Plus, "AddDangerousStatus"))
        {
            Service.Config.DangerousStatus.Add(0);
        }

        //for (int i = 0; i < Service.Config.DangerousStatus.Count; i++)
        //{
        //    ImGuiHelper.SearchCombo($"DangerousStatus{Service.Config.DangerousStatus[i]}",
        //        )
        //}
    }

    private void DrawInvinsibility()
    {
        if (ImGuiHelper.IconButton(FontAwesomeIcon.Plus, "AddInvinsibilityStatus"))
        {
            Service.Config.InvincibleStatus.Add(0);
        }

        //TargetCondition.AllStatus;
    }
}
