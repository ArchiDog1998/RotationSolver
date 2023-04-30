using Dalamud.Utility;
using Lumina.Data.Parsing;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Data;
using RotationSolver.Localization;
using RotationSolver.TextureItems;
using RotationSolver.Updaters;
using System.Xml.Linq;

namespace RotationSolver.UI;

internal partial class RotationConfigWindow
{
    private static uint _territoryId = 0;
    private static TerritoryTypeTexture[] _allTerritories = null;
    public static TerritoryTypeTexture[] AllTerritories
    {
        get
        {
            if (_allTerritories == null)
            {
                _allTerritories = Service.GetSheet<TerritoryType>()
                    .Where(t => t!= null
                        && t.ContentFinderCondition?.Value?.ContentType?.Value?.RowId != 0)
                    .OrderBy(t => t.ContentFinderCondition?.Value?.ContentType?.Value?.RowId)
                    .Select(t => new TerritoryTypeTexture(t))
                    .ToArray();
            }
            return _allTerritories;
        }
    }

    private static StatusTexture[] _allDispelStatus = null;
    public static StatusTexture[] AllDispelStatus
    {
        get
        {
            if (_allDispelStatus == null)
            {
                _allDispelStatus = Service.GetSheet<Status>()
                    .Where(s => s.CanDispel)
                    .Select(s => new StatusTexture(s))
                    .ToArray();
            }
            return _allDispelStatus;
        }
    }

