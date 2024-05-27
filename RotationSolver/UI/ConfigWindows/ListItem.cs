using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using System.ComponentModel;
using XIVConfigUI;
using XIVConfigUI.SearchableConfigs;
using GAction = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.UI.ConfigWindows;

[Description("List")]
public class ListItem : ConfigWindowItemRS
{
    public static Vector3 HoveredPosition { get; private set; } = Vector3.Zero;

    private CollapsingHeaderGroup? _idsHeader;

    public override uint Icon => 21;
    public override string Description => UiString.Item_List.Local();

    public override void Draw(ConfigWindow window)
    {
        ImGui.TextWrapped(UiString.ConfigWindow_List_Description.Local());

        _idsHeader ??= new(new()
            {
                { () => UiString.ConfigWindow_List_Statuses.Local(), DrawListStatuses},
                { () => Service.Config.UseDefenseAbility ? UiString.ConfigWindow_List_Actions.Local() : string.Empty,
                    () => DrawListActions(window)},
                { () =>  UiString.ConfigWindow_List_Territories.Local(), DrawListTerritories},
                { () =>  UiString.ConfigWindow_List_ActionGroups.Local(), DrawActionGroups},
            });
        _idsHeader?.Draw();
    }

    #region List
    private static readonly CollapsingHeaderGroup _actionGrpActionsList = new()
    {
        HeaderSize = FontSize.Fifth,
    };

    private static void DrawActionGroups()
    {
        if (DataCenter.RightNowRotation == null) return;

        if (!Service.Config.ActionGroups.Any(i => !i.ActionIds.Any(i => i != 0)))
        {
            Service.Config.ActionGroups.Add(new());
        }

        for (int i = 0; i < Service.Config.ActionGroups.Count; i++)
        {
            void DeleteActionGroup()
            {
                Service.Config.ActionGroups.RemoveAt(i);
            }

            var group = Service.Config.ActionGroups[i];

            var keyGrp = $"ActionPopupDelete{group.GetHashCode()}";
            var cmd = ImGuiHelperRS.ToCommandStr(OtherCommandType.ToggleActionGroup, $"{group.Name} true");

            ImGuiHelper.DrawHotKeysPopup(keyGrp, cmd,
                (UiString.ConfigWindow_List_Remove.Local(), DeleteActionGroup, ["Delete"]));
            using (var grp = ImRaii.Group())
            {
                var isChecked = group.Enable;

                if (ImGui.Checkbox("##ActionGroupEnable" + i, ref isChecked))
                {
                    group.Enable = isChecked;
                }

                ImGuiHelper.HoveredTooltip(UiString.ConfigWindow_List_ActionGroups_Enable.Local());

                ImGui.SameLine();

                var showInWindow = group.ShowInWindow;
                if (ImGui.Checkbox("##ActionGroupShow" + i, ref showInWindow))
                {
                    group.ShowInWindow = showInWindow;
                }

                ImGuiHelper.HoveredTooltip(UiString.ConfigWindow_List_ActionGroups_Show.Local());

                ImGui.SameLine();

                var color = group.Color;

                if (ImGui.ColorEdit4("##ActionGroupColor" + i, ref color, ImGuiColorEditFlags.NoInputs))
                {
                    group.Color = color;
                }
            }

            ImGuiHelper.ExecuteHotKeysPopup(keyGrp, cmd, string.Empty, true,
                (DeleteActionGroup, [VirtualKey.DELETE]));
            ImGui.SameLine();

            ImGui.SetNextItemWidth(150 * Scale);

            var name = group.Name;
            if (ImGui.InputTextWithHint($"##ActionGroupName{i}", UiString.ConfigWindow_List_ActionGroups_Name.Local(), ref name, 1024))
            {
                group.Name = name;
            }

            var actions = group.ActionIds;

            if (!actions.Any(a => a == 0))
            {
                actions.Add(0);
            }

            for (int j = 0; j < actions.Count; j++)
            {
                var action = actions[j];

                void Delete()
                {
                    actions.RemoveAt(j);
                }

                var popUpKey = $"ActionPopup{group.GetHashCode()}{j}";
                ConditionDrawer.ActionSelectorPopUp(popUpKey, _actionGrpActionsList, DataCenter.RightNowRotation, item => actions[j] = item.ID);

                if (((ActionID)action).GetTexture(out var icon) || ImageLoader.GetTexture(4, out icon))
                {
                    ImGui.SameLine();
                    var cursor = ImGui.GetCursorPos();

                    var key = $"ActionPopupActionDelete{group.GetHashCode()}{j}";

                    if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * ConditionDrawer.IconSize, $"ActionIcon{group.GetHashCode()}{j}"))
                    {
                        if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
                    }

                    ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                        (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]));

