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
    private static BaseStatus[] _allDispelStatus = null;
    public static BaseStatus[] AllDispelStatus
    {
        get
        {
            if (_allDispelStatus == null)
            {
                _allDispelStatus = Service.GetSheet<Status>()
                    .Where(s => s.CanDispel)
                    .Select(s => new BaseStatus(s))
                    .ToArray();
            }
            return _allDispelStatus;
        }
    }

    private static BaseStatus[] _allInvStatus = null;
    public static BaseStatus[] AllInvStatus
    {
        get
        {
            if (_allInvStatus == null)
            {
                _allInvStatus = Service.GetSheet<Status>()
                    .Where(s => !s.CanDispel)
                    .Select(s => new BaseStatus(s))
                    .ToArray();
            }
            return _allInvStatus;
        }
    }

    private void DrawListTab()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("List Items"))
        {
            try
            {
                DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Hostile, DrawParamHostile);

                DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Invinsibility, DrawInvinsibility);

                DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatus, DrawDangerousStatus);
            }
            catch { }


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

    string searchText = string.Empty;
    private void DrawDangerousStatus()
    {
        if (ImGuiHelper.IconButton(FontAwesomeIcon.Plus, "AddDangeriousStatus"))
        {
            Service.Config.DangerousStatus.Add(0);
        }

        uint removeId = 0;
        uint addId = 0;
        foreach (var statusId in Service.Config.DangerousStatus)
        {
            ImGui.SetNextItemWidth(100);
            ImGuiHelper.SearchCombo($"DangeriousStatus{statusId}",
                Service.GetSheet<Status>().GetRow(statusId).Name,
                ref searchText, AllDispelStatus, s =>
                {
                    removeId = statusId;
                    addId = (uint)s.ID;
                });

            if (ImGuiHelper.IconButton(FontAwesomeIcon.SquareXmark, $"RemoveDangerious{statusId}"))
            {
                removeId = statusId;
            }
        }

        if(removeId != 0)
        {
            Service.Config.DangerousStatus.Remove(removeId);
        }
        if (addId != 0)
        {
            Service.Config.DangerousStatus.Add(addId);
        }
    }

    private void DrawInvinsibility()
    {
        if (ImGuiHelper.IconButton(FontAwesomeIcon.Plus, "AddInvinsibilityStatus"))
        {
            Service.Config.InvincibleStatus.Add(0);
        }

        uint removeId = 0;
        uint addId = 0;
        foreach (var statusId in Service.Config.InvincibleStatus)
        {
            ImGui.SetNextItemWidth(100);
            ImGuiHelper.SearchCombo($"InvincibleStatus{statusId}",
                Service.GetSheet<Status>().GetRow(statusId).Name,
                ref searchText, AllInvStatus, s =>
                {
                    removeId = statusId;
                    addId = (uint)s.ID;
                });

            if (ImGuiHelper.IconButton(FontAwesomeIcon.SquareXmark, $"InvincibleStatus{statusId}"))
            {
                removeId = statusId;
            }
        }

        if (removeId != 0)
        {
            Service.Config.InvincibleStatus.Remove(removeId);
        }
        if (addId != 0)
        {
            Service.Config.InvincibleStatus.Add(addId);
        }
    }
}
