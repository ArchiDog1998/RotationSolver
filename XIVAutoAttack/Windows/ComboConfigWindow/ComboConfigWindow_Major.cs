using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Script;
using XIVAutoAttack.Data;
using XIVAutoAttack.SigReplacers;

namespace XIVAutoAttack.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow : Window
{
    public ComboConfigWindow()
        : base("自动攻击设置 (开源免费) v" + typeof(ComboConfigWindow).Assembly.GetName().Version.ToString(), 0, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;
    }

    private static readonly Dictionary<JobRole, string> _roleDescriptionValue = new Dictionary<JobRole, string>()
    {
        {JobRole.Tank, $"{DescType.单体防御} → {CustomCombo<Enum>.Rampart}, {CustomCombo<Enum>.Reprisal}" },
        {JobRole.Melee, $"{DescType.范围防御} → {CustomCombo<Enum>.Feint}" },
        {JobRole.RangedMagicial, $"法系{DescType.范围防御} → {CustomCombo<Enum>.Addle}" },
    };



    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("AutoAttackSettings"))
        {
#if DEBUG
            if (Service.ClientState.LocalPlayer != null && ImGui.BeginTabItem("Debug查看"))
            {
                DrawDebug();
                ImGui.EndTabItem();
            }
#endif

            if (ImGui.BeginTabItem("关于"))
            {
                DrawAbout();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("攻击设定"))
            {
                DrawAttack();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("参数设定"))
            {
                DrawParam();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("技能释放事件"))
            {
                DrawEvent();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("技能释放条件"))
            {
                ImGui.Text("在这个窗口，你可以设定每个技能的释放条件。");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


                if (ImGui.BeginChild("条件列表", new Vector2(0f, -1f), true))
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

            if (ImGui.BeginTabItem("帮助文档"))
            {
                ImGui.Text("在这个窗口，你可以看到战斗用宏，设置用请在设置面板中查看。");

                if (ImGui.BeginChild("帮助", new Vector2(0f, -1f), true))
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
                    CommandHelp("AttackSmart", "如果不在进攻中就开始进攻，如果在进攻就切换选择敌对目标条件。");
                    ImGui.Separator();
                    CommandHelp("AttackManual", "开始进攻，进攻对象为手动选择，此时不会释放AOE。");
                    ImGui.Separator();
                    CommandHelp("AttackCancel", "停止进攻，记得一定要经常关掉！");
                    ImGui.Separator();
                    CommandHelp("HealArea", "开启一段范围治疗的窗口期。");
                    ImGui.Separator();
                    CommandHelp("HealSingle", "开启一段单体治疗的窗口期。");
                    ImGui.Separator();
                    CommandHelp("DefenseArea", "开启一段范围防御的窗口期。");
                    ImGui.Separator();
                    CommandHelp("DefenseSingle", "开启一段单体防御的窗口期。");
                    ImGui.Separator();
                    CommandHelp("EsunaShield", "开启一段康复或者盾姿或者真北的窗口期。");
                    ImGui.Separator();
                    CommandHelp("RaiseShirk", "开启强制救人或退避的窗口期。");
                    ImGui.Separator();
                    CommandHelp("AntiRepulsion", "开启一段防击退的窗口期。");
                    ImGui.Separator();
                    CommandHelp("BreakProvoke", "开启一段爆发或挑衅的窗口期。");
                    ImGui.Separator();
                    CommandHelp("Move", "开启一段位移的窗口期。");
                    ImGui.Separator();
                }
                ImGui.PopStyleVar();

                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }



    internal static void DrawTexture<T>(T texture, Action otherThing, ClassJobID jobId = 0, string[] authors = null) where T : class, IEnableTexture
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

        ImGui.Columns(2, texture.Name, false);

        var t = texture.GetTexture();

        ImGui.SetColumnWidth(0, t.Width + 5);

        var str = texture.Description;

        ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
        }

        ImGui.NextColumn();

        bool enable = texture.IsEnabled;

        if (ImGui.Checkbox(texture.Name, ref enable))
        {
            texture.IsEnabled = enable;
            Service.Configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
        }


        ImGui.SameLine();

        if (!string.IsNullOrEmpty(texture.Author))
        {
            authors ??= new string[] { texture.Author };

            int i;
            for (i = 0; i < authors.Length; i++)
            {
                if (authors[i] == texture.Author)
                {
                    break;
                }
            }

            Spacing();
            ImGui.TextDisabled("-  ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(ImGui.CalcTextSize(authors[i]).X + 30);
            if (ImGui.Combo("##" + texture.Name + "作者", ref i, authors, authors.Length))
            {
                Service.Configuration.ComboChoices[(uint)jobId] = authors[i];
                Service.Configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("点击以切换作者");
            }
        }

        ImGui.SameLine();
        Spacing();

        if (texture is ICustomCombo com)
        {
            if (texture is IScriptCombo script)
            {
                if (ImGuiComponents.IconButton(FontAwesomeIcon.Edit))
                {
                    XIVAutoAttackPlugin.OpenScriptWindow(script);
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("点击以编辑该自定义Combo");
                }
            }
            else
            {
                if (ImGuiComponents.IconButton(texture.GetHashCode(), FontAwesomeIcon.Globe))
                {
                    var url = @"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/" + texture.GetType().FullName.Replace(".", @"/") + ".cs";
                    Process.Start("cmd", $"/C start {url}");
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("打开源码网址");
                }
            }

            ImGui.SameLine();
            Spacing();

            if (Directory.Exists(Service.Configuration.ScriptComboFolder)
                && ImGuiComponents.IconButton(texture.GetHashCode() + 1, FontAwesomeIcon.Plus))
            {
                IconReplacer.AddScripCombo(com.JobIDs[0]);
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("添加一个自定义Combo");
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
        command = XIVAutoAttackPlugin._autoCommand + " " + command;
        if (ImGui.Button(command))
        {
            Service.CommandManager.ProcessCommand(command);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"单击以执行命令: {command}");
        }

        if (!string.IsNullOrEmpty(help))
        {
            ImGui.SameLine();
            ImGui.Text(" → " + help);
        }
    }
    private unsafe static void DrawAction(BaseAction act)
    {
        if (act == null) return;

        DrawTexture(act, () =>
        {
#if DEBUG
            ImGui.NewLine();
            ImGui.Text("Have One:" + act.HaveOneChargeDEBUG.ToString());
            ImGui.Text("Is Real GCD: " + act.IsRealGCD.ToString());
            ImGui.Text("Recast One: " + act.RecastTimeOneChargeDEBUG.ToString());
            ImGui.Text("Recast Elapsed: " + act.RecastTimeElapsedDEBUG.ToString());
            ImGui.Text("Recast Remain: " + act.RecastTimeRemainDEBUG.ToString());
            ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Spell, act.AdjustedID).ToString());

            ImGui.Text("Cast Time: " + act.CastTime.ToString());
            ImGui.Text("MP: " + act.MPNeed.ToString());
            ImGui.Text($"Can Use: {act.ShouldUse(out _)} {act.ShouldUse(out _, mustUse: true)}");

            ImGui.Text("IsUnlocked: " + UIState.Instance()->IsUnlockLinkUnlocked(act.AdjustedID).ToString());
#endif
        });
    }
}
