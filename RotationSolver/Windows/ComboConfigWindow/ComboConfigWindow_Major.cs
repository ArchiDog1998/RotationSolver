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
using XIVAutoAction;
using XIVAutoAction.Actions;
using XIVAutoAction.Combos.CustomCombo;
using XIVAutoAction.Combos.Script;
using XIVAutoAction.Data;
using XIVAutoAction.Localization;
using XIVAutoAction.SigReplacers;
using XIVAutoAction.Actions.BaseAction;
using XIVAutoAction.Actions.BaseCraftAction;

namespace XIVAutoAction.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow : Window
{
    const float DRAG_NUMBER_WIDTH = 100;

    public ComboConfigWindow()
        : base(nameof(ComboConfigWindow), 0, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;
    }

    private static readonly Dictionary<JobRole, string> _roleDescriptionValue = new Dictionary<JobRole, string>()
    {
        {JobRole.Tank, $"{DescType.DefenseSingle.ToName()} ¡ú {CustomCombo.Rampart}, {CustomCombo.Reprisal}" },
        {JobRole.Melee, $"{DescType.DefenseArea.ToName()} ¡ú {CustomCombo.Feint}" },
        {JobRole.RangedMagicial, $"{DescType.DefenseArea.ToName()} ¡ú {CustomCombo.Addle}" },
    };



    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("AutoAttackSettings"))
        {
#if DEBUG
            if (Service.ClientState.LocalPlayer != null && ImGui.BeginTabItem("Debug"))
            {
                DrawDebug();
                ImGui.EndTabItem();
            }
#endif

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_AboutItem))
            {
                DrawAbout();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_AttackItem))
            {
                DrawAttack();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ParamItem))
            {
                DrawParam();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_EventsItem))
            {
                DrawEvent();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ActionsItem))
            {
                ImGui.Text(LocalizationManager.RightLang.ConfigWindow_ActionItem_Description);

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


                if (ImGui.BeginChild("Action List", new Vector2(0f, -1f), true))
                {
                    foreach (var pair in IconReplacer.RightComboBaseActions.GroupBy(a => a.CateName).OrderBy(g => g.Key))
                    {
                        if (ImGui.CollapsingHeader(pair.Key))
                        {
                            foreach (var item in pair)
                            {
                                DrawAction(item);
                                ImGui.Separator();
                            }
                        }
                    }
                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_HelpItem))
            {
                ImGui.Text(LocalizationManager.RightLang.ConfigWindow_HelpItem_Description);

                if (ImGui.BeginChild("Help Infomation", new Vector2(0f, -1f), true))
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                    CommandHelp("AttackSmart", LocalizationManager.RightLang.Configwindow_HelpItem_AttackSmart);
                    ImGui.Separator();

                    CommandHelp("AttackManual", LocalizationManager.RightLang.Configwindow_HelpItem_AttackManual);
                    ImGui.Separator();

                    CommandHelp("AttackCancel", LocalizationManager.RightLang.Configwindow_HelpItem_AttackCancel);
                    ImGui.Separator();

                    CommandHelp("HealArea", LocalizationManager.RightLang.Configwindow_HelpItem_HealArea);
                    ImGui.Separator();

                    CommandHelp("HealSingle", LocalizationManager.RightLang.Configwindow_HelpItem_HealSingle);
                    ImGui.Separator();

                    CommandHelp("DefenseArea", LocalizationManager.RightLang.Configwindow_HelpItem_DefenseArea);
                    ImGui.Separator();

                    CommandHelp("DefenseSingle", LocalizationManager.RightLang.Configwindow_HelpItem_DefenseSingle);
                    ImGui.Separator();

                    CommandHelp("EsunaShield", LocalizationManager.RightLang.Configwindow_HelpItem_EsunaShield);
                    ImGui.Separator();

                    CommandHelp("RaiseShirk", LocalizationManager.RightLang.Configwindow_HelpItem_RaiseShirk);
                    ImGui.Separator();

                    CommandHelp("AntiRepulsion", LocalizationManager.RightLang.Configwindow_HelpItem_AntiRepulsion);
                    ImGui.Separator();

                    CommandHelp("Break", LocalizationManager.RightLang.Configwindow_HelpItem_Break);
                    ImGui.Separator();

                    CommandHelp("Move", LocalizationManager.RightLang.Configwindow_HelpItem_Move);
                    ImGui.Separator();

                    CommandHelp("EndSpecial", LocalizationManager.RightLang.Configwindow_HelpItem_EndSpecial);
                    ImGui.Separator();
                }
                ImGui.PopStyleVar();

                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }



    internal static void DrawTexture<T>(T texture, Action otherThing, ClassJobID jobId = 0, ICustomCombo[] combos = null) where T : class, IEnableTexture
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

        ImGui.Columns(2, texture.Name, false);

        var t = texture.GetTexture();

        ImGui.SetColumnWidth(0, t.Width + 5);

        var com = texture as ICustomCombo;

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
            if (!string.IsNullOrEmpty(com.Author) && combos != null)
            {
                ImGui.SameLine();
                ImGui.TextDisabled("  -  ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.CalcTextSize(com.Author).X + 30);
                if (ImGui.BeginCombo("##" + com.Name + "Author", com.Author))
                {
                    foreach (var c in combos)
                    {
                        if (ImGui.Selectable(c.Author))
                        {
                            Service.Configuration.ComboChoices[(uint)jobId] = c.Author;
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
            Spacing();


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
            Spacing();
            ImGui.PushFont(UiBuilder.IconFont);

            if (Directory.Exists(Service.Configuration.ScriptComboFolder)
                && ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##{texture.GetHashCode() + 1}"))
            {
                var newCom = IconReplacer.AddScripCombo(com.JobIDs[0]);
                if (newCom != null)
                {
                    Service.Configuration.ComboChoices[(uint)jobId] = newCom.Author;
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

    internal static void Spacing(byte count = 1)
    {
        string s = string.Empty;
        for (int i = 0; i < count; i++)
        {
            s += "    ";
        }
        ImGui.Text(s);
        ImGui.SameLine();
    }

    private static void CommandHelp(string command, string help = null)
    {
        command = RotationSolverPlugin._autoCommand + " " + command;
        if (ImGui.Button(command))
        {
            Service.CommandManager.ProcessCommand(command);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"{LocalizationManager.RightLang.Configwindow_Helper_RunCommand}: {command}\n{LocalizationManager.RightLang.Configwindow_Helper_CopyCommand}: {command}");

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                ImGui.SetClipboardText(command);
            }
        }

        if (!string.IsNullOrEmpty(help))
        {
            ImGui.SameLine();
            ImGui.Text(" ¡ú " + help);
        }
    }
    private unsafe static void DrawAction(IAction act)
    {
        if (act == null) return;

        DrawTexture(act, () =>
        {
            if (act is BaseAction baseAct)
            {
                if (baseAct.IsTimeline) CommandHelp($"Insert{act}-{5}",
                    string.Format(LocalizationManager.RightLang.Configwindow_Helper_InsertCommand, act));
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
            else if (act is BaseCraftAction craft)
            {
#if DEBUG
                ImGui.Text("Cost: " + craft.CPCost.ToString());
                ImGui.Text("Progress: " + craft.Progress.ToString());
                ImGui.Text("ProgressBase: " + craft.ProgressBase.ToString());
                ImGui.Text("Quality: " + craft.Quality.ToString());
                ImGui.Text("QualityBase: " + craft.QualityBase.ToString());
#endif
            }
        });
    }
}
