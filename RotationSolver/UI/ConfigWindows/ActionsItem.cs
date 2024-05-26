﻿using Dalamud.Interface.Utility.Raii;
using RotationSolver.Updaters;
using System.ComponentModel;
using XIVConfigUI;
using XIVConfigUI.Attributes;

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

        ImGui.TextWrapped(UiString.ConfigWindow_Actions_ConditionDescription.Local());
        _sequencerList?.Draw();

        static void DrawConfigsOfAction()
        {
            if (_activeAction == null) return;

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

            if (_activeAction is IBaseAction a)
            {
                DrawConfigsOfBaseAction(a);
            }

            ImGui.Separator();

            static void DrawConfigsOfBaseAction(IBaseAction a)
            {
                var config = a.Config;

                if (Service.Config.MistakeRatio > 0
                    && !a.Setting.IsFriendly
                    && a.Setting.TargetType != TargetType.Move)
                {
                    var enable = config.IsInMistake;
                    if (ImGui.Checkbox($"{UiString.ConfigWindow_Actions_IsInMistake.Local()}##{a.Name}InMistake", ref enable))
                    {
                        config.IsInMistake = enable;
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
                ImguiTooltips.HoveredTooltip(ConfigUnitType.Seconds.Local());

                var ttu = config.TimeToUntargetable;
                ImGui.SetNextItemWidth(Scale * 150);
                if (ImGui.DragFloat($"{UiString.ConfigWindow_Actions_TTU.Local()}##{a}",
                    ref ttu, 0.1f, 0, 120, $"{ttu:F2}{ConfigUnitType.Seconds.ToSymbol()}"))
                {
                    config.TimeToUntargetable = ttu;
                }
                ImguiTooltips.HoveredTooltip(ConfigUnitType.Seconds.Local());

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

                var ratio = config.AutoHealRatio;
                ImGui.SetNextItemWidth(Scale * 150);
                if (ImGui.DragFloat($"{UiString.ConfigWindow_Actions_HealRatio.Local()}##{a}",
                    ref ratio, 0.002f, 0, 1, $"{ratio * 100:F1}{ConfigUnitType.Percent.ToSymbol()}"))
                {
                    config.AutoHealRatio = ratio;
                }
                ImguiTooltips.HoveredTooltip(ConfigUnitType.Percent.Local());

            }
        }

        static void DrawActionDebug()
        {
            if (!Service.Config.InDebug) return;

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

                var rotation = DataCenter.RightNowRotation;
                var set = DataCenter.RightSet;

                if (set == null || _activeAction == null || rotation == null) return;
                set.GetCondition(_activeAction.ID)?.DrawMain(rotation);
            } },

            { () => UiString.ConfigWindow_Actions_DisabledConditionSet.Local(), () =>
            {
                ImGui.TextWrapped(UiString.ConfigWindow_Actions_DisabledConditionSet_Description.Local());

                var rotation = DataCenter.RightNowRotation;
                var set = DataCenter.RightSet;

                if (set == null || _activeAction == null || rotation == null) return;
                set.GetDisabledCondition(_activeAction.ID)?.DrawMain(rotation);
            } },
        })
    {
        HeaderSize = FontSize.Fourth,
    };
}
