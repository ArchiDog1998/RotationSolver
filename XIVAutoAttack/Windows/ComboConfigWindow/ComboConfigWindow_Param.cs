using Dalamud.Game.ClientState.Keys;
using ImGuiNET;
using System;
using System.Numerics;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;

namespace XIVAutoAttack.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawParam()
    {
        ImGui.Text("在这个窗口，你可以设定释放技能所需的参数。");

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginChild("参数", new Vector2(0f, -1f), true))
        {
            bool neverReplaceIcon = Service.Configuration.NeverReplaceIcon;
            if (ImGui.Checkbox("不替换图标", ref neverReplaceIcon))
            {
                Service.Configuration.NeverReplaceIcon = neverReplaceIcon;
                Service.Configuration.Save();
            }

            bool useOverlayWindow = Service.Configuration.UseOverlayWindow;
            if (ImGui.Checkbox("使用最高大覆盖窗口", ref useOverlayWindow))
            {
                Service.Configuration.UseOverlayWindow = useOverlayWindow;
                Service.Configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("这个窗口目前用于提前提示身位。");
            }

            if (ImGui.CollapsingHeader("基础设置"))
            {

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
                if (ImGui.DragFloat("间隔多久释放能力技", ref weaponInterval, 0.002f, 0.5f, 0.7f))
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

                int addDotGCDCount = Service.Configuration.AddDotGCDCount;
                if (ImGui.DragInt("还差几个GCD就可以补DOT了", ref addDotGCDCount, 0.2f, 0, 3))
                {
                    Service.Configuration.AddDotGCDCount = addDotGCDCount;
                    Service.Configuration.Save();
                }
            }

            ImGui.Separator();

            if (ImGui.CollapsingHeader("提示增强"))
            {
                bool poslockCasting = Service.Configuration.PoslockCasting;
                VirtualKey poslockModifier = Service.Configuration.PoslockModifier;
                if (ImGui.Checkbox("使用咏唱移动锁", ref poslockCasting))
                {
                    Service.Configuration.PoslockCasting = poslockCasting;
                    Service.Configuration.Save();
                }

                var modifierChoices = new VirtualKey[] { VirtualKey.CONTROL, VirtualKey.SHIFT, VirtualKey.MENU };
                if (poslockCasting && ImGui.BeginCombo("无视咏唱锁热键", poslockModifier.ToName()))
                {
                    foreach (VirtualKey k in modifierChoices)
                    {
                        if (ImGui.Selectable(k.ToName()))
                        {
                            Service.Configuration.PoslockModifier = k;
                            Service.Configuration.Save();
                        }
                    }
                    ImGui.EndCombo();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("手柄玩家为按下LT无视咏唱锁");
                }

                bool usecheckCasting = Service.Configuration.CheckForCasting;
                if (ImGui.Checkbox("使用咏唱结束显示", ref usecheckCasting))
                {
                    Service.Configuration.CheckForCasting = usecheckCasting;
                    Service.Configuration.Save();
                }

                bool teachingMode = Service.Configuration.TeachingMode;
                if (ImGui.Checkbox("循环教育模式", ref teachingMode))
                {
                    Service.Configuration.TeachingMode = teachingMode;
                    Service.Configuration.Save();
                }
                if (teachingMode)
                {
                    ImGui.SameLine();
                    Spacing();

                    var teachingColor = Service.Configuration.TeachingModeColor;

                    if (ImGui.ColorEdit3("教育模式颜色", ref teachingColor))
                    {
                        Service.Configuration.TeachingModeColor = teachingColor;
                        Service.Configuration.Save();
                    }
                }

                bool keyBoardNoise = Service.Configuration.KeyBoardNoise;
                if (ImGui.Checkbox("模拟按下键盘效果", ref keyBoardNoise))
                {
                    Service.Configuration.KeyBoardNoise = keyBoardNoise;
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
                if (useOverlayWindow && ImGui.Checkbox("显示身位错误", ref sayoutLocationWrong))
                {
                    Service.Configuration.SayoutLocationWrong = sayoutLocationWrong;
                    Service.Configuration.Save();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("身位错误不是很准，仅供参考。");
                }

                var str = Service.Configuration.LocationText;
                if (ImGui.InputText("身位错误提示语", ref str, 15))
                {
                    Service.Configuration.LocationText = str;
                    Service.Configuration.Save();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("如果身位错误，你想怎么被骂!");
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
                bool useAOEWhenManual = Service.Configuration.UseAOEWhenManual;
                if (ImGui.Checkbox("在手动选择的时候使用AOE技能", ref useAOEWhenManual))
                {
                    Service.Configuration.UseAOEWhenManual = useAOEWhenManual;
                    Service.Configuration.Save();
                }

                bool autoBreak = Service.Configuration.AutoBreak;
                if (ImGui.Checkbox("自动进行爆发", ref autoBreak))
                {
                    Service.Configuration.AutoBreak = autoBreak;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                Spacing();
                CommandHelp("AutoBreak");


                bool isOnlyGCD = Service.Configuration.OnlyGCD;
                if (ImGui.Checkbox("只使用GCD循环，除去能力技", ref isOnlyGCD))
                {
                    Service.Configuration.OnlyGCD = isOnlyGCD;
                    Service.Configuration.Save();
                }

                bool attackSafeMode = Service.Configuration.AttackSafeMode;
                if (ImGui.Checkbox("绝对单体模式", ref attackSafeMode))
                {
                    Service.Configuration.AttackSafeMode = attackSafeMode;
                    Service.Configuration.Save();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("绝对保证在单目标的时候不打AOE，就算大招也是。但是如果怪的数量达到标准依然会使用AOE。");
                }

                if (!isOnlyGCD)
                {
                    Spacing();
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

                    Spacing();
                    bool autoDefenseforTank = Service.Configuration.AutoDefenseForTank;
                    if (ImGui.Checkbox("自动上减伤(不太准)", ref autoDefenseforTank))
                    {
                        Service.Configuration.AutoDefenseForTank = autoDefenseforTank;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("自动的这个不能识别威力为0的AOE技能，请注意。");
                    }

                    Spacing();
                    bool autoShieled = Service.Configuration.AutoShield;
                    if (ImGui.Checkbox("T自动上盾", ref autoShieled))
                    {
                        Service.Configuration.AutoShield = autoShieled;
                        Service.Configuration.Save();
                    }

                    Spacing();
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

                    Spacing();
                    bool autoUseTrueNorth = Service.Configuration.AutoUseTrueNorth;
                    if (ImGui.Checkbox("近战自动上真北", ref autoUseTrueNorth))
                    {
                        Service.Configuration.AutoUseTrueNorth = autoUseTrueNorth;
                        Service.Configuration.Save();
                    }

                    Spacing();
                    bool raiseSwift = Service.Configuration.RaisePlayerBySwift;
                    if (ImGui.Checkbox("即刻拉人", ref raiseSwift))
                    {
                        Service.Configuration.RaisePlayerBySwift = raiseSwift;
                        Service.Configuration.Save();
                    }

                    Spacing();
                    bool useAreaAbilityFriendly = Service.Configuration.UseAreaAbilityFriendly;
                    if (ImGui.Checkbox("使用友方地面放置技能", ref useAreaAbilityFriendly))
                    {
                        Service.Configuration.UseAreaAbilityFriendly = useAreaAbilityFriendly;
                        Service.Configuration.Save();
                    }
                }

                bool raiseCasting = Service.Configuration.RaisePlayerByCasting;
                if (ImGui.Checkbox("无目标时硬读条拉人", ref raiseCasting))
                {
                    Service.Configuration.RaisePlayerByCasting = raiseCasting;
                    Service.Configuration.Save();
                }

                bool useHealWhenNotAHealer = Service.Configuration.UseHealWhenNotAHealer;
                if (ImGui.Checkbox("非奶妈用奶人的技能", ref useHealWhenNotAHealer))
                {
                    Service.Configuration.UseHealWhenNotAHealer = useHealWhenNotAHealer;
                    Service.Configuration.Save();
                }

                int lessMPNoRaise = Service.Configuration.LessMPNoRaise;
                if (ImGui.DragInt("小于多少蓝就不复活了", ref lessMPNoRaise, 200, 0, 10000))
                {
                    Service.Configuration.LessMPNoRaise = lessMPNoRaise;
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
                    ImGui.SetTooltip("使用爆发药，目前还未写全");
                }
            }

            ImGui.Separator();

            if (ImGui.CollapsingHeader("触发条件"))
            {
                bool autoStartCountdown = Service.Configuration.AutoStartCountdown;
                if (ImGui.Checkbox("倒计时时自动打开攻击", ref autoStartCountdown))
                {
                    Service.Configuration.AutoStartCountdown = autoStartCountdown;
                    Service.Configuration.Save();
                }

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

                float healingOfTimeSubstactArea = Service.Configuration.HealingOfTimeSubstactArea;
                if (ImGui.DragFloat("如果使用群体Hot技能，阈值下降多少", ref healingOfTimeSubstactArea, speed, 0, 1))
                {
                    Service.Configuration.HealingOfTimeSubstactArea = healingOfTimeSubstactArea;
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

                float healingOfTimeSubstact = Service.Configuration.HealingOfTimeSubstactSingle;
                if (ImGui.DragFloat("如果使用单体Hot技能，阈值下降多少", ref healingOfTimeSubstact, speed, 0, 1))
                {
                    Service.Configuration.HealingOfTimeSubstactSingle = healingOfTimeSubstact;
                    Service.Configuration.Save();
                }


                float healthTank = Service.Configuration.HealthForDyingTank;
                if (ImGui.DragFloat("低于多少的HP，坦克要放大招了", ref healthTank, speed, 0, 1))
                {
                    Service.Configuration.HealthForDyingTank = healthTank;
                    Service.Configuration.Save();
                }
            }

            ImGui.Separator();

            if (ImGui.CollapsingHeader("目标选择"))
            {
                int isAllTargetAsHostile = IconReplacer.RightNowTargetToHostileType;
                if (ImGui.Combo("敌对目标筛选条件", ref isAllTargetAsHostile, new string[]
                {
                                "所有能打的目标都是敌对的目标",
                                "如果处于打人的目标数量为零，所有能打的都是敌对的",
                                "只有有目标的目标才是敌对的目标",
                }, 3))
                {
                    IconReplacer.RightNowTargetToHostileType = (byte)isAllTargetAsHostile;
                    Service.Configuration.Save();
                }

                bool addEnemyListToHostile = Service.Configuration.AddEnemyListToHostile;
                if (ImGui.Checkbox("将敌对列表的对象设为敌对", ref addEnemyListToHostile))
                {
                    Service.Configuration.AddEnemyListToHostile = addEnemyListToHostile;
                    Service.Configuration.Save();
                }

                bool chooseAttackMark = Service.Configuration.ChooseAttackMark;
                if (ImGui.Checkbox("优先选中有攻击标记的目标", ref chooseAttackMark))
                {
                    Service.Configuration.ChooseAttackMark = chooseAttackMark;
                    Service.Configuration.Save();
                }

                if (chooseAttackMark)
                {
                    Spacing();
                    bool attackMarkAOE = Service.Configuration.AttackMarkAOE;

                    if (ImGui.Checkbox("是否还要使用AOE", ref attackMarkAOE))
                    {
                        Service.Configuration.AttackMarkAOE = attackMarkAOE;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("如果勾选了，那么可能这个AOE打不到攻击目标的对象，因为为了追求打到更多的目标。");
                    }
                }

                bool filterStopMark = Service.Configuration.FilterStopMark;
                if (ImGui.Checkbox("去掉有停止标记的目标", ref filterStopMark))
                {
                    Service.Configuration.FilterStopMark = filterStopMark;
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

                bool changeTargetForFate = Service.Configuration.ChangeTargetForFate;
                if (ImGui.Checkbox("在Fate中只选择Fate怪", ref changeTargetForFate))
                {
                    Service.Configuration.ChangeTargetForFate = changeTargetForFate;
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

                bool raiseBrinkofDeath = Service.Configuration.RaiseBrinkofDeath;
                if (ImGui.Checkbox("复活濒死（黑头）之人", ref raiseBrinkofDeath))
                {
                    Service.Configuration.RaiseBrinkofDeath = raiseBrinkofDeath;
                    Service.Configuration.Save();
                }
            }

            ImGui.Separator();

            if (ImGui.CollapsingHeader("敌对选择"))
            {
                if (ImGui.Button("添加选择条件"))
                {
                    Service.Configuration.TargetingTypes.Add(TargetingType.Big);
                }
                ImGui.SameLine();
                Spacing();
                ImGui.Text("你可以设定敌对的选择，以便于在战斗中灵活切换选择敌对的逻辑。");
                for (int i = 0; i < Service.Configuration.TargetingTypes.Count; i++)
                {

                    ImGui.Separator();

                    var names = Enum.GetNames(typeof(TargetingType));
                    var targingType = (int)Service.Configuration.TargetingTypes[i];
                    if (ImGui.Combo("敌对目标选择条件" + i.ToString(), ref targingType, names, names.Length))
                    {
                        Service.Configuration.TargetingTypes[i] = (TargetingType)targingType;
                        Service.Configuration.Save();
                    }

                    if (ImGui.Button("上移条件" + i.ToString()))
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
                    if (ImGui.Button("下移条件" + i.ToString()))
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

                    if (ImGui.Button("删除条件" + i.ToString()))
                    {
                        Service.Configuration.TargetingTypes.RemoveAt(i);
                    }
                }
            }

            ImGui.EndChild();
        }
        ImGui.PopStyleVar();
    }
}