                    ImGuiHelper.DrawActionOverlay(cursor, ConditionDrawer.IconSize, 1);

                    ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                        (Delete, [VirtualKey.DELETE]));
                }
            }
        }
    }

    private static void DrawListStatuses()
    {
        ImGui.SetNextItemWidth(ImGui.GetWindowWidth());
        ImGui.InputTextWithHint("##Searching the action", UiString.ConfigWindow_List_StatusNameOrId.Local(), ref _statusSearching, 128);

        using var table = ImRaii.Table("Rotation Solver List Statuses", 4, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame);
        if (table)
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_Invincibility.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_Priority.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_DangerousStatus.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_NoCastingStatus.Local());

            ImGui.TableNextRow();

            ImGui.TableNextColumn();

            ImGui.TextWrapped(UiString.ConfigWindow_List_InvincibilityDesc.Local());
            DrawStatusList(nameof(OtherConfiguration.InvincibleStatus), OtherConfiguration.InvincibleStatus, RotationConfigWindow.AllStatus);

            ImGui.TableNextColumn();

            ImGui.TextWrapped(UiString.ConfigWindow_List_PriorityDesc.Local());
            DrawStatusList(nameof(OtherConfiguration.PriorityStatus), OtherConfiguration.PriorityStatus, RotationConfigWindow.AllStatus);

            ImGui.TableNextColumn();

            ImGui.TextWrapped(UiString.ConfigWindow_List_DangerousStatusDesc.Local());
            DrawStatusList(nameof(OtherConfiguration.DangerousStatus), OtherConfiguration.DangerousStatus, RotationConfigWindow.AllDispelStatus);

            ImGui.TableNextColumn();

            ImGui.TextWrapped(UiString.ConfigWindow_List_NoCastingStatusDesc.Local());
            DrawStatusList(nameof(OtherConfiguration.NoCastingStatus), OtherConfiguration.NoCastingStatus, RotationConfigWindow.BadStatus);
        }
    }

    private static void FromClipBoardButton(HashSet<uint> items)
    {
        if (ImGui.Button(UiString.ConfigWindow_Actions_Copy.Local()))
        {
            try
            {
                ImGui.SetClipboardText(JsonConvert.SerializeObject(items));
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "Failed to copy the values to the clipboard.");
            }
        }

        ImGui.SameLine();

        if (ImGui.Button(UiString.ActionSequencer_FromClipboard.Local()))
        {
            try
            {
                foreach (var aId in JsonConvert.DeserializeObject<uint[]>(ImGui.GetClipboardText())!)
                {
                    items.Add(aId);
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "Failed to copy the values from the clipboard.");
            }
            finally
            {
                OtherConfiguration.Save();
                ImGui.CloseCurrentPopup();
            }
        }
    }

    static string _statusSearching = string.Empty;
    private static void DrawStatusList(string name, HashSet<uint> statuses, Status[] allStatus)
    {
        using var id = ImRaii.PushId(name);
        FromClipBoardButton(statuses);

        uint removeId = 0;
        uint notLoadId = 10100;

        var popupId = "Rotation Solver Popup" + name;

        RotationConfigWindow.StatusPopUp(popupId, allStatus, ref _statusSearching, status =>
        {
            statuses.Add(status.RowId);
            OtherConfiguration.Save();
        }, notLoadId);

        var count = Math.Max(1, (int)MathF.Floor(ImGui.GetColumnWidth() / ((24 * Scale) + ImGui.GetStyle().ItemSpacing.X)));
        var index = 0;

        if (ImageLoader.GetTexture(16220, out var text))
        {
            if (index++ % count != 0)
            {
                ImGui.SameLine();
            }
            if (ImGuiHelper.NoPaddingNoColorImageButton(text.ImGuiHandle, new Vector2(24, 32) * Scale, name))
            {
                if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
            }
            ImguiTooltips.HoveredTooltip(UiString.ConfigWindow_List_AddStatus.Local());
        }

        foreach (var status in statuses.Select(a => Service.GetSheet<Status>().GetRow(a))
            .Where(a => a != null)
            .OrderByDescending(s => Searchable.Similarity(s!.Name + " " + s.RowId.ToString(), _statusSearching)))
        {
            void Delete() => removeId = status.RowId;

            var key = "Status" + status!.RowId.ToString();

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]));

            if (ImageLoader.GetTexture(status.Icon, out var texture, notLoadId))
            {
                if (index++ % count != 0)
                {
                    ImGui.SameLine();
                }
                ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, new Vector2(24, 32) * Scale, "Status" + status.RowId.ToString());

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, $"{status.Name} ({status.RowId})", false,
                    (Delete, [VirtualKey.DELETE]));
            }
        }

        if (removeId != 0)
        {
            statuses.Remove(removeId);
            OtherConfiguration.Save();
        }
    }

    private static void DrawListActions(ConfigWindow window)
    {
        ImGui.SetNextItemWidth(ImGui.GetWindowWidth());
        ImGui.InputTextWithHint("##Searching the action", UiString.ConfigWindow_List_ActionNameOrId.Local(), ref _actionSearching, 128);

        using var table = ImRaii.Table("Rotation Solver List Actions", 3, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame);
        if (table)
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_HostileCastingTank.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_HostileCastingArea.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_HostileCastingKnockback.Local());

            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            ImGui.TextWrapped(UiString.ConfigWindow_List_HostileCastingTankDesc.Local());
            DrawActionsList(nameof(OtherConfiguration.HostileCastingTank), OtherConfiguration.HostileCastingTank);

            ImGui.TableNextColumn();
            window.Collection.DrawItems((int)UiString.ConfigWindow_List_HostileCastingArea);
            ImGui.TextWrapped(UiString.ConfigWindow_List_HostileCastingAreaDesc.Local());
            DrawActionsList(nameof(OtherConfiguration.HostileCastingArea), OtherConfiguration.HostileCastingArea);

            ImGui.TableNextColumn();
            window.Collection.DrawItems((int)UiString.ConfigWindow_List_HostileCastingKnockback);
            ImGui.TextWrapped(UiString.ConfigWindow_List_HostileCastingKnockbackDesc.Local());
            DrawActionsList(nameof(OtherConfiguration.HostileCastingKnockback), OtherConfiguration.HostileCastingKnockback);
        }
    }

    private static string _actionSearching = string.Empty;
    private static void DrawActionsList(string name, HashSet<uint> actions)
    {
        using var id = ImRaii.PushId(name);

        uint removeId = 0;

        var popupId = "Rotation Solver Action Popup" + name;

        if (ImGui.Button(UiString.ConfigWindow_List_AddAction.Local() + "##" + name))
        {
            if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
        }

        ImGui.SameLine();
        FromClipBoardButton(actions);

        ImGui.Spacing();

        foreach (var action in actions.Select(a => Service.GetSheet<GAction>().GetRow(a))
            .Where(a => a != null)
            .OrderByDescending(s => Searchable.Similarity(s!.Name + " " + s.RowId.ToString(), _actionSearching)))
        {
            void Reset() => removeId = action.RowId;

            var key = "Action" + action!.RowId.ToString();

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (UiString.ConfigWindow_List_Remove.Local(), Reset, ["Delete"]));

            ImGui.Selectable($"{action.Name} ({action.RowId})");

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false, (Reset, [VirtualKey.DELETE]));
        }

        if (removeId != 0)
        {
            actions.Remove(removeId);
            OtherConfiguration.Save();
        }

        using var popup = ImRaii.Popup(popupId);
        if (popup)
        {
            ImGui.SetNextItemWidth(200 * Scale);
            ImGui.InputTextWithHint("##Searching the action pop up", UiString.ConfigWindow_List_ActionNameOrId.Local(), ref _actionSearching, 128);

            ImGui.Spacing();

            using var child = ImRaii.Child("Rotation Solver Add action", new Vector2(-1, 400 * Scale));
            if (child)
            {
                foreach (var action in RotationConfigWindow.AllActions.OrderByDescending(s => Searchable.Similarity(s.Name + " " + s.RowId.ToString(), _actionSearching)))
                {
                    var selected = ImGui.Selectable($"{action.Name} ({action.RowId})");
                    if (ImGui.IsItemHovered())
                    {
                        ImguiTooltips.ShowTooltip($"{action.Name} ({action.RowId})");
                        if (selected)
                        {
                            actions.Add(action.RowId);
                            OtherConfiguration.Save();
                            ImGui.CloseCurrentPopup();
                        }
                    }
                }
            }
        }
    }

    private static void DrawListTerritories()
    {
        if (Svc.ClientState == null) return;

        var territoryId = Svc.ClientState.TerritoryType;

        ImGuiHelperRS.DrawTerritoryHeader();
        ImGuiHelperRS.DrawContentFinder(DataCenter.ContentFinder);

        using var table = ImRaii.Table("Rotation Solver List Territories", 3, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame);
        if (table)
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_NoHostile.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_NoProvoke.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_List_BeneficialPositions.Local());

            ImGui.TableNextRow();

            ImGui.TableNextColumn();

            ImGui.TextWrapped(UiString.ConfigWindow_List_NoHostileDesc.Local());

            var width = ImGui.GetColumnWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - (10 * Scale);

            if (!OtherConfiguration.NoHostileNames.TryGetValue(territoryId, out var libs))
            {
                OtherConfiguration.NoHostileNames[territoryId] = libs = [];
            }

            //Add one.
            if (!libs.Any(string.IsNullOrEmpty))
            {
                OtherConfiguration.NoHostileNames[territoryId] = [.. libs, string.Empty];
            }

            int removeIndex = -1;
            for (int i = 0; i < libs.Length; i++)
            {
                ImGui.SetNextItemWidth(width);
                if (ImGui.InputTextWithHint($"##Rotation Solver Territory Target Name {i}", UiString.ConfigWindow_List_NoHostilesName.Local(), ref libs[i], 1024))
                {
                    OtherConfiguration.NoHostileNames[territoryId] = libs;
                    OtherConfiguration.SaveNoHostileNames();
                }
                ImGui.SameLine();

                if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove Territory Target Name {i}"))
                {
                    removeIndex = i;
                }
            }
            if (removeIndex > -1)
            {
                var list = libs.ToList();
                list.RemoveAt(removeIndex);
                OtherConfiguration.NoHostileNames[territoryId] = [.. list];
                OtherConfiguration.SaveNoHostileNames();
            }
            ImGui.TableNextColumn();

            ImGui.TextWrapped(UiString.ConfigWindow_List_NoProvokeDesc.Local());

            width = ImGui.GetColumnWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - (10 * Scale);

            if (!OtherConfiguration.NoProvokeNames.TryGetValue(territoryId, out libs))
            {
                OtherConfiguration.NoProvokeNames[territoryId] = libs = [];
            }
            //Add one.
            if (!libs.Any(string.IsNullOrEmpty))
            {
                OtherConfiguration.NoProvokeNames[territoryId] = [.. libs, string.Empty];
            }
            removeIndex = -1;
            for (int i = 0; i < libs.Length; i++)
            {
                ImGui.SetNextItemWidth(width);
                if (ImGui.InputTextWithHint($"##Rotation Solver Territory Provoke Name {i}", UiString.ConfigWindow_List_NoProvokeName.Local(), ref libs[i], 1024))
                {
                    OtherConfiguration.NoProvokeNames[territoryId] = libs;
                    OtherConfiguration.SaveNoProvokeNames();
                }
                ImGui.SameLine();

                if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove Territory Provoke Name {i}"))
                {
                    removeIndex = i;
                }
            }
            if (removeIndex > -1)
            {
                var list = libs.ToList();
                list.RemoveAt(removeIndex);
                OtherConfiguration.NoProvokeNames[territoryId] = [.. list];
                OtherConfiguration.SaveNoProvokeNames();
            }

            ImGui.TableNextColumn();

            if (!OtherConfiguration.BeneficialPositions.TryGetValue(territoryId, out var pts))
            {
                OtherConfiguration.BeneficialPositions[territoryId] = pts = [];
            }

            if (ImGui.Button(UiString.ConfigWindow_List_AddPosition.Local()) && Player.Available) unsafe
                {
                    var point = Player.Object.Position;
                    int* unknown = stackalloc int[] { 0x4000, 0, 0x4000, 0 };

                    RaycastHit hit = default;

                    OtherConfiguration.BeneficialPositions[territoryId]
                    =
                    [
                        .. pts,
                        FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->BGCollisionModule
                            ->RaycastEx(&hit, point + (Vector3.UnitY * 5), -Vector3.UnitY, 20, 1, unknown) ? hit.Point : point,
                    ];
                    OtherConfiguration.SaveBeneficialPositions();
                }

            HoveredPosition = Vector3.Zero;
            removeIndex = -1;
            for (int i = 0; i < pts.Length; i++)
            {
                void Reset() => removeIndex = i;

                var key = "Beneficial Positions" + i.ToString();

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (UiString.ConfigWindow_List_Remove.Local(), Reset, ["Delete"]));

                ImGui.Selectable(pts[i].ToString());

                if (ImGui.IsItemHovered())
                {
                    HoveredPosition = pts[i];
                }

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false, (Reset, [VirtualKey.DELETE]));
            }
            if (removeIndex > -1)
            {
                var list = pts.ToList();
                list.RemoveAt(removeIndex);
                OtherConfiguration.BeneficialPositions[territoryId] = [.. list];
                OtherConfiguration.SaveBeneficialPositions();
            }
        }
    }
    #endregion

}