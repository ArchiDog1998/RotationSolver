using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Windows;

internal class ConfigWindow : Window
{
    private readonly Vector4 shadedColor = new Vector4(0.68f, 0.68f, 0.68f, 1f);

    public ConfigWindow()
        : base("自动攻击设置 (开源免费)", 0, false)
    {
        RespectCloseHotkey = true;

        SizeCondition = (ImGuiCond)4;
        Size = new Vector2(740f, 490f);
    }
    private static readonly Dictionary<Role, string> _roleDescriptionValue = new Dictionary<Role, string>()
    {
        {Role.防护, $"{CustomCombo.DescType.单体防御} → {CustomCombo.GeneralActions.Rampart.Action.Name}, {CustomCombo.GeneralActions.Reprisal.Action.Name}" },
        {Role.近战, $"{CustomCombo.DescType.范围防御} → {CustomCombo.GeneralActions.Feint.Action.Name}" },
        {Role.远程, $"法系{CustomCombo.DescType.范围防御} → {CustomCombo.GeneralActions.Addle.Action.Name}" },
    };
    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("##tabbar"))
        {
            if (ImGui.BeginTabItem("攻击设定"))
            {
                ImGui.Text("你可以选择开启想要的职业的连续GCD战技、技能。");

                ImGui.BeginChild("攻击", new Vector2(0f, -1f), true);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
                int num = 1;


                foreach (Role key in IconReplacer.CustomCombosDict.Keys)
                {
                    var combos = IconReplacer.CustomCombosDict[key];
                    if (combos == null || combos.Length == 0) continue;

                    if (ImGui.CollapsingHeader(key.ToString()))
                    {
                        if (ImGui.IsItemHovered() && _roleDescriptionValue.TryGetValue(key, out string roleDesc))
                        {
                            ImGui.SetTooltip(roleDesc);
                        }
                        for (int i = 0; i < combos.Length; i++)
                        {
                            if (i > 0) ImGui.Separator();
                            var combo = combos[i];

                            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

                            ImGui.Columns(2, i.ToString(), false);
                            int size = Math.Min(combo.Texture.Width, 45);
                            ImGui.SetColumnWidth(0, size + 5);

                            var str = string.Join('\n', combo.Description.Select(pair => pair.Key.ToString() + " → " + pair.Value));

                            ImGui.Image(combo.Texture.ImGuiHandle, new Vector2(size, size));
                            if (ImGui.IsItemHovered())
                            {
                                if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
                            }

                            ImGui.NextColumn();

                            //ImGui.Spacing();

                            bool enable = combo.IsEnabled;
                            if (ImGui.Checkbox(combo.JobName, ref enable))
                            {
                                combo.IsEnabled = enable;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
                            }
                            string text = $"#{num}: 为{combo.JobName}的连续GCD战技、技能。";
                            ImGui.TextColored(shadedColor, text);

                            if (enable)
                            {
                                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1f, 1f));
                                string spacing = "    ";
                                var actions = combo.Config;
                                foreach (var boolean in actions.bools)
                                {
                                    ImGui.Text(spacing);
                                    ImGui.SameLine();
                                    bool val = boolean.value;
                                    if (ImGui.Checkbox($"#{num}: {boolean.description}", ref val))
                                    {
                                        boolean.value = val;
                                        Service.Configuration.Save();
                                    }
                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.SetTooltip("关键名称为：" + boolean.name);
                                    }
                                }
                                foreach (var doubles in actions.doubles)
                                {
                                    ImGui.Text(spacing);
                                    ImGui.SameLine();
                                    float val = doubles.value;
                                    if (ImGui.DragFloat($"#{num}: {doubles.description}", ref val, doubles.speed, doubles.min, doubles.max))
                                    {
                                        doubles.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                foreach (var textItem in actions.texts)
                                {
                                    ImGui.Text(spacing);
                                    ImGui.SameLine();
                                    string val = textItem.value;
                                    if (ImGui.InputText($"#{num}: {textItem.description}", ref val, 15))
                                    {
                                        textItem.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                foreach (var comboItem in actions.combos)
                                {
                                    ImGui.Text(spacing);
                                    ImGui.SameLine();
                                    int val = comboItem.value;
                                    if (ImGui.Combo($"#{num}: {comboItem.description}", ref val, comboItem.items, comboItem.items.Length))
                                    {
                                        comboItem.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                ImGui.PopStyleVar();

                            }
                            ImGui.Columns(1);

                            num++;
                        }
                    }
                    else
                    {
                        if (ImGui.IsItemHovered() && _roleDescriptionValue.TryGetValue(key, out string roleDesc))
                        {
                            ImGui.SetTooltip(roleDesc);
                        }
                        num += combos.Length;
                    }
                }

                ImGui.PopStyleVar();
                ImGui.EndChild();

                ImGui.EndTabItem();

            }


            if (ImGui.BeginTabItem("参数设定"))
            {
#if DEBUG
                //foreach (var item in Service.ClientState.LocalPlayer.StatusList)
                //{

                //    if (item.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                //    {
                //        ImGui.Text(item.GameData.Name + item.StatusId);
                //    }
                //}



                if (Service.TargetManager.Target is BattleChara b)
                {
                    ImGui.Text("Is Boss: " + b.IsBoss().ToString());
                    ImGui.Text("Has Side: " + b.HasLocationSide().ToString());
                }
                //ImGui.Text(FateManager.Instance()->FateJoined.ToString());
                //ImGui.Text(TargetHelper.AllTargets.Length.ToString());


                //foreach (var item in Service.ObjectTable)
                //{
                //    ImGui.Text($"{item.Name}, {item.SubKind}");

                //}

                //foreach (var item in TargetHelper.PartyMembers)
                //{
                //    ImGui.Text(item.Name.ToString());
                //}

                //if (Service.TargetManager.Target is BattleChara b)
                //foreach (var item in b.StatusList)
                //{
                //        ImGui.Text(item.GameData.Name + item.StatusId);
                //}

                //foreach (var item in Service.ObjectTable)
                //{
                //    if (item is BattleChara battle && item != Service.ClientState.LocalPlayer)
                //    {
                //        foreach (var status in battle.StatusList)
                //        {
                //            if (status.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                //            {
                //                ImGui.Text(status.GameData.Name + status.StatusId);
                //            }
                //        }
                //    }
                //}

#endif
                ImGui.Text("在这个窗口，你可以设定释放技能所需的参数。");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                if (ImGui.BeginChild("参数", new Vector2(0f, -1f), true))
                {
                    bool actionEnable = XIVAutoAttackPlugin.watcher.IsActionHookEnable;
                    if (ImGui.Checkbox("是否监视行为", ref actionEnable))
                    {
                        XIVAutoAttackPlugin.watcher.ChangeActionHook();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("如果不监视行为会影响循环以及身位提示，一般用于无法交任务物品时");
                    }

                    bool neverReplaceIcon = Service.Configuration.NeverReplaceIcon;
                    if (ImGui.Checkbox("不替换图标", ref neverReplaceIcon))
                    {
                        Service.Configuration.NeverReplaceIcon = neverReplaceIcon;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("如果不替换图标，提前判定的所有功能将失效，如提前提示身位");
                    }

                    float weaponDelay = Service.Configuration.WeaponDelay;
                    if (ImGui.DragFloat("需要GCD随机手残多少秒", ref weaponDelay, 0.002f, 0, 1))
                    {
                        Service.Configuration.WeaponDelay = weaponDelay;
                        Service.Configuration.Save();
                    }

                    float weaponFaster = Service.Configuration.WeaponFaster;
                    if (ImGui.DragFloat("需要提前几秒按下技能", ref weaponFaster, 0.002f, 0, 0.1f))
                    {
                        Service.Configuration.WeaponFaster = weaponFaster;
                        Service.Configuration.Save();
                    }

                    float weaponInterval = Service.Configuration.WeaponInterval;
                    if (ImGui.DragFloat("间隔多久释放能力技", ref weaponInterval, 0.002f, 0.6f, 0.7f))
                    {
                        Service.Configuration.WeaponInterval = weaponInterval;
                        Service.Configuration.Save();
                    }

                    float interruptibleTime = Service.Configuration.InterruptibleTime;
                    if (ImGui.DragFloat("打断类技能延迟多久后释放", ref interruptibleTime, 0.002f, 0, 2))
                    {
                        Service.Configuration.InterruptibleTime = interruptibleTime;
                        Service.Configuration.Save();
                    }

                    float specialDuration = Service.Configuration.SpecialDuration;
                    if (ImGui.DragFloat("特殊状态持续多久", ref specialDuration, 0.02f, 1, 20))
                    {
                        Service.Configuration.SpecialDuration = specialDuration;
                        Service.Configuration.Save();
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("目标选择"))
                    {
                        int isAllTargetAsHostile = Service.Configuration.TargetToHostileType;
                        if (ImGui.Combo("敌对目标条件", ref isAllTargetAsHostile, new string[]
                        {
                        "所有能打的目标都是敌对的目标",
                        "如果处于打人的目标数量为零，所有能打的都是敌对的",
                        "只有打人的目标才是敌对的目标",
                        }, 3))
                        {
                            Service.Configuration.TargetToHostileType = isAllTargetAsHostile;
                            Service.Configuration.Save();
                        }

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

                        float minradius = Service.Configuration.ObjectMinRadius;
                        if (ImGui.DragFloat("攻击对象最小底圈大小", ref minradius, 0.02f, 0, 10))
                        {
                            Service.Configuration.ObjectMinRadius = minradius;
                            Service.Configuration.Save();
                        }

                        //bool changeTargetForFate = Service.Configuration.ChangeTargetForFate;
                        //if (ImGui.Checkbox("在Fate中只选择Fate怪", ref changeTargetForFate))
                        //{
                        //    Service.Configuration.ChangeTargetForFate = changeTargetForFate;
                        //    Service.Configuration.Save();
                        //}

                        bool addEnemyListToHostile = Service.Configuration.AddEnemyListToHostile;
                        if (ImGui.Checkbox("将敌对列表的对象设为敌对", ref addEnemyListToHostile))
                        {
                            Service.Configuration.AddEnemyListToHostile = addEnemyListToHostile;
                            Service.Configuration.Save();
                        }

                        bool moveToScreen = Service.Configuration.MoveTowardsScreen;
                        if (ImGui.Checkbox("移动技能选屏幕中心的对象", ref moveToScreen))
                        {
                            Service.Configuration.MoveTowardsScreen = moveToScreen;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("设为是时移动的对象为屏幕中心的那个，否为游戏角色面朝的对象。");
                        }

                        bool raiseAll = Service.Configuration.RaiseAll;
                        if (ImGui.Checkbox("复活所有能复活的人，而非小队", ref raiseAll))
                        {
                            Service.Configuration.RaiseAll = raiseAll;
                            Service.Configuration.Save();
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("提示增强"))
                    {
                        //bool poslockCasting = Service.Configuration.PoslockCasting;
                        //if (ImGui.Checkbox("使用咏唱移动锁", ref poslockCasting))
                        //{
                        //    Service.Configuration.PoslockCasting = poslockCasting;
                        //    Service.Configuration.Save();
                        //}

                        bool usecheckCasting = Service.Configuration.CheckForCasting;
                        if (ImGui.Checkbox("使用咏唱是否结束提示", ref usecheckCasting))
                        {
                            Service.Configuration.CheckForCasting = usecheckCasting;
                            Service.Configuration.Save();
                        }

                        int voiceVolume = Service.Configuration.VoiceVolume;
                        if (ImGui.DragInt("语音音量", ref voiceVolume, 0.2f, 0, 100))
                        {
                            Service.Configuration.VoiceVolume = voiceVolume;
                            Service.Configuration.Save();
                        }

                        bool textlocation = Service.Configuration.TextLocation;
                        if (ImGui.Checkbox("写出战技身位", ref textlocation))
                        {
                            Service.Configuration.TextLocation = textlocation;
                            Service.Configuration.Save();
                        }

                        bool sayingLocation = Service.Configuration.SayingLocation;
                        if (ImGui.Checkbox("喊出战技身位", ref sayingLocation))
                        {
                            Service.Configuration.SayingLocation = sayingLocation;
                            Service.Configuration.Save();
                        }

                        bool sayoutLocationWrong = Service.Configuration.SayoutLocationWrong;
                        if (ImGui.Checkbox("显示身位错误", ref sayoutLocationWrong))
                        {
                            Service.Configuration.SayoutLocationWrong = sayoutLocationWrong;
                            Service.Configuration.Save();
                        }

                        var str = Service.Configuration.LocationText;
                        if (ImGui.InputText($"身位错误提示语", ref str, 15))
                        {
                            Service.Configuration.LocationText = str;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("如果身位错误，你想怎么被骂");
                        }


                        bool autoSayingOut = Service.Configuration.AutoSayingOut;
                        if (ImGui.Checkbox("状态变化时喊出", ref autoSayingOut))
                        {
                            Service.Configuration.AutoSayingOut = autoSayingOut;
                            Service.Configuration.Save();
                        }

                        bool useDtr = Service.Configuration.UseDtr;
                        if (ImGui.Checkbox("状态显示在系统信息上", ref useDtr))
                        {
                            Service.Configuration.UseDtr = useDtr;
                            Service.Configuration.Save();
                        }

                        bool useToast = Service.Configuration.UseToast;
                        if (ImGui.Checkbox("状态显示在屏幕中央", ref useToast))
                        {
                            Service.Configuration.UseToast = useToast;
                            Service.Configuration.Save();
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("技能使用"))
                    {
                        bool isOnlyGCD = Service.Configuration.OnlyGCD;
                        if (ImGui.Checkbox("只使用GCD循环，除去能力技", ref isOnlyGCD))
                        {
                            Service.Configuration.OnlyGCD = isOnlyGCD;
                            Service.Configuration.Save();
                        }

                        bool noHealOrDefenceAbility = Service.Configuration.NoDefenceAbility;
                        if (ImGui.Checkbox("不使用防御能力技", ref noHealOrDefenceAbility))
                        {
                            Service.Configuration.NoDefenceAbility = noHealOrDefenceAbility;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("如果要打高难，建议勾上这个，自己安排治疗和奶轴。");
                        }

                        bool autoBreak = Service.Configuration.AutoBreak;
                        if (ImGui.Checkbox("自动进行爆发", ref autoBreak))
                        {
                            Service.Configuration.AutoBreak = autoBreak;
                            Service.Configuration.Save();
                        }

                        bool autoDefenseforTank = Service.Configuration.AutoDefenseForTank;
                        if (ImGui.Checkbox("自动上减伤(不太准)", ref autoDefenseforTank))
                        {
                            Service.Configuration.AutoDefenseForTank = autoDefenseforTank;
                            Service.Configuration.Save();
                        }

                        bool autoProvokeforTank = Service.Configuration.AutoProvokeForTank;
                        if (ImGui.Checkbox("T自动挑衅", ref autoProvokeforTank))
                        {
                            Service.Configuration.AutoProvokeForTank = autoProvokeforTank;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("当有怪物在打非T的时候，会自动挑衅。");
                        }


                        bool alwaysLowBlow = Service.Configuration.AlwaysLowBlow;
                        if (ImGui.Checkbox("T永远下踢", ref alwaysLowBlow))
                        {
                            Service.Configuration.AlwaysLowBlow = alwaysLowBlow;
                            Service.Configuration.Save();
                        }

                        bool autoUseTrueNorth = Service.Configuration.AutoUseTrueNorth;
                        if (ImGui.Checkbox("近战自动上真北", ref autoUseTrueNorth))
                        {
                            Service.Configuration.AutoUseTrueNorth = autoUseTrueNorth;
                            Service.Configuration.Save();
                        }


                        bool raiseCasting = Service.Configuration.RaisePlayerByCasting;
                        if (ImGui.Checkbox("无目标时硬读条拉人", ref raiseCasting))
                        {
                            Service.Configuration.RaisePlayerByCasting = raiseCasting;
                            Service.Configuration.Save();
                        }

                        bool useItem = Service.Configuration.UseItem;
                        if (ImGui.Checkbox("使用道具", ref useItem))
                        {
                            Service.Configuration.UseItem = useItem;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("使用高级强心剂、强心剂，爆发药仅有黑魔");
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("触发条件"))
                    {
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

                        float healthTank = Service.Configuration.HealthForDyingTank;
                        if (ImGui.DragFloat("低于多少的HP，坦克要放大招了", ref healthTank, speed, 0, 1))
                        {
                            Service.Configuration.HealthForDyingTank = healthTank;
                            Service.Configuration.Save();
                        }
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
                    Service.Configuration.Events.Add(new ActionEventInfo());
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
                    ImGui.Text("/aauto EsunaShield 表示开启一段康复或者盾姿或者真北的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto RaiseShirk 表示开启强制救人或退避的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AntiRepulsion 表示开启一段防击退的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto BreakProvoke 表示开启一段爆发或挑衅的窗口期。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AutoBreak 更改是否自动爆发。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackBig 开始进攻，进攻对象为脚下圈圈最大的。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackSmall 开始进攻，进攻对象为脚下圈圈最小的。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackManual 开始进攻，进攻对象为手动选择。");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackCancel 停止进攻，记得一定要经常关掉！");
                    ImGui.Separator();
                    ImGui.Text("/aauto EndSpecial 停止特殊状态！");
                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();

                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }
}
