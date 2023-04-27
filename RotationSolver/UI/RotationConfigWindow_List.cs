using Lumina.Excel.GeneratedSheets;
using RotationSolver.ActionSequencer;
using RotationSolver.Basic.Data;
using RotationSolver.Localization;
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
                    .Where(s => !s.CanDispel && !s.LockMovement && !s.IsPermanent && !s.IsGaze && !s.IsFcBuff && s.HitEffect.Row == 16 && s.ClassJobCategory.Row == 1 && s.StatusCategory == 1
                        && !string.IsNullOrEmpty(s.Name.ToString()) && s.Icon != 0)
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
                DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Hostile, DrawParamHostile, () =>
                {
                    if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_AddHostileCondition))
                    {
                        Service.Config.TargetingTypes.Add(TargetingType.Big);
                    }
                    ImGui.SameLine();
                    ImGuiHelper.Spacing();
                    ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Param_HostileDesc);
                });

                DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Invincibility, DrawInvincibility, () =>
                {
                    ImGui.SetNextItemWidth(200);
                    ImGuiHelper.SearchCombo("##AddInvincibleStatus",
                        "Add One",
                        ref searchText, AllInvStatus, s =>
                        {
                            Service.Config.InvincibleStatus.Add((uint)s.ID);
                        });

                    ImGui.SameLine();
                    ImGuiHelper.Spacing();

                    ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_InvincibilityDesc);
                });

                DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatus, DrawDangerousStatus, () =>
                {
                    ImGui.SetNextItemWidth(200);
                    ImGuiHelper.SearchCombo("##AddDangerousStatus",
                        "Add One",
                        ref searchText, AllDispelStatus, s =>
                        {
                             Service.Config.DangerousStatus.Add((uint)s.ID);
                        });

                    ImGui.SameLine();
                    ImGuiHelper.Spacing();

                    ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatusDesc);

                });
            }
            catch { }


            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private void DrawParamHostile()
    {
        for (int i = 0; i < Service.Config.TargetingTypes.Count; i++)
        {
            ImGui.Separator();

            var names = Enum.GetNames(typeof(TargetingType));
            var targingType = (int)Service.Config.TargetingTypes[i];

            ImGui.SetNextItemWidth(150);

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
        uint removeId = 0;
        uint addId = 0;
        foreach (var statusId in Service.Config.DangerousStatus)
        {
            var status = Service.GetSheet<Status>().GetRow(statusId);
            ImGui.Image(IconSet.GetTexture(status.Icon).ImGuiHandle, new Vector2(24, 30));

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            ImGui.SetNextItemWidth(150);

            ImGuiHelper.SearchCombo($"##DangerousStatus{statusId}",
                $"{status.Name} ({status.RowId})",
                ref searchText, AllDispelStatus, s =>
                {
                    removeId = statusId;
                    addId = (uint)s.ID;
                });

            ImGui.SameLine();
            ImGuiHelper.Spacing();


            if (ImGuiHelper.IconButton(FontAwesomeIcon.SquareXmark, $"RemoveDangerous{statusId}"))
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

    private void DrawInvincibility()
    {
        uint removeId = 0;
        uint addId = 0;
        foreach (var statusId in Service.Config.InvincibleStatus)
        {
            var status = Service.GetSheet<Status>().GetRow(statusId);
            ImGui.Image(IconSet.GetTexture(status.Icon).ImGuiHandle, new Vector2(24, 30));

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            ImGui.SetNextItemWidth(150);

            ImGuiHelper.SearchCombo($"##InvincibleStatus{statusId}",
                $"{status.Name} ({status.RowId})",
                ref searchText, AllInvStatus, s =>
                {
                    removeId = statusId;
                    addId = (uint)s.ID;
                });

            ImGui.SameLine();
            ImGuiHelper.Spacing();

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