    private static StatusTexture[] _allInvStatus = null;
    public static StatusTexture[] AllInvStatus
    {
        get
        {
            if (_allInvStatus == null)
            {
                _allInvStatus = Service.GetSheet<Status>()
                    .Where(s => !s.CanDispel && !s.LockMovement && !s.IsPermanent && !s.IsGaze && !s.IsFcBuff && s.HitEffect.Row == 16 && s.ClassJobCategory.Row == 1 && s.StatusCategory == 1
                        && !string.IsNullOrEmpty(s.Name.ToString()) && s.Icon != 0)
                    .Select(s => new StatusTexture(s))
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
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Hostile, DrawParamHostile, () =>
            {
                if (ImGuiHelper.IconButton(FontAwesomeIcon.Plus, "Add Hostile"))
                {
                    Service.Config.TargetingTypes.Add(TargetingType.Big);
                }
                ImGui.SameLine();
                ImGuiHelper.Spacing();
                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Param_HostileDesc);
            });

            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_NoHostile, DrawParamNoHostile, () =>
            {
                ImGui.SetNextItemWidth(200);

                if (ImGui.BeginCombo("##AddNoHostileNames",
                    Service.GetSheet<TerritoryType>().GetRow(_territoryId)?.PlaceName?.Value?.Name.ToString() ?? "Everywhere", ImGuiComboFlags.HeightLargest))
                {
                    if (ImGui.Selectable("Everywhere"))
                    {
                        _territoryId = 0;
                    }

                     ImGuiHelper.SearchItems(ref searchText, AllTerritories, s =>
                    {
                        _territoryId = s.ID;
                    });

                    ImGui.EndCombo();
                }

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                if (ImGuiHelper.IconButton(FontAwesomeIcon.Plus, "Add Territory"))
                {
                    if (!OtherConfiguration.NoHostileNames.TryGetValue(_territoryId, out var hostileNames))
                        hostileNames = Array.Empty<string>();
                    OtherConfiguration.NoHostileNames[_territoryId] = hostileNames.Append(string.Empty).ToArray();
                }
                ImGui.SameLine();
                ImGuiHelper.Spacing();
                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_NoHostileDesc);
            });

            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Invincibility, DrawInvincibility, () =>
            {
                ImGui.SetNextItemWidth(200);
                ImGuiHelper.SearchCombo("##AddInvincibleStatus",
                    LocalizationManager.RightLang.ConfigWindow_Param_AddOne,
                    ref searchText, AllInvStatus, s =>
                    {
                        OtherConfiguration.InvincibleStatus.Add((uint)s.ID);
                        OtherConfiguration.SaveInvincibleStatus();

                    });

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_InvincibilityDesc);
            });

            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatus, DrawDangerousStatus, () =>
            {
                ImGui.SetNextItemWidth(200);
                ImGuiHelper.SearchCombo("##AddDangerousStatus",
                    LocalizationManager.RightLang.ConfigWindow_Param_AddOne,
                    ref searchText, AllDispelStatus, s =>
                    {
                        OtherConfiguration.DangerousStatus.Add((uint)s.ID);
                        OtherConfiguration.SaveDangerousStatus();
                    });

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatusDesc);
            });


            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Rotations, DrawRotationDevTab, () =>
            {
                if (ImGuiHelper.IconButton(FontAwesomeIcon.Download, "DownloadRotationsButton"))
                {
                    RotationUpdater.GetAllCustomRotations(RotationUpdater.DownloadOption.MustDownload | RotationUpdater.DownloadOption.ShowList);
                }

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                if (ImGuiHelper.IconButton(FontAwesomeIcon.FileDownload, "##LoadLocalRotations"))
                {
                    RotationUpdater.GetAllCustomRotations(RotationUpdater.DownloadOption.ShowList);
                }

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                if (ImGuiHelper.IconButton(FontAwesomeIcon.Question, "##OpenWiki"))
                {
                    Util.OpenLink("https://archidog1998.github.io/RotationSolver/#/RotationDev/");
                }

                if (ImGuiHelper.IconButton(FontAwesomeIcon.Plus, "Add Rotation"))
                {
                    Service.Config.OtherLibs = Service.Config.OtherLibs.Append(string.Empty).ToArray();
                }
                ImGui.SameLine();
                ImGuiHelper.Spacing();

                ImGui.TextWrapped("Third-party Rotation Libraries");
            });

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

            if (ImGuiHelper.IconButton(FontAwesomeIcon.ArrowUp, $"##HostileUp{i}"))
            {
                if (i > 0)
                {
                    var value = Service.Config.TargetingTypes[i];
                    Service.Config.TargetingTypes.RemoveAt(i);
                    Service.Config.TargetingTypes.Insert(i - 1, value);
                }
            }
            ImGui.SameLine();
            ImGuiHelper.Spacing();
            if (ImGuiHelper.IconButton(FontAwesomeIcon.ArrowDown, $"##HostileDown{i}"))
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

            if (ImGuiHelper.IconButton(FontAwesomeIcon.Ban, $"##HostileDelete{i}"))
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
        foreach (var statusId in OtherConfiguration.DangerousStatus)
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


            if (ImGuiHelper.IconButton(FontAwesomeIcon.Ban, $"##RemoveDangerous{statusId}"))
            {
                removeId = statusId;
            }
        }

        if(removeId != 0)
        {
            OtherConfiguration.DangerousStatus.Remove(removeId);
            OtherConfiguration.SaveDangerousStatus();
        }
        if (addId != 0)
        {
            OtherConfiguration.DangerousStatus.Add(addId);
            OtherConfiguration.SaveDangerousStatus();
        }
    }

    private void DrawInvincibility()
    {
        uint removeId = 0;
        uint addId = 0;
        foreach (var statusId in OtherConfiguration.InvincibleStatus)
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

            if (ImGuiHelper.IconButton(FontAwesomeIcon.Ban, $"##InvincibleStatus{statusId}"))
            {
                removeId = statusId;
            }
        }

        if (removeId != 0)
        {
            OtherConfiguration.InvincibleStatus.Remove(removeId);
            OtherConfiguration.SaveInvincibleStatus();
        }
        if (addId != 0)
        {
            OtherConfiguration.InvincibleStatus.Add(addId);
            OtherConfiguration.SaveInvincibleStatus();
        }
    }

    private void DrawRotationDevTab()
    {
        int removeIndex = -1;
        for (int i = 0; i < Service.Config.OtherLibs.Length; i++)
        {
            if (ImGui.InputText($"##OtherLib{i}", ref Service.Config.OtherLibs[i], 1024))
            {
                Service.Config.Save();
            }
            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGuiHelper.IconButton(FontAwesomeIcon.Ban, $"##RemoveOtherLibs{i}"))
            {
                removeIndex = i;
            }
        }
        if (removeIndex > -1)
        {
            var list = Service.Config.OtherLibs.ToList();
            list.RemoveAt(removeIndex);
            Service.Config.OtherLibs = list.ToArray();
            Service.Config.Save();
        }
    }

    private void DrawParamNoHostile()
    {
        int removeIndex = -1;

        if (!OtherConfiguration.NoHostileNames.TryGetValue(_territoryId, out var names)) names = Array.Empty<string>();

        for (int i = 0; i < names.Length; i++)
        {
            if (ImGui.InputText($"##NoHostileNames{i}", ref names[i], 1024))
            {
                OtherConfiguration.NoHostileNames[_territoryId] = names;
                OtherConfiguration.SaveNoHostileNames();
            }
            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGuiHelper.IconButton(FontAwesomeIcon.Ban, $"##RemoveNoHostileNames{i}"))
            {
                removeIndex = i;
            }
        }
        if (removeIndex > -1)
        {
            var list = names.ToList();
            list.RemoveAt(removeIndex);
            OtherConfiguration.NoHostileNames[_territoryId] = list.ToArray();
            OtherConfiguration.SaveNoHostileNames();
        }
    }
}
