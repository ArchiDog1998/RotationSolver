using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;
using Lumina.Data.Parsing;
using Dalamud.Interface;
using XIVAutoAttack.Combos.Script;
using System.Diagnostics;
using System.Reflection;
using XIVAutoAttack.Windows.Tabs;
using Dalamud.Interface.Components;
using Newtonsoft.Json;

namespace XIVAutoAttack.Windows;

internal class ComboConfigWindow : Window
{
    //private static readonly Vector4 shadedColor = new Vector4(0.68f, 0.68f, 0.68f, 1f);

    public ComboConfigWindow()
        : base("鑷姩鏀诲嚮璁剧疆 (寮�婧愬厤璐�) v"+ typeof(ComboConfigWindow).Assembly.GetName().Version.ToString(), 0, false)
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

    private static string ToName(VirtualKey k)
    {
        return k switch
        {
            VirtualKey.SHIFT => "SHIFT",
            VirtualKey.CONTROL => "CTRL",
            VirtualKey.MENU => "ALT",
            _ => k.ToString(),
        };
    }

    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("AutoAttackSettings"))
        {
#if DEBUG
            if (Service.ClientState.LocalPlayer != null && ImGui.BeginTabItem("Debug鏌ョ湅"))
            {
                if (ImGui.CollapsingHeader("鑷韩闄勫姞缁欒嚜宸辩殑鐘舵��"))
                {
                    foreach (var item in Service.ClientState.LocalPlayer.StatusList)
                    {

                        if (item.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                        {
                            ImGui.Text(item.GameData.Name + item.StatusId);
                        }
                    }
                }

                if (ImGui.CollapsingHeader("鐩爣淇℃伅"))
                {
                    if (Service.TargetManager.Target is BattleChara b)
                    {
                        ImGui.Text("Is Boss: " + b.IsBoss().ToString());
                        ImGui.Text("Has Side: " + b.HasLocationSide().ToString());
                        ImGui.Text("Is Dying: " + b.IsDying().ToString());

                        foreach (var status in b.StatusList)
                        {
                            if (status.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                            {
                                ImGui.Text(status.GameData.Name + status.StatusId);
                            }
                        }
                    }
                    ImGui.Text("");
                    foreach (var item in TargetUpdater.HostileTargets)
                    {
                        ImGui.Text(item.Name.ToString());
                    }
                }

                if (ImGui.CollapsingHeader("涓嬩竴涓妧鑳�"))
                {
                    BaseAction baseAction = null;
                    baseAction ??= ActionUpdater.NextAction as BaseAction;
                    DrawAction(baseAction);

                    ImGui.Text("Ability Remain: " + ActionUpdater.AbilityRemain.ToString());
                    ImGui.Text("Ability Count: " + ActionUpdater.AbilityRemainCount.ToString());

                }

                if (ImGui.CollapsingHeader("涓婁竴涓妧鑳�"))
                {
                    DrawAction(Watcher.LastAction, nameof(Watcher.LastAction));
                    DrawAction(Watcher.LastAbility, nameof(Watcher.LastAbility));
                    DrawAction(Watcher.LastSpell, nameof(Watcher.LastSpell));
                    DrawAction(Watcher.LastWeaponskill, nameof(Watcher.LastWeaponskill));
                    DrawAction(Service.Address.LastComboAction, nameof(Service.Address.LastComboAction));
                }

                if (ImGui.CollapsingHeader("鍊掕鏃躲�佹寜閿�"))
                {
                    ImGui.Text("Count Down: " + CountDown.CountDownTime.ToString());

                    if (ActionUpdater.exception != null)
                    {
                        ImGui.Text(ActionUpdater.exception.Message);
                        ImGui.Text(ActionUpdater.exception.StackTrace);
                    }
                }
                ImGui.EndTabItem();
            }
#endif
            if (ImGui.BeginTabItem("鍏充簬"))
            {
                About.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("鏀诲嚮璁惧畾"))
            {
                ImGui.Text("浣犲彲浠ラ�夋嫨寮�鍚兂瑕佺殑鑱屼笟鐨勮繛缁璆CD鎴樻妧銆佹妧鑳斤紝鑻ヨ亴涓氫笌褰撳墠鑱屼笟鐩稿悓鍒欐湁鍛戒护瀹忔彁绀恒��");

#if DEBUG
                string folderLocation = Service.Configuration.ScriptComboFolder;
                if(ImGui.InputText("自定义循环路径", ref folderLocation, 256))
                {
                    Service.Configuration.ScriptComboFolder = folderLocation;
                    Service.Configuration.Save();
                }

                ImGui.SameLine();
                Spacing();

                if (ImGuiComponents.IconButton(FontAwesomeIcon.FolderOpen))
                {
                    IconReplacer.LoadFromFolder();
                }
#endif

                ImGui.BeginChild("攻击", new Vector2(0f, -1f), true);
                
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
                int num = 1;


                foreach (var key in IconReplacer.CustomCombosDict.Keys)
                {
                    var combos = IconReplacer.CustomCombosDict[key];
                    if (combos == null || combos.Length == 0) continue;

                    if (ImGui.CollapsingHeader(key.ToName()))
                    {
                        if (ImGui.IsItemHovered() && _roleDescriptionValue.TryGetValue(key, out string roleDesc))
                        {
                            ImGui.SetTooltip(roleDesc);
                        }
                        for (int i = 0; i < combos.Length; i++)
                        {
                            if (i > 0) ImGui.Separator();
                            var combo = IconReplacer.GetChooseCombo(combos[i]);
                            var canAddButton = Service.ClientState.LocalPlayer != null && combo.JobIDs.Contains((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id);

                            DrawTexture(combo, () =>
                            {
                                var actions = combo.Config;
                                foreach (var boolean in actions.bools)
                                {
                                    Spacing();
                                    bool val = boolean.value;
                                    if (ImGui.Checkbox($"#{num}: {boolean.description}", ref val))
                                    {
                                        boolean.value = val;
                                        Service.Configuration.Save();
                                    }
                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.SetTooltip("鍏抽敭鍚嶇О涓猴細" + boolean.name);
                                    }

                                    //鏄剧ず鍙互璁剧疆鐨勬浠�
                                    if (canAddButton)
                                    {
                                        ImGui.SameLine();
                                        Spacing();
                                        CommandHelp(boolean.name);
                                    }

                                }
                                foreach (var doubles in actions.doubles)
                                {
                                    Spacing();
                                    float val = doubles.value;
                                    if (ImGui.DragFloat($"#{num}: {doubles.description}", ref val, doubles.speed, doubles.min, doubles.max))
                                    {
                                        doubles.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                foreach (var textItem in actions.texts)
                                {
                                    Spacing();
                                    string val = textItem.value;
                                    if (ImGui.InputText($"#{num}: {textItem.description}", ref val, 15))
                                    {
                                        textItem.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                foreach (var comboItem in actions.combos)
                                {
                                    Spacing();
                                    if (ImGui.BeginCombo($"#{num}: {comboItem.description}", comboItem.items[comboItem.value]))
                                    {
                                        for (int comboIndex = 0; comboIndex < comboItem.items.Length; comboIndex++)
                                        {
                                            if (ImGui.Selectable(comboItem.items[comboIndex]))
                                            {
                                                comboItem.value = comboIndex;
                                                Service.Configuration.Save();
                                            }
                                            if (canAddButton)
                                            {
                                                ImGui.SameLine();
                                                Spacing();
                                                CommandHelp(comboItem.name + comboIndex.ToString());
                                            }
                                        }
                                        ImGui.EndCombo();
                                    }
                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.SetTooltip("鍏抽敭鍚嶇О涓猴細" + comboItem.name);
                                    }

                                    //鏄剧ず鍙互璁剧疆鐨勬浠�
                                    if (canAddButton)
                                    {
                                        ImGui.SameLine();
                                        Spacing();
                                        CommandHelp(comboItem.name);
                                    }
                                }

                                if (canAddButton)
                                {
                                    ImGui.NewLine();

                                    foreach (var customCMD in combo.CommandShow)
                                    {
                                        Spacing();
                                        CommandHelp(customCMD.Key, customCMD.Value);
                                    }
                                }

                            }, combo.JobIDs[0], combos[i].combos.Select(c => c.Author).ToArray());

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

            if (ImGui.BeginTabItem("鍙傛暟璁惧畾"))
            {

                ImGui.Text("鍦ㄨ繖涓獥鍙ｏ紝浣犲彲浠ヨ瀹氶噴鏀炬妧鑳芥墍闇�鐨勫弬鏁般��");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                if (ImGui.BeginChild("鍙傛暟", new Vector2(0f, -1f), true))
                {
                    bool neverReplaceIcon = Service.Configuration.NeverReplaceIcon;
                    if (ImGui.Checkbox("涓嶆浛鎹㈠浘鏍�", ref neverReplaceIcon))
                    {
                        Service.Configuration.NeverReplaceIcon = neverReplaceIcon;
                        Service.Configuration.Save();
                    }

                    bool useOverlayWindow = Service.Configuration.UseOverlayWindow;
                    if (ImGui.Checkbox("浣跨敤鏈�楂樺ぇ瑕嗙洊绐楀彛", ref useOverlayWindow))
                    {
                        Service.Configuration.UseOverlayWindow = useOverlayWindow;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("杩欎釜绐楀彛鐩墠鐢ㄤ簬鎻愬墠鎻愮ず韬綅銆�");
                    }

                    if (ImGui.CollapsingHeader("鍩虹璁剧疆"))
                    {

                        float weaponDelay = Service.Configuration.WeaponDelay;
                        if (ImGui.DragFloat("闇�瑕丟CD闅忔満鎵嬫畫澶氬皯绉�", ref weaponDelay, 0.002f, 0, 1))
                        {
                            Service.Configuration.WeaponDelay = weaponDelay;
                            Service.Configuration.Save();
                        }

                        float weaponFaster = Service.Configuration.WeaponFaster;
                        if (ImGui.DragFloat("闇�瑕佹彁鍓嶅嚑绉掓寜涓嬫妧鑳�", ref weaponFaster, 0.002f, 0, 0.1f))
                        {
                            Service.Configuration.WeaponFaster = weaponFaster;
                            Service.Configuration.Save();
                        }

                        float weaponInterval = Service.Configuration.WeaponInterval;
                        if (ImGui.DragFloat("闂撮殧澶氫箙閲婃斁鑳藉姏鎶�", ref weaponInterval, 0.002f, 0.5f, 0.7f))
                        {
                            Service.Configuration.WeaponInterval = weaponInterval;
                            Service.Configuration.Save();
                        }

                        float interruptibleTime = Service.Configuration.InterruptibleTime;
                        if (ImGui.DragFloat("鎵撴柇绫绘妧鑳藉欢杩熷涔呭悗閲婃斁", ref interruptibleTime, 0.002f, 0, 2))
                        {
                            Service.Configuration.InterruptibleTime = interruptibleTime;
                            Service.Configuration.Save();
                        }

                        float specialDuration = Service.Configuration.SpecialDuration;
                        if (ImGui.DragFloat("鐗规畩鐘舵�佹寔缁涔�", ref specialDuration, 0.02f, 1, 20))
                        {
                            Service.Configuration.SpecialDuration = specialDuration;
                            Service.Configuration.Save();
                        }

                        int addDotGCDCount = Service.Configuration.AddDotGCDCount;
                        if (ImGui.DragInt("杩樺樊鍑犱釜GCD灏卞彲浠ヨˉDOT浜�", ref addDotGCDCount, 0.2f, 0, 3))
                        {
                            Service.Configuration.AddDotGCDCount = addDotGCDCount;
                            Service.Configuration.Save();
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("鎻愮ず澧炲己"))
                    {
                        bool poslockCasting = Service.Configuration.PoslockCasting;
                        VirtualKey poslockModifier = Service.Configuration.PoslockModifier;
                        if (ImGui.Checkbox("浣跨敤鍜忓敱绉诲姩閿�", ref poslockCasting))
                        {
                            Service.Configuration.PoslockCasting = poslockCasting;
                            Service.Configuration.Save();
                        }

                        var modifierChoices = new VirtualKey[]{ VirtualKey.CONTROL, VirtualKey.SHIFT, VirtualKey.MENU };
                        if(poslockCasting && ImGui.BeginCombo("鏃犺鍜忓敱閿佺儹閿�", ToName(poslockModifier)))
                         {
                             foreach (VirtualKey k in modifierChoices)
                             {
                                 if (ImGui.Selectable(ToName(k)))
                                 {
                                     Service.Configuration.PoslockModifier = k;
                                     Service.Configuration.Save();
                                 }
                             }
                            ImGui.EndCombo();
                         }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("鎵嬫焺鐜╁涓烘寜涓婰T+RT鏃犺鍜忓敱閿�");
                        }

                        bool usecheckCasting = Service.Configuration.CheckForCasting;
                        if (ImGui.Checkbox("浣跨敤鍜忓敱缁撴潫鏄剧ず", ref usecheckCasting))
                        {
                            Service.Configuration.CheckForCasting = usecheckCasting;
                            Service.Configuration.Save();
                        }

                        bool teachingMode = Service.Configuration.TeachingMode;
                        if (ImGui.Checkbox("寰幆鏁欒偛妯″紡", ref teachingMode))
                        {
                            Service.Configuration.TeachingMode = teachingMode;
                            Service.Configuration.Save();
                        }
                        if (teachingMode)
                        {
                            ImGui.SameLine();
                            Spacing();

                            var teachingColor = Service.Configuration.TeachingModeColor;

                            if(ImGui.ColorEdit3("鏁欒偛妯″紡棰滆壊", ref teachingColor))
                            {
                                Service.Configuration.TeachingModeColor = teachingColor;
                                Service.Configuration.Save();
                            }
                        }

                        bool keyBoardNoise = Service.Configuration.KeyBoardNoise;
                        if (ImGui.Checkbox("妯℃嫙鎸変笅閿洏鏁堟灉", ref keyBoardNoise))
                        {
                            Service.Configuration.KeyBoardNoise = keyBoardNoise;
                            Service.Configuration.Save();
                        }

                        int voiceVolume = Service.Configuration.VoiceVolume;
                        if (ImGui.DragInt("璇煶闊抽噺", ref voiceVolume, 0.2f, 0, 100))
                        {
                            Service.Configuration.VoiceVolume = voiceVolume;
                            Service.Configuration.Save();
                        }

                        bool textlocation = Service.Configuration.TextLocation;
                        if (ImGui.Checkbox("鍐欏嚭鎴樻妧韬綅", ref textlocation))
                        {
                            Service.Configuration.TextLocation = textlocation;
                            Service.Configuration.Save();
                        }

                        bool sayingLocation = Service.Configuration.SayingLocation;
                        if (ImGui.Checkbox("鍠婂嚭鎴樻妧韬綅", ref sayingLocation))
                        {
                            Service.Configuration.SayingLocation = sayingLocation;
                            Service.Configuration.Save();
                        }

                        bool sayoutLocationWrong = Service.Configuration.SayoutLocationWrong;
                        if (useOverlayWindow && ImGui.Checkbox("鏄剧ず韬綅閿欒", ref sayoutLocationWrong))
                        {
                            Service.Configuration.SayoutLocationWrong = sayoutLocationWrong;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("韬綅閿欒涓嶆槸寰堝噯锛屼粎渚涘弬鑰冦��");
                        }

                        var str = Service.Configuration.LocationText;
                        if (ImGui.InputText("韬綅閿欒鎻愮ず璇�", ref str, 15))
                        {
                            Service.Configuration.LocationText = str;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("濡傛灉韬綅閿欒锛屼綘鎯虫�庝箞琚獋!");
                        }

                        bool autoSayingOut = Service.Configuration.AutoSayingOut;
                        if (ImGui.Checkbox("鐘舵�佸彉鍖栨椂鍠婂嚭", ref autoSayingOut))
                        {
                            Service.Configuration.AutoSayingOut = autoSayingOut;
                            Service.Configuration.Save();
                        }

                        bool useDtr = Service.Configuration.UseDtr;
                        if (ImGui.Checkbox("鐘舵�佹樉绀哄湪绯荤粺淇℃伅涓�", ref useDtr))
                        {
                            Service.Configuration.UseDtr = useDtr;
                            Service.Configuration.Save();
                        }

                        bool useToast = Service.Configuration.UseToast;
                        if (ImGui.Checkbox("鐘舵�佹樉绀哄湪灞忓箷涓ぎ", ref useToast))
                        {
                            Service.Configuration.UseToast = useToast;
                            Service.Configuration.Save();
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("鎶�鑳戒娇鐢�"))
                    {
                        bool useAOEWhenManual = Service.Configuration.UseAOEWhenManual;
                        if (ImGui.Checkbox("鍦ㄦ墜鍔ㄩ�夋嫨鐨勬椂鍊欎娇鐢ˋOE鎶�鑳�", ref useAOEWhenManual))
                        {
                            Service.Configuration.UseAOEWhenManual = useAOEWhenManual;
                            Service.Configuration.Save();
                        }

                        bool autoBreak = Service.Configuration.AutoBreak;
                        if (ImGui.Checkbox("鑷姩杩涜鐖嗗彂", ref autoBreak))
                        {
                            Service.Configuration.AutoBreak = autoBreak;
                            Service.Configuration.Save();
                        }
                        ImGui.SameLine();
                        Spacing();
                        CommandHelp("AutoBreak");


                        bool isOnlyGCD = Service.Configuration.OnlyGCD;
                        if (ImGui.Checkbox("鍙娇鐢℅CD寰幆锛岄櫎鍘昏兘鍔涙妧", ref isOnlyGCD))
                        {
                            Service.Configuration.OnlyGCD = isOnlyGCD;
                            Service.Configuration.Save();
                        }

                        bool attackSafeMode = Service.Configuration.AttackSafeMode;
                        if (ImGui.Checkbox("鏀诲嚮瀹夊叏妯″紡", ref attackSafeMode))
                        {
                            Service.Configuration.AttackSafeMode = attackSafeMode;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("缁濆淇濊瘉鍦ㄥ崟鐩爣鐨勬椂鍊欎笉鎵揂OE锛屽氨绠楀ぇ鎷涗篃鏄�備絾鏄鏋滄�殑鏁伴噺杈惧埌鏍囧噯渚濈劧浼氫娇鐢ˋOE銆�");
                        }

                        if (!isOnlyGCD)
                        {
                            Spacing();
                            bool noHealOrDefenceAbility = Service.Configuration.NoDefenceAbility;
                            if (ImGui.Checkbox("涓嶄娇鐢ㄩ槻寰¤兘鍔涙妧", ref noHealOrDefenceAbility))
                            {
                                Service.Configuration.NoDefenceAbility = noHealOrDefenceAbility;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("濡傛灉瑕佹墦楂橀毦锛屽缓璁嬀涓婅繖涓紝鑷繁瀹夋帓娌荤枟鍜屽ザ杞淬��");
                            }

                            Spacing();
                            bool autoDefenseforTank = Service.Configuration.AutoDefenseForTank;
                            if (ImGui.Checkbox("鑷姩涓婂噺浼�(涓嶅お鍑�)", ref autoDefenseforTank))
                            {
                                Service.Configuration.AutoDefenseForTank = autoDefenseforTank;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("鑷姩鐨勮繖涓笉鑳借瘑鍒▉鍔涗负0鐨凙OE鎶�鑳斤紝璇锋敞鎰忋��");
                            }

                            Spacing();
                            bool autoShieled = Service.Configuration.AutoShield;
                            if (ImGui.Checkbox("T鑷姩涓婄浘", ref autoShieled))
                            {
                                Service.Configuration.AutoShield = autoShieled;
                                Service.Configuration.Save();
                            }

                            Spacing();
                            bool autoProvokeforTank = Service.Configuration.AutoProvokeForTank;
                            if (ImGui.Checkbox("T鑷姩鎸戣", ref autoProvokeforTank))
                            {
                                Service.Configuration.AutoProvokeForTank = autoProvokeforTank;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("褰撴湁鎬墿鍦ㄦ墦闈濼鐨勬椂鍊欙紝浼氳嚜鍔ㄦ寫琛呫��");
                            }

                            Spacing();
                            bool autoUseTrueNorth = Service.Configuration.AutoUseTrueNorth;
                            if (ImGui.Checkbox("杩戞垬鑷姩涓婄湡鍖�", ref autoUseTrueNorth))
                            {
                                Service.Configuration.AutoUseTrueNorth = autoUseTrueNorth;
                                Service.Configuration.Save();
                            }

                            Spacing();
                            bool raiseSwift = Service.Configuration.RaisePlayerBySwift;
                            if (ImGui.Checkbox("鍗冲埢鎷変汉", ref raiseSwift))
                            {
                                Service.Configuration.RaisePlayerBySwift = raiseSwift;
                                Service.Configuration.Save();
                            }

                            Spacing();
                            bool useAreaAbilityFriendly = Service.Configuration.UseAreaAbilityFriendly;
                            if (ImGui.Checkbox("浣跨敤鍙嬫柟鍦伴潰鏀剧疆鎶�鑳�", ref useAreaAbilityFriendly))
                            {
                                Service.Configuration.UseAreaAbilityFriendly = useAreaAbilityFriendly;
                                Service.Configuration.Save();
                            }
                        }

                        bool raiseCasting = Service.Configuration.RaisePlayerByCasting;
                        if (ImGui.Checkbox("鏃犵洰鏍囨椂纭鏉℃媺浜�", ref raiseCasting))
                        {
                            Service.Configuration.RaisePlayerByCasting = raiseCasting;
                            Service.Configuration.Save();
                        }

                        bool useHealWhenNotAHealer = Service.Configuration.UseHealWhenNotAHealer;
                        if (ImGui.Checkbox("闈炲ザ濡堢敤濂朵汉鐨勬妧鑳�", ref useHealWhenNotAHealer))
                        {
                            Service.Configuration.UseHealWhenNotAHealer = useHealWhenNotAHealer;
                            Service.Configuration.Save();
                        }

                        int lessMPNoRaise = Service.Configuration.LessMPNoRaise;
                        if (ImGui.DragInt("灏忎簬澶氬皯钃濆氨涓嶅娲讳簡", ref lessMPNoRaise, 200, 0, 10000))
                        {
                            Service.Configuration.LessMPNoRaise = lessMPNoRaise;
                            Service.Configuration.Save();
                        }

                        bool useItem = Service.Configuration.UseItem;
                        if (ImGui.Checkbox("浣跨敤閬撳叿", ref useItem))
                        {
                            Service.Configuration.UseItem = useItem;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("浣跨敤鐖嗗彂鑽紝鐩墠杩樻湭鍐欏叏");
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("瑙﹀彂鏉′欢"))
                    {
                        bool autoStartCountdown = Service.Configuration.AutoStartCountdown;
                        if (ImGui.Checkbox("鍊掕鏃舵椂鑷姩鎵撳紑鏀诲嚮", ref autoStartCountdown))
                        {
                            Service.Configuration.AutoStartCountdown = autoStartCountdown;
                            Service.Configuration.Save();
                        }

                        float speed = 0.005f;
                        float healthDiff = Service.Configuration.HealthDifference;
                        if (ImGui.DragFloat("澶氬皯鐨凥P鏍囧噯宸互涓嬶紝鍙互鐢ㄧ兢鐤�", ref healthDiff, speed * 2, 0, 0.5f))
                        {
                            Service.Configuration.HealthDifference = healthDiff;
                            Service.Configuration.Save();
                        }

                        float healthAreaA = Service.Configuration.HealthAreaAbility;
                        if (ImGui.DragFloat("澶氬皯鐨凥P锛屽彲浠ョ敤鑳藉姏鎶�缇ょ枟", ref healthAreaA, speed, 0, 1))
                        {
                            Service.Configuration.HealthAreaAbility = healthAreaA;
                            Service.Configuration.Save();
                        }

                        float healthAreaS = Service.Configuration.HealthAreafSpell;
                        if (ImGui.DragFloat("澶氬皯鐨凥P锛屽彲浠ョ敤GCD缇ょ枟", ref healthAreaS, speed, 0, 1))
                        {
                            Service.Configuration.HealthAreafSpell = healthAreaS;
                            Service.Configuration.Save();
                        }

                        float healingOfTimeSubstactArea = Service.Configuration.HealingOfTimeSubstactArea;
                        if (ImGui.DragFloat("濡傛灉浣跨敤缇や綋Hot鎶�鑳斤紝闃堝�间笅闄嶅灏�", ref healingOfTimeSubstactArea, speed, 0, 1))
                        {
                            Service.Configuration.HealingOfTimeSubstactArea = healingOfTimeSubstactArea;
                            Service.Configuration.Save();
                        }

                        float healthSingleA = Service.Configuration.HealthSingleAbility;
                        if (ImGui.DragFloat("澶氬皯鐨凥P锛屽彲浠ョ敤鑳藉姏鎶�鍗曞ザ", ref healthSingleA, speed, 0, 1))
                        {
                            Service.Configuration.HealthSingleAbility = healthSingleA;
                            Service.Configuration.Save();
                        }

                        float healthSingleS = Service.Configuration.HealthSingleSpell;
                        if (ImGui.DragFloat("澶氬皯鐨凥P锛屽彲浠ョ敤GCD鍗曞ザ", ref healthSingleS, speed, 0, 1))
                        {
                            Service.Configuration.HealthSingleSpell = healthSingleS;
                            Service.Configuration.Save();
                        }

                        float healingOfTimeSubstact = Service.Configuration.HealingOfTimeSubstactSingle;
                        if (ImGui.DragFloat("濡傛灉浣跨敤鍗曚綋Hot鎶�鑳斤紝闃堝�间笅闄嶅灏�", ref healingOfTimeSubstact, speed, 0, 1))
                        {
                            Service.Configuration.HealingOfTimeSubstactSingle = healingOfTimeSubstact;
                            Service.Configuration.Save();
                        }


                        float healthTank = Service.Configuration.HealthForDyingTank;
                        if (ImGui.DragFloat("浣庝簬澶氬皯鐨凥P锛屽潶鍏嬭鏀惧ぇ鎷涗簡", ref healthTank, speed, 0, 1))
                        {
                            Service.Configuration.HealthForDyingTank = healthTank;
                            Service.Configuration.Save();
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("鐩爣閫夋嫨"))
                    {
                        int isAllTargetAsHostile = IconReplacer.RightNowTargetToHostileType;
                        if (ImGui.Combo("鏁屽鐩爣绛涢�夋潯浠�", ref isAllTargetAsHostile, new string[]
                        {
                                "鎵�鏈夎兘鎵撶殑鐩爣閮芥槸鏁屽鐨勭洰鏍�",
                                "濡傛灉澶勪簬鎵撲汉鐨勭洰鏍囨暟閲忎负闆讹紝鎵�鏈夎兘鎵撶殑閮芥槸鏁屽鐨�",
                                "鍙湁鎵撲汉鐨勭洰鏍囨墠鏄晫瀵圭殑鐩爣",
                        }, 3))
                        {
                            IconReplacer.RightNowTargetToHostileType = (byte)isAllTargetAsHostile;
                            Service.Configuration.Save();
                        }

                        bool addEnemyListToHostile = Service.Configuration.AddEnemyListToHostile;
                        if (ImGui.Checkbox("灏嗘晫瀵瑰垪琛ㄧ殑瀵硅薄璁句负鏁屽", ref addEnemyListToHostile))
                        {
                            Service.Configuration.AddEnemyListToHostile = addEnemyListToHostile;
                            Service.Configuration.Save();
                        }

                        bool chooseAttackMark = Service.Configuration.ChooseAttackMark;
                        if (ImGui.Checkbox("浼樺厛閫変腑鏈夋敾鍑绘爣璁扮殑鐩爣", ref chooseAttackMark))
                        {
                            Service.Configuration.ChooseAttackMark = chooseAttackMark;
                            Service.Configuration.Save();
                        }

                        if (chooseAttackMark)
                        {
                            Spacing();
                            bool attackMarkAOE = Service.Configuration.AttackMarkAOE;

                            if (ImGui.Checkbox("鏄惁杩樿浣跨敤AOE", ref attackMarkAOE))
                            {
                                Service.Configuration.AttackMarkAOE = attackMarkAOE;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("濡傛灉鍕鹃�変簡锛岄偅涔堝彲鑳借繖涓狝OE鎵撲笉鍒版敾鍑荤洰鏍囩殑瀵硅薄锛屽洜涓轰负浜嗚拷姹傛墦鍒版洿澶氱殑鐩爣銆�");
                            }
                        }

                        bool filterStopMark = Service.Configuration.FilterStopMark;
                        if (ImGui.Checkbox("鍘绘帀鏈夊仠姝㈡爣璁扮殑鐩爣", ref filterStopMark))
                        {
                            Service.Configuration.FilterStopMark = filterStopMark;
                            Service.Configuration.Save();
                        }

                        int multiCount = Service.Configuration.HostileCount;
                        if (ImGui.DragInt("鑼冨洿鏀诲嚮鏈�灏戦渶瑕佸灏戜汉", ref multiCount, 0.02f, 2, 5))
                        {
                            Service.Configuration.HostileCount = multiCount;
                            Service.Configuration.Save();
                        }

                        int partyCount = Service.Configuration.PartyCount;
                        if (ImGui.DragInt("鑼冨洿娌荤枟鏈�灏戦渶瑕佸灏戜汉", ref partyCount, 0.02f, 2, 5))
                        {
                            Service.Configuration.PartyCount = partyCount;
                            Service.Configuration.Save();
                        }

                        float minradius = Service.Configuration.ObjectMinRadius;
                        if (ImGui.DragFloat("鏀诲嚮瀵硅薄鏈�灏忓簳鍦堝ぇ灏�", ref minradius, 0.02f, 0, 10))
                        {
                            Service.Configuration.ObjectMinRadius = minradius;
                            Service.Configuration.Save();
                        }

                        bool changeTargetForFate = Service.Configuration.ChangeTargetForFate;
                        if (ImGui.Checkbox("鍦‵ate涓彧閫夋嫨Fate鎬�", ref changeTargetForFate))
                        {
                            Service.Configuration.ChangeTargetForFate = changeTargetForFate;
                            Service.Configuration.Save();
                        }

                        bool moveToScreen = Service.Configuration.MoveTowardsScreen;
                        if (ImGui.Checkbox("绉诲姩鎶�鑳介�夊睆骞曚腑蹇冪殑瀵硅薄", ref moveToScreen))
                        {
                            Service.Configuration.MoveTowardsScreen = moveToScreen;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("璁句负鏄椂绉诲姩鐨勫璞′负灞忓箷涓績鐨勯偅涓紝鍚︿负娓告垙瑙掕壊闈㈡湞鐨勫璞°��");
                        }

                        bool raiseAll = Service.Configuration.RaiseAll;
                        if (ImGui.Checkbox("澶嶆椿鎵�鏈夎兘澶嶆椿鐨勪汉锛岃�岄潪灏忛槦", ref raiseAll))
                        {
                            Service.Configuration.RaiseAll = raiseAll;
                            Service.Configuration.Save();
                        }

                        bool raiseBrinkofDeath = Service.Configuration.RaiseBrinkofDeath;
                        if (ImGui.Checkbox("澶嶆椿婵掓锛堥粦澶达級涔嬩汉", ref raiseBrinkofDeath))
                        {
                            Service.Configuration.RaiseBrinkofDeath = raiseBrinkofDeath;
                            Service.Configuration.Save();
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("鏁屽閫夋嫨"))
                    {
                        if (ImGui.Button("娣诲姞閫夋嫨鏉′欢"))
                        {
                            Service.Configuration.TargetingTypes.Add(TargetingType.Big);
                        }
                        ImGui.SameLine();
                        Spacing();
                        ImGui.Text("浣犲彲浠ヨ瀹氭晫瀵圭殑閫夋嫨锛屼互渚夸簬鍦ㄦ垬鏂椾腑鐏垫椿鍒囨崲閫夋嫨鏁屽鐨勯�昏緫銆�");
                        for (int i = 0; i < Service.Configuration.TargetingTypes.Count; i++)
                        {

                            ImGui.Separator();

                            var names = Enum.GetNames(typeof(TargetingType));
                            var targingType = (int)Service.Configuration.TargetingTypes[i];
                            if (ImGui.Combo("鏁屽鐩爣閫夋嫨鏉′欢" + i.ToString(), ref targingType, names, names.Length))
                            {
                                Service.Configuration.TargetingTypes[i] = (TargetingType)targingType;
                                Service.Configuration.Save();
                            }

                            if (ImGui.Button("涓婄Щ鏉′欢" + i.ToString()))
                            {
                                if (i != 0)
                                {
                                    var value = Service.Configuration.TargetingTypes[i];
                                    Service.Configuration.TargetingTypes.RemoveAt(i);
                                    Service.Configuration.TargetingTypes.Insert(i - 1, value);
                                }
                            }
                            ImGui.SameLine();
                            Spacing();
                            if (ImGui.Button("涓嬬Щ鏉′欢" + i.ToString()))
                            {
                                if (i < Service.Configuration.TargetingTypes.Count - 1)
                                {
                                    var value = Service.Configuration.TargetingTypes[i];
                                    Service.Configuration.TargetingTypes.RemoveAt(i);
                                    Service.Configuration.TargetingTypes.Insert(i + 1, value);
                                }
                            }

                            ImGui.SameLine();
                            Spacing();

                            if (ImGui.Button("鍒犻櫎鏉′欢" + i.ToString()))
                            {
                                Service.Configuration.TargetingTypes.RemoveAt(i);
                            }
                        }
                    }

                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("鎶�鑳介噴鏀句簨浠�"))
            {

                if (ImGui.Button("娣诲姞浜嬩欢"))
                {
                    Service.Configuration.Events.Add(new ActionEventInfo());
                }
                ImGui.SameLine();
                Spacing();
                ImGui.Text("鍦ㄨ繖涓獥鍙ｏ紝浣犲彲浠ヨ瀹氫竴浜涙妧鑳介噴鏀惧悗锛屼娇鐢ㄤ粈涔堝畯銆�");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


                if (ImGui.BeginChild("浜嬩欢鍒楄〃", new Vector2(0f, -1f), true))
                {
                    for (int i = 0; i < Service.Configuration.Events.Count; i++)
                    {
                        string name = Service.Configuration.Events[i].Name;
                        if (ImGui.InputText("鎶�鑳藉悕绉�" + i.ToString(), ref name, 50))
                        {
                            Service.Configuration.Events[i].Name = name;
                            Service.Configuration.Save();
                        }

                        int macroindex = Service.Configuration.Events[i].MacroIndex;
                        if (ImGui.DragInt("瀹忕紪鍙�" + i.ToString(), ref macroindex, 1, 0, 99))
                        {
                            Service.Configuration.Events[i].MacroIndex = macroindex;
                        }


                        bool isShared = Service.Configuration.Events[i].IsShared;
                        if (ImGui.Checkbox("鍏变韩瀹�" + i.ToString(), ref isShared))
                        {
                            Service.Configuration.Events[i].IsShared = isShared;
                            Service.Configuration.Save();
                        }

                        ImGui.SameLine();
                        ComboConfigWindow.Spacing();
                        if (ImGui.Button("鍒犻櫎浜嬩欢" + i.ToString()))
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

            if (ImGui.BeginTabItem("鎶�鑳介噴鏀炬潯浠�"))
            {
                ImGui.Text("鍦ㄨ繖涓獥鍙ｏ紝浣犲彲浠ヨ瀹氭瘡涓妧鑳界殑閲婃斁鏉′欢銆�");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


                if (ImGui.BeginChild("鏉′欢鍒楄〃", new Vector2(0f, -1f), true))
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

                    if (ImGui.CollapsingHeader("鎵�鏈夎亴鑳芥妧鑳�"))
                    {
                        foreach (var item in IconReplacer.GeneralBaseAction)
                        {
                            DrawAction(item);
                            ImGui.Separator();
                        }
                    }

                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("甯姪鏂囨。"))
            {
                ImGui.Text("鍦ㄨ繖涓獥鍙ｏ紝浣犲彲浠ョ湅鍒版垬鏂楃敤瀹忥紝璁剧疆鐢ㄨ鍦ㄨ缃潰鏉夸腑鏌ョ湅銆�");

                if (ImGui.BeginChild("甯姪", new Vector2(0f, -1f), true))
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
                    CommandHelp("AttackSmart", "濡傛灉涓嶅湪杩涙敾涓氨寮�濮嬭繘鏀伙紝濡傛灉鍦ㄨ繘鏀诲氨鍒囨崲閫夋嫨鏁屽鐩爣鏉′欢銆�");
                    ImGui.Separator();
                    CommandHelp("AttackManual", "寮�濮嬭繘鏀伙紝杩涙敾瀵硅薄涓烘墜鍔ㄩ�夋嫨锛屾鏃朵笉浼氶噴鏀続OE銆�");
                    ImGui.Separator();
                    CommandHelp("AttackCancel", "鍋滄杩涙敾锛岃寰椾竴瀹氳缁忓父鍏虫帀锛�");
                    ImGui.Separator();
                    CommandHelp("HealArea", "寮�鍚竴娈佃寖鍥存不鐤楃殑绐楀彛鏈熴��");
                    ImGui.Separator();
                    CommandHelp("HealSingle", "寮�鍚竴娈靛崟浣撴不鐤楃殑绐楀彛鏈熴��");
                    ImGui.Separator();
                    CommandHelp("DefenseArea", "寮�鍚竴娈佃寖鍥撮槻寰＄殑绐楀彛鏈熴��");
                    ImGui.Separator();
                    CommandHelp("DefenseSingle", "寮�鍚竴娈靛崟浣撻槻寰＄殑绐楀彛鏈熴��");
                    ImGui.Separator();
                    CommandHelp("EsunaShield", "寮�鍚竴娈靛悍澶嶆垨鑰呯浘濮挎垨鑰呯湡鍖楃殑绐楀彛鏈熴��");
                    ImGui.Separator();
                    CommandHelp("RaiseShirk", "寮�鍚己鍒舵晳浜烘垨閫�閬跨殑绐楀彛鏈熴��");
                    ImGui.Separator();
                    CommandHelp("AntiRepulsion", "寮�鍚竴娈甸槻鍑婚��鐨勭獥鍙ｆ湡銆�");
                    ImGui.Separator();
                    CommandHelp("BreakProvoke", "寮�鍚竴娈电垎鍙戞垨鎸戣鐨勭獥鍙ｆ湡銆�");
                    ImGui.Separator();
                    CommandHelp("Move", "寮�鍚竴娈典綅绉荤殑绐楀彛鏈熴��");
                    ImGui.Separator();
                }
                ImGui.PopStyleVar();

                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }



    internal static void DrawTexture<T>(T texture, Action otherThing, ClassJobID jobId = 0, string[] authors = null) where T : class, ITexture
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
            if (ImGui.Combo("##" + texture.Name + "浣滆��", ref i, authors, authors.Length))
            {
                Service.Configuration.ComboChoices[(uint)jobId] = authors[i];
            }
        }

        ImGui.SameLine();
        Spacing();

        if(texture is ICustomCombo com)
        {
            if (texture is IScriptCombo script)
            {
#if DEBUG

                if (ImGuiComponents.IconButton(FontAwesomeIcon.Edit))
                {
                    XIVAutoAttackPlugin.OpenScriptWindow(script);
                }
#endif
            }
            else
            {
                //ImGui.PushFont(UiBuilder.IconFont);
                //ImGui.Button($"源码##源码{texture.Name}")
                if (ImGuiComponents.IconButton(FontAwesomeIcon.InternetExplorer))
                {
                    var url = @"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/" + texture.GetType().FullName.Replace(".", @"/") + ".cs";
                    Process.Start("cmd", $"/C start {url}");
                }
                //ImGui.PopFont();
            }

#if DEBUG
            ImGui.SameLine();
            Spacing();

            if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
            {
                IconReplacer.AddScripCombo(com.JobIDs[0]);
            }
#endif
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

#if DEBUG
    private static void DrawAction(ActionID id, string type)
    {
        var action = new BaseAction(id);

        ImGui.Text($"{type}: {action}");

    }
#endif

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
            ImGui.SetTooltip($"鍗曞嚮浠ユ墽琛屽懡浠�: {command}");
        }

        if (!string.IsNullOrEmpty(help))
        {
            ImGui.SameLine();
            ImGui.Text(" 鈫� " + help);
        }
    }
    private unsafe static void DrawAction(BaseAction act)
    {
        if (act == null) return;

        DrawTexture(act, () =>
        {
#if DEBUG
            //CommandHelp("Enable" + act.Name, $"浣跨敤{act}");
            //CommandHelp("Disable" + act.Name, $"鍏抽棴{act}");
            //CommandHelp($"Insert{act}-{5}", $"5s鍐呮渶楂樹紭鍏堟彃鍏act}");

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
