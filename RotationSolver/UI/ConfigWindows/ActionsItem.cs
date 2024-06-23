using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using RotationSolver.Updaters;
using System.ComponentModel;
using System.Drawing;
using XIVConfigUI;
using XIVConfigUI.Attributes;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.GroupPoseModule;

namespace RotationSolver.UI.ConfigWindows;
[Description("Actions")]
public class ActionsItem : ConfigWindowItemRS
{
    public override uint Icon => 4;

    public override string Description => UiString.Item_Actions.Local();

    public override unsafe void Draw(ConfigWindow window)
    {
        ImGui.TextWrapped(UiString.ConfigWindow_Actions_Description.Local());

        using var table = ImRaii.Table("Rotation Solver Actions", 2, ImGuiTableFlags.Resizable);

        if (!table) return;
        ImGui.TableSetupColumn("Action Column", ImGuiTableColumnFlags.WidthFixed, ImGui.GetWindowWidth() / 2);
        ImGui.TableNextColumn();

        if (_actionsList != null)
        {
            _actionsList.ClearCollapsingHeader();

            if (DataCenter.RightNowRotation != null && RotationUpdater.AllGroupedActions != null)
            {
                var size = 30 * Scale;
                var count = Math.Max(1, (int)MathF.Floor(ImGui.GetColumnWidth() / (size * 1.1f + ImGui.GetStyle().ItemSpacing.X)));
                foreach (var pair in RotationUpdater.AllGroupedActions)
                {
                    _actionsList.AddCollapsingHeader(() => pair.Key, () =>
                    {
                        var index = 0;
                        foreach (var item in pair.OrderBy(t => t.ID))
                        {
                            if (!item.GetTexture(out var icon)) continue;

                            if (index++ % count != 0)
                            {
                                ImGui.SameLine();
                            }

                            ImGui.BeginGroup();
                            var cursor = ImGui.GetCursorPos();
                            if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * size, item.Name + item.ID))
                            {
                                _activeAction = item;
                            }
                            ImGuiHelper.DrawActionOverlay(cursor, size, _activeAction == item ? 1 : 0);

                            if (ImageLoader.GetTexture("ui/uld/readycheck_hr1.tex", out var texture))
                            {
                                var offset = new Vector2(1 / 12f, 1 / 6f);
                                ImGui.SetCursorPos(cursor + new Vector2(0.6f, 0.7f) * size);
                                ImGui.Image(texture.ImGuiHandle, Vector2.One * size * 0.5f,
                                    new Vector2(item.IsEnabled ? 0 : 0.5f, 0) + offset,
                                    new Vector2(item.IsEnabled ? 0.5f : 1, 1) - offset);
                            }
                            ImGui.EndGroup();

                            string key = $"Action Macro Usage {item.Name} {item.ID}";
                            var cmd = ImGuiHelperRS.ToCommandStr(OtherCommandType.DoActions, $"{item}-{5}");
                            ImGuiHelper.DrawHotKeysPopup(key, cmd);
                            ImGuiHelper.ExecuteHotKeysPopup(key, cmd, item.Name, false);
                        }
                    });
                }
            }

