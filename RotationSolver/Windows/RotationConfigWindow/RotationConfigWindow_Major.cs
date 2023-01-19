using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using RotationSolver.Actions;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Combos.Script;
using RotationSolver.SigReplacers;
using RotationSolver.Data;
using RotationSolver.Localization;
using RotationSolver.Commands;
using RotationSolver.Helpers;

namespace RotationSolver.Windows.RotationConfigWindow;

internal partial class RotationConfigWindow : Window
{
    const float DRAG_NUMBER_WIDTH = 100;

    public RotationConfigWindow()
        : base(nameof(RotationConfigWindow), 0, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;
    }

    private static readonly Dictionary<JobRole, string> _roleDescriptionValue = new Dictionary<JobRole, string>()
    {
        {JobRole.Tank, $"{DescType.DefenseSingle.ToName()} ¡ú {CustomRotation.Rampart}, {CustomRotation.Reprisal}" },
        {JobRole.Melee, $"{DescType.DefenseArea.ToName()} ¡ú {CustomRotation.Feint}" },
        {JobRole.RangedMagicial, $"{DescType.DefenseArea.ToName()} ¡ú {CustomRotation.Addle}" },
    };

    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("RotationSolverSettings"))
        {
#if DEBUG
            if (Service.ClientState.LocalPlayer != null && ImGui.BeginTabItem("Debug"))
            {
                DrawDebugTab();
                ImGui.EndTabItem();
            }
#endif
            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_RotationItem))
            {
                DrawRotationTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ParamItem))
            {
                DrawParamTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_EventItem))
            {
                DrawEventTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ActionItem))
            {
                DrawActionTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_HelpItem))
            {
                DrawHelpTab();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }


    private static void DrawTexture<T>(T texture, Action otherThing, ClassJobID jobId = 0, ICustomRotation[] combos = null) where T : class, IEnableTexture
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

        ImGui.Columns(2, texture.Name, false);

        var t = texture.GetTexture();

        ImGui.SetColumnWidth(0, t.Width + 5);

        var com = texture as ICustomRotation;

        var str = com?.Description;

        ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
        }

        ImGui.NextColumn();

        bool enable = texture.IsEnabled;

        if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_AttackItem_Loop + "##" + texture.Name, ref enable))
        {
            texture.IsEnabled = enable;
            Service.Configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
        }

        if (com != null)
        {
            if (!string.IsNullOrEmpty(com.RotationName) && combos != null)
            {
                ImGui.SameLine();
                ImGui.TextDisabled("  -  ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.CalcTextSize(com.RotationName).X + 30);
                if (ImGui.BeginCombo("##" + com.Name + "Author", com.RotationName))
                {
                    foreach (var c in combos)
                    {
                        if (ImGui.Selectable(c.RotationName))
                        {
                            Service.Configuration.ComboChoices[(uint)jobId] = c.RotationName;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip(c.Description);
                        }
                    }
                    ImGui.EndCombo();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Helper_SwitchAuthor);
                }

                ImGui.SameLine();
                ImGui.TextDisabled("    -  " + LocalizationManager.RightLang.Configwindow_Helper_GameVersion + ":  ");
                ImGui.SameLine();
                ImGui.Text(com.GameVersion);
            }

            ImGui.SameLine();
            ImGuiHelper.Spacing();


            if (texture is IScriptCombo script)
            {
                ImGui.PushFont(UiBuilder.IconFont);

                if (ImGui.Button($"{FontAwesomeIcon.Edit.ToIconString()}##{texture.GetHashCode()}"))
                {
                    RotationSolverPlugin.OpenScriptWindow(script);
                }
                ImGui.PopFont();

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Helper_EditCombo);
                }
            }
            else
            {
                ImGui.PushFont(UiBuilder.IconFont);

                if (ImGui.Button($"{FontAwesomeIcon.Globe.ToIconString()}##{texture.GetHashCode()}"))
                {
                    var url = @"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/" + texture.GetType().FullName.Replace(".", @"/") + ".cs";

                    Util.OpenLink(url);
                }
                ImGui.PopFont();

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Helper_OpenSource);
                }
            }

            ImGui.SameLine();
            ImGuiHelper.Spacing();
            ImGui.PushFont(UiBuilder.IconFont);

            if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##{texture.GetHashCode() + 1}"))
            {
                var newCom = IconReplacer.AddScripCombo(com.JobIDs[0]);
                if (newCom != null)
                {
                    Service.Configuration.ComboChoices[(uint)jobId] = newCom.RotationName;
                    Service.Configuration.Save();
                }
            }
            ImGui.PopFont();

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Helper_AddCombo);
            }

        }

        if (enable)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1f, 1f));
            otherThing?.Invoke();
            ImGui.PopStyleVar();
        }
        ImGui.Columns(1);
        ImGui.PopStyleVar();
    }


    private unsafe static void DrawAction(IAction act)
    {
        if (act == null) return;

        DrawTexture(act, () =>
        {
            if (act is BaseAction baseAct)
            {
                //if (baseAct.IsTimeline) CommandHelp($"Insert{act}-{5}",
                //    string.Format(LocalizationManager.RightLang.Configwindow_Helper_InsertCommand, act));
#if DEBUG
                ImGui.Text("Have One:" + baseAct.HaveOneChargeDEBUG.ToString());
                ImGui.Text("Is Real GCD: " + baseAct.IsRealGCD.ToString());
                ImGui.Text("Recast One: " + baseAct.RecastTimeOneChargeDEBUG.ToString());
                ImGui.Text("Recast Elapsed: " + baseAct.RecastTimeElapsedDEBUG.ToString());
                ImGui.Text("Recast Remain: " + baseAct.RecastTimeRemainDEBUG.ToString());
                ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Spell, baseAct.AdjustedID).ToString());

                ImGui.Text("Cast Time: " + baseAct.CastTime.ToString());
                ImGui.Text("MP: " + baseAct.MPNeed.ToString());
                ImGui.Text($"Can Use: {baseAct.ShouldUse(out _)} ");
                ImGui.Text("Must Use:" + baseAct.ShouldUse(out _, mustUse: true).ToString());
                ImGui.Text("Empty Use:" + baseAct.ShouldUse(out _, emptyOrSkipCombo: true).ToString());
                ImGui.Text("IsUnlocked: " + UIState.Instance()->IsUnlockLinkUnlocked(act.AdjustedID).ToString());
                if (baseAct.Target != null)
                {
                    ImGui.Text("Target Name: " + baseAct.Target.Name);
                }
#endif
            }
        });
    }
}
