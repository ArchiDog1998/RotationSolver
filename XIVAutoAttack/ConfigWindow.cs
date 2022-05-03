using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using XIVComboPlus.Attributes;
using XIVComboPlus.Combos;

namespace XIVComboPlus;

internal class ConfigWindow : Window
{
    private readonly Vector4 shadedColor = new Vector4(0.68f, 0.68f, 0.68f, 1f);

    public ConfigWindow()
        : base("自动攻击设置", 0, false)
    {
        RespectCloseHotkey = true;

        SizeCondition = (ImGuiCond)4;
        Size = new Vector2(740f, 490f);
    }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("##tabbar"))
        {
            if (ImGui.BeginTabItem("攻击设定"))
            {
                ImGui.Text("在这个窗口，你可以设定自己喜欢的自动攻击设定。");

                ImGui.BeginChild("攻击", new Vector2(0f, -1f), true);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
                int num = 1;


                //ImGui.Text(IconReplacer.CustomCombosDict.Keys.ToString());
                foreach (string key in IconReplacer.CustomCombosDict.Keys)
                {
                    var combos = IconReplacer.CustomCombosDict[key];
                    if (combos == null || combos.Length == 0) continue;

                    if (ImGui.CollapsingHeader(key))
                    {
                        foreach (var combo in combos)
                        {
                            //ImGui.Text(combo.ComboFancyName);

                            bool enable = combo.IsEnabled;
                            ImGui.PushItemWidth(200f);
                            if (ImGui.Checkbox(combo.JobName, ref enable))
                            {
                                combo.IsEnabled = enable;
                                Service.Configuration.Save();
                            }
                            ImGui.PopItemWidth();
                            string text = $"#{num}: 替换沉静为{combo.JobName}的连续GCD战技、技能。";
                            if(!string.IsNullOrEmpty(combo.Description))
                            {
                                text += '\n' + combo.Description;
                            }
                            ImGui.TextColored(shadedColor, text);
                            //ImGui.Spacing();
                            //if (item == CustomComboPreset.DancerDanceComboCompatibility && enable)
                            //{
                            //    int[] array2 = Service.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();
                            //    if (false | ImGui.InputInt("Emboite (Red) ActionID", ref array2[0], 0) | ImGui.InputInt("Entrechat (Blue) ActionID", ref array2[1], 0) | ImGui.InputInt("Jete (Green) ActionID", ref array2[2], 0) | ImGui.InputInt("Pirouette (Yellow) ActionID", ref array2[3], 0))
                            //    {
                            //        Service.Configuration.DancerDanceCompatActionIDs = array2.Cast<uint>().ToArray();
                            //        Service.IconReplacer.UpdateEnabledActionIDs();
                            //        Service.Configuration.Save();
                            //    }
                            //    ImGui.Spacing();
                            //}
                            num++;
                        }
                    }
                    else
                    {
                        num += combos.Length;
                    }
                }

                ImGui.PopStyleVar();
                ImGui.EndChild();

                ImGui.EndTabItem();

            }


            if (ImGui.BeginTabItem("参数设定"))
            {
                //ImGui.Text(WARCombo.Actions.Overpower.Target.Name.ToString());

                //foreach (var item in Service.ClientState.LocalPlayer.StatusList)
                //{
                //    if (item.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                //    {
                //        ImGui.Text(item.GameData.Name + item.StatusId);
                //    }
                //}
                //ImGui.Text(Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty].ToString());
                //ImGui.Text(TargetHelper.Time.ToString());
                //if (TargetHelper.times.Count > 0)
                //    ImGui.Text(TargetHelper.times.Average().ToString());

                ImGui.Text("在这个窗口，你可以设定释放技能所需的参数。");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                if (ImGui.BeginChild("参数", new Vector2(0f, -1f), true))
                {
                    bool isAllTargetAsHostile = Service.Configuration.AllTargeAsHostile;
                    if (ImGui.Checkbox("是否设定所有可以攻击的目标均为敌对目标", ref isAllTargetAsHostile))
                    {
                        Service.Configuration.AllTargeAsHostile = isAllTargetAsHostile;
                        Service.Configuration.Save();
                    }

                    bool isOnlyGCD = Service.Configuration.OnlyGCD;
                    if (ImGui.Checkbox("是否只使用GCD循环，除去能力技", ref isOnlyGCD))
                    {
                        Service.Configuration.OnlyGCD = isOnlyGCD;
                        Service.Configuration.Save();
                    }

                    float specialDuration = Service.Configuration.SpecialDuration;
                    if (ImGui.DragFloat("特殊状态持续多久", ref specialDuration, 0.02f, 1, 20))
                    {
                        Service.Configuration.SpecialDuration = specialDuration;
                        Service.Configuration.Save();
                    }

                    ImGui.Separator();

                    int multiCount = Service.Configuration.HostileCount;
                    if (ImGui.DragInt("范围攻击最少需要多少人", ref multiCount, 0.02f, 2, 5))
                    {
                        Service.Configuration.HostileCount = multiCount;
                        Service.Configuration.Save();
                    }

                    int partyCount = Service.Configuration.PartyCount;
                    if (ImGui.DragInt("范围治疗最少需要多少人", ref partyCount, 0.02f, 2, 5))
                    {
                        Service.Configuration.PartyCount = partyCount;
                        Service.Configuration.Save();
                    }
                    ImGui.Separator();

                    float speed = 0.005f;
                    float healthDiff = Service.Configuration.HealthDifference;
                    if (ImGui.DragFloat("多少的HP标准差以下，可以用群疗", ref healthDiff, speed * 2, 0, 0.5f))
                    {
                        Service.Configuration.HealthDifference = healthDiff;
                        Service.Configuration.Save();
                    }


                    float healthAreaA = Service.Configuration.HealthAreaAbility;
                    if (ImGui.DragFloat("多少的HP，可以用能力技群疗", ref healthAreaA, speed, 0, 1))
                    {
                        Service.Configuration.HealthAreaAbility = healthAreaA;
                        Service.Configuration.Save();
                    }

                    float healthAreaS = Service.Configuration.HealthAreafSpell;
                    if (ImGui.DragFloat("多少的HP，可以用GCD群疗", ref healthAreaS, speed, 0, 1))
                    {
                        Service.Configuration.HealthAreafSpell = healthAreaS;
                        Service.Configuration.Save();
                    }

                    ImGui.Separator();

                    float healthSingleA = Service.Configuration.HealthSingleAbility;
                    if (ImGui.DragFloat("多少的HP，可以用能力技单奶", ref healthSingleA, speed, 0, 1))
                    {
                        Service.Configuration.HealthSingleAbility = healthSingleA;
                        Service.Configuration.Save();
                    }

                    float healthSingleS = Service.Configuration.HealthSingleSpell;
                    if (ImGui.DragFloat("多少的HP，可以用GCD单奶", ref healthSingleS, speed, 0, 1))
                    {
                        Service.Configuration.HealthSingleSpell = healthSingleS;
                        Service.Configuration.Save();
                    }
                    ImGui.EndChild();
                }

                ImGui.PopStyleVar();


                ImGui.EndTabItem();

            }

            if (ImGui.BeginTabItem("技能释放事件"))
            {
                ImGui.Text("在这个窗口，你可以设定一些技能释放后，使用什么宏。");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                if (ImGui.Button("添加"))
                {
                    Service.Configuration.Events.Add(new ActionEvents());
                }

                if (ImGui.BeginChild("事件", new Vector2(0f, -1f), true))
                {
                    for (int i = 0; i < Service.Configuration.Events.Count; i++)
                    {
                        string name = Service.Configuration.Events[i].Name;
                        if (ImGui.InputText("技能名称" + i.ToString(), ref name, 50))
                        {
                            Service.Configuration.Events[i].Name = name;
                            Service.Configuration.Save();
                        }

                        //ImGui.SameLine();

                        int macroindex = Service.Configuration.Events[i].MacroIndex;
                        if (ImGui.DragInt("宏编号" + i.ToString(), ref macroindex, 1, 0, 99))
                        {
                            Service.Configuration.Events[i].MacroIndex = macroindex;
                        }


                        bool isShared = Service.Configuration.Events[i].IsShared;
                        if (ImGui.Checkbox("共享宏" + i.ToString(), ref isShared))
                        {
                            Service.Configuration.Events[i].IsShared = isShared;
                            Service.Configuration.Save();
                        }

                        ImGui.SameLine();
                        if (ImGui.Button("删除" + i.ToString()))
                        {
                            Service.Configuration.Events.RemoveAt(i);
                        }
                        ImGui.Separator();
                    }
                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("帮助文档"))
            {
                ImGui.Text("在这个窗口，你可以看到一大堆帮助内容。");

                if (ImGui.BeginChild("帮助", new Vector2(0f, -1f), true))
                {

                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                    ImGui.Text("/aauto HealArea 表示开启一段范围治疗的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto HealSingle 表示开启一段单体治疗的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto DefenseArea 表示开启一段范围防御的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto DefenseSingle 表示开启一段单体防御的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto Esuna 表示开启一段康复的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto Raise 表示开启强制救人或突进的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AntiRepulsion 表示开启一段防击退的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto Break 表示开启一段爆发的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackBig 开始进攻，进攻对象为HitBox最大的。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackSmall 开始进攻，进攻对象为HitBox最小的。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackManual 开始进攻，进攻对象为手动选择。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackCancel 停止进攻，记得一定要经常关掉！");

                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();

                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }

    //private static uint GetActionsByName(string name)
    //{
    //    var enumerator = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetEnumerator();

    //    while (enumerator.MoveNext())
    //    {
    //        var action = enumerator.Current;
    //        if (action.Name == name && action.ClassJobLevel != 0 && !action.IsPvP)
    //        {
    //            return action.RowId;
    //        }
    //    }
    //    return 0;
    //}

    //private static string GetActionsByName(uint actionID)
    //{
    //    var act = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(actionID);

    //    return act == null ? "" : act.Name;
    //}
}