            _actionsList.Draw();
        }

        ImGui.TableNextColumn();

        DrawConfigsOfAction();
        DrawActionDebug();

        _sequencerList?.Draw(UiString.ConfigWindow_Actions_ConditionDescription.Local());

        static void DrawConfigsOfAction()
        {
            if (_activeAction == null) return;

            var height = ImGui.GetCursorPosY();
            using (var grp = ImRaii.Group())
            {
                var enable = _activeAction.IsEnabled;
                if (ImGui.Checkbox($"{_activeAction.Name}##{_activeAction.Name} Enabled", ref enable))
                {
                    _activeAction.IsEnabled = enable;
                }

                const string key = "Action Enable Popup";
                var cmd = ImGuiHelperRS.ToCommandStr(OtherCommandType.ToggleActions, _activeAction.ToString()!);
                ImGuiHelper.DrawHotKeysPopup(key, cmd);
                ImGuiHelper.ExecuteHotKeysPopup(key, cmd, string.Empty, false);

                enable = _activeAction.IsInCD;
                if (ImGui.Checkbox($"{UiString.ConfigWindow_Actions_ShowOnCDWindow.Local()}##{_activeAction.Name}InCooldown", ref enable))
                {
                    _activeAction.IsInCD = enable;
                }
            }

            height = ImGui.GetCursorPosY() - height;

            var nextCursor = ImGui.GetCursorPos();
            if (_activeAction.GetTexture(out var icon))
            {
                ImGui.SameLine();
                var cursor = ImGui.GetCursorPos();
                var width = ImGui.GetColumnWidth();
                ImGui.SetCursorPos(cursor += new Vector2(width - height - 10 * Scale, 0));
                ImGui.Image(icon.ImGuiHandle, Vector2.One * height);
                ImGuiHelper.DrawActionOverlay(cursor, height, 1);
            }
            ImGui.SetCursorPos(nextCursor);

            if (_activeAction is IBaseAction a)
            {
                DrawConfigsOfBaseAction(a);
            }

            static void DrawConfigsOfBaseAction(IBaseAction a)
            {
                var config = a.Config;

                var custom = config.UseCustomTargetingData;
                if (ImGui.Checkbox($"{UiString.ConfigWindow_Actions_CustomTargetingData.Local()}##{a.Name}CustomTargetData", ref custom))
                {
                    config.UseCustomTargetingData = custom;
                }

                if (custom)
                {
                    ImGui.SameLine();

                    var names = Service.Config.TargetingWays.Select(i => i.TargetName).ToArray();
                    var index = Array.IndexOf(names, config.TargetingDataName);
                    if (ImGuiHelper.SelectableCombo($"{a.Name}CustomTargetDataPopup", names, ref index))
                    {
                        config.TargetingDataName = names[index];
                    }
                }

                if (Service.Config.MistakeRatio > 0
                    && !a.Setting.IsFriendly
                    && a.Setting.TargetType != TargetType.Move)
                {
                    var mistake = config.IsInMistake;
                    if (ImGui.Checkbox($"{UiString.ConfigWindow_Actions_IsInMistake.Local()}##{a.Name}InMistake", ref mistake))
                    {
                        config.IsInMistake = mistake;
                    }
                }

                ImGui.Separator();

                var ttk = config.TimeToKill;
                ImGui.SetNextItemWidth(Scale * 150);
                if (ImGui.DragFloat($"{UiString.ConfigWindow_Actions_TTK.Local()}##{a}",
                    ref ttk, 0.1f, 0, 120, $"{ttk:F2}{ConfigUnitType.Seconds.ToSymbol()}"))
                {
                    config.TimeToKill = ttk;
                }
                ImGuiHelper.HoveredTooltip(ConfigUnitType.Seconds.Local());

                var ttu = config.TimeToUntargetable;
                ImGui.SetNextItemWidth(Scale * 150);
                if (ImGui.DragFloat($"{UiString.ConfigWindow_Actions_TTU.Local()}##{a}",
                    ref ttu, 0.1f, 0, 120, $"{ttu:F2}{ConfigUnitType.Seconds.ToSymbol()}"))
                {
                    config.TimeToUntargetable = ttu;
                }
                ImGuiHelper.HoveredTooltip(ConfigUnitType.Seconds.Local());

                if (a.Setting.StatusProvide != null || a.Setting.TargetStatusProvide != null)
                {
                    var shouldStatus = config.ShouldCheckStatus;
                    if (ImGui.Checkbox($"{UiString.ConfigWindow_Actions_CheckStatus.Local()}##{a}", ref shouldStatus))
                    {
                        config.ShouldCheckStatus = shouldStatus;
                    }

                    if (shouldStatus)
                    {
                        var statusGcdCount = (int)config.StatusGcdCount;
                        ImGui.SetNextItemWidth(Scale * 150);
                        if (ImGui.DragInt($"{UiString.ConfigWindow_Actions_GcdCount.Local()}##{a}",
                            ref statusGcdCount, 0.05f, 1, 10))
                        {
                            config.StatusGcdCount = (byte)statusGcdCount;
                        }
                    }
                }

                if (!a.TargetInfo.IsSingleTarget)
                {
                    var aoeCount = (int)config.AoeCount;
                    ImGui.SetNextItemWidth(Scale * 150);
                    if (ImGui.DragInt($"{UiString.ConfigWindow_Actions_AoeCount.Local()}##{a}",
                        ref aoeCount, 0.05f, 1, 10))
                    {
                        config.AoeCount = (byte)aoeCount;
                    }
                }

                if (a.Setting.IsFriendly)
                {
                    var ratio = config.AutoHealRatio;
                    ImGui.SetNextItemWidth(Scale * 150);
                    if (ImGui.DragFloat($"{UiString.ConfigWindow_Actions_HealRatio.Local()}##{a}",
                        ref ratio, 0.002f, 0, 1, $"{ratio * 100:F1}{ConfigUnitType.Percent.ToSymbol()}"))
                    {
                        config.AutoHealRatio = ratio;
                    }
                    ImGuiHelper.HoveredTooltip(ConfigUnitType.Percent.Local());
                }
            }
        }

        static void DrawActionDebug()
        {
            if (!Service.Config.InDebug) return;
            ImGui.Separator();

            if (_activeAction is IBaseAction action)
            {

                try
                {
                    ImGui.Text("ID: " + action.Info.ID.ToString());

#if DEBUG
                    ImGui.Text("Is Real GCD: " + action.Info.IsRealGCD.ToString());
                    ImGui.Text("Resources: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->CheckActionResources(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Action, action.AdjustedID).ToString());
                    ImGui.Text("Status: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetActionStatus(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Action, action.AdjustedID).ToString());
                    ImGui.Text("Cast Time: " + action.Info.CastTime.ToString());
                    ImGui.Text("MP: " + action.Info.MPNeed.ToString());
#endif
                    ImGui.Text("AnimationLock: " + action.Info.AnimationLockTime.ToString());
                    ImGui.Text("AttackType: " + action.Info.AttackType.ToString());
                    ImGui.Text("Aspect: " + action.Info.Aspect.ToString());
                    ImGui.Text("Has One:" + action.CD.HasOneCharge.ToString());
                    ImGui.Text("Recast One: " + action.CD.RecastTimeOneChargeRaw.ToString());
                    ImGui.Text("Recast Elapsed: " + action.CD.RecastTimeElapsedRaw.ToString());
                    ImGui.Text($"Charges: {action.CD.CurrentCharges} / {action.CD.MaxCharges}");

                    ImGui.Text($"Can Use: {action.CanUse(out _, skipClippingCheck: true)} ");
                    ImGui.Text($"Why Can't: {action.WhyCant.Local()} ");
                    ImGui.Text("IgnoreCastCheck:" + action.CanUse(out _, skipClippingCheck: true, skipCastingCheck: true).ToString());
                    ImGui.Text($"Why Can't: {action.WhyCant.Local()} ");
                    ImGui.Text("Target Name: " + action.Target.Target?.Name ?? string.Empty);
                }
                catch
                {

                }
            }
            else if (_activeAction is IBaseItem item)
            {
                try
                {
                    ImGui.Text("Status: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetActionStatus(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Item, item.ID).ToString());
                    ImGui.Text("Status HQ: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetActionStatus(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Item, item.ID + 1000000).ToString());
                    var remain = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetRecastTime(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Item, item.ID) - FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetRecastTimeElapsed(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Item, item.ID);
                    ImGui.Text("remain: " + remain.ToString());
                    ImGui.Text("CanUse: " + item.CanUse(out _, true).ToString());

                    if (item is HpPotionItem healPotionItem)
                    {
                        ImGui.Text("MaxHP:" + healPotionItem.MaxHp.ToString());
                    }
                }
                catch
                {

                }
            }
        }
    }

    private static IAction? _activeAction;

    private static readonly CollapsingHeaderGroup _actionsList = new()
    {
        HeaderSize = FontSize.Fourth,
    };

    private static readonly CollapsingHeaderGroup _sequencerList = new(new()
        {
            { () => UiString.ConfigWindow_Actions_ForcedConditionSet.Local(), () =>
            {
                ImGui.TextWrapped(UiString.ConfigWindow_Actions_ForcedConditionSet_Description.Local());

                if (!DownloadHelper.IsSupporter)
                {
                    var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
                    ImGui.TextWrapped(UiString.SupporterOnlyWarning.Local());
                }
                var set = DataCenter.RightSet;

                if (set == null || _activeAction == null) return;
                XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(set.GetCondition(_activeAction.ID));
            } },

            { () => UiString.ConfigWindow_Actions_DisabledConditionSet.Local(), () =>
            {
                ImGui.TextWrapped(UiString.ConfigWindow_Actions_DisabledConditionSet_Description.Local());

                var set = DataCenter.RightSet;

                if (set == null || _activeAction == null) return;
                XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(set.GetDisabledCondition(_activeAction.ID));
            } },

            { () => _activeAction is IBaseAction ? UiString.ConfigWindow_Actions_PriorityTargeting.Local() : string.Empty, () =>
            {
                ImGui.TextWrapped(UiString.ConfigWindow_Actions_PriorityTargeting_Description.Local());

                if (_activeAction is not IBaseAction action) return;
                XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(action.Config.PriorityTargeting);
            } },

            { () => _activeAction is IBaseAction ? UiString.ConfigWindow_Actions_CantTargeting.Local() : string.Empty, () =>
            {
                ImGui.TextWrapped(UiString.ConfigWindow_Actions_CantTargeting_Description.Local());

                if (_activeAction is not IBaseAction action) return;
                XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(action.Config.CantTargeting);
            } },
        })
    {
        HeaderSize = FontSize.Fourth,
    };
}
